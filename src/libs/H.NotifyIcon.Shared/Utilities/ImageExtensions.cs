using System.Globalization;

namespace H.NotifyIcon;

internal static class ImageExtensions
{
#if HAS_WPF
    internal static Stream ToStream(this Uri uri)
#else
    internal static async Task<Stream> ToStreamAsync(this Uri uri)
#endif
    {
#if HAS_WPF
        if (uri.Scheme == Uri.UriSchemeFile)
        {
            return File.OpenRead(uri.LocalPath);
        }

        var streamInfo =
            Application.GetResourceStream(uri) ??
            throw new ArgumentException($"Uri: {uri} is not resolved.");
        return streamInfo.Stream;
#else
#if IS_PACKABLE
        if (Interop.DesktopBridgeHelpers.IsRunningAsUwp()) {
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            return await file.OpenStreamForReadAsync().ConfigureAwait(true);
        } else {
#endif
            string prefix = "";
            if (uri.Scheme == "ms-appx" || uri.Scheme == "ms-appx-web") {
                prefix = AppContext.BaseDirectory;
            }
            // additional schemes, like ms-appdata could be added here
            // see: https://learn.microsoft.com/en-us/windows/uwp/app-resources/uri-schemes
            var absolutePath = $"{prefix}{uri.LocalPath}";
            return File.OpenRead(absolutePath);
#if IS_PACKABLE
        }
#endif
#endif
    }

#if HAS_WPF
    public static Stream ToStream(this ImageSource imageSource)
#else
    public static async Task<Stream> ToStreamAsync(this ImageSource imageSource)
#endif
    {
        switch(imageSource)
        {
            case BitmapImage bitmapImage:
                {
                    var uri = bitmapImage.UriSource;
                    
#if HAS_WPF
                    return uri.ToStream();
#else
                    return await uri.ToStreamAsync().ConfigureAwait(true);
#endif
                }
#if HAS_WPF
            case BitmapFrame frame:
                {
                    var uri = new Uri(frame.ToString(CultureInfo.InvariantCulture));

                    return uri.ToStream();
                }
#endif

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }
    
#if HAS_WPF

    public static Stream ToStream(this BitmapFrame frame)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(frame);

        using var stream = new MemoryStream();
        encoder.Save(stream);

        var iconBytes = stream.ToArray().ConvertPngToIco();
        
        return new MemoryStream(iconBytes);
    }

    public static Stream ToStream(this BitmapSource bitmap)
    {
        return BitmapFrame.Create(bitmap).ToStream();
    }

#endif
}
