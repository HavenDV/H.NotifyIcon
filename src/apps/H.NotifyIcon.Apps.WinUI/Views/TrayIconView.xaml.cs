using CommunityToolkit.Mvvm.Input;

namespace H.NotifyIcon.Apps.Views;

public sealed partial class TrayIconView
{
    public TrayIconView()
    {
        InitializeComponent();
    }

    [RelayCommand]
    public void ShowHideWindow()
    {
        var window = App.MainWindow;
        if (window == null)
        {
            return;
        }

        if (window.Visible)
        {
            window.Hide();
        }
        else
        {
            window.Show();
        }
    }

    [RelayCommand]
    public void ExitApplication()
    {
        App.HandleClosedEvents = false;
        TrayIcon.Dispose();
        App.MainWindow?.Close();
    }
}
