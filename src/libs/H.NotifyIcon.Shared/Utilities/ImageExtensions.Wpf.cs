using System.Globalization;

namespace H.NotifyIcon;

internal static partial class ImageExtensions
{
    internal static Stream ToStream(this Uri uri)
    {
        if (uri.Scheme == Uri.UriSchemeFile)
        {
            return File.OpenRead(uri.LocalPath);
        }

        var streamInfo =
            Application.GetResourceStream(uri) ??
            throw new ArgumentException($"Uri: {uri} is not resolved.");

        return streamInfo.Stream;
    }

    internal static Task<Stream> ToStreamAsync(this Uri uri, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(uri.ToStream());
    }

    public static Stream ToStream(this ImageSource imageSource)
    {
        switch (imageSource)
        {
            case BitmapImage bitmapImage:
                {
                    var uri = bitmapImage.UriSource;

                    return uri.ToStream();
                }
            case BitmapFrame frame:
                {
                    var uri = new Uri(frame.ToString(CultureInfo.InvariantCulture));

                    return uri.ToStream();
                }

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }

    public static async Task<Stream> ToStreamAsync(this ImageSource imageSource, CancellationToken cancellationToken = default)
    {
        switch(imageSource)
        {
            case BitmapImage bitmapImage:
                {
                    var uri = bitmapImage.UriSource;
                    
                    return await uri.ToStreamAsync(cancellationToken).ConfigureAwait(true);
                }
            case BitmapFrame frame:
                {
                    var uri = new Uri(frame.ToString(CultureInfo.InvariantCulture));

                    return await uri.ToStreamAsync(cancellationToken).ConfigureAwait(true);
                }

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }
    
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
}
