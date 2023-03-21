using System.Drawing;
using System.Runtime.InteropServices;
using H.NotifyIcon.Interop;

namespace H.NotifyIcon.Core;

/// <summary>
/// Win32 API imports.
/// </summary>
[SupportedOSPlatform("windows5.0")]
public static class CursorUtilities
{
    [SupportedOSPlatform("windows6.0.6000")]
    internal static unsafe Point GetPhysicalCursorPos()
    {
        var point = new Point();

        _ = PInvoke.GetPhysicalCursorPos(&point).EnsureNonZero();

        return new Point
        {
            X = point.X,
            Y = point.Y,
        };
    }

    internal static unsafe Point GetCursorPos()
    {
        var point = new Point();

        _ = PInvoke.GetCursorPos(&point).EnsureNonZero();

        return new Point
        {
            X = point.X,
            Y = point.Y,
        };
    }

    /// <summary>
    /// Calculates an appropriate pop-up window position using the specified anchor point, pop-up window size, flags, 
    /// and the optional exclude rectangle. When the specified pop-up window size is smaller than the desktop window size, 
    /// use the CalculatePopupWindowPosition function to ensure that the pop-up window is fully visible on the desktop window, 
    /// regardless of the specified anchor point.
    /// </summary>
    /// <returns>A structure that specifies the pop-up window position.</returns>
    /// <exception cref="COMException"></exception>
    /// 
    [SupportedOSPlatform("windows6.1")]
    public static unsafe Rectangle CalculatePopupWindowPosition(
        int x,
        int y,
        int width,
        int height,
        Rectangle? excludeRect = null)
    {
        var _anchorPoint = new Point
        {
            X = x,
            Y = y,
        };
        var _windowSize = new SIZE
        {
            cx = width,
            cy = height,
        };
        var flags = TRACK_POPUP_MENU_FLAGS.TPM_BOTTOMALIGN;
        var _excludeRect = new RECT
        {
            left = excludeRect?.Left ?? 0,
            top = excludeRect?.Top ?? 0,
            right = excludeRect?.Right ?? 0,
            bottom = excludeRect?.Bottom ?? 0,
        };
        var popupWindowPosition = new RECT();

        _ = PInvoke.CalculatePopupWindowPosition(
            anchorPoint: &_anchorPoint,
            windowSize: &_windowSize,
            flags: (uint)flags,
            excludeRect: &_excludeRect,
            popupWindowPosition: &popupWindowPosition).EnsureNonZero();

        return new Rectangle(
            x: popupWindowPosition.left,
            y: popupWindowPosition.top,
            width: popupWindowPosition.right - popupWindowPosition.left,
            height: popupWindowPosition.bottom - popupWindowPosition.top);
    }

    /// <summary>
    /// Retrieves the current double-click time for the mouse.
    /// </summary>
    /// <returns>The return value specifies the current double-click time, in milliseconds. The maximum return value is 5000 milliseconds.</returns>
    public static int GetDoubleClickTime()
    {
        return (int)PInvoke.GetDoubleClickTime();
    }
}
