#if HAS_SYSTEM_DRAWING
namespace H.NotifyIcon;

internal static class ToSystemDrawingExtensions
{
    internal static System.Drawing.Size ToSystemDrawingSize(this Size size)
    {
        return new System.Drawing.Size(
            width: (int)size.Width,
            height: (int)size.Height);
    }

    internal static System.Drawing.PointF ToSystemDrawingPointF(this Point point)
    {
        return new System.Drawing.PointF((float)point.X, (float)point.Y);
    }

    internal static System.Drawing.Color ToSystemDrawingColor(this Color color)
    {
        return System.Drawing.Color.FromArgb(
            alpha: color.A,
            red: color.R,
            green: color.G,
            blue: color.B);
    }

    internal static System.Drawing.Brush ToSystemDrawingBrush(this Brush? brush)
    {
        return brush switch
        {
            null => new System.Drawing.SolidBrush(System.Drawing.Color.Transparent),
            SolidColorBrush solidColorBrush => new System.Drawing.SolidBrush(solidColorBrush.Color.ToSystemDrawingColor()),
            LinearGradientBrush linearGradientBrush => new System.Drawing.Drawing2D.LinearGradientBrush(
                point1: linearGradientBrush.StartPoint.ToSystemDrawingPointF(),
                point2: linearGradientBrush.EndPoint.ToSystemDrawingPointF(),
                color1: linearGradientBrush.GradientStops.ElementAt(0).Color.ToSystemDrawingColor(),
                color2: linearGradientBrush.GradientStops.ElementAt(1).Color.ToSystemDrawingColor()),
            _ => throw new NotImplementedException(),
        };
    }

    internal static System.Drawing.RectangleF ToSystemDrawingRectangleF(
        this Thickness thickness,
        double width,
        double height)
    {
        return new System.Drawing.RectangleF(
            x: (float)thickness.Left,
            y: (float)thickness.Top,
            width: (float)(width - thickness.Left - thickness.Right),
            height: (float)(height - thickness.Top - thickness.Bottom));
    }

    internal static System.Drawing.FontStyle ToSystemDrawingFontStyle(
        this FontStyle style,
        FontWeight weight)
    {
        var fontStyle = System.Drawing.FontStyle.Regular;
        if (style == FontStyles.Italic)
        {
            fontStyle |= System.Drawing.FontStyle.Italic;
        }
        if (style == FontStyles.Oblique)
        {
        }
        if (weight == FontWeights.Bold)
        {
            fontStyle |= System.Drawing.FontStyle.Bold;
        }
        
        return fontStyle;
    }

    internal static System.Drawing.FontFamily ToSystemDrawingFontFamily(
        this FontFamily family)
    {
        return new System.Drawing.FontFamily(
            name: family.Source);
    }

    internal static System.Drawing.Pen ToSystemDrawingPen(
        this Brush? brush,
        float width)
    {
        using var penBrush = brush.ToSystemDrawingBrush();

        return new System.Drawing.Pen(
            brush: penBrush,
            width: width);
    }

}
#endif
