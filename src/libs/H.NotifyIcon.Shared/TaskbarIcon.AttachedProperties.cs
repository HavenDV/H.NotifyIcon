namespace H.NotifyIcon;

[AttachedDependencyProperty<TaskbarIcon>("ParentTaskbarIcon", Inherits = true, XmlDocumentation = @"<summary>
An attached property that is assigned to displayed UI elements (balloons, tooltips, context menus), and
that can be used to bind to this control. The attached property is being derived, so binding is
quite straightforward:
<code>
<TextBlock Text=""{Binding RelativeSource={RelativeSource Self}, Path=(tb:TaskbarIcon.ParentTaskbarIcon).ToolTipText}"" />
</code>
</summary>")]
public partial class TaskbarIcon
{
}
