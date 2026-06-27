using System.Diagnostics.CodeAnalysis;

#if HAS_WINUI
using H.NotifyIcon.Interop;
#endif

namespace H.NotifyIcon;

#if !HAS_MAUI
[DependencyProperty<PopupActivationMode>("PopupActivation", DefaultValue = PopupActivationMode.LeftClick,
    Description = "Defines what mouse events display the TaskbarIconPopup.", Category = CategoryName, ClsCompliant = false)]
[DependencyProperty<UIElement>("TrayPopup",
    Description = "Displayed as a Popup if the user clicks on the taskbar icon.", Category = CategoryName)]
[DependencyProperty<Popup>("TrayPopupResolved", IsReadOnly = true,
    Description = "Returns a Popup which is either the TrayPopup control itself or a Popup control that contains the TrayPopup.", Category = CategoryName)]
[DependencyProperty<PlacementMode>("PopupPlacement",
#if HAS_WPF
    DefaultValue = PlacementMode.AbsolutePoint,
#else
    DefaultValue = PlacementMode.Mouse,
#endif
    Description = "Defines popup placement mode of TaskbarIconPopup.", Category = CategoryName)]
[DependencyProperty<Thickness>("PopupOffset",
    Description = "Defines popup offset of TaskbarIconPopup.", Category = CategoryName)]
[RoutedEvent("TrayPopupOpen", RoutedEventStrategy.Bubble,
    Description = "Bubbled event that occurs when the custom popup is being opened.", Category = CategoryName)]
[RoutedEvent("PreviewTrayPopupOpen", RoutedEventStrategy.Tunnel,
    Description = "Tunneled event that occurs when the custom popup is being opened.", Category = CategoryName)]
[RoutedEvent("PopupOpened", RoutedEventStrategy.Bubble, IsAttached = true, Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Properties

#if HAS_WINUI
    private bool IsTrayPopupVisible { get; set; }
    private bool IsTrayPopupWindowLoaded { get; set; }
    private Window? TrayPopupWindow { get; set; }
    private nint? TrayPopupWindowHandle { get; set; }
    private AppWindow? TrayPopupAppWindow { get; set; }
    private FrameworkElement? TrayPopupWindowRoot { get; set; }
#endif

    #region TrayPopup

    partial void OnTrayPopupChanged(UIElement? oldValue, UIElement? newValue)
    {
#if HAS_WPF
        if (oldValue != null)
        {
            //remove the taskbar icon reference from the previously used element
            SetParentTaskbarIcon(oldValue, null);
        }


        if (newValue != null)
        {
            //set this taskbar icon as a reference to the new tooltip element
            SetParentTaskbarIcon(newValue, this);
        }
#endif

        //create a pop
        CreatePopup();
    }

    #endregion

    #endregion

    #region Methods

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
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(Popup))]
    private void CreatePopup()
    {
#if HAS_WINUI
        CreateTrayPopupWindow();
#else
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

                Placement = PopupPlacement,
                StaysOpen = false,
#endif
                Child = TrayPopup,
            };
        }

        // the popup explicitly gets the DataContext of this instance.
        // If there is no DataContext, the TaskbarIcon assigns itself
        if (popup != null)
        {
            UpdateDataContext(popup, DataContext);
        }

        // store a reference to the used tooltip
        TrayPopupResolved = popup;
#endif
    }

    /// <summary>
    /// Hide the <see cref="TrayPopup"/> control if it was visible.
    /// </summary>
    public void CloseTrayPopup()
    {
        if (IsDisposed)
        {
            return;
        }

        var args = OnPreviewTrayPopupOpen();
#if HAS_WPF
        if (args.Handled)
        {
            return;
        }
#endif

        if (TrayPopup == null)
        {
            return;
        }

#if HAS_WINUI
        CloseTrayPopupWindow();
#else
        if (TrayPopupResolved != null)
        {
            TrayPopupResolved.IsOpen = false;
        }
#endif
    }

    /// <summary>
    /// Displays the <see cref="TrayPopup"/> control if it was set.
    /// </summary>
    public void ShowTrayPopup(System.Drawing.Point cursorPosition)
    {
        if (IsDisposed)
        {
            return;
        }

        // raise preview event no matter whether popup is currently set
        // or not (enables client to set it on demand)
        var args = OnPreviewTrayPopupOpen();
#if HAS_WPF
        if (args.Handled)
        {
            return;
        }
#endif

        if (TrayPopup == null)
        {
            return;
        }
#if HAS_WINUI
        if (!ShowTrayPopupWindow(cursorPosition))
        {
            return;
        }
#else
        if (TrayPopupResolved == null)
        {
            return;
        }

        PlacePopup(cursorPosition);

        // open popup
        TrayPopupResolved.IsOpen = true;

#if HAS_WPF
        var handle = (nint)0;
        if (TrayPopupResolved.Child != null)
        {
            // try to get a handle on the popup itself (via its child)
            if (PresentationSource.FromVisual(TrayPopupResolved.Child) is HwndSource source)
            {
                handle = source.Handle;
            }
        }

        // if we don't have a handle for the popup, fall back to the message sink
        if (handle == 0)
        {
            handle = TrayIcon.WindowHandle;
        }

        // activate either popup or message sink to track deactivation.
        // otherwise, the popup does not close if the user clicks somewhere else
        WindowUtilities.SetForegroundWindow(handle);

        // raise attached event - item should never be null unless developers
        // changed the CustomPopup directly...
        TrayPopup?.RaiseEvent(new RoutedEventArgs(PopupOpenedEvent));
#endif

#endif
        // bubble routed event
        OnTrayPopupOpen();
    }

    private void PlacePopup(System.Drawing.Point cursorPosition)
    {
        if (TrayPopupResolved == null)
        {
            return;
        }

#if HAS_WPF
        TrayPopupResolved.Placement = PopupPlacement;
#endif
        if (PopupPlacement == PlacementMode.Bottom)
        {
            // place popup above system taskbar
            var point = TrayInfo.GetTrayLocation(0);
#if HAS_WPF
            TrayPopupResolved.Placement = PlacementMode.AbsolutePoint;
#endif
            TrayPopupResolved.HorizontalOffset = point.X;
            TrayPopupResolved.VerticalOffset = point.Y;
        }
#if HAS_WPF
        else if (PopupPlacement == PlacementMode.AbsolutePoint)
#else
        else if (PopupPlacement == PlacementMode.Mouse)
#endif
        {
            // place popup near mouse cursor
            TrayPopupResolved.HorizontalOffset = cursorPosition.X;
            TrayPopupResolved.VerticalOffset = cursorPosition.Y;
        }

        TrayPopupResolved.HorizontalOffset += PopupOffset.Left;
        TrayPopupResolved.VerticalOffset += PopupOffset.Top;
    }

#if HAS_WINUI
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(OverlappedPresenter))]
    private void CreateTrayPopupWindow()
    {
        CloseTrayPopupWindow();
        TrayPopupWindow?.Close();
        TrayPopupWindow = null;
        TrayPopupWindowHandle = null;
        TrayPopupAppWindow = null;
        TrayPopupWindowRoot = null;
        TrayPopupResolved = null;
        IsTrayPopupWindowLoaded = false;

        if (TrayPopup == null)
        {
            return;
        }

        if (TrayPopup is FrameworkElement element)
        {
            UpdateDataContext(element, DataContext);
        }

        var root = new Grid
        {
            Background = new SolidColorBrush(Colors.Transparent),
        };
        root.Children.Add(TrayPopup);

        var window = new Window
        {
            Content = root,
        };

        var handle = WindowNative.GetWindowHandle(window);
        if (TrayIcon.WindowHandle != 0)
        {
            HwndUtilities.SetOwnerWindow(handle, TrayIcon.WindowHandle);
        }

#if !HAS_UNO
        var id = Win32Interop.GetWindowIdFromWindow(handle);
        var appWindow = AppWindow.GetFromWindowId(id);
        appWindow.IsShownInSwitchers = false;

        var presenter = (OverlappedPresenter)appWindow.Presenter;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        presenter.IsResizable = false;
        presenter.IsAlwaysOnTop = true;
        presenter.SetBorderAndTitleBar(false, false);
#endif

        root.Loaded += (_, _) =>
        {
            IsTrayPopupWindowLoaded = true;
            HwndUtilities.SetWindowStyleAsPopupWindow(handle);
            _ = WindowUtilities.HideWindow(handle);
        };
        window.Activated += (_, args) =>
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated &&
                IsTrayPopupVisible)
            {
                CloseTrayPopupWindow();
            }
        };

        TrayPopupWindow = window;
        TrayPopupWindowHandle = handle;
#if !HAS_UNO
        TrayPopupAppWindow = appWindow;
#endif
        TrayPopupWindowRoot = root;
    }

    private bool ShowTrayPopupWindow(System.Drawing.Point cursorPosition)
    {
        if (TrayPopupWindow == null ||
            TrayPopupWindowHandle == null ||
            TrayPopupWindowRoot == null)
        {
            return false;
        }

        EnsureTrayPopupWindowLoaded();

        var size = MeasureTrayPopup(TrayPopupWindowRoot);
        var anchor = GetTrayPopupAnchor(cursorPosition);
        var rasterizationScale = TrayPopupWindowRoot.XamlRoot?.RasterizationScale ?? 1.0;
        var rectangle = CursorUtilities.CalculatePopupWindowPosition(
            anchor.X,
            anchor.Y,
            (int)size.Width,
            (int)size.Height,
            CreateTrayPopupExcludeRect(anchor, rasterizationScale));

        IsTrayPopupVisible = true;
        TrayPopupAppWindow?.MoveAndResize(rectangle.ToRectInt32());
        _ = WindowUtilities.ShowWindow(TrayPopupWindowHandle.Value);
        _ = WindowUtilities.SetForegroundWindow(TrayPopupWindowHandle.Value);

        return true;
    }

    private void EnsureTrayPopupWindowLoaded()
    {
        if (IsTrayPopupWindowLoaded ||
            TrayPopupWindow == null ||
            TrayPopupWindowHandle == null)
        {
            return;
        }

        TrayPopupAppWindow?.MoveAndResize(CreateRectInt32(
            x: -32000,
            y: -32000,
            width: 1,
            height: 1));
        TrayPopupWindow.Activate();
        _ = WindowUtilities.HideWindow(TrayPopupWindowHandle.Value);
    }

    private void CloseTrayPopupWindow()
    {
        IsTrayPopupVisible = false;

        if (TrayPopupWindowHandle is { } handle)
        {
            _ = WindowUtilities.HideWindow(handle);
        }
    }

    private static Size MeasureTrayPopup(UIElement popup)
    {
        popup.Measure(new Size(10000.0, 10000.0));

        var scale = popup.XamlRoot?.RasterizationScale ?? 1.0;

        return new Size(
            width: Math.Max(1.0, Math.Ceiling(popup.DesiredSize.Width * scale)),
            height: Math.Max(1.0, Math.Ceiling(popup.DesiredSize.Height * scale)));
    }

    private System.Drawing.Point GetTrayPopupAnchor(System.Drawing.Point cursorPosition)
    {
        var point = PopupPlacement == PlacementMode.Bottom
            ? TrayInfo.GetTrayLocation(0)
            : cursorPosition;

        return new System.Drawing.Point(
            x: point.X + (int)Math.Round(PopupOffset.Left),
            y: point.Y + (int)Math.Round(PopupOffset.Top));
    }

    private static System.Drawing.Rectangle CreateTrayPopupExcludeRect(
        System.Drawing.Point cursorPosition,
        double rasterizationScale)
    {
        var width = Math.Max(1, (int)Math.Round(36 * rasterizationScale));
        var height = Math.Max(1, (int)Math.Round(36 * rasterizationScale));

        return new System.Drawing.Rectangle(
            x: cursorPosition.X - (width / 2),
            y: cursorPosition.Y - (height / 2),
            width: width,
            height: height);
    }

    private static RectInt32 CreateRectInt32(int x, int y, int width, int height)
    {
#if HAS_UNO
        return new RectInt32
        {
            X = x,
            Y = y,
            Width = width,
            Height = height,
        };
#else
        return new RectInt32(
            _X: x,
            _Y: y,
            _Width: width,
            _Height: height);
#endif
    }
#endif

    #endregion
}
#endif
