using System.Drawing;
using H.NotifyIcon.Interop;

namespace H.NotifyIcon.Core;

/// <summary>
/// Resolves the current tray position.
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.1.2600")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
public static class TrayInfo
{
    /// <summary>
    /// Returns <see langword="true"/> if shell is open.
    /// </summary>
    /// <returns>Tray coordinates.</returns>
    public static bool IsShellOpen()
    {
        return PInvoke.FindWindow("Shell_TrayWnd", null).Value != IntPtr.Zero; 
    }

    /// <summary>
    /// Returns <see langword="true"/> if notify icon overflow window is open.
    /// </summary>
    /// <returns>Tray coordinates.</returns>
    public static bool IsNotifyIconOverflowWindowOpen()
    {
        return PInvoke.FindWindow("NotifyIconOverflowWindow", null).Value != IntPtr.Zero;
    }

    /// <summary>
    /// Gets the position of the system tray.
    /// </summary>
    /// <returns>Tray coordinates.</returns>
    public static unsafe Point GetTrayLocation(int space = 2)
    {
        if (!IsShellOpen())
        {
            throw new InvalidOperationException("Shell is closed.");
        }

        uint edge;
        Rectangle workArea;
        if (Environment.Is64BitProcess)
        {
            var data = new APPBARDATA64
            {
                cbSize = (uint)sizeof(APPBARDATA64),
            };
            _ = PInvoke.SHAppBarMessage(PInvoke.ABM_GETTASKBARPOS, &data)
                .EnsureNonZero(new InvalidOperationException("Failed to communicate with the given AppBar"));

            edge = data.uEdge;
            workArea = new Rectangle(
                   data.rc.left,
                   data.rc.top,
                   data.rc.right - data.rc.left,
                   data.rc.bottom - data.rc.top);
        }
        else
        {
            var data = new APPBARDATA32
            {
                cbSize = (uint)sizeof(APPBARDATA64),
            };
            _ = PInvoke.SHAppBarMessage(PInvoke.ABM_GETTASKBARPOS, &data)
                .EnsureNonZero(new InvalidOperationException("Failed to communicate with the given AppBar"));

            edge = data.uEdge;
            workArea = new Rectangle(
                   data.rc.left,
                   data.rc.top,
                   data.rc.right - data.rc.left,
                   data.rc.bottom - data.rc.top);
        }

        return edge switch
        {
            PInvoke.ABE_LEFT => new Point
            {
                X = workArea.Right + space,
                Y = workArea.Bottom,
            },
            PInvoke.ABE_BOTTOM => new Point
            {
                X = workArea.Right,
                Y = workArea.Bottom - workArea.Height - space,
            },
            PInvoke.ABE_TOP => new Point
            {
                X = workArea.Right,
                Y = workArea.Top + workArea.Height + space,
            },
            PInvoke.ABE_RIGHT => new Point
            {
                X = workArea.Right - workArea.Width - space,
                Y = workArea.Bottom,
            },
            _ => throw new NotSupportedException($"Edge: {edge} is not supported."),
        };
    }
}
