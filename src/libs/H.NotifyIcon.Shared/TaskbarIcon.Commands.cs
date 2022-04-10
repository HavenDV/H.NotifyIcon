namespace H.NotifyIcon;

/// <inheritdoc/>
public partial class TaskbarIcon
{
    #region Properties

    #region DoubleClickCommand

    /// <summary>Identifies the <see cref="DoubleClickCommand"/> dependency property.</summary>
    public static readonly DependencyProperty DoubleClickCommandProperty =
        DependencyProperty.Register(
            nameof(DoubleClickCommand),
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

    #region DoubleClickCommandParameter

    /// <summary>Identifies the <see cref="DoubleClickCommandParameter"/> dependency property.</summary>
    public static readonly DependencyProperty DoubleClickCommandParameterProperty =
        DependencyProperty.Register(
            nameof(DoubleClickCommandParameter),
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

    #region DoubleClickCommandTarget

#if HAS_WPF

    /// <summary>Identifies the <see cref="DoubleClickCommandTarget"/> dependency property.</summary>
    public static readonly DependencyProperty DoubleClickCommandTargetProperty =
        DependencyProperty.Register(
            nameof(DoubleClickCommandTarget),
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

    #region LeftClickCommand

    /// <summary>Identifies the <see cref="LeftClickCommand"/> dependency property.</summary>
    public static readonly DependencyProperty LeftClickCommandProperty =
        DependencyProperty.Register(
            nameof(LeftClickCommand),
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

    #region LeftClickCommandParameter

    /// <summary>Identifies the <see cref="LeftClickCommandParameter"/> dependency property.</summary>
    public static readonly DependencyProperty LeftClickCommandParameterProperty =
        DependencyProperty.Register(
            nameof(LeftClickCommandParameter),
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

    #region LeftClickCommandTarget

#if HAS_WPF

    /// <summary>Identifies the <see cref="LeftClickCommandTarget"/> dependency property.</summary>
    public static readonly DependencyProperty LeftClickCommandTargetProperty =
        DependencyProperty.Register(
            nameof(LeftClickCommandTarget),
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

    #region NoLeftClickDelay

    /// <summary>Identifies the <see cref="NoLeftClickDelay"/> dependency property.</summary>
    public static readonly DependencyProperty NoLeftClickDelayProperty =
        DependencyProperty.Register(
            nameof(NoLeftClickDelay),
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

    #endregion
}
