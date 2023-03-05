#if HAS_SYSTEM_DRAWING
using H.NotifyIcon.Interop;

namespace H.NotifyIcon;

internal static class StreamExtensions
{
    internal static Icon ToSmallIcon(this Stream stream)
    {
        var iconSize = IconUtilities.GetRequiredCustomIconSize(largeIcon: false).ScaleWithDpi();

        return new Icon(stream, iconSize);
    }
    
    internal static (int Width, int Height, int BitsPerPixel) GetMetadata(this Stream stream)
    {
        using var image = System.Drawing.Image.FromStream(stream);
        
        return (image.Width, image.Height, System.Drawing.Image.GetPixelFormatSize(image.PixelFormat));
    }
}
#endif
