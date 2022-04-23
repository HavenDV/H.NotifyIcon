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
}
