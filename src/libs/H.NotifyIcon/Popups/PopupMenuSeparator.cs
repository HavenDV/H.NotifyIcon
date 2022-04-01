namespace H.NotifyIcon.Core;

/// <inheritdoc/>
public class PopupMenuSeparator : PopupItem
{
    internal override MENU_ITEM_FLAGS NativeFlags => MENU_ITEM_FLAGS.MF_SEPARATOR;

    internal override PCWSTR NativeHandle => null;
}
