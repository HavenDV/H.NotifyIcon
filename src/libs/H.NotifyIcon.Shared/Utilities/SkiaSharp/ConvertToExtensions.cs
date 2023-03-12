using SkiaSharp;

namespace H.NotifyIcon;

internal static class ToSkiaSharpExtensions
{
    internal static SKSize ToSkiaSharpSize(this Size size)
    {
        return new SKSize(
            width: (int)size.Width,
            height: (int)size.Height);
    }

    internal static SKPoint ToSkiaSharpPoint(this Point point)
    {
        return new SKPoint((float)point.X, (float)point.Y);
    }

    internal static SKColor ToSkiaSharpColor(this Color color)
    {
        return new SKColor(
            alpha: color.A,
            red: color.R,
            green: color.G,
            blue: color.B);
    }

    internal static SKPaint ToSkiaSharpPaint(this Brush? brush)
    {
        return brush switch
        {
            null => new SKPaint
            {
                Color = SKColors.Transparent,
            },
            SolidColorBrush solidColorBrush => new SKPaint
            {
                Color = solidColorBrush.Color.ToSkiaSharpColor(),
            },
            LinearGradientBrush linearGradientBrush => new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    start: linearGradientBrush.StartPoint.ToSkiaSharpPoint(),
                    end: linearGradientBrush.EndPoint.ToSkiaSharpPoint(),
                    colors: new []
                    {
                        linearGradientBrush.GradientStops.ElementAt(0).Color.ToSkiaSharpColor(),
                        linearGradientBrush.GradientStops.ElementAt(1).Color.ToSkiaSharpColor(),
                    },
                    colorPos: new float[] { 0, 1 },
                    SKShaderTileMode.Repeat),
            },
            _ => throw new NotImplementedException(),
        };
    }

    internal static SKRect ToSkiaSharpRectangle(
        this Thickness thickness,
        double width,
        double height)
    {
        return SKRect.Create(
            x: (float)thickness.Left,
            y: (float)thickness.Top,
            width: (float)(width - thickness.Left - thickness.Right),
            height: (float)(height - thickness.Top - thickness.Bottom));
    }

    internal static SKFontStyle ToSkiaSharpFontStyle(
        this FontStyle style,
        FontWeight weight)
    {
        if (style == FontStyles.Italic &&
            weight == FontWeights.Bold)
        {
            return SKFontStyle.BoldItalic;
        }
        if (style == FontStyles.Italic)
        {
            return SKFontStyle.Italic;
        }
        if (style == FontStyles.Oblique)
        {
        }
        if (weight == FontWeights.Bold)
        {
            return SKFontStyle.Bold;
        }
        
        return SKFontStyle.Normal;
    }

    // internal static System.Drawing.FontFamily ToSkiaSharpFontFamily(
    //     this FontFamily family)
    // {
    //     return new System.Drawing.FontFamily(
    //         name: family.Source);
    // }
    
    internal static SKPaint ToSkiaSharpPaint(
        this Brush? brush,
        float width)
    {
        var paint = brush.ToSkiaSharpPaint();
        paint.StrokeWidth = width;
        paint.Style = SKPaintStyle.Stroke;
        
        return paint;
    }

}
