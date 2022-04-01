using System.Runtime.InteropServices;

namespace H.NotifyIcon.Core;

/// <inheritdoc/>
public class PopupMenuItem : PopupItem
{
    /// <inheritdoc/>
    public PopupMenuItem()
    {
    }

    /// <inheritdoc/>
    public PopupMenuItem(string text, EventHandler<EventArgs> onClick)
    {
        Text = text;
        Click += onClick;
    }

    /// <inheritdoc/>
    public event EventHandler<EventArgs>? Click;

    /// <inheritdoc/>
    public bool Checked { get; set; }

    /// <inheritdoc/>
    public bool Enabled { get; set; } = true;

    //public Image? Image { get; set; }

    /// <inheritdoc/>
    public PopupMenu? SubMenu { get; set; }

    /// <inheritdoc/>
    public string Text { get; set; } = string.Empty;

    /// <inheritdoc/>
    public SystemPopupMenuItemBreak Break { get; set; } = SystemPopupMenuItemBreak.None;

    internal override MENU_ITEM_FLAGS NativeFlags
    {
        get
        {
            var flags = (MENU_ITEM_FLAGS)0;

            if (!Enabled)
            {
                flags |= MENU_ITEM_FLAGS.MF_DISABLED;
                flags |= MENU_ITEM_FLAGS.MF_GRAYED;
            }

            if (Checked)
            {
                flags |= MENU_ITEM_FLAGS.MF_CHECKED;
            }

            if (SubMenu != null)
            {
                flags |= MENU_ITEM_FLAGS.MF_POPUP;
            }

            switch (Break)
            {
                case SystemPopupMenuItemBreak.MenuBreak:
                    flags |= MENU_ITEM_FLAGS.MF_MENUBREAK;
                    break;

                case SystemPopupMenuItemBreak.MenuBarBreak:
                    flags |= MENU_ITEM_FLAGS.MF_MENUBARBREAK;
                    break;
            }

            //if (Image != null)
            //{
            //    flags |= MENU_ITEM_FLAGS.MF_BITMAP;
            //}

            return flags;
        }
    }

    internal override PCWSTR NativeHandle
    {
        get
        {
            // HACK: We needed to unsafely convert IntPtr's to char*'s
            var flags = NativeFlags;
            var handle = IntPtr.Zero;

            if (flags.HasFlag(MENU_ITEM_FLAGS.MF_BITMAP))
            {
                //var bitmap = (Bitmap)Image;
                //handle = bitmap.GetHbitmap();
            }
            else if (flags.HasFlag(MENU_ITEM_FLAGS.MF_STRING))
            {
                handle = Marshal.StringToHGlobalUni(Text);
            }

            if (handle == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                unsafe
                {
                    return (char*)handle;
                }
            }
        }
    }

    /// <summary>
    /// Raises the <see cref="Click"/> event.
    /// </summary>
    internal void OnClick()
    {
        Click?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// 
/// </summary>
public enum SystemPopupMenuItemBreak
{
    /// <summary>
    /// 
    /// </summary>
    None = 0,
    /// <summary>
    /// 
    /// </summary>
    MenuBreak,
    /// <summary>
    /// 
    /// </summary>
    MenuBarBreak,
}
