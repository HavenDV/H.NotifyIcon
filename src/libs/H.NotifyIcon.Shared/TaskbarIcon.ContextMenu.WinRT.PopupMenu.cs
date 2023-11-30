namespace H.NotifyIcon;

public partial class TaskbarIcon
{
    #region Methods

    /// <summary>
    /// Displays the ContextMenu/ContextFlyout if it was set.
    /// </summary>
    [SupportedOSPlatform("windows5.1.2600")]
    private void ShowContextMenuInPopupMenuMode(System.Drawing.Point cursorPosition)
    {
        var menu = new H.NotifyIcon.Core.PopupMenu();
#if HAS_MAUI
        PopulateMenu(menu.Items, (MenuFlyout)ContextFlyout);
#else
        PopulateMenu(menu.Items, ((MenuFlyout)ContextFlyout).Items);
#endif

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
#if !HAS_MAUI
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
#endif
                case MenuFlyoutSeparator separator:
                    {
                        // The following line of code ensure that this code block is not trimed when PublishTrimmed is true.
                        var a = separator;
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
#if HAS_MAUI
                        PopulateMenu(item.Items, subItem);
#else
                        PopulateMenu(item.Items, subItem.Items);
#endif
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
            }
        }
    }

#endregion
}
