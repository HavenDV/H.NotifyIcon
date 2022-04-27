using System.Drawing;
using H.NotifyIcon.Core;

namespace H.NotifyIcon;

/// <summary>
/// 
/// </summary>
public class MouseTrayIconEventArgs : TrayIconEventArgs
{
    /// <summary>
    /// 
    /// </summary>
    public MouseEvent MouseEvent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mouseEvent"></param>
    public MouseTrayIconEventArgs(MouseEvent mouseEvent)
    {
        MouseEvent = mouseEvent;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mouseEvent"></param>
    /// <param name="point"></param>
    public MouseTrayIconEventArgs(MouseEvent mouseEvent, Point point) : base(point)
    {
        MouseEvent = mouseEvent;
    }
}
