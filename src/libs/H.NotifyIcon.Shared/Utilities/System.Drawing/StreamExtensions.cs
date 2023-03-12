using H.NotifyIcon.Interop;

namespace H.NotifyIcon;

internal static class StreamExtensions
{
    internal static Icon ToSmallIcon(this Stream stream)
    {
        var iconSize = IconUtilities.GetRequiredCustomIconSize(largeIcon: false).ScaleWithDpi();

        return new Icon(stream, iconSize);
    }
    
    internal static System.Drawing.Bitmap ToBitmap(this Stream stream)
    {
        return new System.Drawing.Bitmap(stream);
    }
    
    internal static (int Width, int Height, int BitsPerPixel) GetMetadata(this Stream stream)
    {
        using var image = System.Drawing.Image.FromStream(stream);
        
        return (image.Width, image.Height, System.Drawing.Image.GetPixelFormatSize(image.PixelFormat));
    }
}
