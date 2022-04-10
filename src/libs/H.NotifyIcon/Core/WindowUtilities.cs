namespace H.NotifyIcon.Core;

/// <summary>
/// Win32 API imports.
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
public static class WindowUtilities
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool SetForegroundWindow(nint hWnd)
    {
        return PInvoke.SetForegroundWindow(new HWND(hWnd));
    }
}
