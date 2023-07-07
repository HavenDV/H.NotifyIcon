namespace H.NotifyIcon;

internal static partial class ImageExtensions
{
    [SupportedOSPlatform("windows")]
    public static Bitmap ToBitmap(this ImageSource imageSource)
    {
        if (imageSource is GeneratedIconSource generatedIconSource)
        {
            return generatedIconSource.ToBitmap();
        }

        using var stream = imageSource.ToStream();

        return stream.ToBitmap();
    }

    [SupportedOSPlatform("windows5.0")]
    public static Icon ToIcon(this ImageSource imageSource)
    {
        if (imageSource is GeneratedIconSource generatedIconSource)
        {
            return generatedIconSource.ToIcon();
        }

        using var stream = imageSource.ToStream();

        return stream.ToSmallIcon();
    }

    [SupportedOSPlatform("windows")]
    public static async Task<Bitmap> ToBitmapAsync(this ImageSource imageSource, CancellationToken cancellationToken = default)
    {
        if (imageSource is GeneratedIconSource generatedIconSource)
        {
            return await generatedIconSource.ToBitmapAsync(cancellationToken).ConfigureAwait(true);
        }

        using var stream = await imageSource.ToStreamAsync(cancellationToken).ConfigureAwait(true);

        return stream.ToBitmap();
    }

    [SupportedOSPlatform("windows5.0")]
    public static async Task<Icon> ToIconAsync(this ImageSource imageSource, CancellationToken cancellationToken = default)
    {
        if (imageSource is GeneratedIconSource generatedIconSource)
        {
            return await generatedIconSource.ToIconAsync(cancellationToken).ConfigureAwait(true);
        }

        using var stream = await imageSource.ToStreamAsync(cancellationToken).ConfigureAwait(true);

        return stream.ToSmallIcon();
    }
}
