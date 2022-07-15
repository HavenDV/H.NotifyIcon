namespace H.NotifyIcon;

[DependencyProperty<GeneratedIcon>("GeneratedIcon",
    Description = "Defines generated icon.", Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Properties

    partial void OnGeneratedIconChanged(
        GeneratedIcon? oldValue,
        GeneratedIcon? newValue)
    {
        oldValue?.Dispose();
        if (newValue != null)
        {
            newValue.TaskbarIcon = this;
            newValue.Refresh();
        }
    }

    #endregion
}
