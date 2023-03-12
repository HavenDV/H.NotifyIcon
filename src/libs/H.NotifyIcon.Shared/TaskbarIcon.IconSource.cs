namespace H.NotifyIcon;

[DependencyProperty<ImageSource>("IconSource",
    Description = "Resolves an image source and updates the Icon property accordingly.", Category = CategoryName)]
public partial class TaskbarIcon
{
    async partial void OnIconSourceChanged(ImageSource? oldValue, ImageSource? newValue)
    {
        if (newValue == null)
        {
            Icon = null;
            return;
        }

#if MACOS
        TrayIcon.Icon = NSImage.FromStream(stream);
#else
        Icon = await newValue.ToIconAsync().ConfigureAwait(true);
#endif
    }
}
