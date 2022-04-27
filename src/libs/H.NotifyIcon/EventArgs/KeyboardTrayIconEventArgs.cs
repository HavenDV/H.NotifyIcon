using System.Drawing;
using H.NotifyIcon.Core;

namespace H.NotifyIcon;

/// <summary>
/// 
/// </summary>
public class KeyboardTrayIconEventArgs : TrayIconEventArgs
{
    /// <summary>
    /// 
    /// </summary>
    public KeyboardEvent KeyboardEvent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keyboardEvent"></param>
    public KeyboardTrayIconEventArgs(KeyboardEvent keyboardEvent)
    {
        KeyboardEvent = keyboardEvent;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="keyboardEvent"></param>
    /// <param name="point"></param>
    public KeyboardTrayIconEventArgs(KeyboardEvent keyboardEvent, Point point) : base(point)
    {
        KeyboardEvent = keyboardEvent;
    }
}
