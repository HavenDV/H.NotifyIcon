namespace H.NotifyIcon;

[RoutedEvent("TrayKeyboardContextMenu", RoutedEventStrategy.Bubble,
    Description = "Occurs when the user moves the mouse over the taskbar icon.", Category = CategoryName)]
[RoutedEvent("TrayKeyboardKeySelect", RoutedEventStrategy.Bubble,
    Description = "Occurs when the user moves the mouse over the taskbar icon.", Category = CategoryName)]
[RoutedEvent("TrayKeyboardSelect", RoutedEventStrategy.Bubble,
    Description = "Occurs when the user moves the mouse over the taskbar icon.", Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Event handlers

    /// <summary>
    /// Processes keyboard events, which are bubbled
    /// through the class' routed events, trigger
    /// certain actions (e.g. show a popup), or
    /// both.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args">Keyboard event args</param>
    private void OnKeyboardEvent(object? sender, KeyboardTrayIconEventArgs args)
    {
        if (IsDisposed)
        {
            return;
        }

        switch (args.KeyboardEvent)
        {
            case KeyboardEvent.ContextMenu:
                OnTrayKeyboardContextMenu();
                break;

            case KeyboardEvent.KeySelect:
                OnTrayKeyboardKeySelect();
                break;

            case KeyboardEvent.Select:
                OnTrayKeyboardSelect();
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(args),
                    $"Missing handler for keyboard event flag: {args.KeyboardEvent}");
        }
    }

    #endregion
}
