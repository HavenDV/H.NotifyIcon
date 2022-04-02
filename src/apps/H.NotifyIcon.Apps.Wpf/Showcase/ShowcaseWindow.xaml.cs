using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using H.NotifyIcon.Interop;

namespace NotifyIconWpf.Sample.ShowCases.Showcase;

/// <summary>
/// Interaction logic for ShowcaseWindow.xaml
/// </summary>
public partial class ShowcaseWindow : Window
{
    public IEnumerable<PopupActivationMode> ActivationModes => Enum
        .GetValues(typeof(PopupActivationMode))
        .Cast<PopupActivationMode>();

    public ShowcaseWindow()
    {
        InitializeComponent();


        Loaded += delegate
        {
            //show balloon at startup
            var balloon = new WelcomeBalloon();
            tb.ShowCustomBalloon(balloon, PopupAnimation.Slide, 12000);
        };
    }


    /// <summary>
    /// Displays a balloon tip.
    /// </summary>
    private void showBalloonTip_Click(object sender, RoutedEventArgs e)
    {
        tb.ShowNotification(
            title: txtBalloonTitle.Text,
            message: txtBalloonText.Text,
            icon: rbInfo.IsChecked == true
                ? NotificationIcon.Info
                : NotificationIcon.Error,
            customIcon: rbCustomIcon.IsChecked == true
                ? tb.Icon
                : null);
    }

    private void hideBalloonTip_Click(object sender, RoutedEventArgs e)
    {
        tb.ClearNotifications();
    }


    /// <summary>
    /// Resets the tooltip.
    /// </summary>
    private void removeToolTip_Click(object sender, RoutedEventArgs e)
    {
        tb.TrayToolTip = null;
    }


    private void showCustomBalloon_Click(object sender, RoutedEventArgs e)
    {
        var balloon = new FancyBalloon();
        balloon.BalloonText = customBalloonTitle.Text;
        //show and close after 2.5 seconds
        tb.ShowCustomBalloon(balloon, PopupAnimation.Slide, 5000);
    }

    private void hideCustomBalloon_Click(object sender, RoutedEventArgs e)
    {
        tb.CloseBalloon();
    }


    private void OnNavigationRequest(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri.ToString(),
            // UseShellExecute is default to false on .NET Core while true on .NET Framework.
            // Only this value is set to true, the url link can be opened.
            UseShellExecute = true
        });
        e.Handled = true;
    }


    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        //clean up notifyicon (would otherwise stay open until application finishes)
        tb.Dispose();
        base.OnClosing(e);
    }
}
