using System.Drawing;
using H.NotifyIcon.Core;

using var iconStream = H.Resources.Red_ico.AsStream();
using var icon = new Icon(iconStream);
using var trayIcon = new TrayIcon
{
    Icon = icon.Handle,
    ToolTip = "Tooltip",
};
trayIcon.CreateAndShow();

while (true)
{
    try
    {
        var line = Console.ReadLine();
        if (line.StartsWith("create-second"))
        {
            using var iconStream2 = H.Resources.icon_ico.AsStream();
            using var icon2 = new Icon(iconStream2);
            using var trayIcon2 = new TrayIcon("H.NotifyIcon.Apps.Console.SecondTrayIcon")
            {
                Icon = icon2.Handle,
                ToolTip = "Tooltip",
            };
            trayIcon2.CreateAndShow();

            Console.WriteLine("Second icon created. It will removed after 5 seconds.");

            await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

            Console.WriteLine("Second icon removed.");
        }
        else if (line.StartsWith("message"))
        {
            trayIcon.ShowNotification(
                title: nameof(NotificationIcon.None),
                message: line.Substring("message".Length + 1),
                icon: NotificationIcon.None);
            Console.WriteLine(nameof(trayIcon.ShowNotification));
        }
        else if (line.StartsWith("info"))
        {
            trayIcon.ShowNotification(
                title: nameof(NotificationIcon.Info),
                message: line.Substring("info".Length + 1),
                icon: NotificationIcon.Info);
            Console.WriteLine(nameof(trayIcon.ShowNotification));
        }
        else if (line.StartsWith("warning"))
        {
            trayIcon.ShowNotification(
                title: nameof(NotificationIcon.Warning),
                message: line.Substring("warning".Length + 1),
                icon: NotificationIcon.Warning);
            Console.WriteLine(nameof(trayIcon.ShowNotification));
        }
        else if (line.StartsWith("error"))
        {
            trayIcon.ShowNotification(
                title: nameof(NotificationIcon.Error),
                message: line.Substring("error".Length + 1),
                icon: NotificationIcon.Error);
            Console.WriteLine(nameof(trayIcon.ShowNotification));
        }
        else if (line.StartsWith("custom"))
        {
            trayIcon.ShowNotification(
                title: "Custom",
                message: line.Substring("custom".Length + 1),
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
            var result = trayIcon.Remove();
            Console.WriteLine($"{line}: {result}");
        }
        else if (line.StartsWith("create"))
        {
            trayIcon.CreateAndShow();
            Console.WriteLine(nameof(trayIcon.CreateAndShow));
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