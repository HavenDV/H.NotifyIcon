namespace H.NotifyIcon.Core;

/// <summary>
/// MouseEvent extensions.
/// </summary>
public static class MouseEventExtensions
{
    private static bool OneOf<T>(this T value, params T[] values)
    {
        value = value ?? throw new ArgumentNullException(nameof(value));

        return values.Contains(value);
    }

    /// <summary>
    /// Checks if a given <see cref="PopupActivationMode"/> is a match for
    /// an effectively pressed mouse button.
    /// </summary>
    public static bool IsMatch(
        this MouseEvent @event,
        PopupActivationMode activationMode)
    {
        return activationMode switch
        {
            PopupActivationMode.LeftClick => @event == MouseEvent.IconLeftMouseUp,
            PopupActivationMode.RightClick => @event == MouseEvent.IconRightMouseUp,
            PopupActivationMode.LeftOrRightClick => @event.OneOf(MouseEvent.IconLeftMouseUp, MouseEvent.IconRightMouseUp),
            PopupActivationMode.LeftOrDoubleClick => @event.OneOf(MouseEvent.IconLeftMouseUp, MouseEvent.IconDoubleClick),
            PopupActivationMode.DoubleClick => @event.OneOf(MouseEvent.IconDoubleClick),
            PopupActivationMode.MiddleClick => @event == MouseEvent.IconMiddleMouseUp,
            PopupActivationMode.All => @event != MouseEvent.MouseMove,
            PopupActivationMode.None => false,
            _ => throw new ArgumentOutOfRangeException(nameof(activationMode)),
        };
    }
}
