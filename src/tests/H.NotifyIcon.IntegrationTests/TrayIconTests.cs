using System.Drawing;
using System.Reflection;
using H.NotifyIcon.Core;

namespace H.NotifyIcon.IntegrationTests;

[TestClass]
public class TrayIconTests
{
    [TestMethod]
    public void ContextMenuOpeningIsRaisedWithoutContextMenu()
    {
        using var trayIcon = new TrayIconWithContextMenu();
        var wasRaised = false;

        trayIcon.ContextMenuOpening += (_, _) => wasRaised = true;
        trayIcon.ShowContextMenu();

        wasRaised.Should().BeTrue();
    }

    [TestMethod]
    public void MessageWindowCallbackPointSupportsNegativeVirtualScreenCoordinates()
    {
        var method = typeof(MessageWindow).GetMethod(
            name: "ToPoint",
            bindingAttr: BindingFlags.Static | BindingFlags.NonPublic);
        var packed = unchecked((uint)((ushort)-120 | ((ushort)-80 << 16)));

        var point = (Point)method!.Invoke(null, [new UIntPtr(packed)])!;

        point.Should().Be(new Point(-120, -80));
    }

    [TestMethod]
    public async Task SimpleTest()
    {
        using var trayIcon = new TrayIcon();
        trayIcon.Create();
        using var iconStream = H.Resources.Red_ico.AsStream();
        using var icon = new Icon(iconStream);
        trayIcon.UpdateIcon(icon.Handle);
        trayIcon.UpdateToolTip(nameof(SimpleTest));
        trayIcon.ShowNotification("test", "test");

        await Task.Delay(TimeSpan.FromSeconds(15));
    }
}
