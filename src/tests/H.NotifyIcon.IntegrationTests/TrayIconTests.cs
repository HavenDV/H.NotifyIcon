using System.Drawing;
using H.NotifyIcon.Interop;

namespace H.NotifyIcon.IntegrationTests;

[TestClass]
public class TrayIconTests
{
    [TestMethod]
    public async Task SimpleTest()
    {
        using var trayIcon = new TrayIcon(false);
        using var iconStream = H.Resources.Red_ico.AsStream();
        using var icon = new Icon(iconStream);
        trayIcon.SetIcon(icon.Handle);
        trayIcon.SetToolTip(nameof(SimpleTest));

        await Task.Delay(TimeSpan.FromSeconds(15));
    }
}
