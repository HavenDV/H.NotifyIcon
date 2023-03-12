using H.NotifyIcon.Interop;
using SkiaSharp;

namespace H.NotifyIcon;

internal static class StreamExtensions
{
    internal static Icon ToSmallIcon(this Stream stream)
    {
        var iconSize = IconUtilities.GetRequiredCustomIconSize(largeIcon: false).ScaleWithDpi();

        return Icon.Decode(stream, new SKImageInfo(width: iconSize.Width, height: iconSize.Height));
    }

    internal static Bitmap ToBitmap(this Stream stream)
    {
        return Bitmap.Decode(stream);
    }

    internal static (int Width, int Height, int BitsPerPixel) GetMetadata(this Stream stream)
    {
        using var image = SKBitmap.Decode(stream);
        
        return (image.Width, image.Height, image.BytesPerPixel * 8);
    }
}
