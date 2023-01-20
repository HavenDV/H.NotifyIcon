namespace H.NotifyIcon.Apps;

public sealed partial class App
{
    #region Properties

    public static TaskbarIcon? TrayIcon { get; private set; }
    public static Window? Window { get; set; }

    public static bool option = false;

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
        var exitApplicationCommand = (XamlUICommand)Resources["ExitApplicationCommand"];
        exitApplicationCommand.ExecuteRequested += ExitApplicationCommand_ExecuteRequested;

        var commandOne = (XamlUICommand)Resources["Command1"];
        commandOne.ExecuteRequested += commandOne_ExecuteRequested;

        TrayIcon = (TaskbarIcon)Resources["TrayIcon"];
        TrayIcon.ForceCreate();
    }

    private void ExitApplicationCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
    {
        TrayIcon?.Dispose();
        Window?.Close();
    }

    private void commandOne_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
    {
        option = !option;
        var commandOne = (XamlUICommand)Resources["Command1"];
        if (option)
        {
            var iconHostOne = (XamlUICommand)Resources["IconHost1"];
            commandOne.IconSource = iconHostOne.IconSource;
            // This way works but as you can see it kind of a bad practice,
            // I would like to be able to do like below instead:
            // commandOne.IconSource.SymbolIconSource.Symbol = "Cancel";
        }
        else
        {
            var iconHostTwo = (XamlUICommand)Resources["IconHost2"];
            commandOne.IconSource = iconHostTwo.IconSource;
        }
        // Also, I'm hoping it is possible to have an option like below,
        // to make it easier for to toggle multiple options altogether:
        // 
        //   < XamlUICommand
        //       x: Key = "Command0"
        //       Label = "Command0"
        //       Description = "Command0"
        // --->  CloseMenuAfterClick = "False"
        //       >
        //       < XamlUICommand.IconSource >
        //           < SymbolIconSource Symbol = "Cancel" />
        //       </ XamlUICommand.IconSource >
        //   </ XamlUICommand >
    }

    #endregion
}
