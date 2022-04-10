using System.Diagnostics;
using H.NotifyIcon.Core;

namespace H.NotifyIcon;

/// <summary>
/// A proxy to for a taskbar icon (NotifyIcon) that sits in the system's
/// taskbar notification area ("system tray").
/// </summary>
#if HAS_WINUI || HAS_UNO
[CLSCompliant(false)]
#endif
public partial class TaskbarIcon : FrameworkElement, IDisposable
{
    #region Members

    /// <summary>
    /// Represents the current icon data.
    /// </summary>
    private TrayIcon TrayIcon { get; }

    /// <summary>
    /// Receives messages from the taskbar icon.
    /// </summary>
    public MessageWindow MessageWindow { get; } = new();

    /// <summary>
    /// An action that is being invoked if the
    /// <see cref="singleClickTimer"/> fires.
    /// </summary>
    private Action? singleClickTimerAction;

    /// <summary>
    /// A timer that is used to differentiate between single
    /// and double clicks.
    /// </summary>
    private readonly Timer singleClickTimer;

    /// <summary>
    /// The time we should wait for a double click.
    /// </summary>
    private int DoubleClickWaitTime => NoLeftClickDelay ? 0 : CursorUtilities.GetDoubleClickTime();

    /// <summary>
    /// Indicates whether the taskbar icon has been created or not.
    /// </summary>
    public bool IsCreated => TrayIcon.IsCreated;

    /// <summary>
    /// Indicates whether custom tooltips are supported, which depends
    /// on the OS. Windows Vista or higher is required in order to
    /// support this feature.
    /// </summary>
    public bool SupportsCustomToolTips => TrayIcon.SupportsCustomToolTips;


    /// <summary>
    /// Checks whether a non-tooltip popup is currently opened.
    /// </summary>
    private bool IsPopupOpen
    {
        get
        {
            var popup = TrayPopupResolved;
#if HAS_WPF
            var menu = ContextMenu;
#else
            var menu = ContextFlyout;
#endif
#if HAS_WPF
            var balloon = CustomBalloon;
#else
            var balloon = (Popup?)null;
#endif

#pragma warning disable CA1508 // Avoid dead conditional code
            return popup != null && popup.IsOpen ||
                   menu != null && menu.IsOpen ||
                   balloon != null && balloon.IsOpen;
#pragma warning restore CA1508 // Avoid dead conditional code
        }
    }

    #endregion

    #region Construction

#if HAS_WPF

    static TaskbarIcon()
    {
        VisibilityProperty.OverrideMetadata(
            typeof(TaskbarIcon),
            new PropertyMetadata(Visibility.Visible, VisibilityPropertyChanged));
        DataContextProperty.OverrideMetadata(
            typeof(TaskbarIcon),
            new FrameworkPropertyMetadata(DataContextPropertyChanged));
        ContextMenuProperty.OverrideMetadata(
            typeof(TaskbarIcon),
            new FrameworkPropertyMetadata(ContextMenuPropertyChanged));
    }

#endif
    
    /// <summary>
    /// Initializes the taskbar icon and registers a message listener
    /// in order to receive events from the taskbar area.
    /// </summary>
    public TaskbarIcon()
    {
#if !HAS_WPF
        RegisterPropertyChangedCallback(VisibilityProperty, (_, _) =>
        {
            SetTrayIconVisibility(Visibility);
        });
        RegisterPropertyChangedCallback(DataContextProperty, (_, _) =>
        {
            UpdateDataContext(null, DataContext);
        });
        RegisterPropertyChangedCallback(ContextFlyoutProperty, (_, _) =>
        {
            SetParentTaskbarIcon(ContextFlyout, this);
            UpdateContextFlyoutDataContext(ContextFlyout, null, DataContext);
        });
#endif
        if (!DesignTimeUtilities.IsDesignMode)
        {
            MessageWindow.Create();
        }

        TrayIcon = new TrayIcon()
        {
            WindowHandle = MessageWindow.Handle,
            CallbackMessage = MessageWindow.CallbackMessageId,
        };
        TrayIcon.VersionChanged += (_, version) => MessageWindow.Version = version;
        Loaded += (_, _) =>
        {
            if (DesignTimeUtilities.IsDesignMode)
            {
                return;
            }

            try
            {
                ForceCreate();
            }
            catch (Exception)
            {
                Debugger.Break();
            }
        };
        MessageWindow.DpiChanged += static (_, _) => DpiUtilities.UpdateDpiFactors();
        MessageWindow.TaskbarCreated += OnTaskbarCreated;

        // register event listeners
        MessageWindow.MouseEventReceived += OnMouseEvent;
        MessageWindow.KeyboardEventReceived += OnKeyboardEvent;
        MessageWindow.ChangeToolTipStateRequest += OnToolTipChange;
        MessageWindow.BalloonToolTipChanged += OnBalloonToolTipChanged;

        // init single click / balloon timers
        singleClickTimer = new Timer(DoSingleClickAction);

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
    /// Use it to force create icon if it placed in resources.
    /// </summary>
    public void ForceCreate()
    {
        TrayIcon.Create();

        // Workaround for https://github.com/HavenDV/H.NotifyIcon/issues/14
        using var refreshTrayIcon = new TrayIcon(TrayIcon.CreateUniqueGuidForEntryAssembly("RefreshWorkaround"));
        refreshTrayIcon.Create();
    }

    #endregion

    #region Single Click Timer event

    /// <summary>
    /// Performs a delayed action if the user requested an action
    /// based on a single click of the left mouse.<br/>
    /// This method is invoked by the <see cref="singleClickTimer"/>.
    /// </summary>
    private void DoSingleClickAction(object? state)
    {
        if (IsDisposed)
        {
            return;
        }

        // run action
        var action = singleClickTimerAction;
        if (action != null)
        {
            // cleanup action
            singleClickTimerAction = null;

#if HAS_WPF
            // switch to UI thread
            this.GetDispatcher().Invoke(action);
#elif HAS_UNO && !HAS_WINUI
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
#else
            DispatcherQueue.TryEnqueue(() => action());
#endif
        }
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
        // don't do anything if the component is already disposed
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
            singleClickTimer?.Dispose();
#if HAS_WPF
            balloonCloseTimer.Dispose();
#endif

            MessageWindow.Dispose();
            TrayIcon.Dispose();
            Icon?.Dispose();
        }
    }

    #endregion
}
