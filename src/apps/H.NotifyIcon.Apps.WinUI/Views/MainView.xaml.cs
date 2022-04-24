namespace H.NotifyIcon.Apps.Views;

public sealed partial class MainView
{
    public MainView()
    {
        InitializeComponent();

        NavigationView.ItemInvoked += NavigationView_ItemInvoked;
    }

    private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var options = new FrameNavigationOptions
        {
            TransitionInfoOverride = args.RecommendedNavigationTransitionInfo,
        };

        switch (((string)args.InvokedItem))
        {
            case "Notifications":
                _ = NavigationViewFrame.NavigateToType(typeof(NotificationView), null, options);
                ((NotificationView)NavigationViewFrame.Content).TaskbarIcon = TaskbarIcon;
                break;
        }
    }
}
