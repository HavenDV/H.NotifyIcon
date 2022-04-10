namespace H.NotifyIcon.Core;

///<summary>
/// Supported icons for the tray's balloon messages.
/// https://docs.microsoft.com/en-us/windows/win32/api/shellapi/ns-shellapi-notifyicondataa#niif_none-0x00000000
///</summary>
public enum NotificationIcon
{
    /// <summary>
    /// The balloon message is displayed without an icon.
    /// </summary>
    None = (int)PInvoke.NIIF_NONE,

    /// <summary>
    /// An information is displayed.
    /// </summary>
    Info = (int)PInvoke.NIIF_INFO,

    /// <summary>
    /// A warning is displayed.
    /// </summary>
    Warning = (int)PInvoke.NIIF_WARNING,

    /// <summary>
    /// An error is displayed.
    /// </summary>
    Error = (int)PInvoke.NIIF_ERROR,
}
