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
#if HAS_WINUI || HAS_UNO
[CLSCompliant(false)]
#endif
#if NET5_0_OR_GREATER
#if MACOS || MACCATALYST
[Advice("Starting with macos10.10 Soft-deprecation, forwards message to button, but will be gone in the future.")]
[System.Runtime.Versioning.UnsupportedOSPlatform("macos10.10")]
[System.Runtime.Versioning.UnsupportedOSPlatform("maccatalyst")]
[System.Runtime.Versioning.SupportedOSPlatform("macos")]
#endif
#endif
public partial class TaskbarIcon : FrameworkElement, IDisposable
{
    #region Properties

    /// <summary>
    /// Represents the current icon data.
    /// </summary>
    private TrayIcon TrayIcon { get; }

    /// <summary>
    /// Indicates whether the taskbar icon has been created or not.
    /// </summary>
    public bool IsCreated => TrayIcon.IsCreated;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the taskbar icon and registers a message listener
    /// in order to receive events from the taskbar area.
    /// </summary>
    public TaskbarIcon()
    {
#if !HAS_WPF
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
        TrayIcon.MessageWindow.KeyboardEventReceived += OnKeyboardEvent;
        TrayIcon.MessageWindow.ChangeToolTipStateRequest += OnToolTipChange;
        TrayIcon.MessageWindow.BalloonToolTipChanged += OnBalloonToolTipChanged;
#endif
        
        // init single click / balloon timers
        SingleClickTimer = new Timer(DoSingleClickAction);

#if HAS_WPF
        balloonCloseTimer = new Timer(CloseBalloonCallback);
#endif

#if HAS_WPF
        // register listener in order to get notified when the application closes
        if (Application.Current != null)
        {
            Application.Current.Exit += OnExit;
        }
#endif
    }

    #endregion

    #region Event handlers

    /// <summary>
    /// Recreates the taskbar icon if the whole taskbar was
    /// recreated (e.g. because Explorer was shut down).
    /// </summary>
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

    #region Dispose / Exit

    /// <summary>
    /// Set to true as soon as <c>Dispose</c> has been invoked.
    /// </summary>
    public bool IsDisposed { get; private set; }


    /// <summary>
    /// Checks if the object has been disposed and
    /// raises a <see cref="ObjectDisposedException"/> in case
    /// the <see cref="IsDisposed"/> flag is true.
    /// </summary>
    private void EnsureNotDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(Name ?? GetType().FullName);
        }
    }


    /// <summary>
    /// Disposes the class if the application exits.
    /// </summary>
    private void OnExit(object sender, EventArgs e)
    {
        Dispose();
    }


    /// <summary>
    /// This destructor will run only if the <see cref="Dispose()"/>
    /// method does not get called. This gives this base class the
    /// opportunity to finalize.
    /// <para>
    /// Important: Do not provide destructor in types derived from this class.
    /// </para>
    /// </summary>
    ~TaskbarIcon()
    {
        Dispose(false);
    }


    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <remarks>This method is not virtual by design. Derived classes
    /// should override <see cref="Dispose(bool)"/>.
    /// </remarks>
#if HAS_UNO
    public new void Dispose()
#else
    public void Dispose()
#endif
    {
        Dispose(true);

        // This object will be cleaned up by the Dispose method.
        // Therefore, you should call GC.SuppressFinalize to
        // take this object off the finalization queue
        // and prevent finalization code for this object
        // from executing a second time.
        GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Closes the tray and releases all resources.
    /// </summary>
    /// <summary>
    /// <c>Dispose(bool disposing)</c> executes in two distinct scenarios.
    /// If disposing equals <c>true</c>, the method has been called directly
    /// or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed.
    /// </summary>
    /// <param name="disposing">If disposing equals <c>false</c>, the method
    /// has been called by the runtime from inside the finalizer and you
    /// should not reference other objects. Only unmanaged resources can
    /// be disposed.</param>
    /// <remarks>Check the <see cref="IsDisposed"/> property to determine whether
    /// the method has already been called.</remarks>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed || !disposing)
        {
            return;
        }

        //lock (lockObject)
        {
            IsDisposed = true;

#if HAS_WPF
            // de-register application event listener
            if (Application.Current != null)
            {
                Application.Current.Exit -= OnExit;
            }
#endif

            // stop timers
            SingleClickTimer?.Dispose();
#if HAS_WPF
            balloonCloseTimer.Dispose();
#endif

#if !HAS_WPF && !HAS_UNO
            ContextMenuWindow?.Close();
            ContextMenuWindow = null;
            ContextMenuWindowHandle = null;
            ContextMenuAppWindow = null;
#endif

            TrayIcon.Dispose();
            Icon?.Dispose();
            GeneratedIcon?.Dispose();
        }
    }

    #endregion
}
