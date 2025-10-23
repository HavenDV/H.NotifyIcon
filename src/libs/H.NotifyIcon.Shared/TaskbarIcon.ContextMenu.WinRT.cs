using System.Diagnostics.CodeAnalysis;

namespace H.NotifyIcon;

[DependencyProperty<ContextMenuMode>("ContextMenuMode", DefaultValue = ContextMenuMode.PopupMenu,
    Description = "Defines the context menu mode.", Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Methods

    /// <summary>
    /// Displays the ContextMenu/ContextFlyout if it was set.
    /// </summary>
    [SupportedOSPlatform("windows5.1.2600")]
    public void ShowContextMenu(System.Drawing.Point cursorPosition)
    {
        if (IsDisposed)
        {
            return;
        }

        // raise preview event no matter whether context menu is currently set
        // or not (enables client to set it on demand)
#if !HAS_MAUI
        var args = OnPreviewTrayContextMenuOpen();
#endif
        if (ContextFlyout == null)
        {
            return;
        }

        switch (ContextMenuMode)
        {
            case ContextMenuMode.PopupMenu:
                ShowContextMenuInPopupMenuMode(cursorPosition);
                break;

#if !HAS_MAUI
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
#endif

            default:
                throw new NotImplementedException($"ContextMenuMode: {ContextMenuMode} is not implemented.");
        }

#if !HAS_MAUI
        _ = OnTrayContextMenuOpen();
#endif
    }

    #endregion

    #region Event Handlers

#if !HAS_MAUI
    partial void OnContextFlyoutChanged()
    {
        SetParentTaskbarIcon(ContextFlyout, this);
        UpdateContextFlyoutDataContext(ContextFlyout, DataContext);
#if !HAS_UNO
        PrepareContextMenuWindow();
#endif
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(MenuFlyout))]
    private void UpdateContextFlyoutDataContext(
        FlyoutBase flyout,
        object? newValue)
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(MenuFlyoutSubItem))]
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
   
#endif
    
    #endregion
}
