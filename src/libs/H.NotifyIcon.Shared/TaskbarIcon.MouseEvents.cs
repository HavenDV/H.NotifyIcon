namespace H.NotifyIcon;

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
    #region Event handlers

    /// <summary>
    /// Processes mouse events, which are bubbled
    /// through the class' routed events, trigger
    /// certain actions (e.g. show a popup), or
    /// both.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args">Mouse event args.</param>
    private void OnMouseEvent(object? sender, MouseTrayIconEventArgs args)
    {
        if (IsDisposed)
        {
            return;
        }

        switch (args.MouseEvent)
        {
            case MouseEvent.MouseMove:
                OnTrayMouseMove();
                // immediately return - there's nothing left to evaluate
                return;
            case MouseEvent.IconRightMouseDown:
                OnTrayRightMouseDown();
                break;
            case MouseEvent.IconLeftMouseDown:
                OnTrayLeftMouseDown();
                break;
            case MouseEvent.IconRightMouseUp:
                OnTrayRightMouseUp();
                break;
            case MouseEvent.IconLeftMouseUp:
                OnTrayLeftMouseUp();
                break;
            case MouseEvent.IconMiddleMouseDown:
                OnTrayMiddleMouseDown();
                break;
            case MouseEvent.IconMiddleMouseUp:
                OnTrayMiddleMouseUp();
                break;
            case MouseEvent.IconDoubleClick:
                // cancel single click timer
                singleClickTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                // bubble event
                OnTrayMouseDoubleClick();
#if HAS_WPF
                DoubleClickCommand?.TryExecute(DoubleClickCommandParameter, DoubleClickCommandTarget ?? this);
#else
                DoubleClickCommand?.TryExecute(DoubleClickCommandParameter);
#endif
                break;
            case MouseEvent.BalloonToolTipClicked:
                OnTrayBalloonTipClicked();
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
        if (args.MouseEvent.IsMatch(MenuActivation))
        {
            if (args.MouseEvent == MouseEvent.IconLeftMouseUp)
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
        if (args.MouseEvent == MouseEvent.IconLeftMouseUp && !isLeftClickCommandInvoked)
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
}
