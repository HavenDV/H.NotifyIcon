using System.Drawing;
using System.Threading;
using H.NotifyIcon.Interop;

namespace H.NotifyIcon.Core;

/// <inheritdoc/>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.1.2600")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
public class TrayIconWithContextMenu : TrayIcon
{
    /// <summary>
    /// 
    /// </summary>
    public PopupMenu? ContextMenu { get; set; }

    private Thread? Thread { get; set; }
    private uint ThreadId { get; set; }

    /// <inheritdoc/>
    public TrayIconWithContextMenu(Guid id) : base(id)
    {
        MessageWindow.MouseEventReceived += OnMouseEvent;
    }

    /// <inheritdoc/>
    public TrayIconWithContextMenu() : base()
    {
        MessageWindow.MouseEventReceived += OnMouseEvent;
    }

    /// <inheritdoc/>
    public TrayIconWithContextMenu(string name) : base(name)
    {
        MessageWindow.MouseEventReceived += OnMouseEvent;
    }

    /// <inheritdoc/>
    public new void Create()
    {
        if (Thread != null)
        {
            base.Create();
            return;
        }

        // This code is required to support the context menu.
        Thread = new Thread(() =>
        {
            ThreadId = PInvoke.GetCurrentThreadId();

            base.Create();

            WindowUtilities.RunMessageLoop();
        });
        Thread.Start();
    }

    /// <inheritdoc/>
    
    protected override void Dispose(bool disposing)
    {
        if (Thread == null)
        {
            base.Dispose(disposing);
            return;
        }

        _ = PInvoke.PostThreadMessage(
            idThread: ThreadId,
            Msg: PInvoke.WM_QUIT,
            wParam: default,
            lParam: default).EnsureNonZero();

        if (PInvoke.GetCurrentThreadId() != ThreadId)
        {
            Thread.Join();
        }
        Thread = null;

        base.Dispose(disposing);
    }

    private void OnMouseEvent(object? sender, MessageWindow.MouseEventReceivedEventArgs args)
    {
        if (args.MouseEvent == MouseEvent.MouseMove)
        {
            return;
        }

        if (args.MouseEvent == MouseEvent.IconRightMouseUp)
        {
            ShowContextMenu();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShowContextMenu()
    {
        var cursorPosition = CursorUtilities.GetCursorPos();

        _ = WindowUtilities.SetForegroundWindow(WindowHandle);
        ContextMenu?.Show(
            ownerHandle: WindowHandle,
            x: cursorPosition.X,
            y: cursorPosition.Y);
    }
}
