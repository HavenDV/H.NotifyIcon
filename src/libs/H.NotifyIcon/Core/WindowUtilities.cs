using H.NotifyIcon.Interop;

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
    /// Brings the thread that created the specified window into the foreground and activates the window.
    /// </summary>
    /// <param name="hWnd">
    /// <para>Type: <b>HWND</b> A handle to the window that should be activated and brought to the foreground.</para>
    /// <para><see href="https://docs.microsoft.com/windows/win32/api//winuser/nf-winuser-setforegroundwindow#parameters">Read more on docs.microsoft.com</see>.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <b>bool</b> If the window was brought to the foreground, the return value is true.</para>
    /// <para>If the window was not brought to the foreground, the return value is false.</para>
    /// </returns>
    /// <remarks>
    /// <para><see href="https://docs.microsoft.com/windows/win32/api//winuser/nf-winuser-setforegroundwindow">Learn more about this API from docs.microsoft.com</see>.</para>
    /// </remarks>
    public static bool SetForegroundWindow(nint hWnd)
    {
        return PInvoke.SetForegroundWindow(new HWND(hWnd));
    }

    /// <summary>Sets the specified window's show state.</summary>
    /// <param name="hWnd">
    /// <para>Type: <b>HWND</b> A handle to the window.</para>
    /// <para><see href="https://docs.microsoft.com/windows/win32/api//winuser/nf-winuser-showwindow#parameters">Read more on docs.microsoft.com</see>.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <b>bool</b> If the window was previously visible, the return value is true. If the window was previously hidden, the return value is false.</para>
    /// </returns>
    /// <remarks>
    /// <para><see href="https://docs.microsoft.com/windows/win32/api//winuser/nf-winuser-showwindow">Learn more about this API from docs.microsoft.com</see>.</para>
    /// </remarks>
    public static bool HideWindow(nint hWnd)
    {
        return PInvoke.ShowWindow(new HWND(hWnd), SHOW_WINDOW_CMD.SW_HIDE);
    }

    /// <summary>Sets the specified window's show state.</summary>
    /// <param name="hWnd">
    /// <para>Type: <b>HWND</b> A handle to the window.</para>
    /// <para><see href="https://docs.microsoft.com/windows/win32/api//winuser/nf-winuser-showwindow#parameters">Read more on docs.microsoft.com</see>.</para>
    /// </param>
    /// <returns>
    /// <para>Type: <b>bool</b> If the window was previously visible, the return value is true. If the window was previously hidden, the return value is false.</para>
    /// </returns>
    /// <remarks>
    /// <para><see href="https://docs.microsoft.com/windows/win32/api//winuser/nf-winuser-showwindow">Learn more about this API from docs.microsoft.com</see>.</para>
    /// </remarks>
    public static bool ShowWindow(nint hWnd)
    {
        return PInvoke.ShowWindow(new HWND(hWnd), SHOW_WINDOW_CMD.SW_SHOW);
    }
}
