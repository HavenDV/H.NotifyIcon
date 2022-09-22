using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;

namespace NotifyIconWpf.Sample.Windowless;

/// <summary>
/// Provides bindable properties and commands for the NotifyIcon. In this sample, the
/// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
/// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
/// </summary>
public partial class NotifyIconViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ShowWindowCommand))]
    public bool canExecuteShowWindow = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(HideWindowCommand))]
    public bool canExecuteHideWindow;

    /// <summary>
    /// Shows a window, if none is already open.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteShowWindow))]
    public void ShowWindow()
    {
        Application.Current.MainWindow ??= new MainWindow();
        Application.Current.MainWindow.Show(disableEfficiencyMode: true);
        CanExecuteShowWindow = false;
        CanExecuteHideWindow = true;
    }

    /// <summary>
    /// Hides the main window. This command is only enabled if a window is open.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteHideWindow))]
    public void HideWindow()
    {
        Application.Current.MainWindow.Hide(enableEfficiencyMode: true);
        CanExecuteShowWindow = true;
        CanExecuteHideWindow = false;
    }

    /// <summary>
    /// Shuts down the application.
    /// </summary>
    [RelayCommand]
    public void ExitApplication()
    {
        Application.Current.Shutdown();
    }
}
