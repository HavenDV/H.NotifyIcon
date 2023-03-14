namespace H.NotifyIcon;

/// <summary>
/// Defines context menu modes.
/// </summary>
public enum ContextMenuMode
{
    /// <summary>
    /// The context menu will be emulated using <see cref="Core.PopupMenu"/> from H.NotifyIcon.PopupMenus package. <br/>
    /// This value is the default value.
    /// </summary>
    PopupMenu,

    /// <summary>
    /// A second window will be created with native implementation. <br/>
    /// This feature is available in preview mode. <br/>
    /// Need full testing on various systems, including Windows 10. <br/>
    /// </summary>
    SecondWindow,

    /// <summary>
    /// The context menu will be displayed in the corner of the active window.
    /// </summary>
    ActiveWindow,
}
