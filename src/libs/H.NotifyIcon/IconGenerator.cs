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
    /// Generates 32x32 standard icon with selected parameters.
    /// </summary>
    /// <returns></returns>
    public static Icon Generate(
        Color backgroundColor,
        Color foregroundColor,
        BackgroundType backgroundType = BackgroundType.Ellipse,
        float cornerRadius = 0.0F,
        RectangleF? rectangle = null,
        string? text = null,
        Font? font = null,
        RectangleF? textRectangle = null,
        Image? baseImage = null)
    {
        using var bitmap = baseImage == null
            ? new Bitmap(32, 32)
            : new Bitmap(baseImage, 32, 32);
        using var backgroundBrush = new SolidBrush(backgroundColor);
        using var foregroundBrush = new SolidBrush(foregroundColor);
        using var graphics = Graphics.FromImage(bitmap);

        var rect = rectangle ?? new Rectangle(0, 0, 32, 32);
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

        if (!string.IsNullOrWhiteSpace(text) &&
            font != null)
        {
            if (textRectangle == null)
            {
                var size = graphics.MeasureString(
                    text: text,
                    font: font,
                    layoutArea: new SizeF(32, 32));

                graphics.DrawString(
                    s: text,
                    font: font,
                    brush: foregroundBrush,
                    layoutRectangle: new RectangleF(
                        32 / 2 - size.Width / 2,
                        32 / 2 - size.Height / 2,
                        size.Width,
                        size.Height));
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
