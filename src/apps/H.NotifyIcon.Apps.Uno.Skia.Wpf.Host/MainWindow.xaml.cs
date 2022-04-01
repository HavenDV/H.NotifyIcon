namespace H.OxyPlot.Apps;

public partial class MainWindow : System.Windows.Window
{
    public MainWindow()
    {
        InitializeComponent();

        Root.Content = new global::Uno.UI.Skia.Platform.WpfHost(Dispatcher, () => new global::H.NotifyIcon.Apps.App());
    }
}
