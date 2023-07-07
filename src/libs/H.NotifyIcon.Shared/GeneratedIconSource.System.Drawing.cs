namespace H.NotifyIcon;

public sealed partial class GeneratedIconSource
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [SupportedOSPlatform("windows")]
    public Icon ToIcon()
    {
        using var bitmap = Generate();

        return Icon.FromHandle(bitmap.GetHicon());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [SupportedOSPlatform("windows")]
    public async Task<Icon> ToIconAsync(CancellationToken cancellationToken = default)
    {
        using var bitmap = await GenerateAsync(cancellationToken).ConfigureAwait(true);

        return Icon.FromHandle(bitmap.GetHicon());
    }

    [SupportedOSPlatform("windows")]
    internal Bitmap Generate(Bitmap? backgroundBitmap = null)
    {
        var size = Size;
        using var fontFamily =
            FontFamily?.ToSystemDrawingFontFamily() ??
            new System.Drawing.FontFamily(string.Empty);
        using var font = new System.Drawing.Font(
            fontFamily,
            (float)FontSize,
            FontStyle.ToSystemDrawingFontStyle(FontWeight));
        using var baseImage = backgroundBitmap ?? BackgroundSource?.ToBitmap();
        using var pen = BorderBrush.ToSystemDrawingPen(BorderThickness);
        using var backgroundBrush = Background.ToSystemDrawingBrush();
        using var foregroundBrush = Foreground.ToSystemDrawingBrush();

        return SystemDrawingIconGenerator.Generate(
            backgroundBrush: backgroundBrush,
            foregroundBrush: foregroundBrush,
            pen: BorderThickness > 0.01F
                ? pen
                : null,
            backgroundType: BackgroundType,
            cornerRadius: (float)CornerRadius.TopLeft,
            rectangle: Margin == default
                ? null
                : Margin.ToSystemDrawingRectangleF(width: size, height: size),
            text: Text,
            font: font,
            textRectangle: TextMargin.ToSystemDrawingRectangleF(width: size, height: size),
            baseImage: baseImage,
            size: size);
    }

    [SupportedOSPlatform("windows")]
    internal async Task<Bitmap> GenerateAsync(CancellationToken cancellationToken = default)
    {
        using var baseImage = BackgroundSource == null
            ? null
            : await BackgroundSource.ToBitmapAsync(cancellationToken).ConfigureAwait(true);

        return Generate(baseImage);
    }
}
