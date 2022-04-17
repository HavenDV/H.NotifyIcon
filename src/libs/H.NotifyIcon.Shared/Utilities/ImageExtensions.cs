#if HAS_WPF
using System.IO;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;
using SystemFonts = System.Windows.SystemFonts;
#elif HAS_WINUI
using Microsoft.UI.Text;
using Windows.UI.Text;
using Brush = Microsoft.UI.Xaml.Media.Brush;
using FontFamily = Microsoft.UI.Xaml.Media.FontFamily;
using FontStyle = Windows.UI.Text.FontStyle;
using FontStyles = Windows.UI.Text.FontStyle;
#else
using Windows.UI.Text;
using Brush = Windows.UI.Xaml.Media.Brush;
using FontFamily = Windows.UI.Xaml.Media.FontFamily;
using FontStyle = Windows.UI.Text.FontStyle;
using FontStyles = Windows.UI.Text.FontStyle;
#endif

namespace H.NotifyIcon;

internal static class ImageExtensions
{
#if HAS_WPF
    internal static Icon ToIcon(this Uri uri)
#else
    internal static async Task<Icon> ToIconAsync(this Uri uri)
#endif
    {
#if HAS_WPF
        if (uri.Scheme == Uri.UriSchemeFile)
        {
            using var fileStream = File.OpenRead(uri.LocalPath);

            return new Icon(fileStream);
        }

        var streamInfo =
            Application.GetResourceStream(uri) ??
            throw new ArgumentException($"Uri: {uri} is not resolved.");
        using var stream = streamInfo.Stream;
#else
        var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
        using var stream = await file.OpenStreamForReadAsync().ConfigureAwait(true);
#endif

        return new Icon(stream);
    }

#if HAS_WPF

    public static Icon ToIcon(this BitmapFrame frame)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(frame);

        using var stream = new MemoryStream();
        encoder.Save(stream);

        var iconBytes = stream.ToArray().ConvertPngToIco();
        using var iconStream = new MemoryStream(iconBytes);
        
        return new Icon(iconStream);
    }

    public static Icon ToIcon(this BitmapSource bitmap)
    {
        return BitmapFrame.Create(bitmap).ToIcon();
    }

#endif

#if HAS_WPF
    public static Icon? ToIcon(this ImageSource imageSource)
#else
    public static async Task<Icon?> ToIconAsync(this ImageSource imageSource)
#endif
    {
        switch(imageSource)
        {
            case null:
                return null;

            case BitmapImage bitmapImage:
                {
                    var uri = bitmapImage.UriSource;
                    
#if HAS_WPF
                    return uri.ToIcon();
#else
                    return await uri.ToIconAsync().ConfigureAwait(true);
#endif
                }
#if HAS_WPF
            case BitmapFrame frame:
                {
                    var uri = new Uri(frame.ToString());

                    return uri.ToIcon();
                }
#endif

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }

    internal static System.Drawing.Color ToSystemDrawingColor(this Brush? brush)
    {
        if (brush == null)
        {
            return System.Drawing.Color.Transparent;
        }

        if (brush is not SolidColorBrush solidColorBrush)
        {
            throw new NotImplementedException();
        }

        var color = solidColorBrush.Color;

        return System.Drawing.Color.FromArgb(
            alpha: color.A,
            red: color.R,
            green: color.G,
            blue: color.B);
    }

    internal static RectangleF ToSystemDrawingRectangleF(
        this Thickness thickness,
        double width,
        double height)
    {
        return new RectangleF(
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
        return new System.Drawing.FontFamily(family.Source);
    }

}
