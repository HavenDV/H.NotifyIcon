namespace H.NotifyIcon;

/// <summary>
/// 
/// </summary>
public sealed partial class GeneratedIconSource
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override Freezable CreateInstanceCore()
    {
        using var bitmap = Generate();
        
        return Create(
            pixelWidth: bitmap.Width,
            pixelHeight: bitmap.Height,
            dpiX: 96,
            dpiY: 96,
            pixelFormat: PixelFormats.Rgb24,
            palette: null,
            buffer: bitmap.GetPixels(),
            bufferSize: bitmap.Width * bitmap.Height,
            stride: bitmap.BytesPerPixel);
    }
}
