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

    #region TrayToolTipOpen (and PreviewTrayToolTipOpen)

    /// <summary>Identifies the <see cref="TrayToolTipOpen"/> routed event.</summary>
    public static readonly RoutedEvent TrayToolTipOpenEvent = EventManager.RegisterRoutedEvent(nameof(TrayToolTipOpen),
        RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

    /// <summary>
    /// Bubbled event that occurs when the custom ToolTip is being displayed.
    /// </summary>
    public event RoutedEventHandler TrayToolTipOpen
    {
        add { AddHandler(TrayToolTipOpenEvent, value); }
        remove { RemoveHandler(TrayToolTipOpenEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayToolTipOpen event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayToolTipOpenEvent()
    {
        return RaiseTrayToolTipOpenEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the TrayToolTipOpen event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayToolTipOpenEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayToolTipOpenEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    /// <summary>Identifies the <see cref="PreviewTrayToolTipOpen"/> routed event.</summary>
    public static readonly RoutedEvent PreviewTrayToolTipOpenEvent =
        EventManager.RegisterRoutedEvent(nameof(PreviewTrayToolTipOpen),
            RoutingStrategy.Tunnel, typeof (RoutedEventHandler), typeof (TaskbarIcon));

    /// <summary>
    /// Tunneled event that occurs when the custom ToolTip is being displayed.
    /// </summary>
    public event RoutedEventHandler PreviewTrayToolTipOpen
    {
        add { AddHandler(PreviewTrayToolTipOpenEvent, value); }
        remove { RemoveHandler(PreviewTrayToolTipOpenEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the PreviewTrayToolTipOpen event.
    /// </summary>
    protected RoutedEventArgs RaisePreviewTrayToolTipOpenEvent()
    {
        return RaisePreviewTrayToolTipOpenEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the PreviewTrayToolTipOpen event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaisePreviewTrayToolTipOpenEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(PreviewTrayToolTipOpenEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region TrayToolTipClose (and PreviewTrayToolTipClose)

    /// <summary>Identifies the <see cref="TrayToolTipClose"/> routed event.</summary>
    public static readonly RoutedEvent TrayToolTipCloseEvent = EventManager.RegisterRoutedEvent(nameof(TrayToolTipClose),
        RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

    /// <summary>
    /// Bubbled event that occurs when a custom tooltip is being closed.
    /// </summary>
    public event RoutedEventHandler TrayToolTipClose
    {
        add { AddHandler(TrayToolTipCloseEvent, value); }
        remove { RemoveHandler(TrayToolTipCloseEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayToolTipClose event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayToolTipCloseEvent()
    {
        return RaiseTrayToolTipCloseEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the TrayToolTipClose event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseTrayToolTipCloseEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(TrayToolTipCloseEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    /// <summary>Identifies the <see cref="PreviewTrayToolTipClose"/> routed event.</summary>
    public static readonly RoutedEvent PreviewTrayToolTipCloseEvent =
        EventManager.RegisterRoutedEvent(nameof(PreviewTrayToolTipClose),
            RoutingStrategy.Tunnel, typeof (RoutedEventHandler), typeof (TaskbarIcon));

    /// <summary>
    /// Tunneled event that occurs when a custom tooltip is being closed.
    /// </summary>
    public event RoutedEventHandler PreviewTrayToolTipClose
    {
        add { AddHandler(PreviewTrayToolTipCloseEvent, value); }
        remove { RemoveHandler(PreviewTrayToolTipCloseEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the PreviewTrayToolTipClose event.
    /// </summary>
    protected RoutedEventArgs RaisePreviewTrayToolTipCloseEvent()
    {
        return RaisePreviewTrayToolTipCloseEvent(this);
    }

    /// <summary>
    /// A static helper method to raise the PreviewTrayToolTipClose event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaisePreviewTrayToolTipCloseEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(PreviewTrayToolTipCloseEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    //ATTACHED EVENTS

    #region ToolTipOpened

    /// <summary>
    /// ToolTipOpened Attached Routed Event
    /// </summary>
    public static readonly RoutedEvent ToolTipOpenedEvent = EventManager.RegisterRoutedEvent("ToolTipOpened",
        RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

    /// <summary>
    /// Adds a handler for the ToolTipOpened attached event
    /// </summary>
    /// <param name="element">UIElement or ContentElement that listens to the event</param>
    /// <param name="handler">Event handler to be added</param>
    public static void AddToolTipOpenedHandler(DependencyObject element, RoutedEventHandler handler)
    {
        RoutedEventHelper.AddHandler(element, ToolTipOpenedEvent, handler);
    }

    /// <summary>
    /// Removes a handler for the ToolTipOpened attached event
    /// </summary>
    /// <param name="element">UIElement or ContentElement that listens to the event</param>
    /// <param name="handler">Event handler to be removed</param>
    public static void RemoveToolTipOpenedHandler(DependencyObject element, RoutedEventHandler handler)
    {
        RoutedEventHelper.RemoveHandler(element, ToolTipOpenedEvent, handler);
    }

    /// <summary>
    /// A static helper method to raise the ToolTipOpened event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseToolTipOpenedEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(ToolTipOpenedEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

    #region ToolTipClose

    /// <summary>
    /// ToolTipClose Attached Routed Event
    /// </summary>
    public static readonly RoutedEvent ToolTipCloseEvent = EventManager.RegisterRoutedEvent("ToolTipClose",
        RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TaskbarIcon));

    /// <summary>
    /// Adds a handler for the ToolTipClose attached event
    /// </summary>
    /// <param name="element">UIElement or ContentElement that listens to the event</param>
    /// <param name="handler">Event handler to be added</param>
    public static void AddToolTipCloseHandler(DependencyObject element, RoutedEventHandler handler)
    {
        RoutedEventHelper.AddHandler(element, ToolTipCloseEvent, handler);
    }

    /// <summary>
    /// Removes a handler for the ToolTipClose attached event
    /// </summary>
    /// <param name="element">UIElement or ContentElement that listens to the event</param>
    /// <param name="handler">Event handler to be removed</param>
    public static void RemoveToolTipCloseHandler(DependencyObject element, RoutedEventHandler handler)
    {
        RoutedEventHelper.RemoveHandler(element, ToolTipCloseEvent, handler);
    }

    /// <summary>
    /// A static helper method to raise the ToolTipClose event on a target element.
    /// </summary>
    /// <param name="target">UIElement or ContentElement on which to raise the event</param>
    internal static RoutedEventArgs RaiseToolTipCloseEvent(DependencyObject target)
    {
        var args = new RoutedEventArgs(ToolTipCloseEvent);
        RoutedEventHelper.RaiseEvent(target, args);
        return args;
    }

    #endregion

#endif
}
