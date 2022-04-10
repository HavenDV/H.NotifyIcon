namespace H.NotifyIcon.Core;

/// <summary>
/// The state of the icon - can be set to
/// hide the icon.
/// </summary>
public enum IconState
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
