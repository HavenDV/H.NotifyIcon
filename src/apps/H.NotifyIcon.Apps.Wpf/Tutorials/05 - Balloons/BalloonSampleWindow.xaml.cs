﻿using System.Drawing;
using System.Windows;
using System.Windows.Controls.Primitives;
using NotifyIconWpf.Sample.ShowCases.Showcase;

namespace NotifyIconWpf.Sample.ShowCases.Tutorials;

/// <summary>
/// Interaction logic for BalloonSampleWindow.xaml
/// </summary>
public partial class BalloonSampleWindow : Window
{
    public BalloonSampleWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        //clean up notifyicon (would otherwise stay open until application finishes)
        MyNotifyIcon.Dispose();

        base.OnClosing(e);
    }


    private void btnShowCustomBalloon_Click(object sender, RoutedEventArgs e)
    {
        var balloon = new FancyBalloon();
        balloon.BalloonText = "Custom Balloon";

        //show balloon and close it after 4 seconds
        MyNotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 4000);
    }

    private void btnCloseCustomBalloon_Click(object sender, RoutedEventArgs e)
    {
        MyNotifyIcon.CloseBalloon();
    }

    private void btnShowStandardBalloon_Click(object sender, RoutedEventArgs e)
    {
        MyNotifyIcon.ShowNotification(
            title: "WPF NotifyIcon",
            message: "This is a standard notification");
    }

    private void btnHideStandardBalloon_Click(object sender, RoutedEventArgs e)
    {
        MyNotifyIcon.ClearNotifications();
    }
}
