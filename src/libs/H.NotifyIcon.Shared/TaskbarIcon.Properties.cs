using H.NotifyIcon.Core;

namespace H.NotifyIcon;

/// <summary>
/// Contains declarations of WPF dependency properties
/// and events.
/// </summary>
public partial class TaskbarIcon
{
    /// <summary>
    /// Category name that is set on designer properties.
    /// </summary>
    public const string CategoryName = "NotifyIcon";

    //POPUP CONTROLS


    #region CustomBalloon

    /// <summary>
    /// Maintains a currently displayed custom balloon.
    /// </summary>
    private static readonly DependencyProperty CustomBalloonProperty
        = DependencyProperty.Register(
            nameof(CustomBalloon),
            typeof(Popup),
            typeof(TaskbarIcon),
            new PropertyMetadata(null));

    /// <summary>
    /// A custom popup that is being displayed in the tray area in order
    /// to display messages to the user.
    /// </summary>
    public Popup? CustomBalloon => (Popup?)GetValue(CustomBalloonProperty);

    /// <summary>
    /// Provides a secure method for setting the <see cref="CustomBalloon"/> property.  
    /// </summary>
    /// <param name="value">The new value for the property.</param>
    protected void SetCustomBalloon(Popup? value)
    {
        SetValue(CustomBalloonProperty, value);
    }

    #endregion

    //DEPENDENCY PROPERTIES

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
            typeof(Icon),
            typeof(TaskbarIcon),
            new PropertyMetadata(null, IconPropertyChanged));

    /// <summary>
    /// Gets or sets the icon to be displayed.
    /// Use this for dynamically generated System.Drawing.Icons.
    /// </summary>
    [Category(CategoryName)]
    [Description("Sets the displayed taskbar icon.")]
    public Icon? Icon
    {
        get => (Icon?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    private static void IconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TaskbarIcon control)
        {
            throw new InvalidOperationException($"Parent should be {nameof(TaskbarIcon)}");
        }
        if (e.OldValue is Icon oldIcon)
        {
            oldIcon.Dispose();
        }
        if (e.NewValue is not Icon newIcon)
        {
            throw new InvalidOperationException($"Value should be {nameof(Icon)}");
        }

        var icon = newIcon?.Handle ?? IntPtr.Zero;
        if (!control.TrayIcon.IsCreated)
        {
            control.TrayIcon.Icon = icon;
            return;
        }

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
            throw new InvalidOperationException($"Value should be {nameof(ImageSource)}");
        }

#if HAS_WPF
        control.Icon = source.ToIcon();
#else
        control.Icon = await source.ToIconAsync().ConfigureAwait(true);
#endif
    }

    #endregion

    #region MenuActivation dependency property

    /// <summary>Identifies the <see cref="MenuActivation"/> dependency property.</summary>
    public static readonly DependencyProperty MenuActivationProperty =
        DependencyProperty.Register(nameof(MenuActivation),
            typeof (PopupActivationMode),
            typeof (TaskbarIcon),
            new PropertyMetadata(PopupActivationMode.RightClick));

    /// <summary>
    /// A property wrapper for the <see cref="MenuActivationProperty"/>
    /// dependency property:<br/>
    /// Defines what mouse events display the context menu.
    /// Defaults to <see cref="PopupActivationMode.RightClick"/>.
    /// </summary>
    [Category(CategoryName)]
    [Description("Defines what mouse events display the context menu.")]
#if !HAS_WPF
    [CLSCompliant(false)]
#endif
    public PopupActivationMode MenuActivation
    {
        get { return (PopupActivationMode) GetValue(MenuActivationProperty); }
        set { SetValue(MenuActivationProperty, value); }
    }

    #endregion

    #region PopupActivation dependency property

    /// <summary>Identifies the <see cref="PopupActivation"/> dependency property.</summary>
    public static readonly DependencyProperty PopupActivationProperty =
        DependencyProperty.Register(nameof(PopupActivation),
            typeof (PopupActivationMode),
            typeof (TaskbarIcon),
            new PropertyMetadata(PopupActivationMode.LeftClick));

    /// <summary>
    /// A property wrapper for the <see cref="PopupActivationProperty"/>
    /// dependency property:<br/>
    /// Defines what mouse events trigger the <see cref="TrayPopup" />.
    /// Default is <see cref="PopupActivationMode.LeftClick" />.
    /// </summary>
    [Category(CategoryName)]
    [Description("Defines what mouse events display the TaskbarIconPopup.")]
#if !HAS_WPF
    [CLSCompliant(false)]
#endif
    public PopupActivationMode PopupActivation
    {
        get { return (PopupActivationMode) GetValue(PopupActivationProperty); }
        set { SetValue(PopupActivationProperty, value); }
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
        if (!IsCreated)
        {
            TrayIcon.Visibility = state;
            return;
        }

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

#if !HAS_WPF

    private void UpdateContextFlyoutDataContext(
        FlyoutBase flyout,
        object? oldValue,
        object? newValue)
    {
        void UpdateMenuFlyoutDataContext(MenuFlyoutItemBase item)
        {
            UpdateDataContext(item, oldValue, newValue);

            if (item is MenuFlyoutSubItem subItem)
            {
                foreach (var innerItem in subItem.Items)
                {
                    UpdateMenuFlyoutDataContext(innerItem);
                }
            }
        }

        if (flyout is MenuFlyout menuFlyout)
        {
            foreach (var item in menuFlyout.Items)
            {
                UpdateMenuFlyoutDataContext(item);
            }
        }
    }

#endif

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

    #region ContextMenu dependency property override

#if HAS_WPF
    /// <summary>
    /// A static callback listener which is being invoked if the
    /// <see cref="FrameworkElement.ContextMenuProperty"/> dependency property has
    /// been changed. Invokes the <see cref="OnContextMenuPropertyChanged"/>
    /// instance method of the changed instance.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void ContextMenuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (TaskbarIcon) d;
        owner.OnContextMenuPropertyChanged(e);
    }


    /// <summary>
    /// Releases the old and updates the new <see cref="ContextMenu"/> property
    /// in order to reflect both the NotifyIcon's <see cref="FrameworkElement.DataContext"/>
    /// property and have the <see cref="ParentTaskbarIconProperty"/> assigned.
    /// </summary>
    /// <param name="e">Provides information about the updated property.</param>
    private void OnContextMenuPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        var contextMenu = (ContextMenu)e.NewValue;
        if (e.OldValue != null)
        {
            //remove the taskbar icon reference from the previously used element
            SetParentTaskbarIcon((DependencyObject) e.OldValue, null);
        }

        if (e.NewValue != null)
        {
            //set this taskbar icon as a reference to the new tooltip element
            SetParentTaskbarIcon((DependencyObject) e.NewValue, this);
        }

        UpdateDataContext(contextMenu, null, DataContext);
    }
#endif

    #endregion

    #region DoubleClickCommand dependency property

    /// <summary>Identifies the <see cref="DoubleClickCommand"/> dependency property.</summary>
    public static readonly DependencyProperty DoubleClickCommandProperty =
        DependencyProperty.Register(nameof(DoubleClickCommand),
            typeof (ICommand),
            typeof (TaskbarIcon),
            new PropertyMetadata(null));

    /// <summary>
    /// A property wrapper for the <see cref="DoubleClickCommandProperty"/>
    /// dependency property:<br/>
    /// Associates a command that is being executed if the tray icon is being
    /// double clicked.
    /// </summary>
    [Category(CategoryName)]
    [Description("A command that is being executed if the tray icon is being double-clicked.")]
    public ICommand? DoubleClickCommand
    {
        get { return (ICommand?) GetValue(DoubleClickCommandProperty); }
        set { SetValue(DoubleClickCommandProperty, value); }
    }

    #endregion

    #region DoubleClickCommandParameter dependency property

    /// <summary>Identifies the <see cref="DoubleClickCommandParameter"/> dependency property.</summary>
    public static readonly DependencyProperty DoubleClickCommandParameterProperty =
        DependencyProperty.Register(nameof(DoubleClickCommandParameter),
            typeof (object),
            typeof (TaskbarIcon),
            new PropertyMetadata(null));

    /// <summary>
    /// A property wrapper for the <see cref="DoubleClickCommandParameterProperty"/>
    /// dependency property:<br/>
    /// Command parameter for the <see cref="DoubleClickCommand"/>.
    /// </summary>
    [Category(CategoryName)]
    [Description("Parameter to submit to the DoubleClickCommand when the user double clicks on the NotifyIcon.")]
    public object? DoubleClickCommandParameter
    {
        get { return GetValue(DoubleClickCommandParameterProperty); }
        set { SetValue(DoubleClickCommandParameterProperty, value); }
    }

    #endregion

    #region DoubleClickCommandTarget dependency property

#if HAS_WPF

    /// <summary>Identifies the <see cref="DoubleClickCommandTarget"/> dependency property.</summary>
    public static readonly DependencyProperty DoubleClickCommandTargetProperty =
        DependencyProperty.Register(nameof(DoubleClickCommandTarget),
            typeof (IInputElement),
            typeof (TaskbarIcon),
            new PropertyMetadata(null));

    /// <summary>
    /// A property wrapper for the <see cref="DoubleClickCommandTargetProperty"/>
    /// dependency property:<br/>
    /// The target of the command that is fired if the notify icon is double clicked.
    /// </summary>
    [Category(CategoryName)]
    [Description("The target of the command that is fired if the notify icon is double clicked.")]
    public IInputElement? DoubleClickCommandTarget
    {
        get { return (IInputElement?) GetValue(DoubleClickCommandTargetProperty); }
        set { SetValue(DoubleClickCommandTargetProperty, value); }
    }

#endif

    #endregion

    #region LeftClickCommand dependency property

    /// <summary>Identifies the <see cref="LeftClickCommand"/> dependency property.</summary>
    public static readonly DependencyProperty LeftClickCommandProperty =
        DependencyProperty.Register(nameof(LeftClickCommand),
            typeof (ICommand),
            typeof (TaskbarIcon),
            new PropertyMetadata(null));

    /// <summary>
    /// A property wrapper for the <see cref="LeftClickCommandProperty"/>
    /// dependency property:<br/>
    /// Associates a command that is being executed if the tray icon is being
    /// left-clicked.
    /// </summary>
    [Category(CategoryName)]
    [Description("A command that is being executed if the tray icon is being left-clicked.")]
    public ICommand? LeftClickCommand
    {
        get { return (ICommand?) GetValue(LeftClickCommandProperty); }
        set { SetValue(LeftClickCommandProperty, value); }
    }

    #endregion

    #region LeftClickCommandParameter dependency property

    /// <summary>Identifies the <see cref="LeftClickCommandParameter"/> dependency property.</summary>
    public static readonly DependencyProperty LeftClickCommandParameterProperty =
        DependencyProperty.Register(nameof(LeftClickCommandParameter),
            typeof (object),
            typeof (TaskbarIcon),
            new PropertyMetadata(null));

    /// <summary>
    /// A property wrapper for the <see cref="LeftClickCommandParameterProperty"/>
    /// dependency property:<br/>
    /// Command parameter for the <see cref="LeftClickCommand"/>.
    /// </summary>
    [Category(CategoryName)]
    [Description("The target of the command that is fired if the notify icon is clicked with the left mouse button."
        )]
    public object? LeftClickCommandParameter
    {
        get { return GetValue(LeftClickCommandParameterProperty); }
        set { SetValue(LeftClickCommandParameterProperty, value); }
    }

    #endregion

    #region LeftClickCommandTarget dependency property

#if HAS_WPF

    /// <summary>Identifies the <see cref="LeftClickCommandTarget"/> dependency property.</summary>
    public static readonly DependencyProperty LeftClickCommandTargetProperty =
        DependencyProperty.Register(nameof(LeftClickCommandTarget),
            typeof (IInputElement),
            typeof (TaskbarIcon),
            new PropertyMetadata(null));

    /// <summary>
    /// A property wrapper for the <see cref="LeftClickCommandTargetProperty"/>
    /// dependency property:<br/>
    /// The target of the command that is fired if the notify icon is clicked.
    /// </summary>
    [Category(CategoryName)]
    [Description("The target of the command that is fired if the notify icon is clicked with the left mouse button."
        )]
    public IInputElement? LeftClickCommandTarget
    {
        get { return (IInputElement?) GetValue(LeftClickCommandTargetProperty); }
        set { SetValue(LeftClickCommandTargetProperty, value); }
    }

#endif

    #endregion

    #region NoLeftClickDelay dependency property

    /// <summary>Identifies the <see cref="NoLeftClickDelay"/> dependency property.</summary>
    public static readonly DependencyProperty NoLeftClickDelayProperty =
        DependencyProperty.Register(nameof(NoLeftClickDelay),
            typeof(bool),
            typeof(TaskbarIcon),
            new PropertyMetadata(false));


    /// <summary>
    /// A property wrapper for the <see cref="NoLeftClickDelayProperty"/>
    /// dependency property:<br/>
    /// Set to true to make left clicks work without delay.
    /// </summary>
    [Category(CategoryName)]
    [Description("Set to true to make left clicks work without delay.")]
    public bool NoLeftClickDelay
    {
        get { return (bool)GetValue(NoLeftClickDelayProperty); }
        set { SetValue(NoLeftClickDelayProperty, value); }
    }

    #endregion
}
