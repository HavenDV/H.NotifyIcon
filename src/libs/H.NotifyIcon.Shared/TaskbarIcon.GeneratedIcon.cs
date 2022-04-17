#if HAS_WPF
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using FontFamily = System.Windows.Media.FontFamily;
using SystemFonts = System.Windows.SystemFonts;
#elif HAS_WINUI
using Brush = Microsoft.UI.Xaml.Media.Brush;
using FontFamily = Microsoft.UI.Xaml.Media.FontFamily;
using Colors = Microsoft.UI.Colors;
#else
using Brush = Windows.UI.Xaml.Media.Brush;
using FontFamily = Windows.UI.Xaml.Media.FontFamily;
using Colors = Windows.UI.Colors;
#endif

namespace H.NotifyIcon;

/// <inheritdoc/>
public partial class TaskbarIcon
{
    #region Constants

    /// <summary>
    /// Category name that is set on designer properties.
    /// </summary>
    public const string GeneratedIconCategoryName = "NotifyIcon GeneratedIcon";

    #endregion

    #region Properties

    #region Text

    /// <summary>Identifies the <see cref="GeneratedIconText"/> dependency property.</summary>
    public static readonly DependencyProperty GeneratedIconTextProperty =
        DependencyProperty.Register(
            nameof(GeneratedIconText),
            typeof(string),
            typeof(TaskbarIcon),
            new PropertyMetadata(string.Empty, (d, _) => ((TaskbarIcon)d).RefreshGeneratedIcon()));

    /// <summary>
    /// A property wrapper for the <see cref="GeneratedIconTextProperty"/>
    /// dependency property:<br/>
    /// Defines generated icon text.
    /// Defaults to string.Empty.
    /// </summary>
    [Category(GeneratedIconCategoryName)]
    [Description("Defines generated icon text.")]
#if HAS_WPF
    [Localizability(LocalizationCategory.Text)]
#endif
    public string? GeneratedIconText
    {
        get { return (string?)GetValue(GeneratedIconTextProperty); }
        set { SetValue(GeneratedIconTextProperty, value); }
    }

    #endregion

    #region TextMargin

    /// <summary>Identifies the <see cref="GeneratedIconTextMargin"/> dependency property.</summary>
    public static readonly DependencyProperty GeneratedIconTextMarginProperty =
        DependencyProperty.Register(
            nameof(GeneratedIconTextMargin),
            typeof(Thickness),
            typeof(TaskbarIcon),
            new PropertyMetadata(default(Thickness), (d, _) => ((TaskbarIcon)d).RefreshGeneratedIcon()));

    /// <summary>
    /// A property wrapper for the <see cref="GeneratedIconTextMarginProperty"/>
    /// dependency property:<br/>
    /// Defines generated icon text margin.
    /// Defaults to (0,0,0,0).
    /// </summary>
    [Category(GeneratedIconCategoryName)]
    [Description("Defines generated icon text margin.")]
    public Thickness GeneratedIconTextMargin
    {
        get { return (Thickness)GetValue(GeneratedIconTextMarginProperty); }
        set { SetValue(GeneratedIconTextMarginProperty, value); }
    }

    #endregion

    #region Foreground

    /// <summary>Identifies the <see cref="GeneratedIconForeground"/> dependency property.</summary>
    public static readonly DependencyProperty GeneratedIconForegroundProperty =
        DependencyProperty.Register(
            nameof(GeneratedIconForeground),
            typeof(Brush),
            typeof(TaskbarIcon),
            new PropertyMetadata(
#if HAS_WPF
                Brushes.Black,
#else
                new SolidColorBrush(Colors.Black),
#endif
                (d, _) => ((TaskbarIcon)d).RefreshGeneratedIcon()));

    /// <summary>
    /// A property wrapper for the <see cref="GeneratedIconForegroundProperty"/>
    /// dependency property:<br/>
    /// Defines generated icon foreground.
    /// Defaults to Brushes.Black.
    /// </summary>
    [Category(GeneratedIconCategoryName)]
    [Description("Defines generated icon foreground.")]
    public Brush? GeneratedIconForeground
    {
        get { return (Brush?)GetValue(GeneratedIconForegroundProperty); }
        set { SetValue(GeneratedIconForegroundProperty, value); }
    }

    #endregion

    #region Background

    /// <summary>Identifies the <see cref="GeneratedIconBackground"/> dependency property.</summary>
    public static readonly DependencyProperty GeneratedIconBackgroundProperty =
        DependencyProperty.Register(
            nameof(GeneratedIconBackground),
            typeof(Brush),
            typeof(TaskbarIcon),
            new PropertyMetadata(null, (d, _) => ((TaskbarIcon)d).RefreshGeneratedIcon()));

    /// <summary>
    /// A property wrapper for the <see cref="GeneratedIconBackgroundProperty"/>
    /// dependency property:<br/>
    /// Defines generated icon background.
    /// Defaults to null.
    /// </summary>
    [Category(GeneratedIconCategoryName)]
    [Description("Defines generated icon background.")]
    public Brush? GeneratedIconBackground
    {
        get { return (Brush?)GetValue(GeneratedIconBackgroundProperty); }
        set { SetValue(GeneratedIconBackgroundProperty, value); }
    }

    #endregion

    #region FontFamily

    /// <summary>Identifies the <see cref="GeneratedIconFontFamily"/> dependency property.</summary>
    public static readonly DependencyProperty GeneratedIconFontFamilyProperty =
        DependencyProperty.Register(
            nameof(GeneratedIconFontFamily),
            typeof(FontFamily),
            typeof(TaskbarIcon),
            new PropertyMetadata(
#if HAS_WPF
                SystemFonts.IconFontFamily,
#else
                new FontFamily("Segoe UI"),
#endif
                (d, _) => ((TaskbarIcon)d).RefreshGeneratedIcon()));

    /// <summary>
    /// A property wrapper for the <see cref="GeneratedIconFontFamilyProperty"/>
    /// dependency property:<br/>
    /// Defines generated icon font size.
    /// Defaults to SystemFonts.IconFontSize.
    /// </summary>
    [Category(GeneratedIconCategoryName)]
    [Description("Defines generated icon font family.")]
#if HAS_WPF
    [Localizability(LocalizationCategory.Font)]
#endif
    public FontFamily? GeneratedIconFontFamily
    {
        get { return (FontFamily?)GetValue(GeneratedIconFontFamilyProperty); }
        set { SetValue(GeneratedIconFontFamilyProperty, value); }
    }

    #endregion

    #region FontSize

    /// <summary>Identifies the <see cref="GeneratedIconFontSize"/> dependency property.</summary>
    public static readonly DependencyProperty GeneratedIconFontSizeProperty =
        DependencyProperty.Register(
            nameof(GeneratedIconFontSize),
            typeof(double),
            typeof(TaskbarIcon),
            new PropertyMetadata(
#if HAS_WPF
                SystemFonts.IconFontSize,
#else
                12.0,
#endif
                (d, _) => ((TaskbarIcon)d).RefreshGeneratedIcon()));

    /// <summary>
    /// A property wrapper for the <see cref="GeneratedIconFontSizeProperty"/>
    /// dependency property:<br/>
    /// Defines generated icon font size.
    /// Defaults to SystemFonts.IconFontSize.
    /// </summary>
    [Category(GeneratedIconCategoryName)]
    [Description("Defines generated icon font size.")]
#if HAS_WPF
    [Localizability(LocalizationCategory.None)]
    [TypeConverter(typeof(FontSizeConverter))]
#endif
    public double GeneratedIconFontSize
    {
        get { return (double)GetValue(GeneratedIconFontSizeProperty); }
        set { SetValue(GeneratedIconFontSizeProperty, value); }
    }

    #endregion

    #endregion

    #region Methods

    private void RefreshGeneratedIcon()
    {
        using var font = new Font(
            GeneratedIconFontFamily?.Source ?? string.Empty,
            (int)GeneratedIconFontSize);
        Icon = IconGenerator.Generate(
            backgroundColor: GeneratedIconBackground.ToSystemDrawingColor(),
            foregroundColor: GeneratedIconForeground.ToSystemDrawingColor(),
            text: GeneratedIconText,
            font: font,
            textRectangle: GeneratedIconTextMargin.ToSystemDrawingRectangleF(width: 32, height: 32));
    }

    #endregion
}
