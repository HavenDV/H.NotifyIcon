namespace H.NotifyIcon;

/// <summary>
/// 
/// </summary>
[DependencyProperty<string>("Text", DefaultValue = "", OnChanged = nameof(Refresh),
    Description = "Defines generated icon text.", Category = Category, Localizability = Localizability.Text)]
[DependencyProperty<Thickness>("Margin", OnChanged = nameof(Refresh),
    Description = "Defines generated icon margin.", Category = Category)]
[DependencyProperty<BackgroundType>("BackgroundType", OnChanged = nameof(Refresh),
    Description = "Defines generated icon background type.", Category = Category)]
[DependencyProperty<Thickness>("TextMargin", OnChanged = nameof(Refresh),
    Description = "Defines generated icon text margin.", Category = Category)]
[DependencyProperty<CornerRadius>("CornerRadius", OnChanged = nameof(Refresh),
    Description = "Defines generated icon corner radius.", Category = Category)]
[DependencyProperty<Brush>("Foreground", OnChanged = nameof(Refresh),
#if HAS_WPF
    DefaultValueExpression = "Brushes.Black",
#else
    DefaultValueExpression = "new SolidColorBrush(Colors.Black)",
#endif
    Description = "Defines generated icon foreground.", Category = Category)]
[DependencyProperty<Brush>("Background", OnChanged = nameof(Refresh),
    Description = "Defines generated icon background.", Category = Category)]
[DependencyProperty<FontFamily>("FontFamily", OnChanged = nameof(Refresh),
#if HAS_WPF
    DefaultValueExpression = "SystemFonts.IconFontFamily",
#else
    DefaultValueExpression = "new FontFamily(\"Segoe UI\")",
#endif
    Description = "Defines generated icon font family.", Category = Category, Localizability = Localizability.Font)]
[DependencyProperty<FontStyle>("FontStyle", OnChanged = nameof(Refresh),
#if HAS_WPF
    DefaultValueExpression = "SystemFonts.IconFontStyle",
#else
    DefaultValue = FontStyle.Normal,
#endif
    Description = "Defines generated icon font style.", Category = Category)]
[DependencyProperty<FontWeight>("FontWeight", OnChanged = nameof(Refresh),
#if HAS_WPF
    DefaultValueExpression = "SystemFonts.IconFontWeight",
#else
    DefaultValueExpression = "FontWeights.Normal",
#endif
    Description = "Defines generated icon font weight.", Category = Category)]
[DependencyProperty<FontStretch>("FontStretch", OnChanged = nameof(Refresh),
#if HAS_WPF
    DefaultValueExpression = "FontStretches.Normal",
#else
    DefaultValue = FontStretch.Normal,
#endif
    Description = "Defines generated icon font stretch.", Category = Category)]
[DependencyProperty<double>("FontSize", OnChanged = nameof(Refresh),
#if HAS_WPF
    DefaultValueExpression = "4 * SystemFonts.IconFontSize",
    TypeConverter = typeof(FontSizeConverter),
#else
    DefaultValue = 4 * 12.0,
#endif
    Description = "Defines generated icon font size.", Category = Category,
    Localizability = Localizability.None)]
[DependencyProperty<int>("Size", OnChanged = nameof(Refresh), DefaultValue = 128,
    Description = "Defines generated icon size.", Category = Category)]
[DependencyProperty<float>("BorderThickness", OnChanged = nameof(Refresh),
    Description = "Defines generated icon border thickness.", Category = Category)]
[DependencyProperty<Brush>("BorderBrush", OnChanged = nameof(Refresh),
#if HAS_WPF
    DefaultValueExpression = "Brushes.Black",
#else
    DefaultValueExpression = "new SolidColorBrush(Colors.Black)",
#endif
    Description = "Defines generated icon border brush.", Category = Category)]
[DependencyProperty<Icon>("Icon", ClsCompliant = false,
    Description = "Defines generated icon. Use this for dynamically generated content.", Category = Category)]
#if HAS_WINUI || HAS_UNO
[CLSCompliant(false)]
#endif
public sealed partial class GeneratedIcon : DependencyObject, IDisposable
{
    #region Constants

    /// <summary>
    /// Category name that is set on designer properties.
    /// </summary>
    public const string Category = "NotifyIcon";

    #endregion

    #region Properties

    internal TaskbarIcon? TaskbarIcon { get; set; }

    partial void OnIconChanged(Icon? oldValue, Icon? newValue)
    {
        oldValue?.Dispose();

        if (TaskbarIcon == null)
        {
            return;
        }

        TaskbarIcon.UpdateIcon(newValue);
    }

    #endregion

    #region Methods

    internal void Refresh()
    {
        if (TaskbarIcon == null)
        {
            return;
        }

#if HAS_SYSTEM_DRAWING
        var size = Size;
        using var fontFamily =
            FontFamily?.ToSystemDrawingFontFamily() ??
            new System.Drawing.FontFamily(string.Empty);
        using var font = new System.Drawing.Font(
            fontFamily,
            (float)FontSize,
            FontStyle.ToSystemDrawingFontStyle(FontWeight));
        using var baseImage = TaskbarIcon.Icon?.ToBitmap();
        using var pen = BorderBrush.ToSystemDrawingPen(BorderThickness);
        using var backgroundBrush = Background.ToSystemDrawingBrush();
        using var foregroundBrush = Foreground.ToSystemDrawingBrush();

        Icon = IconGenerator.Generate(
            backgroundBrush: backgroundBrush,
            foregroundBrush: foregroundBrush,
            pen: BorderThickness > 0.01F
                ? pen
                : null,
            backgroundType: BackgroundType,
            cornerRadius: (float)CornerRadius.TopLeft,
            rectangle: Margin == default
                ? null
                : Margin.ToSystemDrawingRectangleF(width: size, height: size),
            text: Text,
            font: font,
            textRectangle: TextMargin.ToSystemDrawingRectangleF(width: size, height: size),
            baseImage: baseImage,
            size: size);
#elif HAS_SKIA_SHARP
        var size = Size;
        // using var fontFamily =
        //     FontFamily?.ToSystemDrawingFontFamily() ??
        //     new System.Drawing.FontFamily(string.Empty);
        // using var font = new SkiaSharp.SKFont(
        //     fontFamily,
        //     (float)FontSize,
        //     FontStyle.ToSkiaSharpFontStyle(FontWeight));
        //using var baseImage = TaskbarIcon.Icon?.ToBitmap();
        using var pen = BorderBrush.ToSkiaSharpPaint(BorderThickness);
        using var backgroundBrush = Background.ToSkiaSharpPaint();
        using var foregroundBrush = Foreground.ToSkiaSharpPaint();

        Icon = IconGenerator.Generate(
            backgroundBrush: backgroundBrush,
            foregroundBrush: foregroundBrush,
            pen: BorderThickness > 0.01F
                ? pen
                : null,
            backgroundType: BackgroundType,
            cornerRadius: (float)CornerRadius.TopLeft,
            rectangle: Margin == default
                ? null
                : Margin.ToSkiaSharpRectangle(width: size, height: size),
            text: Text,
            //font: font,
            textRectangle: TextMargin.ToSkiaSharpRectangle(width: size, height: size),
            //baseImage: baseImage,
            size: size);
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        Icon?.Dispose();
    }

    #endregion
}
