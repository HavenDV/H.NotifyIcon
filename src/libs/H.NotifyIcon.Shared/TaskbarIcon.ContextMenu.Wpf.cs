namespace H.NotifyIcon;

public partial class TaskbarIcon
{
    #region Methods

    /// <summary>
    /// Displays the ContextMenu/ContextFlyout if it was set.
    /// </summary>
    private void ShowContextMenu(System.Drawing.Point cursorPosition)
    {
        if (IsDisposed)
        {
            return;
        }

        // raise preview event no matter whether context menu is currently set
        // or not (enables client to set it on demand)
        var args = OnPreviewTrayContextMenuOpen();
        if (args.Handled)
        {
            return;
        }

        if (ContextMenu == null)
        {
            return;
        }

        // use absolute positioning. We need to set the coordinates, or a delayed opening
        // (e.g. when left-clicked) opens the context menu at the wrong place if the mouse
        // is moved!
        ContextMenu.Placement = PlacementMode.AbsolutePoint;
        ContextMenu.HorizontalOffset = cursorPosition.X;
        ContextMenu.VerticalOffset = cursorPosition.Y;
        ContextMenu.IsOpen = true;

        var handle = (nint)0;

        // try to get a handle on the context itself
        if (PresentationSource.FromVisual(ContextMenu) is HwndSource source)
        {
            handle = source.Handle;
        }

        // if we don't have a handle for the popup, fall back to the message sink
        if (handle == 0)
        {
            handle = TrayIcon.WindowHandle;
        }

        // activate the context menu or the message window to track deactivation - otherwise, the context menu
        // does not close if the user clicks somewhere else. With the message window
        // fallback, the context menu can't receive keyboard events - should not happen though
        _ = WindowUtilities.SetForegroundWindow(handle);

        // bubble event
        _ = OnTrayContextMenuOpen();
    }

    #endregion

    #region Event Handlers

    partial void OnContextMenuChanged(ContextMenu? oldValue, ContextMenu? newValue)
    {
        if (oldValue != null)
        {
            //remove the taskbar icon reference from the previously used element
            SetParentTaskbarIcon(oldValue, null);
        }

        if (newValue != null)
        {
            //set this taskbar icon as a reference to the new tooltip element
            SetParentTaskbarIcon(newValue, this);
        }

        UpdateDataContext(newValue, DataContext);
    }


    #endregion
}
