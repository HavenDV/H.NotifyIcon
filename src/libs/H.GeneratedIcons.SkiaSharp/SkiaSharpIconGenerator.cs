using SkiaSharp;

namespace H.NotifyIcon;

/// <summary>
/// 
/// </summary>
[CLSCompliant(false)]
public static class SkiaSharpIconGenerator
{
    private static SKPath GetRoundedRectGraphicsPath(SKRect bounds, float radius)
    {
        var path = new SKPath();
        if (radius == 0)
        {
            path.AddRect(bounds);
            return path;
        }
        
        var diameter = radius * 2;
        var size = new SKSize(diameter, diameter);
        var arc = SKRect.Create(bounds.Location, size);

        // top left arc  
        path.AddArc(arc, 180, 90);

        // top right arc  
        arc.Left = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);

        // bottom right arc  
        arc.Top = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);

        // bottom left arc 
        arc.Left = bounds.Left;
        path.AddArc(arc, 90, 90);

        path.Close();

        return path;
    }

    /// <summary>
    /// Generates <paramref name="size"/> x <paramref name="size"/> standard icon with selected parameters.
    /// </summary>
    /// <returns></returns>
    public static SKBitmap Generate(
        SKPaint backgroundBrush,
        SKPaint? foregroundBrush = null,
        SKPaint? pen = null,
        BackgroundType backgroundType = BackgroundType.Ellipse,
        float cornerRadius = 0.0F,
        SKRect? rectangle = null,
        string? text = null,
        SKFont? font = null,
        SKRect? textRectangle = null,
        SKBitmap? baseImage = null,
        int size = 128)
    {
        backgroundBrush = backgroundBrush ?? throw new ArgumentNullException(nameof(backgroundBrush));
        
        var bitmap = baseImage == null
            ? new SKBitmap(size, size)
            : baseImage;
        using var graphics = new SKCanvas(bitmap);
        //graphics.CompositingQuality = CompositingQuality.HighQuality;
        //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        
        var borderOffset = pen == null
            ? 0.0F
            : pen.StrokeWidth / 2;
        var rect = rectangle ?? new SKRect(borderOffset, borderOffset, size - borderOffset * 2, size - borderOffset * 2);
        backgroundBrush.Style = SKPaintStyle.Fill;
        switch (backgroundType)
        {
            case BackgroundType.Rectangle:
                graphics.DrawRect(
                    rect: rect,
                    paint: backgroundBrush);
                break;
        
            case BackgroundType.RoundedRectangle:
                {
                    using var path = GetRoundedRectGraphicsPath(rect, cornerRadius);
                    graphics.DrawPath(
                        path: path,
                        paint: backgroundBrush);
                }
                break;
        
            case BackgroundType.Ellipse:
                graphics.DrawOval(
                    rect: rect,
                    paint: backgroundBrush);
                break;
        }
        
        if (pen != null)
        {
            switch (backgroundType)
            {
                case BackgroundType.Rectangle:
                    graphics.DrawRect(
                        rect: SKRect.Create((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height),
                        paint: pen);
                    break;
        
                case BackgroundType.RoundedRectangle:
                    {
                        using var path = GetRoundedRectGraphicsPath(rect, cornerRadius);
                        graphics.DrawPath(
                            path: path,
                            paint: pen);
                    }
                    break;
        
                case BackgroundType.Ellipse:
                    graphics.DrawOval(
                        rect: rect,
                        paint: pen);
                    break;
            }
        }

        if (!string.IsNullOrWhiteSpace(text) &&
            font != null &&
            foregroundBrush != null)
        {
            if (textRectangle == null)
            {
                _ = font.MeasureText(text, out var bounds, foregroundBrush);
                
                graphics.DrawText(
                    text: text,
                    font: font,
                    paint: foregroundBrush,
                    x: size / 2.0F - bounds.Left - bounds.Width / 2,
                    y: size / 2.0F - bounds.Top - bounds.Height / 2);
            }
            else
            {
                graphics.DrawText(
                    text: text,
                    font: font,
                    paint: foregroundBrush,
                    x: textRectangle.Value.Left,
                    y: textRectangle.Value.Top);
            }
        }

        return bitmap;
    }
}
