using System.ComponentModel;
using System.Runtime.InteropServices;

namespace H.NotifyIcon.Interop;

/// <summary>
/// A set of HWND Helper Methods
/// Codes are edited from: https://github.com/dotMorten/WinUIEx
/// </summary>
public static class HwndUtilities
{
    /// <summary>
    /// Sets the current window style
    /// </summary>
    /// <param name="hWnd">Window handle</param>
    /// <param name="newStyle"></param>
    [SupportedOSPlatform("windows5.0")]
    internal static void SetWindowStyle(IntPtr hWnd, WINDOW_STYLE newStyle)
    {
        var h = new HWND(hWnd);
        
        _ = PInvoke.SetWindowLong(h, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)newStyle).EnsureNonZero();
        
        // Redraw window
        _ = PInvoke.SetWindowPos(
            hWnd: h, 
            hWndInsertAfter: new HWND(IntPtr.Zero), 
            X: 0, 
            Y: 0, 
            cx: 0, 
            cy: 0, 
            uFlags: SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED |
                    SET_WINDOW_POS_FLAGS.SWP_NOMOVE |
                    SET_WINDOW_POS_FLAGS.SWP_NOSIZE |
                    SET_WINDOW_POS_FLAGS.SWP_NOZORDER |
                    SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    [SupportedOSPlatform("windows5.0")]
    public static void SetWindowStyleAsPopupWindow(IntPtr hWnd)
    {
        SetWindowStyle(hWnd, WINDOW_STYLE.WS_POPUPWINDOW);
    }
}
