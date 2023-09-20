using H.NotifyIcon.Interop;

namespace H.NotifyIcon.Core;

/// <summary>
/// Win32 API imports.
/// </summary>
[SupportedOSPlatform("windows5.0")]
public static class WindowUtilities
{
    /// <summary>
    /// Starts message processing on the current thread and blocks it until a WM_QUIT message or error is received.
    /// </summary>
    public static unsafe void RunMessageLoop()
    {
        while (true)
        {
            var msg = new MSG();
            var result = PInvoke.GetMessage(
                lpMsg: &msg,
                hWnd: new HWND(),
                wMsgFilterMin: 0,
                wMsgFilterMax: 0);
            if (result == 0 ||
                result == -1)
            {
                break;
            }

            _ = PInvoke.TranslateMessage(&msg);
            _ = PInvoke.DispatchMessage(&msg);
        }
    }

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

    /// <summary>
    /// Returns true if app is packaged.
    /// </summary>
    /// <returns></returns>
    [SupportedOSPlatform("windows8.0")]
    public static unsafe bool IsPackaged()
    {
        var size = 0U;
        var result = PInvoke.GetCurrentPackageId(
            bufferLength: &size,
            buffer: default);

        return result != WIN32_ERROR.APPMODEL_ERROR_NO_PACKAGE;
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
        return PInvoke.ShowWindow(new HWND(hWnd), SHOW_WINDOW_CMD.SW_SHOWNORMAL);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <returns></returns>
    public static void ShowWindowInTaskbar(nint hWnd)
    {
        var window = new HWND(hWnd);
        var style = (WINDOW_EX_STYLE)User32Methods.GetWindowLong(window, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        style |= WINDOW_EX_STYLE.WS_EX_APPWINDOW;
        style &= ~(WINDOW_EX_STYLE.WS_EX_TOOLWINDOW);

        _ = User32Methods.SetWindowLong(window, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (nint)style);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <returns></returns>
    public static void HideWindowInTaskbar(nint hWnd)
    {
        var window = new HWND(hWnd);
        var style = (WINDOW_EX_STYLE)User32Methods.GetWindowLong(window, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        style |= WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
        style &= ~(WINDOW_EX_STYLE.WS_EX_APPWINDOW);

        _ = User32Methods.SetWindowLong(window, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, (nint)style);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [SupportedOSPlatform("windows5.1.2600")]
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
            crKey: new COLORREF((uint)ToWin32(System.Drawing.Color.Black)),
            bAlpha: 255,
            dwFlags: LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_COLORKEY | LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_ALPHA).EnsureNonZero();
    }
    
    private static int ToWin32(System.Drawing.Color c) => (int) c.B << 16 | (int) c.G << 8 | (int) c.R;
    
    private static SUBCLASSPROC? SubClassDelegate;

    [SupportedOSPlatform("windows5.1.2600")]
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
                        color: new COLORREF((uint)ToWin32(System.Drawing.Color.Black))).EnsureNonZero();
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
