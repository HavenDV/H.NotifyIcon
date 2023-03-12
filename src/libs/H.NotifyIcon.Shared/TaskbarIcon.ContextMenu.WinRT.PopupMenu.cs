namespace H.NotifyIcon;

public partial class TaskbarIcon
{
    #region Methods

    /// <summary>
    /// Displays the ContextMenu/ContextFlyout if it was set.
    /// </summary>
    private void ShowContextMenuInPopupMenuMode(System.Drawing.Point cursorPosition)
    {
        var menu = new PopupMenu();
        PopulateMenu(menu.Items, ((MenuFlyout)ContextFlyout).Items);
        
#if !MACOS
        var handle = TrayIcon.WindowHandle;

        _ = WindowUtilities.SetForegroundWindow(handle);
        menu.Show(
            ownerHandle: handle,
            x: cursorPosition.X,
            y: cursorPosition.Y);
#endif
    }

    private static void PopulateMenu(ICollection<PopupItem> menuItems, IList<MenuFlyoutItemBase> flyoutItemBases)
    {
        foreach (var flyoutItemBase in flyoutItemBases)
        {
            switch (flyoutItemBase)
            {
                case ToggleMenuFlyoutItem toggleItem:
                    {
                        var item = new PopupMenuItem
                        {
                            Text = toggleItem.Text,
                            Enabled = toggleItem.IsEnabled,
                            Checked = toggleItem.IsChecked,
                        };
                        item.Click += (_, _) =>
                        {
                            toggleItem.Command?.TryExecute(toggleItem.CommandParameter);
                        };
                        menuItems.Add(item);
                        break;
                    }
                case MenuFlyoutItem flyoutItem:
                    {
                        var item = new PopupMenuItem
                        {
                            Text = flyoutItem.Text,
                            Enabled = flyoutItem.IsEnabled,
                        };
                        item.Click += (_, _) =>
                        {
                            flyoutItem.Command?.TryExecute(flyoutItem.CommandParameter);
                        };
                        menuItems.Add(item);
                        break;
                    }
                case MenuFlyoutSeparator:
                    {
                        menuItems.Add(new PopupMenuSeparator());
                        break;
                    }
                case MenuFlyoutSubItem subItem:
                    {
                        var item = new PopupSubMenu
                        {
                            Text = subItem.Text,
                        };
                        menuItems.Add(item);
                        PopulateMenu(item.Items, subItem.Items);
                        break;
                    }
            }
        }
    }

    #endregion
}
