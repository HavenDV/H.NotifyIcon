using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using H.NotifyIcon.Interop;

namespace H.NotifyIcon.Core;

/// <summary>
/// A Interop proxy to for a taskbar icon (NotifyIcon) that sits in the system's
/// taskbar notification area ("system tray").
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.1.2600")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
public class TrayIcon : IDisposable
{
    #region Properties

    /// <summary>
    /// Unique ID. <br/>
    /// It will be used by the system to store your TrayIcon settings, 
    /// so it is recommended to make it fixed and unique for each application TrayIcon, not random.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Receives messages from the taskbar icon.
    /// </summary>
    public WindowMessageSink MessageSink { get; } = new();

    /// <summary>
    /// Indicates whether the taskbar icon has been created or not.
    /// </summary>
    public bool IsCreated { get; private set; }

    /// <summary>
    /// IsEnabled?
    /// </summary>
    public bool IsDesignMode { get; set; }

    /// <summary>
    /// A handle to the icon that should be displayed. Just
    /// <c>Icon.Handle</c>.
    /// </summary>
    public IntPtr Icon { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ToolTip { get; set; } = string.Empty;

    /// <summary>
    /// Handle to the window that receives notification messages associated with an icon in the
    /// taskbar status area. The Shell uses hWnd and uID to identify which icon to operate on
    /// when Shell_NotifyIcon is invoked.
    /// </summary>
    public nint WindowHandle => MessageSink.MessageWindowHandle;

    /// <summary>
    /// State of the icon. Remember to also set the StateMask.
    /// </summary>
    public IconState Visibility { get; set; } = IconState.Hidden;

    /// <summary>
    /// Current version. Updates after <see cref="Create"/>.
    /// </summary>
    public NotifyIconVersion Version { get; private set; } = NotifyIconVersion.Vista;

    /// <summary>
    /// Indicates whether custom tooltips are supported, which depends
    /// on the OS. Windows Vista or higher is required in order to
    /// support this feature.
    /// </summary>
    public bool SupportsCustomToolTips => MessageSink.Version == NotifyIconVersion.Vista;

    /// <summary>
    /// Windows Vista and later. 
    /// Use the standard tooltip. 
    /// Normally, when uVersion is set to NOTIFYICON_VERSION_4, 
    /// the standard tooltip is suppressed and can be replaced by the application-drawn, 
    /// pop-up UI. If the application wants to show the standard tooltip with NOTIFYICON_VERSION_4, 
    /// it can specify NIF_SHOWTIP to indicate the standard tooltip should still be shown.
    /// </summary>
    public bool UseStandardTooltip { get; set; }

    #endregion

    #region Events

    /// <summary>
    /// TrayIcon was created.<br/>
    /// This can happen in the following cases:<br/>
    /// - Via direct <see cref="Create"/> or <see cref="CreateAndShow"/> call<br/>
    /// - Through the <see cref="ClearNotifications"/> call since its implementation uses TrayIcon re-creation<br/>
    /// - After Explorer has been restarted<br/>
    /// </summary>
    public event EventHandler? Created;

    /// <summary>
    /// TrayIcon was removed.<br/>
    /// This can happen in the following cases:<br/>
    /// - Via direct <see cref="Remove"/> call<br/>
    /// - Through the <see cref="ClearNotifications"/> call since its implementation uses TrayIcon re-creation<br/>
    /// - After Explorer has been restarted<br/>
    /// </summary>
    public event EventHandler? Removed;

    private void OnCreated()
    {
        Created?.Invoke(this, EventArgs.Empty);
    }

    private void OnRemoved()
    {
        Removed?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the taskbar icon and registers a message listener
    /// in order to receive events from the taskbar area.
    /// </summary>
    /// <param name="id">
    /// Unique ID. <br/>
    /// It will be used by the system to store your TrayIcon settings, 
    /// so it is recommended to make it fixed and unique for each application TrayIcon, not random.
    /// </param>
    /// <param name="isDesignMode"></param>
    public TrayIcon(Guid id, bool isDesignMode = false)
    {
        Id = id;
        IsDesignMode = isDesignMode;
        if (!IsDesignMode)
        {
            MessageSink.Create();
        }
    }

    /// <summary>
    /// Initializes the taskbar icon and registers a message listener
    /// in order to receive events from the taskbar area. <br/>
    /// Creates <see cref="Id"/> based on the simple name of an Entry assembly. <br/>
    /// Use other overloads to create multiple icons for the same application.
    /// </summary>
    /// <param name="isDesignMode"></param>
    public TrayIcon(bool isDesignMode = false) : this(CreateUniqueGuidForEntryAssembly(), isDesignMode)
    {
    }

    /// <summary>
    /// Initializes the taskbar icon and registers a message listener
    /// in order to receive events from the taskbar area. <br/>
    /// Creates <see cref="Id"/> based on the specified name. <br/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isDesignMode"></param>
    public TrayIcon(string name, bool isDesignMode = false) : this(CreateUniqueGuidFromString(name), isDesignMode)
    {
    }

    #endregion

    #region Static methods

    /// <summary>
    /// Creates a unique Guid for the given string using hashing.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static Guid CreateUniqueGuidFromString(string input)
    {
        input = input ?? throw new ArgumentNullException(nameof(input));
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Please enter a non-empty string.", nameof(input));
        }

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

        return new Guid(hash.Take(16).ToArray());
    }

    /// <summary>
    /// Creates a unique Guid for the entry assembly simple name using hashing.
    /// </summary>
    /// <returns></returns>
    public static Guid CreateUniqueGuidForEntryAssembly()
    {
        var assembly =
            Assembly.GetEntryAssembly() ??
            throw new InvalidOperationException("Entry assembly is not found.");
        var name =
            assembly.GetName().Name ??
            throw new InvalidOperationException("Entry assembly should have simple name.");

        return CreateUniqueGuidFromString(name);
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Creates the taskbar icon. This message is invoked during initialization,
    /// if the taskbar is restarted, and whenever the icon is displayed. <br/>
    /// Created icon will be hidden. Use Show() to show it.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public bool Create()
    {
        if (IsCreated)
        {
            return true;
        }

        // We are trying to delete in case the last launch of the application did not clear the icon correctly.
        // Otherwise, in this case TryCreate will return false.
        _ = TrayIconMethods.TryDelete(Id);

        var additionalFlags = (NOTIFY_ICON_DATA_FLAGS)0;
        if (UseStandardTooltip)
        {
            additionalFlags |= NOTIFY_ICON_DATA_FLAGS.NIF_SHOWTIP;
        }

        if (!TrayIconMethods.TryCreate(
            id: Id,
            handle: MessageSink.MessageWindowHandle,
            additionalFlags: additionalFlags,
            toolTip: ToolTip,
            uCallbackMessage: WindowMessageSink.CallbackMessageId,
            iconHandle: new HICON(Icon)))
        {
            // couldn't create the icon - we can assume this is because explorer is not running (yet!)
            // -> try a bit later again rather than throwing an exception. Typically, if the windows
            // shell is being loaded later, this method is being re-invoked from OnTaskbarCreated
            // (we could also retry after a delay, but that's currently YAGNI)
            return false;
        }

        if (!TrayIconMethods.TrySetMostRecentVersion(
            id: Id,
            out var version))
        {
            throw new InvalidOperationException("SetVersion failed.");
        }

        Version = version;
        MessageSink.Version = version;

        IsCreated = true;
        OnCreated();
        MessageSink.TaskbarCreated += OnTaskbarCreated;
        return true;
    }

    /// <summary>
    /// Creates and shows icon.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void CreateAndShow()
    {
        if (!Create())
        {
            throw new InvalidOperationException("Create failed.");
        }

        Show();
    }

    /// <summary>
    /// Closes the taskbar icon if required.
    /// </summary>
    public bool Remove()
    {
        if (!IsCreated)
        {
            return true;
        }

        if (!TrayIconMethods.TryDelete(Id))
        {
            return false;
        }
        
        IsCreated = false;
        OnRemoved();
        MessageSink.TaskbarCreated -= OnTaskbarCreated;
        return true;
    }

    /// <summary>
    /// Sets tooltip message.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public unsafe void UpdateToolTip(string text)
    {
        EnsureNotDisposed();
        EnsureCreated();

        if (!TrayIconMethods.TryModifyToolTip(Id, text))
        {
            throw new InvalidOperationException("UpdateToolTip failed.");
        }
        ToolTip = text;
    }

    /// <summary>
    /// Set new icon data.
    /// </summary>
    /// <param name="handle">The title to display on the balloon tip.</param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void UpdateIcon(IntPtr handle)
    {
        EnsureNotDisposed();
        EnsureCreated();

        if (!TrayIconMethods.TryModifyIcon(Id, handle))
        {
            throw new InvalidOperationException("UpdateIcon failed.");
        }
        Icon = handle;
    }

    /// <summary>
    /// Set new icon state.
    /// </summary>
    /// <param name="visibility"></param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void UpdateVisibility(IconState visibility)
    {
        EnsureNotDisposed();
        EnsureCreated();

        if (!TrayIconMethods.TryModifyState(Id, (uint)visibility))
        {
            throw new InvalidOperationException("UpdateState failed.");
        }
        Visibility = visibility;
    }

    /// <summary>
    /// Shows tray icon.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Show()
    {
        UpdateVisibility(IconState.Visible);
    }

    /// <summary>
    /// Hides tray icon.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void Hide()
    {
        UpdateVisibility(IconState.Hidden);
    }

    /// <summary>
    /// Displays a balloon notification with the specified title,
    /// text, and predefined icon or custom icon in the taskbar for the specified time period.
    /// </summary>
    /// <param name="title">The title to display on the balloon tip.</param>
    /// <param name="message">The text to display on the balloon tip.</param>
    /// <param name="icon">A symbol that indicates the severity.</param>
    /// <param name="customIcon">A custom icon.</param>
    /// <param name="largeIcon">True to allow large icons (Windows Vista and later).</param>
    /// <param name="sound">If false do not play the associated sound.</param>
    /// <param name="respectQuietTime">
    /// Do not display the balloon notification if the current user is in "quiet time", 
    /// which is the first hour after a new user logs into his or her account for the first time. 
    /// During this time, most notifications should not be sent or shown. 
    /// This lets a user become accustomed to a new computer system without those distractions. 
    /// Quiet time also occurs for each user after an operating system upgrade or clean installation. 
    /// A notification sent with this flag during quiet time is not queued; 
    /// it is simply dismissed unshown. The application can resend the notification later 
    /// if it is still valid at that time. <br/>
    /// Because an application cannot predict when it might encounter quiet time, 
    /// we recommended that this flag always be set on all appropriate notifications 
    /// by any application that means to honor quiet time. <br/>
    /// During quiet time, certain notifications should still be sent because 
    /// they are expected by the user as feedback in response to a user action, 
    /// for instance when he or she plugs in a USB device or prints a document.<br/>
    /// If the current user is not in quiet time, this flag has no effect.
    /// </param>
    /// <param name="realtime">
    /// Windows Vista and later. <br/>
    /// If the balloon notification cannot be displayed immediately, discard it. 
    /// Use this flag for notifications that represent real-time information 
    /// which would be meaningless or misleading if displayed at a later time.  <br/>
    /// For example, a message that states "Your telephone is ringing."
    /// </param>
    /// <param name="timeout">
    /// This member is deprecated as of Windows Vista. <br/>
    /// Notification display times are now based on system accessibility settings. <br/>
    /// The system enforces minimum and maximum timeout values.  <br/>
    /// Values specified in uTimeout that are too large are set to the maximum value. <br/>
    /// Values that are too small default to the minimum value. <br/>
    /// The system minimum and maximum timeout values are currently set at 10 seconds and 30 seconds, respectively.
    /// </param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void ShowNotification(
        string title,
        string message,
        NotificationIcon icon = NotificationIcon.None,
        IntPtr? customIcon = null,
        bool largeIcon = false,
        bool sound = true,
        bool respectQuietTime = true,
        bool realtime = false,
        TimeSpan? timeout = null)
    {
        EnsureNotDisposed();
        EnsureCreated();

        var additionalFlags = (NOTIFY_ICON_DATA_FLAGS)0;
        if (realtime)
        {
            additionalFlags |= NOTIFY_ICON_DATA_FLAGS.NIF_REALTIME;
        }

        var infoFlags = customIcon != null
            ? PInvoke.NIIF_USER
            : (uint)icon;
        if (!sound)
        {
            infoFlags |= PInvoke.NIIF_NOSOUND;
        }
        if (respectQuietTime)
        {
            infoFlags |= PInvoke.NIIF_RESPECT_QUIET_TIME;
        }
        if (largeIcon)
        {
            infoFlags |= PInvoke.NIIF_LARGE_ICON;
        }

        if (!TrayIconMethods.TryShowNotification(
            id: Id,
            additionalFlags: additionalFlags,
            title: title,
            message: message,
            infoFlags: infoFlags,
            balloonIconHandle: customIcon ?? IntPtr.Zero,
            timeoutInMilliseconds: (uint)(timeout ?? TimeSpan.Zero).TotalMilliseconds))
        {
            throw new InvalidOperationException("Show notification failed.");
        }
    }

    /// <summary>
    /// Clears all notifications(active and deffered) by recreating tray icon.
    /// https://docs.microsoft.com/en-us/windows/win32/api/shellapi/ns-shellapi-notifyicondataa#nif_info-0x00000010
    /// There's a way to remove notifications without recreating here,
    /// but I haven't been able to get it to work.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void ClearNotifications()
    {
        EnsureNotDisposed();
        EnsureCreated();

        if (!Remove())
        {
            throw new InvalidOperationException("Remove failed.");
        }
        if (!Create())
        {
            throw new InvalidOperationException("Create failed.");
        }
    }

    /// <summary>
    /// Returns focus to the taskbar notification area. 
    /// Notification area icons should use this when they have completed their UI operation. 
    /// For example, if the icon displays a shortcut menu, but the user presses ESC to cancel it, 
    /// use it to return focus to the notification area.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public void SetFocus()
    {
        EnsureNotDisposed();
        EnsureCreated();

        if (!TrayIconMethods.TrySetFocus(Id))
        {
            throw new InvalidOperationException("SetFocus failed.");
        }
    }

    #endregion

    #region Event handlers

    /// <summary>
    /// Recreates the taskbar icon if the whole taskbar was
    /// recreated (e.g. because Explorer was shut down).
    /// </summary>
    private void OnTaskbarCreated()
    {
        try
        {
            _ = Remove();
            _ = Create();
        }
        catch (Exception)
        {
            // ignored.
        }
    }

    #endregion

    #region Dispose

    /// <summary>
    /// Set to true as soon as <c>Dispose</c> has been invoked.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Checks if the object has been disposed and
    /// raises a <see cref="ObjectDisposedException"/> in case
    /// the <see cref="IsDisposed"/> flag is true.
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    private void EnsureNotDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException("TrayIcon is disposed.");
        }
    }

    /// <summary>
    /// Checks if the object has been disposed and
    /// raises a <see cref="InvalidOperationException"/> in case
    /// the <see cref="IsDisposed"/> flag is true.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private void EnsureCreated()
    {
        if (!IsCreated)
        {
            throw new InvalidOperationException("TrayIcon is not created.");
        }
    }

    /// <summary>
    /// This destructor will run only if the <see cref="Dispose()"/>
    /// method does not get called. This gives this base class the
    /// opportunity to finalize.
    /// <para>
    /// Important: Do not provide destructor in types derived from this class.
    /// </para>
    /// </summary>
    ~TrayIcon()
    {
        Dispose(false);
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <remarks>This method is not virtual by design. Derived classes
    /// should override <see cref="Dispose(bool)"/>.
    /// </remarks>
    public void Dispose()
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

        IsDisposed = true;
        MessageSink.Dispose();
        _ = Remove();
    }

    #endregion
}
