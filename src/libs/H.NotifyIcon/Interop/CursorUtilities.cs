using System.Drawing;

namespace H.NotifyIcon.Interop;

/// <summary>
/// Win32 API imports.
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
public static class CursorUtilities
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows6.0.6000")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
    public static unsafe Point GetPhysicalCursorPos()
    {
        var point = new POINT();

        _ = PInvoke.GetPhysicalCursorPos(&point);

        return new Point
        {
            X = point.x,
            Y = point.y,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static unsafe Point GetCursorPos()
    {
        var point = new POINT();

        _ = PInvoke.GetCursorPos(&point);

        return new Point
        {
            X = point.x,
            Y = point.y,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static int GetDoubleClickTime()
    {
        return (int)PInvoke.GetDoubleClickTime();
    }
}
