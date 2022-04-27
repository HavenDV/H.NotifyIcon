using System.Drawing;

namespace H.NotifyIcon;

/// <summary>
/// 
/// </summary>
public class TrayIconEventArgs : EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    public Point Point { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public TrayIconEventArgs()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    public TrayIconEventArgs(Point point)
    {
        Point = point;
    }
}
