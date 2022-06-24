namespace H.NotifyIcon;

[DependencyProperty<string>("GeneratedIconText", DefaultValue = "",
    Description = "Defines generated icon text.", Category = GeneratedIconCategory, Localizability = Localizability.Text)]
[DependencyProperty<Thickness>("GeneratedIconMargin",
    Description = "Defines generated icon margin.", Category = GeneratedIconCategory)]
[DependencyProperty<BackgroundType>("GeneratedIconBackgroundType",
    Description = "Defines generated icon background type.", Category = GeneratedIconCategory)]
[DependencyProperty<Thickness>("GeneratedIconTextMargin",
    Description = "Defines generated icon text margin.", Category = GeneratedIconCategory)]
[DependencyProperty<CornerRadius>("GeneratedIconCornerRadius",
    Description = "Defines generated icon corner radius.", Category = GeneratedIconCategory)]
[DependencyProperty<Brush>("GeneratedIconForeground",
#if HAS_WPF
    DefaultValueExpression = "Brushes.Black",
#else
    DefaultValueExpression = "new SolidColorBrush(Colors.Black)",
#endif
    Description = "Defines generated icon foreground.", Category = GeneratedIconCategory)]
[DependencyProperty<Brush>("GeneratedIconBackground",
    Description = "Defines generated icon background.", Category = GeneratedIconCategory)]
[DependencyProperty<FontFamily>("GeneratedIconFontFamily",
#if HAS_WPF
    DefaultValueExpression = "SystemFonts.IconFontFamily",
#else
    DefaultValueExpression = "new FontFamily(\"Segoe UI\")",
#endif
    Description = "Defines generated icon font family.", Category = GeneratedIconCategory, Localizability = Localizability.Font)]
[DependencyProperty<FontStyle>("GeneratedIconFontStyle",
#if HAS_WPF
    DefaultValueExpression = "SystemFonts.IconFontStyle",
#else
    DefaultValue = FontStyle.Normal,
#endif
    Description = "Defines generated icon font style.", Category = GeneratedIconCategory)]
[DependencyProperty<FontWeight>("GeneratedIconFontWeight",
#if HAS_WPF
    DefaultValueExpression = "SystemFonts.IconFontWeight",
#else
    DefaultValueExpression = "FontWeights.Normal",
#endif
    Description = "Defines generated icon font weight.", Category = GeneratedIconCategory)]
[DependencyProperty<FontStretch>("GeneratedIconFontStretch",
#if HAS_WPF
    DefaultValueExpression = "FontStretches.Normal",
#else
    DefaultValue = FontStretch.Normal,
#endif
    Description = "Defines generated icon font stretch.", Category = GeneratedIconCategory)]
[DependencyProperty<double>("GeneratedIconFontSize",
#if HAS_WPF
    DefaultValueExpression = "4 * SystemFonts.IconFontSize",
    TypeConverter = typeof(FontSizeConverter),
#else
    DefaultValue = 4 * 12.0,
#endif
    Description = "Defines generated icon font size.", Category = GeneratedIconCategory,
    Localizability = Localizability.None)]
[DependencyProperty<System.Drawing.Icon>("GeneratedIcon",
    Description = "Defines generated icon. Use this for dynamically generated System.Drawing.Icons", Category = GeneratedIconCategory)]
[DependencyProperty<int>("GeneratedIconSize", DefaultValue = 128,
    Description = "Defines generated icon size.", Category = GeneratedIconCategory)]
[DependencyProperty<float>("GeneratedIconBorderThickness",
    Description = "Defines generated icon border thickness.", Category = GeneratedIconCategory)]
[DependencyProperty<Brush>("GeneratedIconBorderBrush",
#if HAS_WPF
    DefaultValueExpression = "Brushes.Black",
#else
    DefaultValueExpression = "new SolidColorBrush(Colors.Black)",
#endif
    Description = "Defines generated icon border brush.", Category = GeneratedIconCategory)]
public partial class TaskbarIcon
{
    #region Constants

    /// <summary>
    /// Category name that is set on designer properties.
    /// </summary>
    public const string GeneratedIconCategory = "NotifyIcon GeneratedIcon";

    #endregion

    #region Properties

    partial void OnGeneratedIconTextChanged(string? oldValue, string? newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconMarginChanged(Thickness oldValue, Thickness newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconBackgroundTypeChanged(BackgroundType oldValue, BackgroundType newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconTextMarginChanged(Thickness oldValue, Thickness newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconCornerRadiusChanged(CornerRadius oldValue, CornerRadius newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconForegroundChanged(Brush? oldValue, Brush? newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconBackgroundChanged(Brush? oldValue, Brush? newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconFontFamilyChanged(FontFamily? oldValue, FontFamily? newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconFontStyleChanged(FontStyle oldValue, FontStyle newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconFontWeightChanged(FontWeight oldValue, FontWeight newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconFontStretchChanged(FontStretch oldValue, FontStretch newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconFontSizeChanged(double oldValue, double newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconChanged(System.Drawing.Icon? oldValue, System.Drawing.Icon? newValue)
    {
        oldValue?.Dispose();

        TrayIcon.UpdateIcon((nint?)newValue?.Handle ?? 0);
    }

    partial void OnGeneratedIconSizeChanged(int oldValue, int newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconBorderThicknessChanged(float oldValue, float newValue)
    {
        RefreshGeneratedIcon();
    }

    partial void OnGeneratedIconBorderBrushChanged(Brush? oldValue, Brush? newValue)
    {
        RefreshGeneratedIcon();
    }

    #endregion

    #region Methods

    private void RefreshGeneratedIcon()
    {
        var size = GeneratedIconSize;
        using var fontFamily =
            GeneratedIconFontFamily?.ToSystemDrawingFontFamily() ??
            new System.Drawing.FontFamily(string.Empty);
        using var font = new System.Drawing.Font(
            fontFamily,
            (float)GeneratedIconFontSize,
            GeneratedIconFontStyle.ToSystemDrawingFontStyle(GeneratedIconFontWeight));
        using var baseImage = Icon?.ToBitmap();
        using var pen = GeneratedIconBorderBrush.ToSystemDrawingPen(GeneratedIconBorderThickness);
        using var backgroundBrush = GeneratedIconBackground.ToSystemDrawingBrush();
        using var foregroundBrush = GeneratedIconForeground.ToSystemDrawingBrush();

        GeneratedIcon = IconGenerator.Generate(
            backgroundBrush: backgroundBrush,
            foregroundBrush: foregroundBrush,
            pen: GeneratedIconBorderThickness > 0.01F
                ? pen
                : null,
            backgroundType: GeneratedIconBackgroundType,
            cornerRadius: (float)GeneratedIconCornerRadius.TopLeft,
            rectangle: GeneratedIconMargin == default
                ? null
                : GeneratedIconMargin.ToSystemDrawingRectangleF(width: size, height: size),
            text: GeneratedIconText,
            font: font,
            textRectangle: GeneratedIconTextMargin == default
                ? null
                : GeneratedIconTextMargin.ToSystemDrawingRectangleF(width: size, height: size),
            baseImage: baseImage,
            size: size);
    }

    #endregion
}
