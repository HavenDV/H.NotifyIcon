using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using H.NotifyIcon.Interop;

namespace H.NotifyIcon.Core;

/// <inheritdoc/>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
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

        unsafe
        {
            using var _hMenu = PInvoke.CreatePopupMenu_SafeHandle();

            var lastId = 1;
            foreach (var item in Items)
            {
                if (!item.Visible)
                {
                    continue;
                }

                item.Id = lastId++;
                _ = item switch
                {
                    PopupMenuItem menuItem => PInvoke.AppendMenu(
                        hMenu: _hMenu,
                        uFlags: menuItem.NativeFlags,
                        uIDNewItem: (nuint)item.Id,
                        lpNewItem: menuItem.Text).EnsureNonZero(),
                    PopupMenuSeparator => PInvoke.AppendMenu(
                        hMenu: _hMenu,
                        uFlags: MENU_ITEM_FLAGS.MF_SEPARATOR,
                        uIDNewItem: (nuint)item.Id,
                        lpNewItem: null).EnsureNonZero(),
                    _ => throw new NotImplementedException(),
                };
            }

            id = PInvoke.TrackPopupMenuEx(
                hMenu: _hMenu,
                uFlags: (uint)flags,
                x: x,
                y: y,
                hwnd: new HWND(ownerHandle),
                lptpm: null);
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
            if (item is not PopupMenuItem menuItem)
            {
                continue;
            }

            if (menuItem.Id == itemId)
            {
                return item;
            }
            else if (menuItem.SubMenu != null)
            {
                var foundItems = SearchForId(menuItem.SubMenu.Items, itemId);
                if (foundItems != null)
                {
                    return foundItems;
                }
            }
        }

        return null;
    }
}
