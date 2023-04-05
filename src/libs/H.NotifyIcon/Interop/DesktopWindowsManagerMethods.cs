namespace H.NotifyIcon.Interop;

/// <summary>
/// 
/// </summary>
public static class DesktopWindowsManagerMethods
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    [SupportedOSPlatform("windows6.0.6000")]
    public static unsafe void SetRoundedCorners(nint handle)
    {
        var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
        
        _ = PInvoke.DwmSetWindowAttribute(
            hwnd: new HWND(value: handle),
            dwAttribute: DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
            pvAttribute: &preference,
            cbAttribute: sizeof(uint));
    }
}
