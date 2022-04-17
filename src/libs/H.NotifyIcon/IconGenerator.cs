using System.Drawing;

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
    /// <summary>
    /// Generates 32x32 standard icon with selected parameters.
    /// </summary>
    /// <returns></returns>
    public static Icon Generate(
        Color backgroundColor,
        Color foregroundColor,
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

        graphics.FillRectangle(
            brush: backgroundBrush,
            rect: new Rectangle(0, 0, 32, 32));
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
