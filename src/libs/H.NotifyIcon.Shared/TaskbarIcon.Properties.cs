namespace H.NotifyIcon;

[DependencyProperty<Guid>("Id",
    Description = "Gets or sets the TrayIcon Id. Use this for second TrayIcon in same app.", Category = CategoryName)]
[DependencyProperty<System.Drawing.Icon>("Icon",
    Description = "Gets or sets the icon to be displayed. Use this for dynamically generated System.Drawing.Icons.", Category = CategoryName)]
[DependencyProperty<ImageSource>("IconSource",
    Description = "Resolves an image source and updates the Icon property accordingly.", Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Constants

    /// <summary>
    /// Category name that is set on designer properties.
    /// </summary>
    public const string CategoryName = "NotifyIcon";

    #endregion

    #region Id
    
    partial void OnIdChanged(Guid oldValue, Guid newValue)
    {
        var wasCreated = IsCreated;
        if (oldValue != Guid.Empty &&
            wasCreated)
        {
            _ = TrayIcon.TryRemove();
        }

        Id = newValue;

        if (wasCreated)
        {
            TrayIcon.Create();
        }
    }

    #endregion

    #region Icon/IconSource

    partial void OnIconChanged(System.Drawing.Icon? oldValue, System.Drawing.Icon? newValue)
    {
        oldValue?.Dispose();
        UpdateIcon(newValue);
    }

    /// <summary>
    /// Updates TrayIcon.Icon without changing Icon property.
    /// </summary>
    /// <param name="value"></param>
    public void UpdateIcon(System.Drawing.Icon? value)
    {
        TrayIcon.UpdateIcon((nint?)value?.Handle ?? 0);
    }

#if HAS_WPF
    partial void OnIconSourceChanged(ImageSource? oldValue, ImageSource? newValue)
#else
    async partial void OnIconSourceChanged(ImageSource? oldValue, ImageSource? newValue)
#endif
    {
        if (newValue == null)
        {
            Icon = null;
            GeneratedIcon?.Refresh();
            return;
        }

#if HAS_WPF
        Icon = newValue.ToIcon();
#else
        Icon = await newValue.ToIconAsync().ConfigureAwait(true);
#endif

        GeneratedIcon?.Refresh();
    }

    #endregion

    #region Visibility

    private static void VisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (TaskbarIcon) d;
        var newValue = (Visibility)e.NewValue;

        owner.SetTrayIconVisibility(newValue);
    }

    private void SetTrayIconVisibility(Visibility value)
    {
        var state = value == Visibility.Visible
            ? IconVisibility.Visible
            : IconVisibility.Hidden;

        TrayIcon.UpdateVisibility(state);
    }

    #endregion

    #region DataContext

    /// <summary>
    /// Updates the DataContextProperty of a given
    /// <see cref="FrameworkElement"/>. This method only updates target elements
    /// that do not already have a data context of their own, and either assigns
    /// the DataContext of the NotifyIcon, or the
    /// NotifyIcon itself, if no data context was assigned at all.
    /// </summary>
    private void UpdateDataContext(
        FrameworkElement? target,
        object? oldDataContextValue,
        object? newDataContextValue)
    {
        //if there is no target or it's data context is determined through a binding
        //of its own, keep it
        if (target == null ||
            target.GetBindingExpression(DataContextProperty) != null)
        { 
            return;
        }

        //if the target's data context is the NotifyIcon's old DataContext or the NotifyIcon itself,
        //update it
        if (ReferenceEquals(this, target.DataContext) ||
            Equals(oldDataContextValue, target.DataContext))
        {
            //assign own data context, if available. If there is no data
            //context at all, assign NotifyIcon itself.
            target.DataContext = newDataContextValue ?? this;
        }
    }

    private static void DataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (TaskbarIcon) d;
        owner.UpdateDataContext(e.OldValue, e.NewValue);
    }

    private void UpdateDataContext(object? oldValue, object? newValue)
    {
        UpdateDataContext(TrayPopupResolved, oldValue, newValue);
        UpdateDataContext(TrayToolTipResolved, oldValue, newValue);
#if HAS_WPF
        UpdateDataContext(ContextMenu, oldValue, newValue);
#else
        UpdateContextFlyoutDataContext(ContextFlyout, oldValue, newValue);
#endif
    }

    #endregion
}
