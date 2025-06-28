using System.Globalization;

namespace H.NotifyIcon;

/// <summary>
/// Provides WPF-specific extension methods for converting ImageSource objects, URIs, and bitmap objects to streams.
/// </summary>
public static partial class ImageExtensions
{
    /// <summary>
    /// Converts a URI to a Stream, supporting file URIs and application resource URIs.
    /// </summary>
    /// <param name="uri">The URI to convert to a stream.</param>
    /// <returns>A Stream representation of the URI content.</returns>
    /// <exception cref="ArgumentNullException">Thrown when uri is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the URI cannot be resolved.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file specified by the URI is not found.</exception>
    public static Stream ToStream(this Uri uri)
    {
        uri = uri ?? throw new ArgumentNullException(nameof(uri));
        
        if (uri.Scheme == Uri.UriSchemeFile)
        {
            return File.OpenRead(uri.LocalPath);
        }

        var streamInfo =
            Application.GetResourceStream(uri) ??
            throw new ArgumentException($"Uri: {uri} is not resolved.");

        return streamInfo.Stream;
    }

    /// <summary>
    /// Asynchronously converts a URI to a Stream.
    /// </summary>
    /// <param name="uri">The URI to convert to a stream.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Stream representation of the URI content.</returns>
    /// <exception cref="ArgumentNullException">Thrown when uri is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the URI cannot be resolved.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file specified by the URI is not found.</exception>
    public static Task<Stream> ToStreamAsync(this Uri uri, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(uri.ToStream());
    }

    /// <summary>
    /// Converts an ImageSource to a Stream for WPF applications.
    /// </summary>
    /// <param name="imageSource">The ImageSource to convert.</param>
    /// <returns>A Stream representation of the ImageSource.</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageSource is null.</exception>
    /// <exception cref="NotImplementedException">Thrown when the ImageSource type is not supported.</exception>
    /// <exception cref="ArgumentException">Thrown when the ImageSource URI cannot be resolved.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the image file is not found.</exception>
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
            case BitmapFrame frame:
                {
                    var uri = new Uri(frame.ToString(CultureInfo.InvariantCulture));

                    return uri.ToStream();
                }

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }

    /// <summary>
    /// Asynchronously converts an ImageSource to a Stream for WPF applications.
    /// </summary>
    /// <param name="imageSource">The ImageSource to convert.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a Stream representation of the ImageSource.</returns>
    /// <exception cref="ArgumentNullException">Thrown when imageSource is null.</exception>
    /// <exception cref="NotImplementedException">Thrown when the ImageSource type is not supported.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    /// <exception cref="ArgumentException">Thrown when the ImageSource URI cannot be resolved.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the image file is not found.</exception>
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
            case BitmapFrame frame:
                {
                    var uri = new Uri(frame.ToString(CultureInfo.InvariantCulture));

                    return await uri.ToStreamAsync(cancellationToken).ConfigureAwait(true);
                }

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }
    
    /// <summary>
    /// Converts a BitmapFrame to a Stream in ICO format.
    /// </summary>
    /// <param name="frame">The BitmapFrame to convert.</param>
    /// <returns>A Stream containing the BitmapFrame data in ICO format.</returns>
    /// <exception cref="ArgumentNullException">Thrown when frame is null.</exception>
    public static Stream ToStream(this BitmapFrame frame)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(frame);

        using var stream = new MemoryStream();
        encoder.Save(stream);

        var iconBytes = stream.ToArray().ConvertPngToIco();
        
        return new MemoryStream(iconBytes);
    }

    /// <summary>
    /// Converts a BitmapSource to a Stream in ICO format.
    /// </summary>
    /// <param name="bitmap">The BitmapSource to convert.</param>
    /// <returns>A Stream containing the BitmapSource data in ICO format.</returns>
    /// <exception cref="ArgumentNullException">Thrown when bitmap is null.</exception>
    public static Stream ToStream(this BitmapSource bitmap)
    {
        return BitmapFrame.Create(bitmap).ToStream();
    }
}
