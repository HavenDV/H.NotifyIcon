using H.NotifyIcon.EfficiencyMode;

namespace H.NotifyIcon;

/// <summary>
/// A proxy to for a taskbar icon (NotifyIcon) that sits in the system's
/// taskbar notification area ("system tray").
/// </summary>
[OverrideMetadata<Visibility>("Visibility", DefaultValue = Visibility.Visible)]
[OverrideMetadata<object>("DataContext")]
#if HAS_WPF
[OverrideMetadata<ContextMenu>("ContextMenu")]
#else
[OverrideMetadata<FlyoutBase>("ContextFlyout")]
#endif
#if HAS_WINUI || HAS_UNO || HAS_MAUI
[CLSCompliant(false)]
#endif
#if MACOS || MACCATALYST
[Advice("Starting with macos10.10 Soft-deprecation, forwards message to button, but will be gone in the future.")]
[UnsupportedOSPlatform("macos10.10")]
[UnsupportedOSPlatform("maccatalyst")]
[SupportedOSPlatform("macos")]
#endif
public partial class TaskbarIcon : FrameworkElement
{
    #region Properties

    /// <summary>
    /// Represents the current icon data.
    /// </summary>
    private TrayIcon TrayIcon { get; }

    /// <summary>
    /// Indicates whether the taskbar icon has been created or not.
    /// </summary>
    [SupportedOSPlatform("windows5.1.2600")]
    public bool IsCreated => TrayIcon.IsCreated;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the taskbar icon and registers a message listener
    /// in order to receive events from the taskbar area.
    /// </summary>
    [SupportedOSPlatform("windows5.1.2600")]
    public TaskbarIcon()
    {
#if !HAS_WPF && !HAS_MAUI
        RegisterPropertyChangedCallbacks();
#endif

        TrayIcon = new TrayIcon();
        Loaded += (_, _) =>
        {
            try
            {
                ForceCreate(enablesEfficiencyMode: false);
            }
            catch (Exception)
            {
                Debugger.Break();
            }
        };
        
#if !MACOS
        // https://github.com/HavenDV/H.NotifyIcon/issues/34
        //Unloaded += (_, _) => Dispose();
        TrayIcon.MessageWindow.DpiChanged += static (_, _) => DpiUtilities.UpdateDpiFactors();
        TrayIcon.MessageWindow.TaskbarCreated += OnTaskbarCreated;

        // register event listeners
        TrayIcon.MessageWindow.MouseEventReceived += OnMouseEvent;
#if HAS_MAUI
        BindingContextChanged += OnBindingContextChanged;
#else
        TrayIcon.MessageWindow.KeyboardEventReceived += OnKeyboardEvent;
        TrayIcon.MessageWindow.ChangeToolTipStateRequest += OnToolTipChange;
        TrayIcon.MessageWindow.BalloonToolTipChanged += OnBalloonToolTipChanged;
#endif
#endif

        // init single click / balloon timers
        SingleClickTimer = new Timer(DoSingleClickAction);

#if HAS_WPF
        balloonCloseTimer = new Timer(CloseBalloonCallback);
#endif

        DisposeAfterExit();
    }

    #endregion

    #region Event handlers

    /// <summary>
    /// Recreates the taskbar icon if the whole taskbar was
    /// recreated (e.g. because Explorer was shut down).
    /// </summary>
    [SupportedOSPlatform("windows5.1.2600")]
    private void OnTaskbarCreated(object? sender, EventArgs args)
    {
        try
        {
            _ = TrayIcon.TryRemove();
            TrayIcon.Create();
        }
        catch (Exception)
        {
            // ignored.
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Use it to force create icon if it placed in resources. <br/>
    /// This also turns on Efficiency Mode by default, meaning you run the app in a hidden state.
    /// </summary>
    [SupportedOSPlatform("windows5.1.2600")]
    public void ForceCreate(bool enablesEfficiencyMode = true)
    {
        TrayIcon.Create();

        if (enablesEfficiencyMode &&
            Environment.OSVersion.Platform == PlatformID.Win32NT &&
            Environment.OSVersion.Version >= new Version(6, 2))
        {
#pragma warning disable CA1416 // Validate platform compatibility
            EfficiencyModeUtilities.SetEfficiencyMode(true);
#pragma warning restore CA1416 // Validate platform compatibility
        }

        // Workaround for https://github.com/HavenDV/H.NotifyIcon/issues/14
        // This seems to have been fixed in Windows 22598.1 but I'll leave it here for now
        //using var refreshTrayIcon = new TrayIcon(TrayIcon.CreateUniqueGuidForEntryAssembly("RefreshWorkaround"));
        //refreshTrayIcon.Create();
    }

    #endregion
}
