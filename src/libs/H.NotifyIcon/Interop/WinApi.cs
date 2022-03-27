using System.Runtime.InteropServices;

namespace Hardcodet.Wpf.TaskbarNotification.Interop;

/// <summary>
/// Win32 API imports.
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
public static class WinApi
{
    /// <summary>
    /// Creates, updates or deletes the taskbar icon.
    /// </summary>
    [DllImport("shell32.Dll", CharSet = CharSet.Unicode)]
    public static extern bool Shell_NotifyIcon(NotifyCommand cmd, [In] ref NotifyIconData data);
}
