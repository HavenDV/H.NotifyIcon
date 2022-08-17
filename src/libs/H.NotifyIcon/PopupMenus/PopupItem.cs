namespace H.NotifyIcon.Core;

/// <summary>
/// 
/// </summary>
public abstract class PopupItem
{
    /// <summary>
    /// 
    /// </summary>
    internal int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Visible { get; set; } = true;
}
