namespace H.NotifyIcon;

internal static class ImageExtensions
{
#if HAS_WPF
    internal static Icon ToIcon(this Uri uri)
#else
    internal static async Task<Icon> ToIconAsync(this Uri uri)
#endif
    {
#if HAS_WPF
        var streamInfo =
            Application.GetResourceStream(uri) ??
            throw new ArgumentException($"Uri: {uri} is not resolved.");
        var stream = streamInfo.Stream;
#else
        var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
        var stream = await file.OpenStreamForReadAsync();
#endif

        return new Icon(stream);
    }

#if HAS_WPF
    public static Icon? ToIcon(this ImageSource imageSource)
#else
    public static async Task<Icon?> ToIconAsync(this ImageSource imageSource)
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
                    var uri = new Uri(frame.ToString());

                    return uri.ToIcon();
                }
#endif

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }
}
