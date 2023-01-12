namespace H.NotifyIcon.Core;

/// <summary>
/// Supported icons for the tray's notification messages.
/// https://docs.microsoft.com/en-us/windows/win32/api/shellapi/ns-shellapi-notifyicondataa#niif_none-0x00000000
/// </summary>
public enum NotificationIcon
{
    /// <summary>
    /// The balloon message is displayed without an icon.
    /// </summary>
    None = (int)NOTIFY_ICON_INFOTIP_FLAGS.NIIF_NONE,

    /// <summary>
    /// An information is displayed.
    /// </summary>
    Info = (int)NOTIFY_ICON_INFOTIP_FLAGS.NIIF_INFO,

    /// <summary>
    /// A warning is displayed.
    /// </summary>
    Warning = (int)NOTIFY_ICON_INFOTIP_FLAGS.NIIF_WARNING,

    /// <summary>
    /// An error is displayed.
    /// </summary>
    Error = (int)NOTIFY_ICON_INFOTIP_FLAGS.NIIF_ERROR,
}
