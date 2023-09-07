using System.Drawing;
using EventGenerator;
using H.NotifyIcon.Interop;

namespace H.NotifyIcon.Core;

/// <summary>
/// Receives messages from the taskbar icon through
/// window messages of an underlying helper window.
/// </summary>
[Event<bool>("ChangeToolTipStateRequest",
    Description = "The custom tooltip should be closed or hidden.",
    PropertyNames = new[] { "IsVisible" })]
[Event<MouseEvent, Point>("MouseEventReceived",
    Description = "Fired in case the user clicked or moved within the taskbar icon area.",
    PropertyNames = new[] { "MouseEvent", "Point" })]
[Event<KeyboardEvent, Point>("KeyboardEventReceived",
    Description = "Fired in case the user interacted with the taskbar icon area with keyboard shortcuts.",
    PropertyNames = new[] { "KeyboardEvent", "Point" })]
[Event<bool>("BalloonToolTipChanged",
    Description = "Fired if a balloon ToolTip was either displayed or closed (indicated by the boolean flag).",
    PropertyNames = new[] { "IsVisible" })]
[Event("TaskbarCreated",
    Description = "Fired if the taskbar was created or restarted. Requires the taskbar icon to be reset")]
[Event("DpiChanged",
    Description = "Fired if dpi change window message received.")]
[Event("InitMenuPopup",
    Description = "Sent when a drop-down menu or submenu is about to become active. " +
                  "This allows an application to modify the menu before it is displayed, " +
                  "without changing the entire menu.")]
[SupportedOSPlatform("windows5.0")]
public partial class MessageWindow : IDisposable
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
    public bool IsCreated => Handle != 0;

    /// <summary>
    /// Handle for the message window.
    /// </summary>
    public nint Handle => HWND;

    /// <summary>
    /// The version of the underlying icon. Defines how
    /// incoming messages are interpreted.
    /// </summary>
    public IconVersion Version { get; set; } = IconVersion.Win95;

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
            hWndParent: new HWND(IntPtr.Zero),
            hMenu: null,
            hInstance: null,
            lpParam: null).EnsureNonNull();
    }

    #endregion

    #region Event handlers

    private LRESULT OnWindowMessageReceived(
        HWND hWnd,
        uint msg,
        WPARAM wParam,
        LPARAM lParam)
    {
        if (msg == TaskbarRestartMessageId)
        {
            //recreate the icon if the taskbar was restarted (e.g. due to Win Explorer shutdown)
            _ = OnTaskbarCreated();
        }

        ProcessWindowMessage(msg, wParam, lParam);

        return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private static Point ToPoint(nuint value)
    {
        return new Point((int)value & 0xFFFF, ((int)value >> 16) & 0xFFFF);
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/inputdev/mouse-input-notifications
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    private void ProcessWindowMessage(uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (msg != CallbackMessageId)
        {
            switch (msg)
            {
                case PInvoke.WM_INITMENUPOPUP:
                    _ = OnInitMenuPopup();
                    break;
                
                case PInvoke.WM_DPICHANGED:
                    _ = OnDpiChanged();
                    break;
            }
            return;
        }

        var point = Version switch
        {
            IconVersion.Vista => ToPoint(wParam),
            _ => CursorUtilities.GetCursorPos(),
        };

        switch ((uint)lParam.Value)
        {
            // Can come from both mouse and keyboard events
            case PInvoke.WM_CONTEXTMENU:
                _ = OnKeyboardEventReceived(KeyboardEvent.ContextMenu, point);
                break;

            case PInvoke.WM_MOUSEMOVE:
                _ = OnMouseEventReceived(MouseEvent.MouseMove, point);
                break;

            case PInvoke.WM_LBUTTONDOWN:
                _ = OnMouseEventReceived(MouseEvent.IconLeftMouseDown, point);
                break;

            case PInvoke.WM_LBUTTONUP:
                if (!IsDoubleClick)
                {
                    _ = OnMouseEventReceived(MouseEvent.IconLeftMouseUp, point);
                }
                IsDoubleClick = false;
                break;

            case PInvoke.WM_LBUTTONDBLCLK:
                IsDoubleClick = true;
                _ = OnMouseEventReceived(MouseEvent.IconDoubleClick, point);
                _ = OnMouseEventReceived(MouseEvent.IconLeftDoubleClick, point);
                break;

            case PInvoke.WM_RBUTTONDOWN:
                _ = OnMouseEventReceived(MouseEvent.IconRightMouseDown, point);
                break;

            case PInvoke.WM_RBUTTONUP:
                _ = OnMouseEventReceived(MouseEvent.IconRightMouseUp, point);
                break;

            case PInvoke.WM_RBUTTONDBLCLK:
                _ = OnMouseEventReceived(MouseEvent.IconRightDoubleClick, point);
                break;

            case PInvoke.WM_MBUTTONDOWN:
                _ = OnMouseEventReceived(MouseEvent.IconMiddleMouseDown, point);
                break;

            case PInvoke.WM_MBUTTONUP:
                _ = OnMouseEventReceived(MouseEvent.IconMiddleMouseUp, point);
                break;

            case PInvoke.WM_MBUTTONDBLCLK:
                _ = OnMouseEventReceived(MouseEvent.IconMiddleDoubleClick, point);
                break;

            case PInvoke.NIN_BALLOONSHOW:
                _ = OnBalloonToolTipChanged(isVisible: true);
                break;

            case PInvoke.NIN_BALLOONHIDE:
            case PInvoke.NIN_BALLOONTIMEOUT:
                _ = OnBalloonToolTipChanged(isVisible: false);
                break;

            case PInvoke.NIN_BALLOONUSERCLICK:
                _ = OnMouseEventReceived(MouseEvent.BalloonToolTipClicked, point);
                break;

            case PInvoke.NIN_POPUPOPEN:
                _ = OnChangeToolTipStateRequest(isVisible: true);
                break;

            case PInvoke.NIN_POPUPCLOSE:
                _ = OnChangeToolTipStateRequest(isVisible: false);
                break;

            case PInvoke.NIN_SELECT:
                _ = OnKeyboardEventReceived(KeyboardEvent.Select, point);
                break;

            case PInvoke.NIN_SELECT | PInvoke.NINF_KEY:
                _ = OnKeyboardEventReceived(KeyboardEvent.KeySelect, point);
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
        _ = PInvoke.DestroyWindow(HWND); // .EnsureNonZero() Dispose should not throw.
    }

    #endregion
}
