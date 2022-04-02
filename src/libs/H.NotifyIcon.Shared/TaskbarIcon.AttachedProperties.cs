namespace H.NotifyIcon;

/// <summary>
/// Contains declarations of WPF dependency properties
/// and events.
/// </summary>
public partial class TaskbarIcon
{
    #region ParentTaskbarIcon

    /// <summary>
    /// An attached property that is assigned to displayed UI elements (balloons, tooltips, context menus), and
    /// that can be used to bind to this control. The attached property is being derived, so binding is
    /// quite straightforward:
    /// <code>
    /// <TextBlock Text="{Binding RelativeSource={RelativeSource Self}, Path=(tb:TaskbarIcon.ParentTaskbarIcon).ToolTipText}" />
    /// </code>
    /// </summary>  
    public static readonly DependencyProperty ParentTaskbarIconProperty =
        DependencyProperty.RegisterAttached(
            "ParentTaskbarIcon",
            typeof (TaskbarIcon),
            typeof (TaskbarIcon),
#if HAS_WPF
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
#else
            new PropertyMetadata(null));
#endif

    /// <summary>
    /// Gets the ParentTaskbarIcon property.  This dependency property 
    /// indicates ....
    /// </summary>
    public static TaskbarIcon? GetParentTaskbarIcon(DependencyObject d)
    {
        return (TaskbarIcon?) d?.GetValue(ParentTaskbarIconProperty);
    }

    /// <summary>
    /// Sets the ParentTaskbarIcon property.  This dependency property 
    /// indicates ....
    /// </summary>
    public static void SetParentTaskbarIcon(DependencyObject d, TaskbarIcon? value)
    {
        d?.SetValue(ParentTaskbarIconProperty, value);
    }

    #endregion
}
