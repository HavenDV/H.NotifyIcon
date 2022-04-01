namespace H.NotifyIcon.Core;

/// <inheritdoc/>
public abstract class PopupItem
{
    /// <inheritdoc/>
    public int? Id { get; set; }

    internal abstract MENU_ITEM_FLAGS NativeFlags { get; }

    internal abstract PCWSTR NativeHandle { get; }

    /// <inheritdoc/>
    public bool Visible { get; set; } = true;
}
