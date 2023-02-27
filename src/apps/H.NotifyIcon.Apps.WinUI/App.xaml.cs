using H.NotifyIcon.Apps.Views;

#nullable enable

namespace H.NotifyIcon.Apps;

public sealed partial class App
{
    #region Properties

    public static Window? MainWindow { get; set; }
    public static bool HandleClosedEvents { get; set; } = true;

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
        MainWindow = new Window
        {
            Content = new Frame
            {
                Content = new MainView(),
            },
        };
        MainWindow.Closed += (sender, args) =>
        {
            if (HandleClosedEvents)
            {
                args.Handled = true;
                MainWindow.Hide();
            }
        };
        MainWindow.Activate();
    }

    #endregion
}
