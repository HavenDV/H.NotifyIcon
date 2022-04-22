using System.Windows;
using System.Windows.Input;
using H.NotifyIcon;

namespace NotifyIconWpf.Sample.Windowless;

/// <summary>
/// Provides bindable properties and commands for the NotifyIcon. In this sample, the
/// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
/// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
/// </summary>
public class NotifyIconViewModel
{
    /// <summary>
    /// Shows a window, if none is already open.
    /// </summary>
    public ICommand ShowWindowCommand => new DelegateCommand
    {
        CanExecuteFunc = () => Application.Current.MainWindow?.Visibility is not Visibility.Visible,
        CommandAction = () =>
        {
            Application.Current.MainWindow ??= new MainWindow();
            Application.Current.MainWindow.Show(disableEfficiencyMode: true);
        },
    };

    /// <summary>
    /// Hides the main window. This command is only enabled if a window is open.
    /// </summary>
    public ICommand HideWindowCommand => new DelegateCommand
    {
        CommandAction = () => Application.Current.MainWindow.Hide(enableEfficiencyMode: true),
        CanExecuteFunc = () => Application.Current.MainWindow?.Visibility is Visibility.Visible,
    };


    /// <summary>
    /// Shuts down the application.
    /// </summary>
    public ICommand ExitApplicationCommand => new DelegateCommand { CommandAction = Application.Current.Shutdown };
}
