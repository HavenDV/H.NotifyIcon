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
        var bits = bitmap.LockBits(
            rect: new System.Drawing.Rectangle(
                x: 0,
                y: 0,
                width: bitmap.Width,
                height: bitmap.Height), 
            flags: System.Drawing.Imaging.ImageLockMode.ReadOnly,
            bitmap.PixelFormat);
        
        return Create(
            pixelWidth: bitmap.Width,
            pixelHeight: bitmap.Height,
            dpiX: 96,
            dpiY: 96,
            pixelFormat: PixelFormats.Rgb24,
            palette: null,
            buffer: bits.Scan0,
            bufferSize: bits.Width * bits.Height,
            stride: bits.Stride);
    }
}
