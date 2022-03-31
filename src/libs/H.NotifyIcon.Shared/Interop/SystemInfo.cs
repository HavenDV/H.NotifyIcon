namespace H.NotifyIcon;

/// <summary>
/// This class is a helper for system information, currently to get the DPI factors
/// </summary>
internal static class SystemInfo
{
    /// <summary>
    /// Make sure the initial value is calculated at the first access
    /// </summary>
    static SystemInfo()
    {
        UpdateDpiFactors();
    }

    /// <summary>
    /// This calculates the current DPI values and sets this into the DpiFactorX/DpiFactorY values
    /// </summary>
    internal static void UpdateDpiFactors()
    {
#if HAS_WPF
        using (var source = new HwndSource(new HwndSourceParameters()))
        {
            if (source.CompositionTarget?.TransformToDevice != null)
            {
                DpiFactorX = source.CompositionTarget.TransformToDevice.M11;
                DpiFactorY = source.CompositionTarget.TransformToDevice.M22;
                return;
            }
        }
#endif

        DpiFactorX = DpiFactorY = 1;
    }

    /// <summary>
    /// Returns the DPI X Factor
    /// </summary>
    public static double DpiFactorX { get; private set; } = 1;

    /// <summary>
    /// Returns the DPI Y Factor
    /// </summary>
    public static double DpiFactorY { get; private set; } = 1;

    /// <summary>
    /// Scale the supplied point to the current DPI settings
    /// </summary>
    /// <param name="point"></param>
    /// <returns>Point</returns>
    [Pure]
    public static Point ScaleWithDpi(this Point point)
    {
        return new Point
        {
            X = (int)(point.X / DpiFactorX),
            Y = (int)(point.Y / DpiFactorY)
        };
    }
}
