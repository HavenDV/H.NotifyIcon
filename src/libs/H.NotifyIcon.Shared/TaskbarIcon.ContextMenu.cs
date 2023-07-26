namespace H.NotifyIcon;

[DependencyProperty<PopupActivationMode>("MenuActivation", DefaultValue = PopupActivationMode.RightClick,
    Description = "Defines what mouse events display the context menu.", Category = CategoryName, ClsCompliant = false)]
[RoutedEvent("TrayContextMenuOpen", RoutedEventStrategy.Bubble,
    Description = "Bubbled event that occurs when the context menu of the taskbar icon is being displayed.", Category = CategoryName)]
[RoutedEvent("PreviewTrayContextMenuOpen", RoutedEventStrategy.Tunnel,
    Description = "Tunneled event that occurs when the context menu of the taskbar icon is being displayed.", Category = CategoryName)]
public partial class TaskbarIcon
{
#if HAS_MAUI

    private FlyoutBase ContextFlyout
    {
        get => FlyoutBase.GetContextFlyout(this);
        set => FlyoutBase.SetContextFlyout(this, value);
    }

#endif
}
