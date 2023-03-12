namespace H.NotifyIcon;

public sealed partial class GeneratedIconSource : BitmapSource
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Icon ToIcon()
    {
        return Generate();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<Icon> ToIconAsync(CancellationToken cancellationToken = default)
    {
        return await GenerateAsync(cancellationToken).ConfigureAwait(true);
    }

    internal Bitmap Generate(Bitmap? backgroundBitmap = null)
    {
        var size = Size;
        // using var fontFamily =
        //     FontFamily?.ToSystemDrawingFontFamily() ??
        //     new System.Drawing.FontFamily(string.Empty);
        // using var font = new SkiaSharp.SKFont(
        //     fontFamily,
        //     (float)FontSize,
        //     FontStyle.ToSkiaSharpFontStyle(FontWeight));
        using var baseImage = backgroundBitmap ?? BackgroundSource?.ToBitmap();
        using var pen = BorderBrush.ToSkiaSharpPaint(BorderThickness);
        using var backgroundBrush = Background.ToSkiaSharpPaint();
        using var foregroundBrush = Foreground.ToSkiaSharpPaint();

        return IconGenerator.Generate(
            backgroundBrush: backgroundBrush,
            foregroundBrush: foregroundBrush,
            pen: BorderThickness > 0.01F
                ? pen
                : null,
            backgroundType: BackgroundType,
            cornerRadius: (float)CornerRadius.TopLeft,
            rectangle: Margin == default
                ? null
                : Margin.ToSkiaSharpRectangle(width: size, height: size),
            text: Text,
            //font: font,
            textRectangle: TextMargin.ToSkiaSharpRectangle(width: size, height: size),
            baseImage: baseImage,
            size: size);
    }

    internal async Task<Bitmap> GenerateAsync(CancellationToken cancellationToken = default)
    {
        using var baseImage = BackgroundSource == null
            ? null
            : await BackgroundSource.ToBitmapAsync(cancellationToken).ConfigureAwait(true);

        return Generate(baseImage);
    }
}
