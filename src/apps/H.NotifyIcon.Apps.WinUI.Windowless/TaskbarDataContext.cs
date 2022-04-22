using System.Diagnostics;
using System.Windows.Input;

namespace H.NotifyIcon.Apps;

public class TaskbarDataContext
{
    private Window? Window { get; set; }

    public ICommand ShowHideWindowCommand
    {
        get
        {
            var command = new XamlUICommand();
            command.ExecuteRequested += (sender, e) =>
            {
                if (Window == null)
                {
                    Window = new Window();
                    Window.Show();
                    return;
                }

                if (Window.Visible)
                {
                    Window.Hide();
                }
                else
                {
                    Window.Show();
                }
            };

            return command;
        }
    }

    public ICommand ExitApplicationCommand
    {
        get
        {
            var command = new XamlUICommand();
            command.ExecuteRequested += (sender, e) =>
            {
                App.TaskbarIcon?.Dispose();

                if (Window != null)
                {
                    Window.Close();
                }
                else
                {
                    Process.GetCurrentProcess().Kill();
                }
            };

            return command;
        }
    }
}
