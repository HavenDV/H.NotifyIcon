namespace H.NotifyIcon;

[DependencyProperty<ICommand>("DoubleClickCommand",
    Description = "A command that is being executed if the tray icon is being double-clicked.", Category = CategoryName)]
[DependencyProperty<object>("DoubleClickCommandParameter",
    Description = "Parameter to submit to the DoubleClickCommand when the user double clicks on the NotifyIcon.", Category = CategoryName)]
[DependencyProperty<ICommand>("LeftClickCommand",
    Description = "A command that is being executed if the tray icon is being left-clicked.", Category = CategoryName)]
[DependencyProperty<object>("LeftClickCommandParameter",
    Description = "The target of the command that is fired if the notify icon is clicked with the left mouse button.", Category = CategoryName)]
[DependencyProperty<ICommand>("RightClickCommand",
    Description = "A command that is being executed if the tray icon is being right-clicked.", Category = CategoryName)]
[DependencyProperty<object>("RightClickCommandParameter",
    Description = "The target of the command that is fired if the notify icon is clicked with the right mouse button.", Category = CategoryName)]
[DependencyProperty<bool>("NoLeftClickDelay",
    Description = "Set to true to make left clicks work without delay.", Category = CategoryName)]
#if HAS_WPF
[DependencyProperty<IInputElement>("DoubleClickCommandTarget",
    Description = "The target of the command that is fired if the notify icon is double clicked.", Category = CategoryName)]
[DependencyProperty<IInputElement>("LeftClickCommandTarget",
    Description = "The target of the command that is fired if the notify icon is clicked with the left mouse button.", Category = CategoryName)]
[DependencyProperty<IInputElement>("RightClickCommandTarget",
    Description = "The target of the command that is fired if the notify icon is clicked with the right mouse button.", Category = CategoryName)]
#endif
[RoutedEvent("TrayLeftMouseDown", RoutedEventStrategy.Bubble,
    Description = "Occurs when the user presses the left mouse button.", Category = CategoryName)]
[RoutedEvent("TrayRightMouseDown", RoutedEventStrategy.Bubble,
    Description = "Occurs when the presses the right mouse button.", Category = CategoryName)]
[RoutedEvent("TrayMiddleMouseDown", RoutedEventStrategy.Bubble,
    Description = "Occurs when the user presses the middle mouse button.", Category = CategoryName)]
[RoutedEvent("TrayLeftMouseUp", RoutedEventStrategy.Bubble,
    Description = "Occurs when the user releases the left mouse button.", Category = CategoryName)]
[RoutedEvent("TrayRightMouseUp", RoutedEventStrategy.Bubble,
    Description = "Occurs when the user releases the right mouse button.", Category = CategoryName)]
[RoutedEvent("TrayMiddleMouseUp", RoutedEventStrategy.Bubble,
    Description = "Occurs when the user releases the middle mouse button.", Category = CategoryName)]
[RoutedEvent("TrayMouseDoubleClick", RoutedEventStrategy.Bubble,
    Description = "Occurs when the user double-clicks the taskbar icon.", Category = CategoryName)]
[RoutedEvent("TrayMouseMove", RoutedEventStrategy.Bubble,
    Description = "Occurs when the user moves the mouse over the taskbar icon.", Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Properties

    /// <summary>
    /// An action that is being invoked if the
    /// <see cref="SingleClickTimer"/> fires.
    /// </summary>
    private Action? SingleClickTimerAction { get; set; }

    /// <summary>
    /// A timer that is used to differentiate between single
    /// and double clicks.
    /// </summary>
    private Timer SingleClickTimer { get; }

    /// <summary>
    /// The time we should wait for a double click.
    /// </summary>
    private int DoubleClickWaitTime => NoLeftClickDelay ? 0 : CursorUtilities.GetDoubleClickTime();

    #endregion

    #region Event handlers

    /// <summary>
    /// Processes mouse events, which are bubbled
    /// through the class' routed events, trigger
    /// certain actions (e.g. show a popup), or
    /// both.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args">Mouse event args.</param>
    private void OnMouseEvent(object? sender, MessageWindow.MouseEventReceivedEventArgs args)
    {
        if (IsDisposed)
        {
            return;
        }

        switch (args.MouseEvent)
        {
            case MouseEvent.MouseMove:
                _ = OnTrayMouseMove();
                // immediately return - there's nothing left to evaluate
                return;
            case MouseEvent.IconRightMouseDown:
                _ = OnTrayRightMouseDown();
                break;
            case MouseEvent.IconLeftMouseDown:
                _ = OnTrayLeftMouseDown();
                break;
            case MouseEvent.IconRightMouseUp:
                _ = OnTrayRightMouseUp();
#if HAS_WPF
                RightClickCommand?.TryExecute(RightClickCommandParameter, RightClickCommandTarget ?? this);
#else
                RightClickCommand?.TryExecute(RightClickCommandParameter);
#endif
                break;
            case MouseEvent.IconLeftMouseUp:
                _ = OnTrayLeftMouseUp();
                break;
            case MouseEvent.IconMiddleMouseDown:
                _ = OnTrayMiddleMouseDown();
                break;
            case MouseEvent.IconMiddleMouseUp:
                _ = OnTrayMiddleMouseUp();
                break;
            case MouseEvent.IconDoubleClick:
                // cancel single click timer
                SingleClickTimer.Change(Timeout.Infinite, Timeout.Infinite);
                // bubble event
                _ = OnTrayMouseDoubleClick();
#if HAS_WPF
                DoubleClickCommand?.TryExecute(DoubleClickCommandParameter, DoubleClickCommandTarget ?? this);
#else
                DoubleClickCommand?.TryExecute(DoubleClickCommandParameter);
#endif
                break;
            case MouseEvent.BalloonToolTipClicked:
                _ = OnTrayBalloonTipClicked();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(args), "Missing handler for mouse event flag: " + args.MouseEvent);
        }

        var cursorPosition = args.Point.ScaleWithDpi();
        var isLeftClickCommandInvoked = false;

        // show popup, if requested
        if (args.MouseEvent.IsMatch(PopupActivation))
        {
            if (args.MouseEvent == MouseEvent.IconLeftMouseUp)
            {
                // show popup once we are sure it's not a double click
                SingleClickTimerAction = () =>
                {
#if HAS_WPF
                    LeftClickCommand?.TryExecute(LeftClickCommandParameter, LeftClickCommandTarget ?? this);
#else
                    LeftClickCommand?.TryExecute(LeftClickCommandParameter);
#endif
                    ShowTrayPopup(cursorPosition);
                };
                SingleClickTimer.Change(DoubleClickWaitTime, Timeout.Infinite);
                isLeftClickCommandInvoked = true;
            }
            else
            {
                // show popup immediately
                ShowTrayPopup(cursorPosition);
            }
        }


        // show context menu, if requested
        if (args.MouseEvent.IsMatch(MenuActivation))
        {
            if (args.MouseEvent == MouseEvent.IconLeftMouseUp)
            {
                // show context menu once we are sure it's not a double click
                SingleClickTimerAction = () =>
                {
#if HAS_WPF
                    LeftClickCommand?.TryExecute(LeftClickCommandParameter, LeftClickCommandTarget ?? this);
#else
                    LeftClickCommand?.TryExecute(LeftClickCommandParameter);
#endif
                    ShowContextMenu(cursorPosition);
                };
                SingleClickTimer.Change(DoubleClickWaitTime, Timeout.Infinite);
                isLeftClickCommandInvoked = true;
            }
            else
            {
                // show context menu immediately
                ShowContextMenu(cursorPosition);
            }
        }

        // make sure the left click command is invoked on mouse clicks
        if (args.MouseEvent == MouseEvent.IconLeftMouseUp && !isLeftClickCommandInvoked)
        {
            // show context menu once we are sure it's not a double click
            SingleClickTimerAction = () =>
            {
#if HAS_WPF
                LeftClickCommand?.TryExecute(LeftClickCommandParameter, LeftClickCommandTarget ?? this);
#else
                LeftClickCommand?.TryExecute(LeftClickCommandParameter);
#endif
            };
            SingleClickTimer.Change(DoubleClickWaitTime, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Performs a delayed action if the user requested an action
    /// based on a single click of the left mouse.<br/>
    /// This method is invoked by the <see cref="SingleClickTimer"/>.
    /// </summary>
    private void DoSingleClickAction(object? state)
    {
        if (IsDisposed)
        {
            return;
        }

        // run action
        var action = SingleClickTimerAction;
        if (action != null)
        {
            // cleanup action
            SingleClickTimerAction = null;

#if HAS_WPF
            // switch to UI thread
            this.GetDispatcher().Invoke(action);
#elif HAS_UNO && (!HAS_WINUI && !HAS_UNO_WINUI)
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
#else
            DispatcherQueue.TryEnqueue(() => action());
#endif
        }
    }

    #endregion
}
