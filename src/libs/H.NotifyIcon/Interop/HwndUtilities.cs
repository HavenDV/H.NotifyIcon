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

    /// <summary>
    /// Sets the owner for a top-level popup window.
    /// </summary>
    /// <param name="hWnd">Window handle.</param>
    /// <param name="ownerHWnd">Owner window handle.</param>
    [SupportedOSPlatform("windows5.0")]
    public static void SetOwnerWindow(IntPtr hWnd, IntPtr ownerHWnd)
    {
        var window = new HWND(hWnd);

        // GWLP_HWNDPARENT is not exposed consistently across all projections we build against.
        const WINDOW_LONG_PTR_INDEX ownerIndex = (WINDOW_LONG_PTR_INDEX)(-8);

        Marshal.SetLastPInvokeError(0);
        if (Environment.Is64BitProcess)
        {
            _ = PInvoke.SetWindowLongPtr(window, ownerIndex, ownerHWnd);
        }
        else
        {
            _ = PInvoke.SetWindowLong(window, ownerIndex, ownerHWnd.ToInt32());
        }

        var error = Marshal.GetLastPInvokeError();
        if (error != 0)
        {
            throw new Win32Exception(error);
        }

        _ = PInvoke.SetWindowPos(
            hWnd: window,
            hWndInsertAfter: new HWND(IntPtr.Zero),
            X: 0,
            Y: 0,
            cx: 0,
            cy: 0,
            uFlags: SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED |
                    SET_WINDOW_POS_FLAGS.SWP_NOMOVE |
                    SET_WINDOW_POS_FLAGS.SWP_NOSIZE |
                    SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE |
                    SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
    }
}
