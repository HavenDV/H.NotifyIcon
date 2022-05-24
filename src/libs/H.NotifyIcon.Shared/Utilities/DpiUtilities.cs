namespace H.NotifyIcon;

internal static class DpiUtilities
{
    public static double DpiFactorX { get; private set; } = 1;
    public static double DpiFactorY { get; private set; } = 1;

    static DpiUtilities()
    {
        UpdateDpiFactors();
    }

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

    public static System.Drawing.Point ScaleWithDpi(this System.Drawing.Point point)
    {
        return new System.Drawing.Point
        {
            X = (int)(point.X / DpiFactorX),
            Y = (int)(point.Y / DpiFactorY),
        };
    }

    public static System.Drawing.Size ScaleWithDpi(this System.Drawing.Size size)
    {
        return new System.Drawing.Size
        {
            Width = (int)(size.Width / DpiFactorX),
            Height = (int)(size.Height / DpiFactorY),
        };
    }
}
