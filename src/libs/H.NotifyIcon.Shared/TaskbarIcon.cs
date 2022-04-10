//using Microsoft.UI;
//using Microsoft.UI.Windowing;
//using Windows.Graphics;
//using WinRT.Interop;

using System.Diagnostics;
using H.NotifyIcon.Core;

namespace H.NotifyIcon;

/// <summary>
/// A proxy to for a taskbar icon (NotifyIcon) that sits in the system's
/// taskbar notification area ("system tray").
/// </summary>
#if HAS_WINUI || HAS_UNO
[CLSCompliant(false)]
#endif
public partial class TaskbarIcon : FrameworkElement, IDisposable
{
    #region Members

    /// <summary>
    /// Represents the current icon data.
    /// </summary>
    private TrayIcon TrayIcon { get; }

    /// <summary>
    /// Receives messages from the taskbar icon.
    /// </summary>
    public MessageWindow MessageWindow { get; } = new();

    /// <summary>
    /// An action that is being invoked if the
    /// <see cref="singleClickTimer"/> fires.
    /// </summary>
    private Action? singleClickTimerAction;

    /// <summary>
    /// A timer that is used to differentiate between single
    /// and double clicks.
    /// </summary>
    private readonly Timer singleClickTimer;

    /// <summary>
    /// The time we should wait for a double click.
    /// </summary>
    private int DoubleClickWaitTime => NoLeftClickDelay ? 0 : CursorUtilities.GetDoubleClickTime();

#if HAS_WPF
    /// <summary>
    /// A timer that is used to close open balloon tooltips.
    /// </summary>
    private readonly Timer balloonCloseTimer;
#endif

    /// <summary>
    /// Indicates whether the taskbar icon has been created or not.
    /// </summary>
    public bool IsCreated => TrayIcon.IsCreated;

    /// <summary>
    /// Indicates whether custom tooltips are supported, which depends
    /// on the OS. Windows Vista or higher is required in order to
    /// support this feature.
    /// </summary>
    public bool SupportsCustomToolTips => TrayIcon.SupportsCustomToolTips;


    /// <summary>
    /// Checks whether a non-tooltip popup is currently opened.
    /// </summary>
    private bool IsPopupOpen
    {
        get
        {
            var popup = TrayPopupResolved;
#if HAS_WPF
            var menu = ContextMenu;
#else
            var menu = ContextFlyout;
#endif
            var balloon = CustomBalloon;

            return popup != null && popup.IsOpen ||
                   menu != null && menu.IsOpen ||
                   balloon != null && balloon.IsOpen;
        }
    }

    #endregion

    #region Construction

#if HAS_WPF

    static TaskbarIcon()
    {
        VisibilityProperty.OverrideMetadata(
            typeof(TaskbarIcon),
            new PropertyMetadata(Visibility.Visible, VisibilityPropertyChanged));
        DataContextProperty.OverrideMetadata(
            typeof(TaskbarIcon),
            new FrameworkPropertyMetadata(DataContextPropertyChanged));
        ContextMenuProperty.OverrideMetadata(
            typeof(TaskbarIcon),
            new FrameworkPropertyMetadata(ContextMenuPropertyChanged));
    }

#endif
    
    /// <summary>
    /// Initializes the taskbar icon and registers a message listener
    /// in order to receive events from the taskbar area.
    /// </summary>
    public TaskbarIcon()
    {
#if !HAS_WPF
        RegisterPropertyChangedCallback(VisibilityProperty, (_, _) =>
        {
            SetTrayIconVisibility(Visibility);
        });
        RegisterPropertyChangedCallback(DataContextProperty, (_, _) =>
        {
            UpdateDataContext(null, DataContext);
        });
        RegisterPropertyChangedCallback(ContextFlyoutProperty, (_, _) =>
        {
            SetParentTaskbarIcon(ContextFlyout, this);
            UpdateContextFlyoutDataContext(ContextFlyout, null, DataContext);
        });
#endif
        if (!DesignTimeUtilities.IsDesignMode)
        {
            MessageWindow.Create();
        }

        TrayIcon = new TrayIcon()
        {
            WindowHandle = MessageWindow.Handle,
            CallbackMessage = MessageWindow.CallbackMessageId,
        };
        TrayIcon.VersionChanged += (_, version) => MessageWindow.Version = version;
        Loaded += (_, _) =>
        {
            if (DesignTimeUtilities.IsDesignMode)
            {
                return;
            }

            try
            {
                ForceCreate();
            }
            catch (Exception)
            {
                Debugger.Break();
            }
        };
        MessageWindow.DpiChanged += static (_, _) => DpiUtilities.UpdateDpiFactors();
        MessageWindow.TaskbarCreated += OnTaskbarCreated;

        // register event listeners
        MessageWindow.MouseEventReceived += OnMouseEvent;
        MessageWindow.KeyboardEventReceived += OnKeyboardEvent;
        MessageWindow.ChangeToolTipStateRequest += OnToolTipChange;
        MessageWindow.BalloonToolTipChanged += OnBalloonToolTipChanged;

        // init single click / balloon timers
        singleClickTimer = new Timer(DoSingleClickAction);

#if HAS_WPF
        balloonCloseTimer = new Timer(CloseBalloonCallback);
#endif

#if HAS_WPF
        // register listener in order to get notified when the application closes
        if (Application.Current != null)
        {
            Application.Current.Exit += OnExit;
        }
#endif
    }

    #endregion

    #region Event handlers

    /// <summary>
    /// Recreates the taskbar icon if the whole taskbar was
    /// recreated (e.g. because Explorer was shut down).
    /// </summary>
    private void OnTaskbarCreated(object? sender, EventArgs args)
    {
        try
        {
            _ = TrayIcon.TryRemove();
            TrayIcon.Create();
        }
        catch (Exception)
        {
            // ignored.
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Use it to force create icon if it placed in resources.
    /// </summary>
    public void ForceCreate()
    {
        TrayIcon.Create();

        // Workaround for https://github.com/HavenDV/H.NotifyIcon/issues/14
        using var refreshTrayIcon = new TrayIcon(TrayIcon.CreateUniqueGuidForEntryAssembly("RefreshWorkaround"));
        refreshTrayIcon.Create();
    }

    #endregion

    #region Custom Balloons

#if HAS_WPF

    /// <summary>
    /// A delegate to handle customer popup positions.
    /// </summary>
    public delegate Point GetCustomPopupPosition();

    /// <summary>
    /// Specify a custom popup position
    /// </summary>
    public GetCustomPopupPosition? CustomPopupPosition { get; set; }

    /// <summary>
    /// Returns the location of the system tray
    /// </summary>
    /// <returns>Point</returns>
    public static Point GetPopupTrayPosition()
    {
        return TrayInfo.GetTrayLocation().ScaleWithDpi();
    }

    /// <summary>
    /// Shows a custom control as a tooltip in the tray location.
    /// </summary>
    /// <param name="balloon"></param>
    /// <param name="animation">An optional animation for the popup.</param>
    /// <param name="timeout">The time after which the popup is being closed.
    /// Submit null in order to keep the balloon open indefinitely
    /// </param>
    /// <exception cref="ArgumentNullException">If <paramref name="balloon"/>
    /// is a null reference.</exception>
    public void ShowCustomBalloon(UIElement balloon, PopupAnimation animation, int? timeout)
    {
        var dispatcher = this.GetDispatcher();
        if (!dispatcher.CheckAccess())
        {
            var action = new Action(() => ShowCustomBalloon(balloon, animation, timeout));
            dispatcher.Invoke(DispatcherPriority.Normal, action);
            return;
        }

        if (balloon == null)
        {
            throw new ArgumentNullException(nameof(balloon));
        }

        if (timeout.HasValue && timeout < 500)
        {
            throw new ArgumentOutOfRangeException(
                nameof(timeout),
                $"Invalid timeout of {timeout} milliseconds. Timeout must be at least 500 ms");
        }

        EnsureNotDisposed();

        // make sure we don't have an open balloon
        //lock (lockObject)
        {
            CloseBalloon();
        }

        // create an invisible popup that hosts the UIElement
        var popup = new Popup
        {
            AllowsTransparency = true
        };

        // provide the popup with the taskbar icon's data context
        UpdateDataContext(popup, null, DataContext);

        // don't animate by default - developers can use attached events or override
        popup.PopupAnimation = animation;

        // in case the balloon is cleaned up through routed events, the
        // control didn't remove the balloon from its parent popup when
        // if was closed the last time - just make sure it doesn't have
        // a parent that is a popup
        var parent = LogicalTreeHelper.GetParent(balloon) as Popup;
        if (parent != null)
        {
            parent.Child = null;
        }

        if (parent != null)
        {
            throw new InvalidOperationException(
                $"Cannot display control [{balloon}] in a new balloon popup - " +
                $"that control already has a parent. You may consider creating " +
                $"new balloons every time you want to show one.");
        }

        popup.Child = balloon;

        //don't set the PlacementTarget as it causes the popup to become hidden if the
        //TaskbarIcon's parent is hidden, too...
        //popup.PlacementTarget = this;

        popup.Placement = PlacementMode.AbsolutePoint;
        popup.StaysOpen = true;


        var position = CustomPopupPosition != null
            ? CustomPopupPosition()
            : GetPopupTrayPosition();
        popup.HorizontalOffset = position.X - 1;
        popup.VerticalOffset = position.Y - 1;

        //store reference
        //lock (lockObject)
        {
            SetCustomBalloon(popup);
        }

        // assign this instance as an attached property
        SetParentTaskbarIcon(balloon, this);

        // fire attached event
        RaiseBalloonShowingEvent(balloon, this);

        // display item
        popup.IsOpen = true;

        if (timeout.HasValue)
        {
            // register timer to close the popup
            balloonCloseTimer.Change(timeout.Value, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Resets the closing timeout, which effectively
    /// keeps a displayed balloon message open until
    /// it is either closed programmatically through
    /// <see cref="CloseBalloon"/> or due to a new
    /// message being displayed.
    /// </summary>
    public void ResetBalloonCloseTimer()
    {
        if (IsDisposed)
        {
            return;
        }

        //lock (lockObject)
        {
            //reset timer in any case
            balloonCloseTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Closes the current <see cref="CustomBalloon"/>, if the
    /// property is set.
    /// </summary>
    public void CloseBalloon()
    {
        if (IsDisposed)
        {
            return;
        }

        var dispatcher = this.GetDispatcher();
        if (!dispatcher.CheckAccess())
        {
            Action action = CloseBalloon;
            dispatcher.Invoke(DispatcherPriority.Normal, action);
            return;
        }

        //lock (lockObject)
        {
            // reset timer in any case
            balloonCloseTimer.Change(Timeout.Infinite, Timeout.Infinite);

            // reset old popup, if we still have one
            var popup = CustomBalloon;
            if (popup == null)
            {
                return;
            }

            var element = popup.Child;

            // announce closing
            var eventArgs = RaiseBalloonClosingEvent(element, this);
            if (!eventArgs.Handled)
            {
                // if the event was handled, clear the reference to the popup,
                // but don't close it - the handling code has to manage this stuff now

                // close the popup
                popup.IsOpen = false;

                // remove the reference of the popup to the balloon in case we want to reuse
                // the balloon (then added to a new popup)
                popup.Child = null;

                // reset attached property
                if (element != null)
                {
                    SetParentTaskbarIcon(element, null);
                }
            }

            // remove custom balloon anyway
            SetCustomBalloon(null);
        }
    }


    /// <summary>
    /// Timer-invoke event which closes the currently open balloon and
    /// resets the <see cref="CustomBalloon"/> dependency property.
    /// </summary>
    private void CloseBalloonCallback(object? state)
    {
        if (IsDisposed)
        {
            return;
        }

        // switch to UI thread
        Action action = CloseBalloon;
        this.GetDispatcher().Invoke(action);
    }

#endif

    #endregion

    #region Process Incoming Mouse Events

    /// <summary>
    /// Processes mouse events, which are bubbled
    /// through the class' routed events, trigger
    /// certain actions (e.g. show a popup), or
    /// both.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="me">Event flag.</param>
    private void OnMouseEvent(object? sender, MouseEvent me)
    {
        if (IsDisposed)
        {
            return;
        }

        switch (me)
        {
            case MouseEvent.MouseMove:
#if HAS_WPF
                RaiseTrayMouseMoveEvent();
#endif
                // immediately return - there's nothing left to evaluate
                return;
            case MouseEvent.IconRightMouseDown:
#if HAS_WPF
                RaiseTrayRightMouseDownEvent();
#endif
                break;
            case MouseEvent.IconLeftMouseDown:
#if HAS_WPF
                RaiseTrayLeftMouseDownEvent();
#endif
                break;
            case MouseEvent.IconRightMouseUp:
#if HAS_WPF
                RaiseTrayRightMouseUpEvent();
#endif
                break;
            case MouseEvent.IconLeftMouseUp:
#if HAS_WPF
                RaiseTrayLeftMouseUpEvent();
#endif
                break;
            case MouseEvent.IconMiddleMouseDown:
#if HAS_WPF
                RaiseTrayMiddleMouseDownEvent();
#endif
                break;
            case MouseEvent.IconMiddleMouseUp:
#if HAS_WPF
                RaiseTrayMiddleMouseUpEvent();
#endif
                break;
            case MouseEvent.IconDoubleClick:
                // cancel single click timer
                singleClickTimer?.Change(Timeout.Infinite, Timeout.Infinite);
#if HAS_WPF
                // bubble event
                RaiseTrayMouseDoubleClickEvent();
#else
                DoubleClickCommand?.TryExecute(DoubleClickCommandParameter);
#endif
                break;
            case MouseEvent.BalloonToolTipClicked:
#if HAS_WPF
                RaiseTrayBalloonTipClickedEvent();
#endif
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(me), "Missing handler for mouse event flag: " + me);
        }


        // get mouse coordinates
        var cursorPosition = TrayIcon.Version == NotifyIconVersion.Vista
            ? CursorUtilities.GetPhysicalCursorPos()
            : CursorUtilities.GetCursorPos();

        cursorPosition = cursorPosition.ScaleWithDpi();

        var isLeftClickCommandInvoked = false;

        // show popup, if requested
        if (me.IsMatch(PopupActivation))
        {
            if (me == MouseEvent.IconLeftMouseUp)
            {
                // show popup once we are sure it's not a double click
                singleClickTimerAction = () =>
                {
#if HAS_WPF
                    LeftClickCommand?.TryExecute(LeftClickCommandParameter, LeftClickCommandTarget ?? this);
#else
                    LeftClickCommand?.TryExecute(LeftClickCommandParameter);
#endif
                    ShowTrayPopup(cursorPosition);
                };
                singleClickTimer?.Change(DoubleClickWaitTime, Timeout.Infinite);
                isLeftClickCommandInvoked = true;
            }
            else
            {
                // show popup immediately
                ShowTrayPopup(cursorPosition);
            }
        }


        // show context menu, if requested
        if (me.IsMatch(MenuActivation))
        {
            if (me == MouseEvent.IconLeftMouseUp)
            {
                // show context menu once we are sure it's not a double click
                singleClickTimerAction = () =>
                {
#if HAS_WPF
                    LeftClickCommand?.TryExecute(LeftClickCommandParameter, LeftClickCommandTarget ?? this);
#else
                    LeftClickCommand?.TryExecute(LeftClickCommandParameter);
#endif
                    ShowContextMenu(cursorPosition);
                };
                singleClickTimer?.Change(DoubleClickWaitTime, Timeout.Infinite);
                isLeftClickCommandInvoked = true;
            }
            else
            {
                // show context menu immediately
                ShowContextMenu(cursorPosition);
            }
        }

        // make sure the left click command is invoked on mouse clicks
        if (me == MouseEvent.IconLeftMouseUp && !isLeftClickCommandInvoked)
        {
            // show context menu once we are sure it's not a double click
            singleClickTimerAction =
                () =>
                {
#if HAS_WPF
                    LeftClickCommand?.TryExecute(LeftClickCommandParameter, LeftClickCommandTarget ?? this);
#else
                    LeftClickCommand?.TryExecute(LeftClickCommandParameter);
#endif
                };
            singleClickTimer?.Change(DoubleClickWaitTime, Timeout.Infinite);
        }
    }

    #endregion

    #region Process Incoming Keyboard Events

    /// <summary>
    /// Processes keyboard events, which are bubbled
    /// through the class' routed events, trigger
    /// certain actions (e.g. show a popup), or
    /// both.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="event">Keyboard event</param>
    private void OnKeyboardEvent(object? sender, KeyboardEvent @event)
    {
        if (IsDisposed)
        {
            return;
        }

        switch (@event)
        {
            case KeyboardEvent.ContextMenu:
#if HAS_WPF
                RaiseTrayKeyboardContextMenuEvent();
#endif
                break;
            case KeyboardEvent.KeySelect:
#if HAS_WPF
                RaiseTrayKeyboardKeySelectEvent();
#endif
                break;
            case KeyboardEvent.Select:
#if HAS_WPF
                RaiseTrayKeyboardSelectEvent();
#endif
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(@event),
                    $"Missing handler for keyboard event flag: {@event}");
        }
    }

    #endregion

    #region ToolTips

    /// <summary>
    /// Displays a custom tooltip, if available. This method is only
    /// invoked for Windows Vista and above.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="visible">Whether to show or hide the tooltip.</param>
    private void OnToolTipChange(object? sender, bool visible)
    {
        // if we don't have a tooltip, there's nothing to do here...
        if (TrayToolTipResolved == null)
        {
            return;
        }

        if (visible)
        {
            if (IsPopupOpen)
            {
                // ignore if we are already displaying something down there
                return;
            }

#if HAS_WPF
            var args = RaisePreviewTrayToolTipOpenEvent();
            if (args.Handled)
            {
                return;
            }
#endif

            TrayToolTipResolved.IsOpen = true;

#if HAS_WPF
            // raise attached event first
            if (TrayToolTip != null)
            {
                RaiseToolTipOpenedEvent(TrayToolTip);
            }

            // bubble routed event
            RaiseTrayToolTipOpenEvent();
#endif
        }
        else
        {
#if HAS_WPF
            var args = RaisePreviewTrayToolTipCloseEvent();
            if (args.Handled)
            {
                return;
            }

            // raise attached event first
            if (TrayToolTip != null)
            {
                RaiseToolTipCloseEvent(TrayToolTip);
            }
#endif

            TrayToolTipResolved.IsOpen = false;

#if HAS_WPF
            // bubble event
            RaiseTrayToolTipCloseEvent();
#endif
        }
    }


    /// <summary>
    /// Creates a <see cref="ToolTip"/> control that either
    /// wraps the currently set <see cref="TrayToolTip"/>
    /// control or the <see cref="ToolTipText"/> string.<br/>
    /// If <see cref="TrayToolTip"/> itself is already
    /// a <see cref="ToolTip"/> instance, it will be used directly.
    /// </summary>
    /// <remarks>We use a <see cref="ToolTip"/> rather than
    /// <see cref="Popup"/> because there was no way to prevent a
    /// popup from causing cyclic open/close commands if it was
    /// placed under the mouse. ToolTip internally uses a Popup of
    /// its own, but takes advance of Popup's internal <see cref="UIElement.IsHitTestVisible"/>
    /// property which prevents this issue.</remarks>
    private void CreateCustomToolTip()
    {
        // check if the item itself is a tooltip
        var tt = TrayToolTip as ToolTip;

        if (tt == null && TrayToolTip != null)
        {
            // create an invisible wrapper tooltip that hosts the UIElement
            tt = new ToolTip
            {
                Placement = PlacementMode.Mouse,
                // do *not* set the placement target, as it causes the popup to become hidden if the
                // TaskbarIcon's parent is hidden, too. At runtime, the parent can be resolved through
                // the ParentTaskbarIcon attached dependency property:
                // PlacementTarget = this;

                // make sure the tooltip is invisible
#if HAS_WPF
                HasDropShadow = false,
                Background = System.Windows.Media.Brushes.Transparent,
                // setting the
                StaysOpen = true,
#endif
                BorderThickness = new Thickness(0),
                Content = TrayToolTip,
            };
        }
        else if (tt == null && !string.IsNullOrEmpty(ToolTipText))
        {
            // create a simple tooltip for the ToolTipText string
            tt = new ToolTip
            {
                Content = ToolTipText
            };
        }

        // the tooltip explicitly gets the DataContext of this instance.
        // If there is no DataContext, the TaskbarIcon assigns itself
        if (tt != null)
        {
            UpdateDataContext(tt, null, DataContext);
        }

        // store a reference to the used tooltip
        SetTrayToolTipResolved(tt);
    }


    /// <summary>
    /// Sets tooltip settings for the class depending on defined
    /// dependency properties and OS support.
    /// </summary>
    private void WriteToolTipSettings()
    {
        var text = ToolTipText;
        if (TrayIcon.Version == NotifyIconVersion.Vista)
        {
            // we need to set a tooltip text to get tooltip events from the
            // taskbar icon
            if (string.IsNullOrEmpty(text) && TrayToolTipResolved != null)
            {
                // if we have not tooltip text but a custom tooltip, we
                // need to set a dummy value (we're displaying the ToolTip control, not the string)
                text = "ToolTip";
            }
        }

        if (!TrayIcon.IsCreated)
        {
            return;
        }

        TrayIcon.UpdateToolTip(text);
    }

    #endregion

    #region Custom Popup

    /// <summary>
    /// Creates a <see cref="ToolTip"/> control that either
    /// wraps the currently set <see cref="TrayToolTip"/>
    /// control or the <see cref="ToolTipText"/> string.<br/>
    /// If <see cref="TrayToolTip"/> itself is already
    /// a <see cref="ToolTip"/> instance, it will be used directly.
    /// </summary>
    /// <remarks>We use a <see cref="ToolTip"/> rather than
    /// <see cref="Popup"/> because there was no way to prevent a
    /// popup from causing cyclic open/close commands if it was
    /// placed under the mouse. ToolTip internally uses a Popup of
    /// its own, but takes advance of Popup's internal <see cref="UIElement.IsHitTestVisible"/>
    /// property which prevents this issue.</remarks>
    private void CreatePopup()
    {
        // check if the item itself is a popup
        var popup = TrayPopup as Popup;

        if (popup == null && TrayPopup != null)
        {
            // create an invisible popup that hosts the UIElement
            popup = new Popup
            {
#if HAS_WPF
                AllowsTransparency = true,
                // don't animate by default - developers can use attached events or override
                PopupAnimation = PopupAnimation.None,
                // the CreateRootPopup method outputs binding errors in the debug window because
                // it tries to bind to "Popup-specific" properties in case they are provided by the child.
                // We don't need that so just assign the control as the child.
                // do *not* set the placement target, as it causes the popup to become hidden if the
                // TaskbarIcon's parent is hidden, too. At runtime, the parent can be resolved through
                // the ParentTaskbarIcon attached dependency property:
                // PlacementTarget = this;

                Placement = PlacementMode.AbsolutePoint,
                StaysOpen = false,
#endif
                Child = TrayPopup,
            };
        }

        // the popup explicitly gets the DataContext of this instance.
        // If there is no DataContext, the TaskbarIcon assigns itself
        if (popup != null)
        {
            UpdateDataContext(popup, null, DataContext);
        }

        // store a reference to the used tooltip
        SetTrayPopupResolved(popup);
    }


    /// <summary>
    /// Displays the <see cref="TrayPopup"/> control if it was set.
    /// </summary>
    private void ShowTrayPopup(Point cursorPosition)
    {
        if (IsDisposed)
        {
            return;
        }

#if HAS_WPF
        // raise preview event no matter whether popup is currently set
        // or not (enables client to set it on demand)
        var args = RaisePreviewTrayPopupOpenEvent();
        if (args.Handled)
        {
            return;
        }
#endif

        if (TrayPopup == null)
        {
            return;
        }
        if (TrayPopupResolved == null)
        {
            return;
        }

#if HAS_WPF
        // use absolute position, but place the popup centered above the icon
        TrayPopupResolved.Placement = PlacementMode.AbsolutePoint;
#endif
        TrayPopupResolved.HorizontalOffset = cursorPosition.X;
        TrayPopupResolved.VerticalOffset = cursorPosition.Y;

        // open popup
        TrayPopupResolved.IsOpen = true;

#if HAS_WPF
        var handle = IntPtr.Zero;
        if (TrayPopupResolved.Child != null)
        {
            // try to get a handle on the popup itself (via its child)
            var source = (HwndSource)PresentationSource.FromVisual(TrayPopupResolved.Child);
            if (source != null)
            {
                handle = source.Handle;
            }
        }

        // if we don't have a handle for the popup, fall back to the message sink
        if (handle == IntPtr.Zero)
        {
            handle = MessageWindow.Handle;
        }

        // activate either popup or message sink to track deactivation.
        // otherwise, the popup does not close if the user clicks somewhere else
        WindowUtilities.SetForegroundWindow(handle);

        // raise attached event - item should never be null unless developers
        // changed the CustomPopup directly...
        if (TrayPopup != null)
        {
            RaisePopupOpenedEvent(TrayPopup);
        }

        // bubble routed event
        RaiseTrayPopupOpenEvent();
#endif
    }

    #endregion

    #region Context Menu

    /// <summary>
    /// Displays the ContextMenu/ContextFlyout if it was set.
    /// </summary>
    private void ShowContextMenu(Point cursorPosition)
    {
        if (IsDisposed)
        {
            return;
        }

#if HAS_WPF
        // raise preview event no matter whether context menu is currently set
        // or not (enables client to set it on demand)
        var args = RaisePreviewTrayContextMenuOpenEvent();
        if (args.Handled)
        {
            return;
        }

        if (ContextMenu == null)
        {
            return;
        }

        // use absolute positioning. We need to set the coordinates, or a delayed opening
        // (e.g. when left-clicked) opens the context menu at the wrong place if the mouse
        // is moved!
        ContextMenu.Placement = PlacementMode.AbsolutePoint;
        ContextMenu.HorizontalOffset = cursorPosition.X;
        ContextMenu.VerticalOffset = cursorPosition.Y;
        ContextMenu.IsOpen = true;

        var handle = IntPtr.Zero;

        // try to get a handle on the context itself
        var source = (HwndSource)PresentationSource.FromVisual(ContextMenu);
        if (source != null)
        {
            handle = source.Handle;
        }

        // if we don't have a handle for the popup, fall back to the message sink
        if (handle == IntPtr.Zero)
        {
            handle = MessageWindow.Handle;
        }

        // activate the context menu or the message window to track deactivation - otherwise, the context menu
        // does not close if the user clicks somewhere else. With the message window
        // fallback, the context menu can't receive keyboard events - should not happen though
        WindowUtilities.SetForegroundWindow(handle);

        // bubble event
        RaiseTrayContextMenuOpenEvent();
#else
        if (ContextFlyout == null)
        {
            return;
        }

        // use absolute positioning. We need to set the coordinates, or a delayed opening
        // (e.g. when left-clicked) opens the context menu at the wrong place if the mouse
        // is moved!
        //ContextFlyout.Placement = FlyoutPlacementMode.Auto;
        //var window = new Window()
        //{
        //};
        //window.Activated += (_, args) =>
        //{
        //    if (args.WindowActivationState == WindowActivationState.Deactivated)
        //    {
        //        window.Close();
        //    }
        //};
        //var handle = WindowNative.GetWindowHandle(window);
        //PInvoke.SetLayeredWindowAttributes(new HWND(handle), 0U, 255, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_ALPHA);

        //var id = Win32Interop.GetWindowIdFromWindow(handle);
        //var appWindow = AppWindow.GetFromWindowId(id);
        //appWindow.MoveAndResize(new RectInt32(cursorPosition.X - 100, cursorPosition.Y - 100, 100, 100));

        //var presenter = appWindow.Presenter as OverlappedPresenter;
        //presenter.IsMaximizable = false;
        //presenter.IsMinimizable = false;
        //presenter.IsResizable = false;
        //presenter.IsAlwaysOnTop = true;
        //presenter.SetBorderAndTitleBar(false, false);

        //var flyout = new MenuFlyout
        //{
        //    Items =
        //    {
        //        new MenuFlyoutItem
        //        {
        //            Text = "Show/Hide Window",
        //        },
        //        new MenuFlyoutSeparator(),
        //        new MenuFlyoutItem
        //        {
        //            Text = "Exit",
        //        },
        //    },
        //};
        //var grid = new Grid
        //{
        //    ContextFlyout = flyout,
        //};
        //window.Content = new Frame
        //{
        //    Content = new Page
        //    {
        //        Content = grid,
        //    },
        //};
        //window.Activate();

        //flyout.Hide();
        //flyout.ShowAt(grid, new FlyoutShowOptions
        //{
        //    Placement = FlyoutPlacementMode.Auto,
        //    Position = new Windows.Foundation.Point(0, 0),
        //    ShowMode = FlyoutShowMode.Auto,
        //});

        using var menu = new PopupMenu();
        foreach(var flyoutItemBase in ((MenuFlyout)ContextFlyout).Items)
        {
            switch (flyoutItemBase)
            {
                case MenuFlyoutItem flyoutItem:
                    {
                        var item = new PopupMenuItem()
                        {   
                            Text = flyoutItem.Text,
                        };
                        item.Click += (_, args) =>
                        {
                            flyoutItem.Command?.TryExecute(flyoutItem.CommandParameter);
                        };
                        menu.Add(item);
                        break;
                    }
                case MenuFlyoutSeparator separator:
                    {
                        menu.Add(new PopupMenuSeparator());
                        break;
                    }
            }
        }

        var handle = MessageWindow.Handle;

        WindowUtilities.SetForegroundWindow(handle);
        menu.Show(
            ownerHandle: handle,
            x: cursorPosition.X,
            y: cursorPosition.Y);
        
        //ContextFlyout.Hide();
        //ContextFlyout.ShowAt(this, new FlyoutShowOptions
        //{
        //    Placement = FlyoutPlacementMode.Auto,
        //    Position = new Windows.Foundation.Point(cursorPosition.X, cursorPosition.Y),
        //    ShowMode = FlyoutShowMode.Auto,
        //});
#endif
    }

    #endregion

    #region Balloon Tips

    /// <summary>
    /// Bubbles events if a balloon ToolTip was displayed
    /// or removed.
    /// </summary>
    /// <param name="visible">Whether the ToolTip was just displayed
    /// or removed.</param>
    /// <param name="sender"></param>
    private void OnBalloonToolTipChanged(object? sender, bool visible)
    {
#if HAS_WPF
        if (visible)
        {
            RaiseTrayBalloonTipShownEvent();
        }
        else
        {
            RaiseTrayBalloonTipClosedEvent();
        }
#endif
    }

    /// <summary>
    /// Displays a balloon notification with the specified title,
    /// text, and predefined icon or custom icon in the taskbar for the specified time period.
    /// </summary>
    /// <param name="title">The title to display on the balloon tip.</param>
    /// <param name="message">The text to display on the balloon tip.</param>
    /// <param name="icon">A symbol that indicates the severity.</param>
    /// <param name="customIcon">A custom icon.</param>
    /// <param name="largeIcon">True to allow large icons (Windows Vista and later).</param>
    /// <param name="sound">If false do not play the associated sound.</param>
    /// <param name="respectQuietTime">
    /// Do not display the balloon notification if the current user is in "quiet time", 
    /// which is the first hour after a new user logs into his or her account for the first time. 
    /// During this time, most notifications should not be sent or shown. 
    /// This lets a user become accustomed to a new computer system without those distractions. 
    /// Quiet time also occurs for each user after an operating system upgrade or clean installation. 
    /// A notification sent with this flag during quiet time is not queued; 
    /// it is simply dismissed unshown. The application can resend the notification later 
    /// if it is still valid at that time. <br/>
    /// Because an application cannot predict when it might encounter quiet time, 
    /// we recommended that this flag always be set on all appropriate notifications 
    /// by any application that means to honor quiet time. <br/>
    /// During quiet time, certain notifications should still be sent because 
    /// they are expected by the user as feedback in response to a user action, 
    /// for instance when he or she plugs in a USB device or prints a document.<br/>
    /// If the current user is not in quiet time, this flag has no effect.
    /// </param>
    /// <param name="realtime">
    /// Windows Vista and later. <br/>
    /// If the balloon notification cannot be displayed immediately, discard it. 
    /// Use this flag for notifications that represent real-time information 
    /// which would be meaningless or misleading if displayed at a later time.  <br/>
    /// For example, a message that states "Your telephone is ringing."
    /// </param>
    /// <param name="timeout">
    /// This member is deprecated as of Windows Vista. <br/>
    /// Notification display times are now based on system accessibility settings. <br/>
    /// The system enforces minimum and maximum timeout values.  <br/>
    /// Values specified in uTimeout that are too large are set to the maximum value. <br/>
    /// Values that are too small default to the minimum value. <br/>
    /// The system minimum and maximum timeout values are currently set at 10 seconds and 30 seconds, respectively.
    /// </param>
#if !HAS_WPF
    [CLSCompliant(false)]
#endif
    public void ShowNotification(
        string title,
        string message,
        NotificationIcon icon = NotificationIcon.None,
        Icon? customIcon = null,
        bool largeIcon = false,
        bool sound = true,
        bool respectQuietTime = true,
        bool realtime = false,
        TimeSpan? timeout = null)
    {
        EnsureNotDisposed();

        TrayIcon.ShowNotification(
            title: title,
            message: message,
            icon: icon,
            customIcon: customIcon?.Handle,
            largeIcon: largeIcon,
            sound: sound,
            respectQuietTime: respectQuietTime,
            realtime: realtime,
            timeout: timeout);
    }

    /// <summary>
    /// Clears all notifications(active and deffered) by recreating tray icon.
    /// https://docs.microsoft.com/en-us/windows/win32/api/shellapi/ns-shellapi-notifyicondataa#nif_info-0x00000010
    /// There's a way to remove notifications without recreating here,
    /// but I haven't been able to get it to work.
    /// </summary>
    /// <returns></returns>
    public void ClearNotifications()
    {
        EnsureNotDisposed();

        TrayIcon.ClearNotifications();
    }

#endregion

    #region Single Click Timer event

    /// <summary>
    /// Performs a delayed action if the user requested an action
    /// based on a single click of the left mouse.<br/>
    /// This method is invoked by the <see cref="singleClickTimer"/>.
    /// </summary>
    private void DoSingleClickAction(object? state)
    {
        if (IsDisposed)
        {
            return;
        }

        // run action
        var action = singleClickTimerAction;
        if (action != null)
        {
            // cleanup action
            singleClickTimerAction = null;

#if HAS_WPF
            // switch to UI thread
            this.GetDispatcher().Invoke(action);
#elif HAS_UNO && !HAS_WINUI
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
#else
            DispatcherQueue.TryEnqueue(() => action());
#endif
        }
    }

    #endregion

    #region Dispose / Exit

    /// <summary>
    /// Set to true as soon as <c>Dispose</c> has been invoked.
    /// </summary>
    public bool IsDisposed { get; private set; }


    /// <summary>
    /// Checks if the object has been disposed and
    /// raises a <see cref="ObjectDisposedException"/> in case
    /// the <see cref="IsDisposed"/> flag is true.
    /// </summary>
    private void EnsureNotDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(Name ?? GetType().FullName);
        }
    }


    /// <summary>
    /// Disposes the class if the application exits.
    /// </summary>
    private void OnExit(object sender, EventArgs e)
    {
        Dispose();
    }


    /// <summary>
    /// This destructor will run only if the <see cref="Dispose()"/>
    /// method does not get called. This gives this base class the
    /// opportunity to finalize.
    /// <para>
    /// Important: Do not provide destructor in types derived from this class.
    /// </para>
    /// </summary>
    ~TaskbarIcon()
    {
        Dispose(false);
    }


    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <remarks>This method is not virtual by design. Derived classes
    /// should override <see cref="Dispose(bool)"/>.
    /// </remarks>
#if HAS_UNO
    public new void Dispose()
#else
    public void Dispose()
#endif
    {
        Dispose(true);

        // This object will be cleaned up by the Dispose method.
        // Therefore, you should call GC.SuppressFinalize to
        // take this object off the finalization queue
        // and prevent finalization code for this object
        // from executing a second time.
        GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Closes the tray and releases all resources.
    /// </summary>
    /// <summary>
    /// <c>Dispose(bool disposing)</c> executes in two distinct scenarios.
    /// If disposing equals <c>true</c>, the method has been called directly
    /// or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed.
    /// </summary>
    /// <param name="disposing">If disposing equals <c>false</c>, the method
    /// has been called by the runtime from inside the finalizer and you
    /// should not reference other objects. Only unmanaged resources can
    /// be disposed.</param>
    /// <remarks>Check the <see cref="IsDisposed"/> property to determine whether
    /// the method has already been called.</remarks>
    protected virtual void Dispose(bool disposing)
    {
        // don't do anything if the component is already disposed
        if (IsDisposed || !disposing)
        {
            return;
        }

        //lock (lockObject)
        {
            IsDisposed = true;

#if HAS_WPF
            // de-register application event listener
            if (Application.Current != null)
            {
                Application.Current.Exit -= OnExit;
            }
#endif

            // stop timers
            singleClickTimer?.Dispose();
#if HAS_WPF
            balloonCloseTimer.Dispose();
#endif

            MessageWindow.Dispose();
            TrayIcon.Dispose();
            Icon?.Dispose();
        }
    }

    #endregion
}
