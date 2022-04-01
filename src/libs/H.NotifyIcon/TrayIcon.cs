namespace H.NotifyIcon.Interop;

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
    #region Fields

    private NOTIFYICONDATAW32 iconData32;
    private NOTIFYICONDATAW64 iconData64;

    #endregion

    #region Properties

    /// <summary>
    /// Unique ID.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Receives messages from the taskbar icon.
    /// </summary>
    public WindowMessageSink MessageSink { get; }

    /// <summary>
    /// Indicates whether the taskbar icon has been created or not.
    /// </summary>
    public bool IsCreated { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsDesignMode { get; }

    /// <summary>
    /// 
    /// </summary>
    public string ToolTipText => Environment.Is64BitProcess
        ? iconData64.szTip.ToString()
        : iconData32.szTip.ToString();

    /// <summary>
    /// Handle to the window that receives notification messages associated with an icon in the
    /// taskbar status area. The Shell uses hWnd and uID to identify which icon to operate on
    /// when Shell_NotifyIcon is invoked.
    /// </summary>
    public nint WindowHandle => Environment.Is64BitProcess
        ? iconData64.hWnd
        : iconData32.hWnd;

    /// <summary>
    /// Application-defined identifier of the taskbar icon. The Shell uses hWnd and uID to identify
    /// which icon to operate on when Shell_NotifyIcon is invoked. You can have multiple icons
    /// associated with a single hWnd by assigning each a different uID. This feature, however
    /// is currently not used.
    /// </summary>
    public uint TaskbarIconId => Environment.Is64BitProcess
        ? iconData64.uID
        : iconData32.uID;

    /// <summary>
    /// Flags that indicate which of the other members contain valid data. This member can be
    /// a combination of the NIF_XXX constants.
    /// </summary>
    public uint ValidMembers => Environment.Is64BitProcess
        ? (uint)iconData64.uFlags
        : (uint)iconData32.uFlags;

    /// <summary>
    /// Application-defined message identifier. The system uses this identifier to send
    /// notifications to the window identified in hWnd.
    /// </summary>
    public uint CallbackMessageId => Environment.Is64BitProcess
        ? iconData64.uCallbackMessage
        : iconData32.uCallbackMessage;

    /// <summary>
    /// A handle to the icon that should be displayed. Just
    /// <c>Icon.Handle</c>.
    /// </summary>
    public IntPtr IconHandle => Environment.Is64BitProcess
        ? iconData64.hIcon
        : iconData32.hIcon;

    /// <summary>
    /// State of the icon. Remember to also set the <see cref="StateMask"/>.
    /// </summary>
    public uint IconState => Environment.Is64BitProcess
        ? iconData64.dwState
        : iconData32.dwState;

    /// <summary>
    /// A value that specifies which bits of the state member are retrieved or modified.
    /// For example, setting this member to Hidden.
    /// causes only the item's hidden
    /// state to be retrieved.
    /// </summary>
    public uint StateMask => Environment.Is64BitProcess
        ? iconData64.dwStateMask
        : iconData32.dwStateMask;

    /// <summary>
    /// String with the text for a balloon ToolTip. It can have a maximum of 255 characters.
    /// To remove the ToolTip, set the NIF_INFO flag in uFlags and set szInfo to an empty string.
    /// </summary>
    public string BalloonText => Environment.Is64BitProcess
        ? iconData64.szInfo.ToString()
        : iconData32.szInfo.ToString();

    /// <summary>
    /// Current version. Updates after <see cref="Create"/>.
    /// </summary>
    public NotifyIconVersion Version { get; private set; } = NotifyIconVersion.Vista;

    /// <summary>
    /// String containing a title for a balloon ToolTip. This title appears in boldface
    /// above the text. It can have a maximum of 63 characters.
    /// </summary>
    public string BalloonTitle => Environment.Is64BitProcess
        ? iconData64.szInfoTitle.ToString()
        : iconData32.szInfoTitle.ToString();

    /// <summary>
    /// Adds an icon to a balloon ToolTip, which is placed to the left of the title. If the
    /// <see cref="BalloonTitle"/> member is zero-length, the icon is not shown.
    /// </summary>
    public uint BalloonFlags => Environment.Is64BitProcess
        ? iconData64.dwInfoFlags
        : iconData32.dwInfoFlags;

    /// <summary>
    /// Windows XP (Shell32.dll version 6.0) and later.<br/>
    /// - Windows 7 and later: A registered GUID that identifies the icon.
    ///   This value overrides uID and is the recommended method of identifying the icon.<br/>
    /// - Windows XP through Windows Vista: Reserved.
    /// </summary>
    public Guid TaskbarIconGuid => Environment.Is64BitProcess
        ? iconData64.guidItem
        : iconData32.guidItem;

    /// <summary>
    /// Windows Vista (Shell32.dll version 6.0.6) and later. The handle of a customized
    /// balloon icon provided by the application that should be used independently
    /// of the tray icon. If this member is non-NULL and the User.
    /// flag is set, this icon is used as the balloon icon.<br/>
    /// If this member is NULL, the legacy behavior is carried out.
    /// </summary>
    public IntPtr CustomBalloonIconHandle => Environment.Is64BitProcess
        ? iconData64.hBalloonIcon
        : iconData32.hBalloonIcon;

    /// <summary>
    /// Indicates whether custom tooltips are supported, which depends
    /// on the OS. Windows Vista or higher is required in order to
    /// support this feature.
    /// </summary>
    public bool SupportsCustomToolTips => MessageSink.Version == NotifyIconVersion.Vista;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the taskbar icon and registers a message listener
    /// in order to receive events from the taskbar area.
    /// </summary>
    public TrayIcon(bool isDesignMode)
    {
        IsDesignMode = isDesignMode;
        MessageSink = isDesignMode
            ? WindowMessageSink.CreateEmpty()
            : new WindowMessageSink(NotifyIconVersion.Win95);

        iconData32 = CreateDefault32(MessageSink.MessageWindowHandle, Id);
        iconData64 = CreateDefault64(MessageSink.MessageWindowHandle, Id);

        Create();

        MessageSink.TaskbarCreated += OnTaskbarCreated;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Creates a default data structure that provides
    /// a hidden taskbar icon without the icon being set.
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="id"></param>
    /// <returns>NotifyIconData</returns>
    private static unsafe NOTIFYICONDATAW32 CreateDefault32(
        IntPtr handle,
        Guid id)
    {
        return new NOTIFYICONDATAW32
        {
            cbSize = (uint)sizeof(NOTIFYICONDATAW32),

            hWnd = new HWND(handle),
            guidItem = id,
            uCallbackMessage = WindowMessageSink.CallbackMessageId,

            hIcon = new HICON(IntPtr.Zero),

            dwState = PInvoke.NIS_HIDDEN,
            dwStateMask = PInvoke.NIS_HIDDEN,

            uFlags =
                NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE |
                NOTIFY_ICON_DATA_FLAGS.NIF_ICON |
                NOTIFY_ICON_DATA_FLAGS.NIF_TIP,

            Anonymous =
            {
                uVersion = (uint)NotifyIconVersion.Win95,
            },
        };
    }

    /// <summary>
    /// Creates a default data structure that provides
    /// a hidden taskbar icon without the icon being set.
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="id"></param>
    /// <returns>NotifyIconData</returns>
    private static unsafe NOTIFYICONDATAW64 CreateDefault64(
        IntPtr handle,
        Guid id)
    {
        return new NOTIFYICONDATAW64
        {
            cbSize = (uint)sizeof(NOTIFYICONDATAW64),

            hWnd = new HWND(handle),
            guidItem = id,
            uCallbackMessage = WindowMessageSink.CallbackMessageId,

            hIcon = new HICON(IntPtr.Zero),

            dwState = PInvoke.NIS_HIDDEN,
            dwStateMask = PInvoke.NIS_HIDDEN,

            uFlags =
                NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE |
                NOTIFY_ICON_DATA_FLAGS.NIF_ICON |
                NOTIFY_ICON_DATA_FLAGS.NIF_TIP,

            Anonymous =
            {
                uVersion = (uint)NotifyIconVersion.Win95,
            },
        };
    }

    private bool SendMessage(NOTIFY_ICON_MESSAGE command, NOTIFY_ICON_DATA_FLAGS flags)
    {
        if (IsDesignMode)
        {
            return true;
        }

        BOOL result;
        if (Environment.Is64BitProcess)
        {
            iconData64.uFlags = flags;

            result = PInvoke.Shell_NotifyIcon(command, in iconData64);
        }
        else
        {
            iconData32.uFlags = flags;

            result = PInvoke.Shell_NotifyIcon(command, in iconData32);
        }

        return result;
    }

    private bool SendMessage(NOTIFY_ICON_MESSAGE command)
    {
        return SendMessage(command, Environment.Is64BitProcess
            ? iconData64.uFlags
            : iconData32.uFlags);
    }

    private bool SendModifyMessage(NOTIFY_ICON_DATA_FLAGS flags)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_MODIFY, flags);
    }

    private bool SendAddMessage(NOTIFY_ICON_DATA_FLAGS flags)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_ADD, flags);
    }

    private bool SendDeleteMessage(NOTIFY_ICON_DATA_FLAGS flags)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_DELETE, flags);
    }

    private bool SendSetFocusMessage(NOTIFY_ICON_DATA_FLAGS flags)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_SETFOCUS, flags);
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Creates the taskbar icon. This message is invoked during initialization,
    /// if the taskbar is restarted, and whenever the icon is displayed.
    /// </summary>
    public bool Create()
    {
        if (IsCreated)
        {
            return true;
        }

        if (!SendAddMessage(
            NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE |
            NOTIFY_ICON_DATA_FLAGS.NIF_ICON |
            NOTIFY_ICON_DATA_FLAGS.NIF_TIP))
        {
            // couldn't create the icon - we can assume this is because explorer is not running (yet!)
            // -> try a bit later again rather than throwing an exception. Typically, if the windows
            // shell is being loaded later, this method is being re-invoked from OnTaskbarCreated
            // (we could also retry after a delay, but that's currently YAGNI)
            return false;
        }

        if (!TrayIconMethods.TrySetMostRecentVersion(
            handle: MessageSink.MessageWindowHandle,
            id: Id,
            out var version))
        {
            throw new InvalidOperationException("Could not set version");
        }

        Version = version;
        MessageSink.Version = version;

        IsCreated = true;
        return true;
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

        if (!SendDeleteMessage(NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE))
        {
            return false;
        }
        
        IsCreated = false;
        return true;
    }

    /// <summary>
    /// Sets tooltip message.
    /// </summary>
    public unsafe bool SetToolTip(string text)
    {
        if (Environment.Is64BitProcess)
        {
            fixed (char* p0 = &iconData64.szTip._0)
            {
                text.SetTo(p0, iconData64.szTip.Length);
            }
        }
        else
        {
            fixed (char* p0 = &iconData32.szTip._0)
            {
                text.SetTo(p0, iconData32.szTip.Length);
            }
        }

        return SendModifyMessage(NOTIFY_ICON_DATA_FLAGS.NIF_TIP);
    }

    /// <summary>
    /// Set new icon data.
    /// </summary>
    /// <param name="handle">The title to display on the balloon tip.</param>
    public bool SetIcon(IntPtr handle)
    {
        // Dispose previos?
        if (Environment.Is64BitProcess)
        {
            iconData64.hIcon = new HICON(handle);
        }
        else
        {
            iconData32.hIcon = new HICON(handle);
        }

        return SendModifyMessage(NOTIFY_ICON_DATA_FLAGS.NIF_ICON);
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
    public bool ShowNotification(
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

        var flags = NOTIFY_ICON_DATA_FLAGS.NIF_INFO;
        if (realtime)
        {
            flags |= NOTIFY_ICON_DATA_FLAGS.NIF_REALTIME;
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

        return TrayIconMethods.ShowNotification(
            handle: MessageSink.MessageWindowHandle,
            id: Id,
            flags: flags,
            title: title,
            message: message,
            infoFlags: infoFlags,
            balloonIconHandle: customIcon ?? IntPtr.Zero,
            timeoutInMilliseconds: (uint)(timeout ?? TimeSpan.Zero).TotalMilliseconds);
    }

    /// <summary>
    /// Clears all notifications(active and deffered) by recreating tray icon.
    /// https://docs.microsoft.com/en-us/windows/win32/api/shellapi/ns-shellapi-notifyicondataa#nif_info-0x00000010
    /// There's a way to remove notifications without recreating here,
    /// but I haven't been able to get it to work.
    /// </summary>
    /// <returns></returns>
    public bool ClearNotifications()
    {
        EnsureNotDisposed();

        if (!Remove())
        {
            return false;
        }
        if (!Create())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Hides a balloon ToolTip, if any is displayed.
    /// </summary>
    public bool HideBalloonTip()
    {
        EnsureNotDisposed();

        // reset balloon by just setting the info to an empty string
        if (Environment.Is64BitProcess)
        {
            iconData64.szInfo = default;
            iconData64.szInfoTitle = default;
        }
        else
        {
            iconData32.szInfo = default;
            iconData32.szInfoTitle = default;
        }

        return SendModifyMessage(NOTIFY_ICON_DATA_FLAGS.NIF_INFO);
    }

    #endregion

    #region Event handlers

    /// <summary>
    /// Recreates the taskbar icon if the whole taskbar was
    /// recreated (e.g. because Explorer was shut down).
    /// </summary>
    private void OnTaskbarCreated()
    {
        Remove();
        Create();
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
    private void EnsureNotDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException("TrayIcon is disposed.");
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
    private void Dispose(bool disposing)
    {
        if (IsDisposed || !disposing)
        {
            return;
        }

        IsDisposed = true;
        MessageSink.Dispose();
        _ =Remove();
    }

    #endregion
}
