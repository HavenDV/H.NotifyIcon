namespace H.NotifyIcon;

[DependencyProperty<ContextMenuMode>("ContextMenuMode", DefaultValue = ContextMenuMode.PopupMenu,
    Description = "Defines the context menu mode.", Category = CategoryName)]
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
        if (ContextFlyout == null)
        {
            return;
        }

        switch (ContextMenuMode)
        {
            case ContextMenuMode.PopupMenu:
                ShowContextMenuInPopupMenuMode(cursorPosition);
                break;

#if !HAS_UNO
            case ContextMenuMode.SecondWindow:
                ShowContextMenuInSecondWindowMode(cursorPosition);
                break;
#endif

            case ContextMenuMode.ActiveWindow:
                {
                    ContextFlyout.Hide();
                    ContextFlyout.ShowAt(this, new FlyoutShowOptions
                    {
                        Placement = FlyoutPlacementMode.Auto,
                        Position = new Point(cursorPosition.X, cursorPosition.Y),
                        ShowMode = FlyoutShowMode.Auto,
                    });
                }
                break;

            default:
                throw new NotImplementedException($"ContextMenuMode: {ContextMenuMode} is not implemented.");
        }

        // bubble event
        _ = OnTrayContextMenuOpen();
    }
    
    #endregion

    #region Event Handlers

    partial void OnContextFlyoutChanged()
    {
        SetParentTaskbarIcon(ContextFlyout, this);
        UpdateContextFlyoutDataContext(ContextFlyout, DataContext);
#if !HAS_UNO
        PrepareContextMenuWindow();
#endif
    }

    private void UpdateContextFlyoutDataContext(
        FlyoutBase flyout,
        object? newValue)
    {
        void UpdateMenuFlyoutDataContext(MenuFlyoutItemBase item)
        {
            UpdateDataContext(item, newValue);

            if (item is MenuFlyoutSubItem subItem)
            {
                foreach (var innerItem in subItem.Items)
                {
                    UpdateMenuFlyoutDataContext(innerItem);
                }
            }
        }

        if (flyout is MenuFlyout menuFlyout)
        {
            foreach (var item in menuFlyout.Items)
            {
                UpdateMenuFlyoutDataContext(item);
            }
        }
    }
    
    #endregion
}
