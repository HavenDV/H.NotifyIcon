using System.Drawing;
using H.NotifyIcon.Core;

namespace H.NotifyIcon.IntegrationTests;

[TestClass]
public class TrayIconTests
{
    [TestMethod]
    public async Task SimpleTest()
    {
        using var trayIcon = new TrayIcon();
        trayIcon.CreateAndShow();
        using var iconStream = H.Resources.Red_ico.AsStream();
        using var icon = new Icon(iconStream);
        trayIcon.UpdateIcon(icon.Handle);
        trayIcon.UpdateToolTip(nameof(SimpleTest));
        trayIcon.ShowNotification("test", "test");

        await Task.Delay(TimeSpan.FromSeconds(15));
    }
}
