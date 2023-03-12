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
[DependencyProperty<ImageSource>("BackgroundSource",
    Description = "Resolves an image source and uses this as background.", Category = Category)]
[CLSCompliant(false)]
public sealed partial class GeneratedIconSource : BitmapSource
{
    #region Constants

    /// <summary>
    /// Category name that is set on designer properties.
    /// </summary>
    public const string Category = "NotifyIcon";

    #endregion

    #region Methods

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Bitmap ToBitmap()
    {
        return Generate();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<Bitmap> ToBitmapAsync(CancellationToken cancellationToken = default)
    {
        return await GenerateAsync(cancellationToken).ConfigureAwait(true);
    }

    internal void Refresh()
    {
#if HAS_WPF
        OnChanged();
#endif
    }

    #endregion
}
