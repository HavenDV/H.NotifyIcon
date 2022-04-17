using System.Drawing;
using System.Drawing.Drawing2D;

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
public static class IconGenerator
{
    private static GraphicsPath GetRoundedRectGraphicsPath(RectangleF bounds, float radius)
    {
        var path = new GraphicsPath();
        if (radius == 0)
        {
            path.AddRectangle(bounds);
            return path;
        }

        var diameter = radius * 2;
        var size = new SizeF(diameter, diameter);
        var arc = new RectangleF(bounds.Location, size);

        // top left arc  
        path.AddArc(arc, 180, 90);

        // top right arc  
        arc.X = bounds.Right - diameter;
        path.AddArc(arc, 270, 90);

        // bottom right arc  
        arc.Y = bounds.Bottom - diameter;
        path.AddArc(arc, 0, 90);

        // bottom left arc 
        arc.X = bounds.Left;
        path.AddArc(arc, 90, 90);

        path.CloseFigure();

        return path;
    }

    /// <summary>
    /// Generates <paramref name="size"/> x <paramref name="size"/> standard icon with selected parameters.
    /// </summary>
    /// <returns></returns>
    public static Icon Generate(
        Color backgroundColor,
        Color foregroundColor,
        Pen? pen = null,
        BackgroundType backgroundType = BackgroundType.Ellipse,
        float cornerRadius = 0.0F,
        RectangleF? rectangle = null,
        string? text = null,
        Font? font = null,
        RectangleF? textRectangle = null,
        Image? baseImage = null,
        int size = 128)
    {
        using var bitmap = baseImage == null
            ? new Bitmap(size, size)
            : new Bitmap(baseImage, size, size);
        using var backgroundBrush = new SolidBrush(backgroundColor);
        using var foregroundBrush = new SolidBrush(foregroundColor);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

        var rect = rectangle ?? new Rectangle(0, 0, size, size);
        switch (backgroundType)
        {
            case BackgroundType.Rectangle:
                graphics.FillRectangle(
                    brush: backgroundBrush,
                    rect: rect);
                break;

            case BackgroundType.RoundedRectangle:
                {
                    using var path = GetRoundedRectGraphicsPath(rect, cornerRadius);
                    graphics.FillPath(
                        brush: backgroundBrush,
                        path: path);
                }
                break;

            case BackgroundType.Ellipse:
                graphics.FillEllipse(
                    brush: backgroundBrush,
                    rect: rect);
                break;
        }

        if (pen != null)
        {
            switch (backgroundType)
            {
                case BackgroundType.Rectangle:
                    graphics.DrawRectangle(
                        pen: pen,
                        rect: new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));
                    break;

                case BackgroundType.RoundedRectangle:
                    {
                        using var path = GetRoundedRectGraphicsPath(rect, cornerRadius);
                        graphics.DrawPath(
                            pen: pen,
                            path: path);
                    }
                    break;

                case BackgroundType.Ellipse:
                    graphics.DrawEllipse(
                        pen: pen,
                        rect: rect);
                    break;
            }
        }

        if (!string.IsNullOrWhiteSpace(text) &&
            font != null)
        {
            if (textRectangle == null)
            {
                var textSize = graphics.MeasureString(
                    text: text,
                    font: font,
                    layoutArea: new SizeF(size, size));

                graphics.DrawString(
                    s: text,
                    font: font,
                    brush: foregroundBrush,
                    layoutRectangle: new RectangleF(
                        size / 2 - textSize.Width / 2,
                        size / 2 - textSize.Height / 2,
                        textSize.Width,
                        textSize.Height));
            }
            else
            {
                graphics.DrawString(
                    s: text,
                    font: font,
                    brush: foregroundBrush,
                    layoutRectangle: textRectangle.Value);
            }
        }

        return Icon.FromHandle(bitmap.GetHicon());
    }
}
