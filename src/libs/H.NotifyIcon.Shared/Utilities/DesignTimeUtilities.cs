namespace H.NotifyIcon;

internal static class DesignTimeUtilities
{
    #region Properties

    public static bool IsDesignMode { get; }

    #endregion

    #region Constructors

    static DesignTimeUtilities()
    {
#if HAS_WPF
        IsDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(
            DesignerProperties.IsInDesignModeProperty,
            typeof (FrameworkElement)).Metadata.DefaultValue;
#endif
    }

    #endregion
}
