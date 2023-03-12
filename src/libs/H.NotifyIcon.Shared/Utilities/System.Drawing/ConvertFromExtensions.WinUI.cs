namespace H.NotifyIcon;

internal static class FromSystemDrawingExtensions
{
    internal static RectInt32 ToRectInt32(this System.Drawing.Rectangle rectangle)
    {
#if HAS_UNO
        return new RectInt32
        { 
            X = rectangle.X,
            Y = rectangle.Y,
            Width = rectangle.Width,
            Height = rectangle.Height,
        };
#else
        return new RectInt32(
            _X: rectangle.X,
            _Y: rectangle.Y,
            _Width: rectangle.Width,
            _Height: rectangle.Height);
#endif
    }
}
