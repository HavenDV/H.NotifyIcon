using H.NotifyIcon.Core;

namespace H.NotifyIcon.IntegrationTests;

[TestClass]
public class TrayInfoTests
{
    [TestMethod]
    public void GetTrayLocationTest()
    {
        var location = TrayInfo.GetTrayLocation();

        Console.WriteLine($"Location: {location}");
    }

    [TestMethod]
    public void IsShellOpenTest()
    {
        var isShellOpen = TrayInfo.IsShellOpen();
        
        Console.WriteLine($"IsShellOpen: {isShellOpen}");
    }

    [TestMethod]
    public void IsNotifyIconOverflowWindowOpenTest()
    {
        Console.WriteLine($"IsNotifyIconOverflowWindowOpen: {TrayInfo.IsNotifyIconOverflowWindowOpen()}");
    }
}
