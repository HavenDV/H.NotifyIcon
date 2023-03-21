using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;

using H.NotifyIcon.Interop;

namespace H.NotifyIcon.Core;

/// <inheritdoc/>
[SupportedOSPlatform("windows5.0")]
public class PopupMenu
{
    /// <summary>
    /// 
    /// </summary>
    public ICollection<PopupItem> Items { get; } = new List<PopupItem>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ownerHandle"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Show(nint ownerHandle, int x, int y)
    {
        var flags =
           TRACK_POPUP_MENU_FLAGS.TPM_RETURNCMD |
           TRACK_POPUP_MENU_FLAGS.TPM_NONOTIFY |
           TRACK_POPUP_MENU_FLAGS.TPM_BOTTOMALIGN;

        BOOL id;

        var lastId = 1;
        List<DestroyMenuSafeHandle> safeHandles = new();

        void AppendToMenu(HMENU menu, ICollection<PopupItem> items)
        {
            var handle = new DestroyMenuSafeHandle(menu, true);
            safeHandles.Add(handle);

            BOOL AddSubMenu(PopupSubMenu subMenu, int itemId)
            {
                var subMenuHandle = PInvoke.CreatePopupMenu();
                AppendToMenu(subMenuHandle, subMenu.Items);
                return PInvoke.AppendMenu(
                    hMenu: handle,
                    uFlags: MENU_ITEM_FLAGS.MF_POPUP,
                    uIDNewItem: (nuint)subMenuHandle.Value.ToInt64(),
                    lpNewItem: subMenu.Text).EnsureNonZero();
            }

            foreach (var item in items)
            {
                if (item.Visible == false) continue;

                item.Id = lastId++;
                _ = item switch
                {
                    PopupMenuItem menuItem => PInvoke.AppendMenu(
                        hMenu: handle,
                        uFlags: menuItem.NativeFlags,
                        uIDNewItem: (nuint)item.Id,
                        lpNewItem: menuItem.Text).EnsureNonZero(),
                    PopupMenuSeparator => PInvoke.AppendMenu(
                        hMenu: handle,
                        uFlags: MENU_ITEM_FLAGS.MF_SEPARATOR,
                        uIDNewItem: (nuint)item.Id,
                        lpNewItem: null).EnsureNonZero(),
                    PopupSubMenu subMenu => AddSubMenu(subMenu, item.Id),
                    _ => throw new NotImplementedException()
                };
            }
        }

        unsafe
        {
            var _hMenu = PInvoke.CreatePopupMenu();
            AppendToMenu(_hMenu, Items);

            id = PInvoke.TrackPopupMenuEx(
                hMenu: _hMenu,
                uFlags: (uint)flags,
                x: x,
                y: y,
                hwnd: new HWND(ownerHandle),
                lptpm: null);

            var exceptions = new List<Exception>();
            foreach (var handle in safeHandles)
            {
                try
                {
                    handle.Dispose();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }
            if (exceptions.Any()) throw new AggregateException(exceptions);
        }

        // If the user cancels the menu without making a selection,
        // or if an error occurs, the return value is zero.
        if (id.Value == 0)
        {
            //var errorCode = Marshal.GetLastWin32Error();
            return;
        }

        {
            var item = SearchForId(Items, id.Value);
            if (item is PopupMenuItem menuItem)
            {
                menuItem.OnClick();
            }
        }
    }

    private static PopupItem? SearchForId(IEnumerable<PopupItem> items, int itemId)
    {
        foreach (var item in items)
        {
            if (item is PopupSubMenu subMenu)
            {
                var foundItem = SearchForId(subMenu.Items, itemId);
                if (foundItem is not null)
                {
                    return foundItem;
                }
            }

            if (item is not PopupMenuItem menuItem)
            {
                continue;
            }

            if (menuItem.Id == itemId)
            {
                return item;
            }
        }

        return null;
    }
}
