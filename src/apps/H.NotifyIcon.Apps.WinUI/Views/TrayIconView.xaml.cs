using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace H.NotifyIcon.Apps.Views;

[ObservableObject]
public sealed partial class TrayIconView : UserControl
{
    [ObservableProperty]
    private bool _isWindowVisible;
    
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
        IsWindowVisible = window.Visible;
    }

    [RelayCommand]
    public void ExitApplication()
    {
        App.HandleClosedEvents = false;
        TrayIcon.Dispose();
        App.MainWindow?.Close();
    }
}
