#if !MACOS
namespace H.NotifyIcon;

[RoutedEvent("TrayBalloonTipShown", RoutedEventStrategy.Bubble,
    Description = "Occurs when a balloon ToolTip is displayed.", Category = CategoryName)]
[RoutedEvent("TrayBalloonTipClosed", RoutedEventStrategy.Bubble,
    Description = "Occurs when a balloon ToolTip was closed.", Category = CategoryName)]
[RoutedEvent("TrayBalloonTipClicked", RoutedEventStrategy.Bubble,
    Description = "Occurs when the user clicks on a balloon ToolTip.", Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Methods

    /// <summary>
    /// Displays a balloon notification with the specified title,
    /// text, and predefined icon or custom icon in the taskbar for the specified time period.
    /// </summary>
    /// <param name="title">The title to display on the balloon tip.</param>
    /// <param name="message">The text to display on the balloon tip.</param>
    /// <param name="icon">A symbol that indicates the severity.</param>
    /// <param name="customIcon">A custom icon.</param>
    /// <param name="largeIcon">True to allow large icons (Windows Vista and later).</param>
    /// <param name="sound">If false do not play the associated sound.</param>
    /// <param name="respectQuietTime">
    /// Do not display the balloon notification if the current user is in "quiet time", 
    /// which is the first hour after a new user logs into his or her account for the first time. 
    /// During this time, most notifications should not be sent or shown. 
    /// This lets a user become accustomed to a new computer system without those distractions. 
    /// Quiet time also occurs for each user after an operating system upgrade or clean installation. 
    /// A notification sent with this flag during quiet time is not queued; 
    /// it is simply dismissed unshown. The application can resend the notification later 
    /// if it is still valid at that time. <br/>
    /// Because an application cannot predict when it might encounter quiet time, 
    /// we recommended that this flag always be set on all appropriate notifications 
    /// by any application that means to honor quiet time. <br/>
    /// During quiet time, certain notifications should still be sent because 
    /// they are expected by the user as feedback in response to a user action, 
    /// for instance when he or she plugs in a USB device or prints a document.<br/>
    /// If the current user is not in quiet time, this flag has no effect.
    /// </param>
    /// <param name="realtime">
    /// Windows Vista and later. <br/>
    /// If the balloon notification cannot be displayed immediately, discard it. 
    /// Use this flag for notifications that represent real-time information 
    /// which would be meaningless or misleading if displayed at a later time.  <br/>
    /// For example, a message that states "Your telephone is ringing."
    /// </param>
    /// <param name="timeout">
    /// This member is deprecated as of Windows Vista. <br/>
    /// Notification display times are now based on system accessibility settings. <br/>
    /// The system enforces minimum and maximum timeout values.  <br/>
    /// Values specified in uTimeout that are too large are set to the maximum value. <br/>
    /// Values that are too small default to the minimum value. <br/>
    /// The system minimum and maximum timeout values are currently set at 10 seconds and 30 seconds, respectively.
    /// </param>
#if !HAS_WPF
    [CLSCompliant(false)]
#endif
    public void ShowNotification(
        string title,
        string message,
        NotificationIcon icon = NotificationIcon.None,
        System.Drawing.Icon? customIcon = null,
        bool largeIcon = false,
        bool sound = true,
        bool respectQuietTime = true,
        bool realtime = false,
        TimeSpan? timeout = null)
    {
        EnsureNotDisposed();

        TrayIcon.ShowNotification(
            title: title,
            message: message,
            icon: icon,
            customIcon: customIcon?.Handle,
            largeIcon: largeIcon,
            sound: sound,
            respectQuietTime: respectQuietTime,
            realtime: realtime,
            timeout: timeout);
    }

    /// <summary>
    /// Clears all notifications(active and deffered) by recreating tray icon.
    /// https://docs.microsoft.com/en-us/windows/win32/api/shellapi/ns-shellapi-notifyicondataa#nif_info-0x00000010
    /// There's a way to remove notifications without recreating here,
    /// but I haven't been able to get it to work.
    /// </summary>
    /// <returns></returns>
    public void ClearNotifications()
    {
        EnsureNotDisposed();

        TrayIcon.ClearNotifications();
    }

    #endregion

    #region Event handlers

    /// <summary>
    /// Bubbles events if a balloon ToolTip was displayed
    /// or removed.
    /// </summary>
    /// <param name="args">Whether the ToolTip was just displayed
    /// or removed.</param>
    /// <param name="sender"></param>
    private void OnBalloonToolTipChanged(object? sender, MessageWindow.BalloonToolTipChangedEventArgs args)
    {
        if (args.IsVisible)
        {
            _ = OnTrayBalloonTipShown();
        }
        else
        {
            _ = OnTrayBalloonTipClosed();
        }
    }

    #endregion
}
#endif
