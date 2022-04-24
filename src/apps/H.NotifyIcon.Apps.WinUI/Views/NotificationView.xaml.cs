namespace H.NotifyIcon.Apps.Views;

public sealed partial class NotificationView
{
    public TaskbarIcon? TrayIcon { get; set; }

    public NotificationView()
    {
        InitializeComponent();
    }

    private void ShowNotificationButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedIcon = (Type.SelectedItem as RadioButton)?.Content;

        TrayIcon?.ShowNotification(
            title: TitleTextBox.Text,
            message: MessageTextBox.Text,
            icon: selectedIcon switch
            {
                "None" => NotificationIcon.None,
                "Information" => NotificationIcon.Info,
                "Warning" => NotificationIcon.Warning,
                "Error" => NotificationIcon.Error,
                _ => NotificationIcon.None,
            },
            customIcon: selectedIcon switch
            {
                "Custom" => TrayIcon.Icon,
                _ => null,
            },
            //largeIcon: LargeIconCheckBox.IsChecked ?? false,
            sound: SoundCheckBox.IsChecked ?? true,
            respectQuietTime: true,
            realtime: false,
            timeout: null);
    }

    private void ClearNotificationsButton_Click(object sender, RoutedEventArgs e)
    {
        TrayIcon?.ClearNotifications();
    }
}
