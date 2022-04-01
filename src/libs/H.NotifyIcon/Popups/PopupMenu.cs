using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace H.NotifyIcon.Core;

/// <inheritdoc/>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
public class PopupMenu : IDisposable, IList<PopupItem>
{
    private readonly HMENU _hMenu;
    private List<PopupItem> _items;
    private bool _disposed;

    /// <inheritdoc/>
    public PopupMenu()
    {
        _hMenu = PInvoke.CreatePopupMenu();
        _items = new();
    }

    /// <inheritdoc/>
    ~PopupMenu()
    {
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public int Count => _items.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public PopupItem this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    /// <inheritdoc/>
    public void Add(PopupItem item)
    {
        if (!item.Visible)
        {
            return;
        }

        if (!item.Id.HasValue)
        {
            item.Id = NextId + 1;
        }

        var result = PInvoke.AppendMenu(
            hMenu: _hMenu,
            uFlags: item.NativeFlags,
            uIDNewItem: (nuint)item.Id,
            lpNewItem: item.NativeHandle);
        if (!result)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        _items.Add(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Contains(PopupItem item)
    {
        return _items.Contains(item);
    }

    /// <inheritdoc/>
    public void CopyTo(PopupItem[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<PopupItem> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(PopupItem item)
    {
        return _items.IndexOf(item);
    }

    /// <inheritdoc/>
    public void Insert(int index, PopupItem item)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Remove(PopupItem item)
    {
        RemoveAt(IndexOf(item));
        return true;
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        PInvoke.DeleteMenu(_hMenu, (uint)index, MENU_ITEM_FLAGS.MF_BYPOSITION);
        _items.RemoveAt(index);
    }

    /// <inheritdoc/>
    public void Show(IntPtr ownerHandle, int x, int y)
    {
        var flags =
            TRACK_POPUP_MENU_FLAGS.TPM_RETURNCMD |
            TRACK_POPUP_MENU_FLAGS.TPM_NONOTIFY;
        
        BOOL id;

        unsafe
        {
            id = PInvoke.TrackPopupMenuEx(
                hMenu: new SafeFileHandle(_hMenu, false),
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
            var errorCode = Marshal.GetLastWin32Error();
            return;
        }

        var item = SearchForId(this, id.Value);
        if (item is PopupMenuItem menuItem)
        {
            menuItem.OnClick();
        }
    }

    /// <inheritdoc/>
    public int NextId
    {
        get
        {
            int itemCount = Count;

            foreach (var item in this)
            {
                if (item is PopupMenuItem menuItem && menuItem.SubMenu is PopupMenu subMenu)
                {
                    itemCount += subMenu.NextId;
                }
            }

            return itemCount;
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
                var foundItems = SearchForId(menuItem.SubMenu, itemId);
                if (foundItems != null)
                    return foundItems;
            }
        }

        return null;
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            PInvoke.DestroyMenu(_hMenu);
            _disposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
