namespace H.NotifyIcon;

[DependencyProperty<ImageSource>("IconSource",
    Description = "Resolves an image source and updates the Icon property accordingly.", Category = CategoryName)]
public partial class TaskbarIcon
{
    [SupportedOSPlatform("windows5.0")]
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
        
        // If the source is a GeneratedIconSource, we need to update the icon when the source changes.
        if (oldValue is GeneratedIconSource oldGeneratedIconSource)
        {
            oldGeneratedIconSource.DependencyPropertyChanged -= OnGeneratedIconSourceOnDependencyPropertyChanged;
        }
        if (newValue is GeneratedIconSource generatedIconSource)
        {
            generatedIconSource.DependencyPropertyChanged += OnGeneratedIconSourceOnDependencyPropertyChanged;
        }
        
        async void OnGeneratedIconSourceOnDependencyPropertyChanged(object? sender, EventArgs args)
        {
            Icon = await newValue.ToIconAsync().ConfigureAwait(true);
        }
    }
}
