namespace H.NotifyIcon;

/// <summary>
/// Provides MAUI-specific extension methods for converting ImageSource objects to streams.
/// </summary>
public static partial class ImageExtensions
{
    /// <summary>
    /// Converts an ImageSource to a Stream for MAUI applications.
    /// </summary>
    /// <param name="imageSource">The ImageSource to convert.</param>
    /// <returns>A Stream representation of the ImageSource.</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageSource is null.</exception>
    /// <exception cref="NotImplementedException">Thrown when the ImageSource type is not supported.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file specified in FileImageSource is not found.</exception>
    [CLSCompliant(false)]
    public static Stream ToStream(this ImageSource imageSource)
    {
        imageSource = imageSource ?? throw new ArgumentNullException(nameof(imageSource));
        
        switch (imageSource)
        {
            case FileImageSource bitmapImage:
                {
                    return File.OpenRead(bitmapImage.File);
                }

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }

    /// <summary>
    /// Asynchronously converts an ImageSource to a Stream for MAUI applications.
    /// </summary>
    /// <param name="imageSource">The ImageSource to convert.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Stream representation of the ImageSource.</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageSource is null.</exception>
    /// <exception cref="NotImplementedException">Thrown when the ImageSource type is not supported.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file specified in FileImageSource is not found in the app package.</exception>
    [CLSCompliant(false)]
    public static async Task<Stream> ToStreamAsync(this ImageSource imageSource, CancellationToken cancellationToken = default)
    {
        imageSource = imageSource ?? throw new ArgumentNullException(nameof(imageSource));
        
        switch(imageSource)
        {
            case FileImageSource bitmapImage:
                return await FileSystem.Current.OpenAppPackageFileAsync(bitmapImage.File).ConfigureAwait(true);

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }
}
