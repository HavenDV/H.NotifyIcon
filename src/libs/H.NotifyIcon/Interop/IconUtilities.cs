using System.Drawing;

namespace H.NotifyIcon.Interop;

/// <summary>
/// 
/// </summary>
[SupportedOSPlatform("windows5.0")]
public static class IconUtilities
{
    /// <summary>
    /// Based on: <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-iconinfo#members"/>
    /// </summary>
    /// <param name="hIcon"></param>
    /// <returns></returns>
    internal static unsafe Size GetSize(nint hIcon)
    {
        var iconInfo = new ICONINFO();
        _ = PInvoke.GetIconInfo(
            hIcon: new HICON(hIcon),
            piconinfo: &iconInfo).EnsureNonZero();

        return new Size(
            width: (int)iconInfo.xHotspot * 2,
            height: (int)iconInfo.yHotspot * 2);
    }

    /// <summary>
    /// Based on: <see href="https://docs.microsoft.com/en-us/windows/win32/api/shellapi/ns-shellapi-notifyicondataa#niif_large_icon-0x00000020"/>
    /// </summary>
    /// <param name="largeIcon"></param>
    /// <returns></returns>
    public static Size GetRequiredCustomIconSize(bool largeIcon)
    {
        return largeIcon
            ? new Size(
                width: PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXICON),
                height: PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYICON))
            : new Size(
                width: PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXSMICON),
                height: PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYSMICON));
    }
}
