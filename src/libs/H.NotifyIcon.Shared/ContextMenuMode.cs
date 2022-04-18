#if !HAS_WPF
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
    /// A second window will be created with native implementation.
    /// </summary>
    SecondWindow,
}
#endif