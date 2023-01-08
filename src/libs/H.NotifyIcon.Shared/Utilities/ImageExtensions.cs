using System.Globalization;
using H.NotifyIcon.Interop;

namespace H.NotifyIcon;

internal static class ImageExtensions
{
    private static System.Drawing.Icon ToSmallIcon(this Stream stream)
    {
        var iconSize = IconUtilities.GetRequiredCustomIconSize(largeIcon: false).ScaleWithDpi();

        return new System.Drawing.Icon(stream, iconSize);
    }

#if HAS_WPF
    internal static System.Drawing.Icon ToIcon(this Uri uri)
#else
    internal static async Task<System.Drawing.Icon> ToIconAsync(this Uri uri)
#endif
    {
#if HAS_WPF
        if (uri.Scheme == Uri.UriSchemeFile)
        {
            using var fileStream = File.OpenRead(uri.LocalPath);

            return fileStream.ToSmallIcon();
        }

        var streamInfo =
            Application.GetResourceStream(uri) ??
            throw new ArgumentException($"Uri: {uri} is not resolved.");
        using var stream = streamInfo.Stream;
        return stream.ToSmallIcon();
#else
        DesktopBridge.Helpers helpers = new DesktopBridge.Helpers();
        if (helpers.IsRunningAsUwp()) {
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            using var stream = await file.OpenStreamForReadAsync().ConfigureAwait(true);
            return stream.ToSmallIcon();
        } else {
            string prefix = "";
            if (uri.Scheme == "ms-appx" || uri.Scheme == "ms-appx-web") {
                prefix = AppContext.BaseDirectory;
            }
            // additional schemes, like ms-appdata could be added here
            // see: https://learn.microsoft.com/en-us/windows/uwp/app-resources/uri-schemes
            var absolutePath = $"{prefix}{uri.LocalPath}";
            using var fileStream = File.OpenRead(absolutePath);
            return fileStream.ToSmallIcon();
        }
#endif
    }

#if HAS_WPF

    public static System.Drawing.Icon ToIcon(this BitmapFrame frame)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(frame);

        using var stream = new MemoryStream();
        encoder.Save(stream);

        var iconBytes = stream.ToArray().ConvertPngToIco();
        using var iconStream = new MemoryStream(iconBytes);
        
        return iconStream.ToSmallIcon();
    }

    public static System.Drawing.Icon ToIcon(this BitmapSource bitmap)
    {
        return BitmapFrame.Create(bitmap).ToIcon();
    }

#endif

#if HAS_WPF
    public static System.Drawing.Icon? ToIcon(this ImageSource imageSource)
#else
    public static async Task<System.Drawing.Icon?> ToIconAsync(this ImageSource imageSource)
#endif
    {
        switch(imageSource)
        {
            case null:
                return null;

            case BitmapImage bitmapImage:
                {
                    var uri = bitmapImage.UriSource;
                    
#if HAS_WPF
                    return uri.ToIcon();
#else
                    return await uri.ToIconAsync().ConfigureAwait(true);
#endif
                }
#if HAS_WPF
            case BitmapFrame frame:
                {
                    var uri = new Uri(frame.ToString(CultureInfo.InvariantCulture));

                    return uri.ToIcon();
                }
#endif

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }
}
