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
        RectangleF? textRectangle = null)
    {
        using var bitmap = new Bitmap(32, 32);
        using var backgroundBrush = new SolidBrush(backgroundColor);
        using var foregroundBrush = new SolidBrush(foregroundColor);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.FillRectangle(backgroundBrush, new Rectangle(0, 0, 32, 32));
        if (!string.IsNullOrWhiteSpace(text) && 
            font != null)
        {
            graphics.DrawString(text, font, foregroundBrush, textRectangle ?? new Rectangle(0, 0, 32, 32));
        }

        return Icon.FromHandle(bitmap.GetHicon());
    }
}
