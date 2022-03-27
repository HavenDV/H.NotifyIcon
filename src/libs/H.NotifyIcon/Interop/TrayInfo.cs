// Some interop code taken from Mike Marshall's AnyForm

using System.Drawing;

namespace Hardcodet.Wpf.TaskbarNotification.Interop;

/// <summary>
/// Resolves the current tray position.
/// </summary>
public static class TrayInfo
{
    /// <summary>
    /// Gets the position of the system tray.
    /// </summary>
    /// <returns>Tray coordinates.</returns>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows5.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
    public static Point GetTrayLocation()
    {
        int space = 2;
        var info = new AppBarInfo();
        info.GetSystemTaskBarPosition();

        Rectangle rcWorkArea = info.WorkArea;

        int x = 0, y = 0;
        switch (info.Edge)
        {
            case AppBarInfo.ScreenEdge.Left:
                x = rcWorkArea.Right + space;
                y = rcWorkArea.Bottom;
                break;
            case AppBarInfo.ScreenEdge.Bottom:
                x = rcWorkArea.Right;
                y = rcWorkArea.Bottom - rcWorkArea.Height - space;
                break;
            case AppBarInfo.ScreenEdge.Top:
                x = rcWorkArea.Right;
                y = rcWorkArea.Top + rcWorkArea.Height + space;
                break;
            case AppBarInfo.ScreenEdge.Right:
                x = rcWorkArea.Right - rcWorkArea.Width - space;
                y = rcWorkArea.Bottom;
                break;
        }

        return new Point {X = x, Y = y};
    }
}
