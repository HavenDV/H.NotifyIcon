namespace H.NotifyIcon;

/// <inheritdoc/>
public partial class TaskbarIcon
{
    #region Events

#if HAS_WPF

    #region TrayLeftMouseDown

    /// <summary>Identifies the <see cref="TrayLeftMouseDown"/> routed event.</summary>
    public static readonly RoutedEvent TrayLeftMouseDownEvent =
        EventManager.RegisterRoutedEvent(
            nameof(TrayLeftMouseDown),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TaskbarIcon));

    /// <summary>
    /// Occurs when the user presses the left mouse button.
    /// </summary>
    [Category(CategoryName)]
    public event RoutedEventHandler TrayLeftMouseDown
    {
        add { AddHandler(TrayLeftMouseDownEvent, value); }
        remove { RemoveHandler(TrayLeftMouseDownEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayLeftMouseDown event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayLeftMouseDownEvent()
    {
        return this.RaiseRoutedEvent(new RoutedEventArgs(TrayLeftMouseDownEvent));
    }

    #endregion

    #region TrayRightMouseDown

    /// <summary>Identifies the <see cref="TrayRightMouseDown"/> routed event.</summary>
    public static readonly RoutedEvent TrayRightMouseDownEvent =
        EventManager.RegisterRoutedEvent(
            nameof(TrayRightMouseDown),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TaskbarIcon));

    /// <summary>
    /// Occurs when the presses the right mouse button.
    /// </summary>
    public event RoutedEventHandler TrayRightMouseDown
    {
        add { AddHandler(TrayRightMouseDownEvent, value); }
        remove { RemoveHandler(TrayRightMouseDownEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayRightMouseDown event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayRightMouseDownEvent()
    {
        return this.RaiseRoutedEvent(new RoutedEventArgs(TrayRightMouseDownEvent));
    }

    #endregion

    #region TrayMiddleMouseDown

    /// <summary>Identifies the <see cref="TrayMiddleMouseDown"/> routed event.</summary>
    public static readonly RoutedEvent TrayMiddleMouseDownEvent =
        EventManager.RegisterRoutedEvent(
            nameof(TrayMiddleMouseDown),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TaskbarIcon));

    /// <summary>
    /// Occurs when the user presses the middle mouse button.
    /// </summary>
    public event RoutedEventHandler TrayMiddleMouseDown
    {
        add { AddHandler(TrayMiddleMouseDownEvent, value); }
        remove { RemoveHandler(TrayMiddleMouseDownEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayMiddleMouseDown event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayMiddleMouseDownEvent()
    {
        return this.RaiseRoutedEvent(new RoutedEventArgs(TrayMiddleMouseDownEvent));
    }

    #endregion

    #region TrayLeftMouseUp

    /// <summary>Identifies the <see cref="TrayLeftMouseUp"/> routed event.</summary>
    public static readonly RoutedEvent TrayLeftMouseUpEvent =
        EventManager.RegisterRoutedEvent(
            nameof(TrayLeftMouseUp),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TaskbarIcon));

    /// <summary>
    /// Occurs when the user releases the left mouse button.
    /// </summary>
    public event RoutedEventHandler TrayLeftMouseUp
    {
        add { AddHandler(TrayLeftMouseUpEvent, value); }
        remove { RemoveHandler(TrayLeftMouseUpEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayLeftMouseUp event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayLeftMouseUpEvent()
    {
        return this.RaiseRoutedEvent(new RoutedEventArgs(TrayLeftMouseUpEvent));
    }

    #endregion

    #region TrayRightMouseUp

    /// <summary>Identifies the <see cref="TrayRightMouseUp"/> routed event.</summary>
    public static readonly RoutedEvent TrayRightMouseUpEvent =
        EventManager.RegisterRoutedEvent(
            nameof(TrayRightMouseUp),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TaskbarIcon));

    /// <summary>
    /// Occurs when the user releases the right mouse button.
    /// </summary>
    public event RoutedEventHandler TrayRightMouseUp
    {
        add { AddHandler(TrayRightMouseUpEvent, value); }
        remove { RemoveHandler(TrayRightMouseUpEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayRightMouseUp event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayRightMouseUpEvent()
    {
        return this.RaiseRoutedEvent(new RoutedEventArgs(TrayRightMouseUpEvent));
    }

    #endregion

    #region TrayMiddleMouseUp

    /// <summary>Identifies the <see cref="TrayMiddleMouseUp"/> routed event.</summary>
    public static readonly RoutedEvent TrayMiddleMouseUpEvent =
        EventManager.RegisterRoutedEvent(
            nameof(TrayMiddleMouseUp),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TaskbarIcon));

    /// <summary>
    /// Occurs when the user releases the middle mouse button.
    /// </summary>
    public event RoutedEventHandler TrayMiddleMouseUp
    {
        add { AddHandler(TrayMiddleMouseUpEvent, value); }
        remove { RemoveHandler(TrayMiddleMouseUpEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayMiddleMouseUp event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayMiddleMouseUpEvent()
    {
        return this.RaiseRoutedEvent(new RoutedEventArgs(TrayMiddleMouseUpEvent));
    }

    #endregion

    #region TrayMouseDoubleClick

    /// <summary>Identifies the <see cref="TrayMouseDoubleClick"/> routed event.</summary>
    public static readonly RoutedEvent TrayMouseDoubleClickEvent =
        EventManager.RegisterRoutedEvent(
            nameof(TrayMouseDoubleClick),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TaskbarIcon));

    /// <summary>
    /// Occurs when the user double-clicks the taskbar icon.
    /// </summary>
    public event RoutedEventHandler TrayMouseDoubleClick
    {
        add { AddHandler(TrayMouseDoubleClickEvent, value); }
        remove { RemoveHandler(TrayMouseDoubleClickEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayMouseDoubleClick event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayMouseDoubleClickEvent()
    {
        var args = this.RaiseRoutedEvent(new RoutedEventArgs(TrayMouseDoubleClickEvent));

        DoubleClickCommand?.TryExecute(DoubleClickCommandParameter, DoubleClickCommandTarget ?? this);

        return args;
    }

    #endregion

    #region TrayMouseMove

    /// <summary>Identifies the <see cref="TrayMouseMove"/> routed event.</summary>
    public static readonly RoutedEvent TrayMouseMoveEvent =
        EventManager.RegisterRoutedEvent(
            nameof(TrayMouseMove),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TaskbarIcon));

    /// <summary>
    /// Occurs when the user moves the mouse over the taskbar icon.
    /// </summary>
    public event RoutedEventHandler TrayMouseMove
    {
        add { AddHandler(TrayMouseMoveEvent, value); }
        remove { RemoveHandler(TrayMouseMoveEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayMouseMove event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayMouseMoveEvent()
    {
        return this.RaiseRoutedEvent(new RoutedEventArgs(TrayMouseMoveEvent));
    }

    #endregion

#endif

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
    private void OnMouseEvent(object? sender, MouseTrayIconEventArgs args)
    {
        if (IsDisposed)
        {
            return;
        }

        switch (args.MouseEvent)
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
