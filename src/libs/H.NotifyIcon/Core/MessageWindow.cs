using H.NotifyIcon.Interop;

namespace H.NotifyIcon.Core;

/// <summary>
/// Receives messages from the taskbar icon through
/// window messages of an underlying helper window.
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
public class MessageWindow : IDisposable
{
    #region Constants

    /// <summary>
    /// The ID of messages that are received from the the
    /// taskbar icon.
    /// </summary>
    public const int CallbackMessageId = 0x400;

    #endregion

    #region Properties

    /// <summary>
    /// The ID of the message that is being received if the
    /// taskbar is (re)started.
    /// </summary>
    private uint TaskbarRestartMessageId { get; set; }

    /// <summary>
    /// Used to track whether a mouse-up event is just
    /// the aftermath of a double-click and therefore needs
    /// to be suppressed.
    /// </summary>
    private bool IsDoubleClick { get; set; }

    /// <summary>
    /// A delegate that processes messages of the hidden
    /// native window that receives window messages. Storing
    /// this reference makes sure we don't loose our reference
    /// to the message window.
    /// </summary>
    private WNDPROC MessageHandler { get; set; }

    /// <summary>
    /// Window class ID.
    /// </summary>
    internal string WindowId { get; private set; }

    /// <summary>
    /// Handle for the message window.
    /// </summary>
    private HWND HWND { get; set; }

    /// <summary>
    /// Handle for the message window.
    /// </summary>
    public IntPtr Handle => HWND;

    /// <summary>
    /// The version of the underlying icon. Defines how
    /// incoming messages are interpreted.
    /// </summary>
    public IconVersion Version { get; set; } = IconVersion.Win95;

    #endregion

    #region Events

    /// <summary>
    /// The custom tooltip should be closed or hidden.
    /// </summary>
    public event EventHandler<bool>? ChangeToolTipStateRequest;

    /// <summary>
    /// Fired in case the user clicked or moved within
    /// the taskbar icon area.
    /// </summary>
    public event EventHandler<MouseEvent>? MouseEventReceived;

    /// <summary>
    /// Fired in case the user interacted with the taskbar
    /// icon area with keyboard shortcuts.
    /// </summary>
    public event EventHandler<KeyboardEvent>? KeyboardEventReceived;

    /// <summary>
    /// Fired if a balloon ToolTip was either displayed
    /// or closed (indicated by the boolean flag).
    /// </summary>
    public event EventHandler<bool>? BalloonToolTipChanged;

    /// <summary>
    /// Fired if the taskbar was created or restarted. Requires the taskbar
    /// icon to be reset.
    /// </summary>
    public event EventHandler? TaskbarCreated;

    /// <summary>
    /// Fired if dpi change window message received.
    /// </summary>
    public event EventHandler? DpiChanged;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new message sink that receives message from
    /// a given taskbar icon.
    /// </summary>
    public MessageWindow()
    {
        WindowId = "H.NotifyIcon_" + Guid.NewGuid();
        MessageHandler = OnWindowMessageReceived;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates the helper message window that is used
    /// to receive messages from the taskbar icon.
    /// </summary>
    public unsafe void Create()
    {
        fixed (char* className = WindowId)
        {
            var @class = new WNDCLASSW
            {
                lpfnWndProc = MessageHandler,
                lpszClassName = className,
            };

            _ = PInvoke.RegisterClass(@class).EnsureNonZero();
        }

        // Get the message used to indicate the taskbar has been restarted
        // This is used to re-add icons when the taskbar restarts
        TaskbarRestartMessageId = PInvoke.RegisterWindowMessage("TaskbarCreated").EnsureNonZero();

        HWND = PInvoke.CreateWindowEx(
            dwExStyle: 0,
            lpClassName: WindowId,
            lpWindowName: WindowId,
            dwStyle: 0,
            X: 0,
            Y: 0,
            nWidth: 0,
            nHeight: 0,
            hWndParent: PInvoke.HWND_MESSAGE,
            hMenu: null,
            hInstance: null,
            lpParam: null).EnsureNonNull();
    }

    #endregion

    #region Event handlers

    private LRESULT OnWindowMessageReceived(
        HWND hWnd,
        uint messageId,
        WPARAM wParam,
        LPARAM lParam)
    {
        if (messageId == TaskbarRestartMessageId)
        {
            //recreate the icon if the taskbar was restarted (e.g. due to Win Explorer shutdown)
            TaskbarCreated?.Invoke(this, EventArgs.Empty);
        }

        ProcessWindowMessage(messageId, wParam, lParam);

        return PInvoke.DefWindowProc(hWnd, messageId, wParam, lParam);
    }

    /// <summary>
    /// Processes incoming system messages.
    /// </summary>
    /// <param name="msg">Callback ID.</param>
    /// <param name="wParam">If the version is <see cref="IconVersion.Vista"/>
    /// or higher, this parameter can be used to resolve mouse coordinates.
    /// Currently not in use.</param>
    /// <param name="lParam">Provides information about the event.</param>
    private void ProcessWindowMessage(uint msg, WPARAM wParam, LPARAM lParam)
    {
        // Check if it was a callback message
        if (msg != CallbackMessageId)
        {
            // It was not a callback message, but make sure it's not something else we need to process
            switch (msg)
            {
                case PInvoke.WM_DPICHANGED:
                    DpiChanged?.Invoke(this, EventArgs.Empty);
                    break;
            }
            return;
        }

        switch ((uint)lParam.Value)
        {
            case PInvoke.WM_CONTEXTMENU:
                KeyboardEventReceived?.Invoke(this, KeyboardEvent.ContextMenu);
                break;

            case PInvoke.WM_MOUSEMOVE:
                MouseEventReceived?.Invoke(this, MouseEvent.MouseMove);
                break;

            case PInvoke.WM_LBUTTONDOWN:
                MouseEventReceived?.Invoke(this, MouseEvent.IconLeftMouseDown);
                break;

            case PInvoke.WM_LBUTTONUP:
                if (!IsDoubleClick)
                {
                    MouseEventReceived?.Invoke(this, MouseEvent.IconLeftMouseUp);
                }
                IsDoubleClick = false;
                break;

            case PInvoke.WM_LBUTTONDBLCLK:
                IsDoubleClick = true;
                MouseEventReceived?.Invoke(this, MouseEvent.IconDoubleClick);
                break;

            case PInvoke.WM_RBUTTONDOWN:
                MouseEventReceived?.Invoke(this, MouseEvent.IconRightMouseDown);
                break;

            case PInvoke.WM_RBUTTONUP:
                MouseEventReceived?.Invoke(this, MouseEvent.IconRightMouseUp);
                break;

            case PInvoke.WM_RBUTTONDBLCLK:
                //double click with right mouse button - do not trigger event
                break;

            case PInvoke.WM_MBUTTONDOWN:
                MouseEventReceived?.Invoke(this, MouseEvent.IconMiddleMouseDown);
                break;

            case PInvoke.WM_MBUTTONUP:
                MouseEventReceived?.Invoke(this, MouseEvent.IconMiddleMouseUp);
                break;

            case PInvoke.WM_MBUTTONDBLCLK:
                //double click with middle mouse button - do not trigger event
                break;

            case PInvoke.NIN_BALLOONSHOW:
                BalloonToolTipChanged?.Invoke(this, true);
                break;

            case PInvoke.NIN_BALLOONHIDE:
            case PInvoke.NIN_BALLOONTIMEOUT:
                BalloonToolTipChanged?.Invoke(this, false);
                break;

            case PInvoke.NIN_BALLOONUSERCLICK:
                MouseEventReceived?.Invoke(this, MouseEvent.BalloonToolTipClicked);
                break;

            case PInvoke.NIN_POPUPOPEN:
                ChangeToolTipStateRequest?.Invoke(this, true);
                break;

            case PInvoke.NIN_POPUPCLOSE:
                ChangeToolTipStateRequest?.Invoke(this, false);
                break;

            case PInvoke.NIN_SELECT:
                KeyboardEventReceived?.Invoke(this, KeyboardEvent.Select);
                break;

            case PInvoke.NIN_SELECT | PInvoke.NINF_KEY:
                KeyboardEventReceived?.Invoke(this, KeyboardEvent.KeySelect);
                break;

            default:
                //Debug.WriteLine("Unhandled NotifyIcon message ID: " + lParam);
                break;
        }
    }

    #endregion

    #region Dispose

    /// <summary>
    /// Set to true as soon as <c>Dispose</c> has been invoked.
    /// </summary>
    public bool IsDisposed { get; private set; }

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
    /// This destructor will run only if the <see cref="Dispose()"/>
    /// method does not get called. This gives this base class the
    /// opportunity to finalize.
    /// <para>
    /// Important: Do not provide destructor in types derived from
    /// this class.
    /// </para>
    /// </summary>
    ~MessageWindow()
    {
        Dispose(false);
    }

    /// <summary>
    /// Removes the windows hook that receives window
    /// messages and closes the underlying helper window.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

        //always destroy the unmanaged handle (even if called from the GC)
        PInvoke.DestroyWindow(HWND); // .EnsureNonZero() Dispose should not throw.
    }

    #endregion
}
