using System.Diagnostics.CodeAnalysis;

namespace H.NotifyIcon;

/// <summary>
/// Provides WinRT-specific extension methods for converting ImageSource objects and URIs to streams.
/// </summary>
public static partial class ImageExtensions
{
#if !HAS_MAUI
    /// <summary>
    /// Converts a URI to a Stream, supporting ms-appx and ms-appx-web schemes.
    /// </summary>
    /// <param name="uri">The URI to convert to a stream.</param>
    /// <returns>A Stream representation of the URI content.</returns>
    /// <exception cref="ArgumentNullException">Thrown when uri is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file specified by the URI is not found.</exception>
    internal static Stream ToStream(this Uri uri)
    {
        var prefix = uri.Scheme switch
        {
            "ms-appx" or "ms-appx-web" => AppContext.BaseDirectory,
            _ => string.Empty,
        };
        // additional schemes, like ms-appdata could be added here
        // see: https://learn.microsoft.com/en-us/windows/uwp/app-resources/uri-schemes
        var absolutePath = $"{prefix}{uri.LocalPath}";

        return File.OpenRead(absolutePath);
    }

    /// <summary>
    /// Asynchronously converts a URI to a Stream, with special handling for UWP applications.
    /// </summary>
    /// <param name="uri">The URI to convert to a stream.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Stream representation of the URI content.</returns>
    /// <exception cref="ArgumentNullException">Thrown when uri is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file specified by the URI is not found.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    internal static async Task<Stream> ToStreamAsync(this Uri uri, CancellationToken cancellationToken = default)
    {
#if IS_PACKABLE
        if (Interop.DesktopBridgeHelpers.IsRunningAsUwp())
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

            return await file.OpenStreamForReadAsync().ConfigureAwait(true);
        }
#endif

        return uri.ToStream();
    }

    /// <summary>
    /// Converts an ImageSource to a Stream for WinRT applications.
    /// </summary>
    /// <param name="imageSource">The ImageSource to convert.</param>
    /// <returns>A Stream representation of the ImageSource.</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageSource is null.</exception>
    /// <exception cref="NotImplementedException">Thrown when the ImageSource type is not supported.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the image file is not found.</exception>
    [CLSCompliant(false)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(BitmapImage))]
    public static Stream ToStream(this ImageSource imageSource)
    {
        imageSource = imageSource ?? throw new ArgumentNullException(nameof(imageSource));
        
        switch (imageSource)
        {
            case BitmapImage bitmapImage:
                {
                    var uri = bitmapImage.UriSource;

                    return uri.ToStream();
                }

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }

    /// <summary>
    /// Asynchronously converts an ImageSource to a Stream for WinRT applications.
    /// </summary>
    /// <param name="imageSource">The ImageSource to convert.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Stream representation of the ImageSource.</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageSource is null.</exception>
    /// <exception cref="NotImplementedException">Thrown when the ImageSource type is not supported.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the image file is not found.</exception>
    [CLSCompliant(false)]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(BitmapImage))]
    public static async Task<Stream> ToStreamAsync(this ImageSource imageSource, CancellationToken cancellationToken = default)
    {
        imageSource = imageSource ?? throw new ArgumentNullException(nameof(imageSource));
        
        switch(imageSource)
        {
            case BitmapImage bitmapImage:
                {
                    var uri = bitmapImage.UriSource;
                    
                    return await uri.ToStreamAsync(cancellationToken).ConfigureAwait(true);
                }

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }
#endif
}
