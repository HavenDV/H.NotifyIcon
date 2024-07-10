using Uno.UI.Runtime.Skia.Wpf;

namespace H.NotifyIcon.Apps.Skia;

public partial class MainWindow : System.Windows.Window
{
    public MainWindow()
    {
        InitializeComponent();

        Root.Content = new WpfHost(Dispatcher, () => new global::H.NotifyIcon.Apps.App());
    }
}
