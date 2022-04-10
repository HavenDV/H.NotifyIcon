namespace H.NotifyIcon.Core;

/// <summary>
/// The visibility of the icon.
/// </summary>
public enum IconVisibility
{
    /// <summary>
    /// The icon is visible.
    /// </summary>
    Visible = 0x00,

    /// <summary>
    /// The icon is hidden.
    /// </summary>
    Hidden = (int)PInvoke.NIS_HIDDEN,

    /// <summary>
    /// The icon resource is shared between multiple icons.
    /// </summary>
    Shared = (int)PInvoke.NIS_SHAREDICON,
}
