namespace H.NotifyIcon;

internal static partial class ImageExtensions
{
    public static Stream ToStream(this ImageSource imageSource)
    {
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

    public static async Task<Stream> ToStreamAsync(this ImageSource imageSource, CancellationToken cancellationToken = default)
    {
        switch(imageSource)
        {
            case FileImageSource bitmapImage:
                return await FileSystem.Current.OpenAppPackageFileAsync(bitmapImage.File).ConfigureAwait(true);

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }
}
