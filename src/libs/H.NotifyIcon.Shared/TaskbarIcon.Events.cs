namespace H.NotifyIcon;

/// <summary>
/// Contains declarations of WPF dependency properties
/// and events.
/// </summary>
public partial class TaskbarIcon
{
#if HAS_WPF

    //EVENTS

    #region TrayLeftMouseDown

    /// <summary>Identifies the <see cref="TrayLeftMouseDown"/> routed event.</summary>
    public static readonly RoutedEvent TrayLeftMouseDownEvent = EventManager.RegisterRoutedEvent(
        nameof(TrayLeftMouseDown),
        RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

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
        var args = RaiseTrayLeftMouseDownEvent(this);
        return args;
    }

    /// <summary>
    /// A static helper method to raise the TrayLeftMouseDown event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayLeftMouseDownEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayLeftMouseDownEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region TrayRightMouseDown

    /// <summary>Identifies the <see cref="TrayRightMouseDown"/> routed event.</summary>
    public static readonly RoutedEvent TrayRightMouseDownEvent =
        EventManager.RegisterRoutedEvent(nameof(TrayRightMouseDown),
            RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

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
        return RaiseTrayRightMouseDownEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the TrayRightMouseDown event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayRightMouseDownEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayRightMouseDownEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region TrayMiddleMouseDown

    /// <summary>Identifies the <see cref="TrayMiddleMouseDown"/> routed event.</summary>
    public static readonly RoutedEvent TrayMiddleMouseDownEvent =
        EventManager.RegisterRoutedEvent(nameof(TrayMiddleMouseDown),
            RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

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
        return RaiseTrayMiddleMouseDownEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the TrayMiddleMouseDown event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayMiddleMouseDownEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayMiddleMouseDownEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region TrayLeftMouseUp

    /// <summary>Identifies the <see cref="TrayLeftMouseUp"/> routed event.</summary>
    public static readonly RoutedEvent TrayLeftMouseUpEvent = EventManager.RegisterRoutedEvent(nameof(TrayLeftMouseUp),
        RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

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
        return RaiseTrayLeftMouseUpEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the TrayLeftMouseUp event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayLeftMouseUpEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayLeftMouseUpEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region TrayRightMouseUp

    /// <summary>Identifies the <see cref="TrayRightMouseUp"/> routed event.</summary>
    public static readonly RoutedEvent TrayRightMouseUpEvent = EventManager.RegisterRoutedEvent(nameof(TrayRightMouseUp),
        RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

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
        return RaiseTrayRightMouseUpEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the TrayRightMouseUp event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayRightMouseUpEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayRightMouseUpEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region TrayMiddleMouseUp

    /// <summary>Identifies the <see cref="TrayMiddleMouseUp"/> routed event.</summary>
    public static readonly RoutedEvent TrayMiddleMouseUpEvent = EventManager.RegisterRoutedEvent(
        nameof(TrayMiddleMouseUp),
        RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

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
        return RaiseTrayMiddleMouseUpEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the TrayMiddleMouseUp event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayMiddleMouseUpEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayMiddleMouseUpEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region TrayMouseDoubleClick

    /// <summary>Identifies the <see cref="TrayMouseDoubleClick"/> routed event.</summary>
    public static readonly RoutedEvent TrayMouseDoubleClickEvent =
        EventManager.RegisterRoutedEvent(nameof(TrayMouseDoubleClick),
            RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

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
        var args = RaiseTrayMouseDoubleClickEvent(this);
        DoubleClickCommand?.TryExecute(DoubleClickCommandParameter, DoubleClickCommandTarget ?? this);
        return args;
    }

    /// <summary>
    /// A static helper method to raise the TrayMouseDoubleClick event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayMouseDoubleClickEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayMouseDoubleClickEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region TrayMouseMove

    /// <summary>Identifies the <see cref="TrayMouseMove"/> routed event.</summary>
    public static readonly RoutedEvent TrayMouseMoveEvent = EventManager.RegisterRoutedEvent(nameof(TrayMouseMove),
        RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

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
        return RaiseTrayMouseMoveEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the TrayMouseMove event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayMouseMoveEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayMouseMoveEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region TrayKeyboardContextMenu

    /// <summary>Identifies the <see cref="TrayKeyboardContextMenu"/> routed event.</summary>
    public static readonly RoutedEvent TrayKeyboardContextMenuEvent = EventManager.RegisterRoutedEvent(
        nameof(TrayKeyboardContextMenu),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(TaskbarIcon));

    /// <summary>
    /// Occurs when the user moves the mouse over the taskbar icon.
    /// </summary>
    public event RoutedEventHandler TrayKeyboardContextMenu
    {
        add { AddHandler(TrayKeyboardContextMenuEvent, value); }
        remove { RemoveHandler(TrayKeyboardContextMenuEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayKeyboardContextMenu event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayKeyboardContextMenuEvent()
    {
        return RaiseTrayKeyboardContextMenuEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the TrayKeyboardContextMenu event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayKeyboardContextMenuEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayKeyboardContextMenuEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region TrayKeyboardKeySelect

    /// <summary>Identifies the <see cref="TrayKeyboardKeySelect"/> routed event.</summary>
    public static readonly RoutedEvent TrayKeyboardKeySelectEvent = EventManager.RegisterRoutedEvent(
        nameof(TrayKeyboardKeySelect),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(TaskbarIcon));

    /// <summary>
    /// Occurs when the user moves the mouse over the taskbar icon.
    /// </summary>
    public event RoutedEventHandler TrayKeyboardKeySelect
    {
        add { AddHandler(TrayKeyboardKeySelectEvent, value); }
        remove { RemoveHandler(TrayKeyboardKeySelectEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayKeyboardKeySelect event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayKeyboardKeySelectEvent()
    {
        return RaiseTrayKeyboardKeySelectEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the TrayKeyboardKeySelect event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayKeyboardKeySelectEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayKeyboardKeySelectEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region TrayKeyboardSelect

    /// <summary>Identifies the <see cref="TrayKeyboardSelect"/> routed event.</summary>
    public static readonly RoutedEvent TrayKeyboardSelectEvent = EventManager.RegisterRoutedEvent(
        nameof(TrayKeyboardSelect),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(TaskbarIcon));

    /// <summary>
    /// Occurs when the user moves the mouse over the taskbar icon.
    /// </summary>
    public event RoutedEventHandler TrayKeyboardSelect
    {
        add { AddHandler(TrayKeyboardSelectEvent, value); }
        remove { RemoveHandler(TrayKeyboardSelectEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayKeyboardSelect event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayKeyboardSelectEvent()
    {
        return RaiseTrayKeyboardSelectEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the TrayKeyboardSelect event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayKeyboardSelectEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayKeyboardSelectEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion


#endif
}
