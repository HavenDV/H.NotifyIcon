using H.NotifyIcon.Core;

namespace H.NotifyIcon;

/// <inheritdoc/>
public partial class TaskbarIcon
{
    #region Events

#if HAS_WPF

    #region TrayKeyboardContextMenu

    /// <summary>Identifies the <see cref="TrayKeyboardContextMenu"/> routed event.</summary>
    public static readonly RoutedEvent TrayKeyboardContextMenuEvent =
        EventManager.RegisterRoutedEvent(
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
        return this.RaiseRoutedEvent(new RoutedEventArgs(TrayKeyboardContextMenuEvent));
    }

    #endregion

    #region TrayKeyboardKeySelect

    /// <summary>Identifies the <see cref="TrayKeyboardKeySelect"/> routed event.</summary>
    public static readonly RoutedEvent TrayKeyboardKeySelectEvent =
        EventManager.RegisterRoutedEvent(
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
        return this.RaiseRoutedEvent(new RoutedEventArgs(TrayKeyboardKeySelectEvent));
    }

    #endregion

    #region TrayKeyboardSelect

    /// <summary>Identifies the <see cref="TrayKeyboardSelect"/> routed event.</summary>
    public static readonly RoutedEvent TrayKeyboardSelectEvent =
        EventManager.RegisterRoutedEvent(
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
        return this.RaiseRoutedEvent(new RoutedEventArgs(TrayKeyboardSelectEvent));
    }

    #endregion

#endif

    #endregion

    #region Event handlers

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
}
