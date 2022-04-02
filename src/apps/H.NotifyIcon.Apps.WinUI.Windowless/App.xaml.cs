namespace H.NotifyIcon.Apps;

public sealed partial class App
{
    #region Properties

    public static TaskbarIcon? TaskbarIcon { get; private set; }

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
        TaskbarIcon = (TaskbarIcon)Resources["TaskbarIcon"];
    }

    #endregion
}
