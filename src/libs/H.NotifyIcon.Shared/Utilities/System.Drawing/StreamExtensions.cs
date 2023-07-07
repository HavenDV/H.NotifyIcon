using H.NotifyIcon.Interop;

namespace H.NotifyIcon;

internal static class StreamExtensions
{
    [SupportedOSPlatform("windows5.0")]
    internal static Icon ToSmallIcon(this Stream stream)
    {
        var iconSize = IconUtilities.GetRequiredCustomIconSize(largeIcon: false).ScaleWithDpi();

        return new Icon(stream, iconSize);
    }
    
    [SupportedOSPlatform("windows")]
    internal static Bitmap ToBitmap(this Stream stream)
    {
        return new Bitmap(stream);
    }
    
    [SupportedOSPlatform("windows")]
    internal static (int Width, int Height, int BitsPerPixel) GetMetadata(this Stream stream)
    {
        using var image = System.Drawing.Image.FromStream(stream);
        
        return (image.Width, image.Height, System.Drawing.Image.GetPixelFormatSize(image.PixelFormat));
    }
}
