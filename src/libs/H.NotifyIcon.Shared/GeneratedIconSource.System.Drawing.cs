namespace H.NotifyIcon;

public sealed partial class GeneratedIconSource : BitmapSource
{
    internal System.Drawing.Bitmap Generate()
    {
        var size = Size;
        using var fontFamily =
            FontFamily?.ToSystemDrawingFontFamily() ??
            new System.Drawing.FontFamily(string.Empty);
        using var font = new System.Drawing.Font(
            fontFamily,
            (float)FontSize,
            FontStyle.ToSystemDrawingFontStyle(FontWeight));
        //using var baseImageStream = Background?.ToStream();
        //using var baseImage = baseImageStream?.ToBitmap();
        using var pen = BorderBrush.ToSystemDrawingPen(BorderThickness);
        using var backgroundBrush = Background.ToSystemDrawingBrush();
        using var foregroundBrush = Foreground.ToSystemDrawingBrush();

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
                : Margin.ToSystemDrawingRectangleF(width: size, height: size),
            text: Text,
            font: font,
            textRectangle: TextMargin.ToSystemDrawingRectangleF(width: size, height: size),
            baseImage: null,
            size: size);
    }
}
