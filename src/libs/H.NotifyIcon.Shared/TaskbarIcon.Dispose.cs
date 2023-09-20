namespace H.NotifyIcon;

public partial class TaskbarIcon : IDisposable
{
    /// <summary>
    /// Set to true as soon as <c>Dispose</c> has been invoked.
    /// </summary>
    public bool IsDisposed { get; private set; }
    
    [SupportedOSPlatform("windows5.1.2600")]
    private void DisposeAfterExit()
    {
#if HAS_WPF
        // register listener in order to get notified when the application closes
        if (Application.Current != null)
        {
            Application.Current.Exit += OnExit;
        }
#elif HAS_MAUI
        MauiAppBuilderExtensions.WindowClosed += OnExit;
#endif
    }

    private void EnsureNotDisposed()
    {
        if (IsDisposed)
        {
#if HAS_MAUI            
            throw new ObjectDisposedException(GetType().FullName);
#else
            throw new ObjectDisposedException(Name ?? nameof(TaskbarIcon));
#endif
        }
    }

#if HAS_WPF || HAS_MAUI
    [SupportedOSPlatform("windows5.1.2600")]
    private void OnExit(object? sender, EventArgs e)
    {
        Dispose();
    }
#endif

    /// <summary>
    /// This destructor will run only if the <see cref="Dispose()"/>
    /// method does not get called. This gives this base class the
    /// opportunity to finalize.
    /// <para>
    /// Important: Do not provide destructor in types derived from this class.
    /// </para>
    /// </summary>
    [SupportedOSPlatform("windows5.1.2600")]
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
    [SupportedOSPlatform("windows5.1.2600")]
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
    [SupportedOSPlatform("windows5.1.2600")]
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
#elif HAS_MAUI
            MauiAppBuilderExtensions.WindowClosed -= OnExit;
#endif

            // stop timers
            SingleClickTimer.Dispose();
#if HAS_WPF
            balloonCloseTimer.Dispose();
#endif

#if !HAS_WPF && !HAS_UNO && !HAS_MAUI
            ContextMenuWindow?.Close();
            ContextMenuWindow = null;
            ContextMenuWindowHandle = null;
            ContextMenuAppWindow = null;
#endif

            TrayIcon.Dispose();
            Icon?.Dispose();
        }
    }
}
