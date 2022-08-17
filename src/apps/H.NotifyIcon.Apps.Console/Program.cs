using System.Drawing;
using H.NotifyIcon.Core;

using var iconStream = H.Resources.Red_ico.AsStream();
using var icon = new Icon(iconStream);
using var trayIcon = new TrayIconWithContextMenu
{
    Icon = icon.Handle,
    ToolTip = "ToolTip",
};
trayIcon.ContextMenu = new PopupMenu
{
    Items =
    {
        new PopupMenuItem("Create Second", (sender, args) => CreateSecond()),
        new PopupMenuSeparator(),
        new PopupMenuItem("Show Message", (sender, args) => ShowMessage(trayIcon, "message")),
        new PopupMenuItem("Show Info", (sender, args) => ShowInfo(trayIcon, "info")),
        new PopupMenuItem("Show Warning", (sender, args) => ShowWarning(trayIcon, "warning")),
        new PopupMenuItem("Show Error", (sender, args) => ShowError(trayIcon, "error")),
        new PopupMenuItem("Show Custom", (sender, args) => ShowCustom(trayIcon, "custom", icon)),
        new PopupMenuSeparator(),
        new PopupMenuItem("Remove", (sender, args) => Remove(trayIcon)),
        new PopupMenuItem("Hide", (sender, args) => Hide(trayIcon)),
        new PopupMenuSeparator(),
        new PopupMenuItem("Exit", (sender, args) =>
        {
            trayIcon.Dispose();
            Environment.Exit(0);
        }),
    },
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
            CreateSecond();
        }
        else if (line.StartsWith("message"))
        {
            ShowMessage(trayIcon, line.Substring("message".Length).TrimStart());
        }
        else if (line.StartsWith("info"))
        {
            ShowInfo(trayIcon, line.Substring("info".Length).TrimStart());
        }
        else if (line.StartsWith("warning"))
        {
            ShowWarning(trayIcon, line.Substring("warning".Length).TrimStart());
        }
        else if (line.StartsWith("error"))
        {
            ShowError(trayIcon, line.Substring("error".Length).TrimStart());
        }
        else if (line.StartsWith("custom"))
        {
            ShowCustom(trayIcon, line.Substring("custom".Length).TrimStart(), icon);
        }
        else if (line.StartsWith("clear"))
        {
            ClearNotifications(trayIcon);
        }
        else if (line.StartsWith("set-focus"))
        {
            SetFocus(trayIcon);
        }
        else if (line.StartsWith("remove"))
        {
            Remove(trayIcon);
        }
        else if (line.StartsWith("create"))
        {
            Create(trayIcon);
        }
        else if (line.StartsWith("show"))
        {
            Show(trayIcon);
        }
        else if (line.StartsWith("hide"))
        {
            Hide(trayIcon);
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

static async void CreateSecond()
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

static void ShowMessage(TrayIcon trayIcon, string message)
{
    trayIcon.ShowNotification(
        title: nameof(NotificationIcon.None),
        message: message,
        icon: NotificationIcon.None);
    Console.WriteLine(nameof(trayIcon.ShowNotification));
}

static void ShowInfo(TrayIcon trayIcon, string message)
{
    trayIcon.ShowNotification(
        title: nameof(NotificationIcon.Info),
        message: message,
        icon: NotificationIcon.Info);
    Console.WriteLine(nameof(trayIcon.ShowNotification));
}

static void ShowWarning(TrayIcon trayIcon, string message)
{
    trayIcon.ShowNotification(
        title: nameof(NotificationIcon.Warning),
        message: message,
        icon: NotificationIcon.Warning);
    Console.WriteLine(nameof(trayIcon.ShowNotification));
}

static void ShowError(TrayIcon trayIcon, string message)
{
    trayIcon.ShowNotification(
        title: nameof(NotificationIcon.Error),
        message: message,
        icon: NotificationIcon.Error);
    Console.WriteLine(nameof(trayIcon.ShowNotification));
}

static void ShowCustom(TrayIcon trayIcon, string message, Icon icon)
{
    trayIcon.ShowNotification(
        title: "Custom",
        message: message,
        customIcon: icon.Handle);
    Console.WriteLine(nameof(trayIcon.ShowNotification));
}

static void ClearNotifications(TrayIcon trayIcon)
{
    trayIcon.ClearNotifications();
    Console.WriteLine(nameof(trayIcon.ClearNotifications));
}

static void SetFocus(TrayIcon trayIcon)
{
    trayIcon.SetFocus();
    Console.WriteLine(nameof(trayIcon.SetFocus));
}

static void Remove(TrayIcon trayIcon)
{
    trayIcon.Remove();
    Console.WriteLine(nameof(trayIcon.Remove));
}

static void Create(TrayIcon trayIcon)
{
    trayIcon.Create();
    Console.WriteLine(nameof(trayIcon.Create));
}

static void Show(TrayIcon trayIcon)
{
    trayIcon.Show();
    Console.WriteLine(nameof(trayIcon.Show));
}

static void Hide(TrayIcon trayIcon)
{
    trayIcon.Hide();
    Console.WriteLine(nameof(trayIcon.Hide));
}
