namespace H.NotifyIcon;

internal static class ImageExtensions
{
#if HAS_WPF
    internal static System.Drawing.Icon ToIcon(this Uri uri)
#else
    internal static async Task<System.Drawing.Icon> ToIconAsync(this Uri uri)
#endif
    {
#if HAS_WPF
        if (uri.Scheme == Uri.UriSchemeFile)
        {
            using var fileStream = File.OpenRead(uri.LocalPath);

            return new System.Drawing.Icon(fileStream);
        }

        var streamInfo =
            Application.GetResourceStream(uri) ??
            throw new ArgumentException($"Uri: {uri} is not resolved.");
        using var stream = streamInfo.Stream;
#else
        var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
        using var stream = await file.OpenStreamForReadAsync().ConfigureAwait(true);
#endif

        return new System.Drawing.Icon(stream);
    }

#if HAS_WPF

    public static System.Drawing.Icon ToIcon(this BitmapFrame frame)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(frame);

        using var stream = new MemoryStream();
        encoder.Save(stream);

        var iconBytes = stream.ToArray().ConvertPngToIco();
        using var iconStream = new MemoryStream(iconBytes);
        
        return new System.Drawing.Icon(iconStream);
    }

    public static System.Drawing.Icon ToIcon(this BitmapSource bitmap)
    {
        return BitmapFrame.Create(bitmap).ToIcon();
    }

#endif

#if HAS_WPF
    public static System.Drawing.Icon? ToIcon(this ImageSource imageSource)
#else
    public static async Task<System.Drawing.Icon?> ToIconAsync(this ImageSource imageSource)
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

#if !HAS_WPF

    internal static RectInt32 ToRectInt32(this System.Drawing.Rectangle rectangle)
    {
        return new RectInt32(
            _X: rectangle.X,
            _Y: rectangle.Y,
            _Width: rectangle.Width,
            _Height: rectangle.Height);
    }
#endif

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
