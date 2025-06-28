namespace H.NotifyIcon;

/// <summary>
/// Provides extension methods for converting ImageSource objects to various formats.
/// </summary>
public static partial class ImageExtensions
{
    /// <summary>
    /// Converts an ImageSource to a System.Drawing.Bitmap.
    /// </summary>
    /// <param name="imageSource">The ImageSource to convert.</param>
    /// <returns>A Bitmap representation of the ImageSource.</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageSource is null.</exception>
    [SupportedOSPlatform("windows")]
    [CLSCompliant(false)]
    public static Bitmap ToBitmap(this ImageSource imageSource)
    {
        if (imageSource is GeneratedIconSource generatedIconSource)
        {
            return generatedIconSource.ToBitmap();
        }

        using var stream = imageSource.ToStream();

        return stream.ToBitmap();
    }

    /// <summary>
    /// Converts an ImageSource to a System.Drawing.Icon.
    /// </summary>
    /// <param name="imageSource">The ImageSource to convert.</param>
    /// <returns>An Icon representation of the ImageSource.</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageSource is null.</exception>
    [SupportedOSPlatform("windows5.0")]
    [CLSCompliant(false)]
    public static Icon ToIcon(this ImageSource imageSource)
    {
        if (imageSource is GeneratedIconSource generatedIconSource)
        {
            return generatedIconSource.ToIcon();
        }

        using var stream = imageSource.ToStream();

        return stream.ToSmallIcon();
    }

    /// <summary>
    /// Asynchronously converts an ImageSource to a System.Drawing.Bitmap.
    /// </summary>
    /// <param name="imageSource">The ImageSource to convert.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Bitmap representation of the ImageSource.</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageSource is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    [SupportedOSPlatform("windows")]
    [CLSCompliant(false)]
    public static async Task<Bitmap> ToBitmapAsync(this ImageSource imageSource, CancellationToken cancellationToken = default)
    {
        if (imageSource is GeneratedIconSource generatedIconSource)
        {
            return await generatedIconSource.ToBitmapAsync(cancellationToken).ConfigureAwait(true);
        }

        using var stream = await imageSource.ToStreamAsync(cancellationToken).ConfigureAwait(true);

        return stream.ToBitmap();
    }

    /// <summary>
    /// Asynchronously converts an ImageSource to a System.Drawing.Icon.
    /// </summary>
    /// <param name="imageSource">The ImageSource to convert.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an Icon representation of the ImageSource.</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageSource is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    [SupportedOSPlatform("windows5.0")]
    [CLSCompliant(false)]
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
