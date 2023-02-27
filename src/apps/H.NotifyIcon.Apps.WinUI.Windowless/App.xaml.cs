namespace H.NotifyIcon.Apps;

public sealed partial class App
{
    #region Properties

    public TaskbarIcon? TrayIcon { get; private set; }
    public Window? Window { get; set; }
    public bool HandleClosedEvents { get; set; } = true;

    #endregion

    #region Constructors

    public App()
    {
        InitializeComponent();
    }

    #endregion

    #region Event Handlers

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        InitializeTrayIcon();
    }

    private void InitializeTrayIcon()
    {
        var showHideWindowCommand = (XamlUICommand)Resources["ShowHideWindowCommand"];
        showHideWindowCommand.ExecuteRequested += ShowHideWindowCommand_ExecuteRequested;

        var exitApplicationCommand = (XamlUICommand)Resources["ExitApplicationCommand"];
        exitApplicationCommand.ExecuteRequested += ExitApplicationCommand_ExecuteRequested;

        TrayIcon = (TaskbarIcon)Resources["TrayIcon"];
        TrayIcon.ForceCreate();
    }

    private void ShowHideWindowCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
    {
        if (Window == null)
        {
            Window = new Window();
            Window.Closed += (sender, args) =>
            {
                if (HandleClosedEvents)
                {
                    args.Handled = true;
                    Window.Hide();
                }
            };
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
    }

    private void ExitApplicationCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
    {
        HandleClosedEvents = false;
        TrayIcon?.Dispose();
        Window?.Close();

        // https://github.com/HavenDV/H.NotifyIcon/issues/66
        if (Window == null)
        {
            Environment.Exit(0);
        }
    }

    #endregion
}
