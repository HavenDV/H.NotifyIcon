namespace H.NotifyIcon.Core;

/// <inheritdoc/>
public abstract class PopupItem
{
    /// <inheritdoc/>
    public int? Id { get; set; }

    /// <inheritdoc/>
    public bool Visible { get; set; } = true;
}
