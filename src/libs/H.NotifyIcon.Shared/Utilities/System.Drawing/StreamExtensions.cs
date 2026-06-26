using H.NotifyIcon.Interop;

namespace H.NotifyIcon;

internal static class StreamExtensions
{
    [SupportedOSPlatform("windows5.0")]
    internal static Icon ToSmallIcon(this Stream stream)
    {
        var iconSize = IconUtilities.GetRequiredCustomIconSize(largeIcon: false).ScaleWithDpi();
        var data = stream.ToArray();

        if (IsPng(data))
        {
            data = data.ConvertPngToIco();
        }

        using var iconStream = new MemoryStream(data);

        return new Icon(iconStream, iconSize);
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

    private static byte[] ToArray(this Stream stream)
    {
        if (stream is MemoryStream memoryStream)
        {
            return memoryStream.ToArray();
        }

        using var copy = new MemoryStream();
        stream.CopyTo(copy);

        return copy.ToArray();
    }

    private static bool IsPng(byte[] data)
    {
        return data.Length >= 8 &&
               data[0] == 0x89 &&
               data[1] == 0x50 &&
               data[2] == 0x4E &&
               data[3] == 0x47 &&
               data[4] == 0x0D &&
               data[5] == 0x0A &&
               data[6] == 0x1A &&
               data[7] == 0x0A;
    }
}
