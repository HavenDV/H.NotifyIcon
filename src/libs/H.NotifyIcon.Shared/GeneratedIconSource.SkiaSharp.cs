namespace H.NotifyIcon;

public sealed partial class GeneratedIconSource : BitmapSource
{
    internal System.Drawing.Bitmap Generate()
    {
        var size = Size;
        // using var fontFamily =
        //     FontFamily?.ToSystemDrawingFontFamily() ??
        //     new System.Drawing.FontFamily(string.Empty);
        // using var font = new SkiaSharp.SKFont(
        //     fontFamily,
        //     (float)FontSize,
        //     FontStyle.ToSkiaSharpFontStyle(FontWeight));
        //using var baseImage = TaskbarIcon.Icon?.ToBitmap();
        using var pen = BorderBrush.ToSkiaSharpPaint(BorderThickness);
        using var backgroundBrush = Background.ToSkiaSharpPaint();
        using var foregroundBrush = Foreground.ToSkiaSharpPaint();

        Icon = IconGenerator.Generate(
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
            //baseImage: baseImage,
            size: size);
    }
}
