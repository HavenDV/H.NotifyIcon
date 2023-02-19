#if HAS_SKIA_SHARP
using System.Drawing;
using SkiaSharp;

namespace H.NotifyIcon;

/// <summary>
/// 
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
[CLSCompliant(false)]
public static class IconGenerator
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
    public static nint Generate(
        SKPaint backgroundBrush,
        SKPaint foregroundBrush,
        SKPaint? pen = null,
        BackgroundType backgroundType = BackgroundType.Ellipse,
        float cornerRadius = 0.0F,
        SKRect? rectangle = null,
        string? text = null,
        SKFont? font = null,
        RectangleF? textRectangle = null,
        SKImage? baseImage = null,
        int size = 128)
    {
        backgroundBrush = backgroundBrush ?? throw new ArgumentNullException(nameof(backgroundBrush));
        
        using var bitmap = baseImage == null
            ? new SKBitmap(size, size)
            : SKBitmap.FromImage(baseImage);
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

        // if (!string.IsNullOrWhiteSpace(text) &&
        //     font != null)
        // {
        //     if (textRectangle == null)
        //     {
        //         var textSize = graphics.MeasureString(
        //             text: text,
        //             font: font,
        //             layoutArea: new SizeF(size, size));
        //
        //         graphics.DrawText(
        //             text: text,
        //             font: font,
        //             paint: foregroundBrush,
        //             layoutRectangle: new RectangleF(
        //                 size / 2 - textSize.Width / 2,
        //                 size / 2 - textSize.Height / 2,
        //                 textSize.Width,
        //                 textSize.Height),
        //             format: StringFormat.GenericTypographic);
        //     }
        //     else
        //     {
        //         graphics.DrawText(
        //             text: text,
        //             font: font,
        //             paint: foregroundBrush,
        //             layoutRectangle: textRectangle.Value,
        //             format: StringFormat.GenericTypographic);
        //     }
        // }

        return bitmap.Handle;
    }
}
#endif
