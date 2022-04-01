using System.Windows.Input;

namespace H.NotifyIcon.Apps.ViewModels;

public class MainViewModel
{
    /// <summary>
    /// Shows a window, if none is already open.
    /// </summary>
    public ICommand ShowHideWindowCommand
    {
        get
        {
            var command = new XamlUICommand();
            command.ExecuteRequested += (sender, e) =>
            {
                var window = App.MainWindow;
                if (window.Visible)
                {
                    //App.MainWindow.Hide();
                }
                else
                {
                    //App.MainWindow.Show();
                }
            };

            return command;
        }
    }


    /// <summary>
    /// Shuts down the application.
    /// </summary>
    public ICommand ExitApplicationCommand
    {
        get
        {
            var command = new XamlUICommand();
            command.ExecuteRequested += (sender, e) =>
            {
                App.MainWindow.Close();
            };

            return command;
        }
    }
}
