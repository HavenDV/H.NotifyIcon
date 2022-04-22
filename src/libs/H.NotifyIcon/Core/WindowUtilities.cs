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

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows5.1.2600")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
    public static unsafe void MakeTransparent(nint hWndHandle)
    {
        var hWnd = new HWND(hWndHandle);

        SubClassDelegate = new SUBCLASSPROC(WindowSubClass);

        _ = PInvoke.SetWindowSubclass(
            hWnd: hWnd,
            pfnSubclass: SubClassDelegate,
            uIdSubclass: 0,
            dwRefData: 0).EnsureNonZero();

        var exStyle = (WINDOW_EX_STYLE)User32Methods.GetWindowLong(
            hWnd: hWnd,
            nIndex: WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        _ = User32Methods.SetWindowLong(
            hWnd: hWnd,
            nIndex: WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE,
            dwNewLong: (nint)(exStyle | WINDOW_EX_STYLE.WS_EX_LAYERED)).EnsureNonZero();

        _ = PInvoke.SetLayeredWindowAttributes(
            hwnd: hWnd,
            crKey: (uint)System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.Black),
            bAlpha: 255,
            dwFlags: LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_COLORKEY | LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_ALPHA).EnsureNonZero();
    }

    private static SUBCLASSPROC? SubClassDelegate;

#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows5.1.2600")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
    private static unsafe LRESULT WindowSubClass(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        switch (uMsg)
        {
            case PInvoke.WM_ERASEBKGND:
                {
                    RECT rect;
                    _ = PInvoke.GetClientRect(
                        hWnd: hWnd,
                        lpRect: &rect).EnsureNonZero();
                    var hBrush = PInvoke.CreateSolidBrush(
                        color: (uint)System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.Black)).EnsureNonZero();
                    _ = PInvoke.FillRect(
                        hDC: new HDC((nint)wParam.Value),
                        lprc: &rect,
                        hbr: hBrush).EnsureNonZero();
                    _ = PInvoke.DeleteObject(
                        ho: new HGDIOBJ(hBrush)).EnsureNonZero();

                    return new LRESULT(1);
                }
        }

        return PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }
}
