using System.Drawing;
using H.NotifyIcon.Core;

using var messageWindow = new MessageWindow();
messageWindow.Create();

using var iconStream = H.Resources.Red_ico.AsStream();
using var icon = new Icon(iconStream);
using var trayIcon = new TrayIcon
{
    Icon = icon.Handle,
    ToolTip = "ToolTip",
    WindowHandle = messageWindow.Handle,
};
trayIcon.Create();

Console.WriteLine("TrayIcon created.");
Console.WriteLine("Available commands:");
Console.WriteLine("create-second   - Creates second tray icon");
Console.WriteLine("message [text]  - Shows notification without icon");
Console.WriteLine("warning [text]  - Shows notification with warning icon");
Console.WriteLine("error [text]    - Shows notification with error icon");
Console.WriteLine("custom [text]   - Shows notification with custom icon");
Console.WriteLine("clear           - Clears notifications");
Console.WriteLine("set-focus       - Sets focus on tray icon");
Console.WriteLine("remove          - Removes tray icon");
Console.WriteLine("create          - Creates tray icon");
Console.WriteLine("show            - Shows tray icon");
Console.WriteLine("hide            - Hides tray icon");
Console.WriteLine("exit            - Closes console application and clears resources.");

while (true)
{
    try
    {
        var line = Console.ReadLine() ?? string.Empty;
        if (line.StartsWith("create-second"))
        {
            using var iconStream2 = H.Resources.icon_ico.AsStream();
            using var icon2 = new Icon(iconStream2);
            using var trayIcon2 = new TrayIcon("H.NotifyIcon.Apps.Console.SecondTrayIcon")
            {
                Icon = icon2.Handle,
                ToolTip = "Second Tray Icon",
            };
            trayIcon2.Create();

            Console.WriteLine("Second icon created. It will removed after 5 seconds.");

            await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

            Console.WriteLine("Second icon removed.");
        }
        else if (line.StartsWith("message"))
        {
            trayIcon.ShowNotification(
                title: nameof(NotificationIcon.None),
                message: line.Substring("message".Length).TrimStart(),
                icon: NotificationIcon.None);
            Console.WriteLine(nameof(trayIcon.ShowNotification));
        }
        else if (line.StartsWith("info"))
        {
            trayIcon.ShowNotification(
                title: nameof(NotificationIcon.Info),
                message: line.Substring("info".Length).TrimStart(),
                icon: NotificationIcon.Info);
            Console.WriteLine(nameof(trayIcon.ShowNotification));
        }
        else if (line.StartsWith("warning"))
        {
            trayIcon.ShowNotification(
                title: nameof(NotificationIcon.Warning),
                message: line.Substring("warning".Length).TrimStart(),
                icon: NotificationIcon.Warning);
            Console.WriteLine(nameof(trayIcon.ShowNotification));
        }
        else if (line.StartsWith("error"))
        {
            trayIcon.ShowNotification(
                title: nameof(NotificationIcon.Error),
                message: line.Substring("error".Length).TrimStart(),
                icon: NotificationIcon.Error);
            Console.WriteLine(nameof(trayIcon.ShowNotification));
        }
        else if (line.StartsWith("custom"))
        {
            trayIcon.ShowNotification(
                title: "Custom",
                message: line.Substring("custom".Length).TrimStart(),
                customIcon: icon.Handle);
            Console.WriteLine(nameof(trayIcon.ShowNotification));
        }
        else if (line.StartsWith("clear"))
        {
            trayIcon.ClearNotifications();
            Console.WriteLine(nameof(trayIcon.ClearNotifications));
        }
        else if (line.StartsWith("set-focus"))
        {
            trayIcon.SetFocus();
            Console.WriteLine(nameof(trayIcon.SetFocus));
        }
        else if (line.StartsWith("remove"))
        {
            trayIcon.Remove();
            Console.WriteLine(nameof(trayIcon.Remove));
        }
        else if (line.StartsWith("create"))
        {
            trayIcon.Create();
            Console.WriteLine(nameof(trayIcon.Create));
        }
        else if (line.StartsWith("show"))
        {
            trayIcon.Show();
            Console.WriteLine(nameof(trayIcon.Show));
        }
        else if (line.StartsWith("hide"))
        {
            trayIcon.Hide();
            Console.WriteLine(nameof(trayIcon.Hide));
        }
        else if (line.StartsWith("exit"))
        {
            Console.WriteLine($"Exit.");
            break;
        }
        else
        {
            Console.WriteLine($"Command {line} not found.");
        }
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Exception: {exception}");
    }
}