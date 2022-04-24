using H.NotifyIcon.Apps.Views;

#nullable enable

namespace H.NotifyIcon.Apps;

public sealed partial class App
{
    #region Properties

    public static Window? MainWindow { get; private set; }

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
        MainWindow.Activate();
    }

    #endregion
}
