namespace H.NotifyIcon;

/// <summary>
/// Contains declarations of WPF dependency properties
/// and events.
/// </summary>
public partial class TaskbarIcon
{
    #region Constants

    /// <summary>
    /// Category name that is set on designer properties.
    /// </summary>
    public const string CategoryName = "NotifyIcon";

    #endregion

    #region Id

    /// <summary>Identifies the <see cref="Id"/> dependency property.</summary>
    public static readonly DependencyProperty IdProperty =
        DependencyProperty.Register(nameof(Id),
            typeof(Guid),
            typeof(TaskbarIcon),
            new PropertyMetadata(Guid.Empty, IdPropertyChanged));

    /// <summary>
    /// Gets or sets the TrayIcon Id.
    /// Use this for second TrayIcon in same app.
    /// </summary>
    [Category(CategoryName)]
    [Description("Sets the TrayIcon Id.")]
    public Guid Id
    {
        get => (Guid)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    private static void IdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TaskbarIcon control)
        {
            throw new InvalidOperationException($"Parent should be {nameof(TaskbarIcon)}");
        }
        if (e.OldValue is not Guid oldId)
        {
            throw new InvalidOperationException($"Value should be {nameof(Guid)}");
        }
        if (e.NewValue is not Guid newId)
        {
            throw new InvalidOperationException($"Value should be {nameof(Guid)}");
        }

        var wasCreated = control.IsCreated;
        if (oldId != Guid.Empty &&
            wasCreated)
        {
            _ = control.TrayIcon.TryRemove();
        }

        control.Id = newId;

        if (wasCreated)
        {
            control.TrayIcon.Create();
        }
    }

    #endregion

    #region Icon/IconSource

    /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon),
            typeof(System.Drawing.Icon),
            typeof(TaskbarIcon),
            new PropertyMetadata(null, IconPropertyChanged));

    /// <summary>
    /// Gets or sets the icon to be displayed.
    /// Use this for dynamically generated System.Drawing.Icons.
    /// </summary>
    [Category(CategoryName)]
    [Description("Sets the displayed taskbar icon.")]
    public System.Drawing.Icon? Icon
    {
        get => (System.Drawing.Icon?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    private static void IconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TaskbarIcon control)
        {
            throw new InvalidOperationException($"Parent should be {nameof(TaskbarIcon)}");
        }
        if (e.OldValue is System.Drawing.Icon oldIcon)
        {
            oldIcon.Dispose();
        }
        var newIcon = (System.Drawing.Icon?)e.NewValue;

        var icon = (nint?)newIcon?.Handle ?? 0;

        control.TrayIcon.UpdateIcon(icon);
    }

    /// <summary>Identifies the <see cref="IconSource"/> dependency property.</summary>
    public static readonly DependencyProperty IconSourceProperty =
        DependencyProperty.Register(nameof(IconSource),
            typeof (ImageSource),
            typeof (TaskbarIcon),
            new PropertyMetadata(null, IconSourcePropertyChanged));

    /// <summary>
    /// A property wrapper for the <see cref="IconSourceProperty"/>
    /// dependency property:<br/>
    /// Resolves an image source and updates the <see cref="Icon" /> property accordingly.
    /// </summary>
    [Category(CategoryName)]
    [Description("Sets the displayed taskbar icon.")]
    public ImageSource? IconSource
    {
        get => (ImageSource?)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

#if HAS_WPF
    private static void IconSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
#else
    private static async void IconSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
#endif
    {
        if (d is not TaskbarIcon control)
        {
            throw new InvalidOperationException($"Parent should be {nameof(TaskbarIcon)}");
        }
        if (e.NewValue is not ImageSource source)
        {
            control.Icon = null;
            if (!string.IsNullOrWhiteSpace(control.GeneratedIconText))
            {
                control.RefreshGeneratedIcon();
            }
            return;
        }

#if HAS_WPF
        control.Icon = source.ToIcon();
#else
        control.Icon = await source.ToIconAsync().ConfigureAwait(true);
#endif

        if (!string.IsNullOrWhiteSpace(control.GeneratedIconText))
        {
            control.RefreshGeneratedIcon();
        }
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
