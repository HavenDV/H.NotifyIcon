namespace H.NotifyIcon;

internal static class FromSystemDrawingExtensions
{
#if HAS_WINUI && !HAS_UNO

    internal static RectInt32 ToRectInt32(this System.Drawing.Rectangle rectangle)
    {
        return new RectInt32(
            _X: rectangle.X,
            _Y: rectangle.Y,
            _Width: rectangle.Width,
            _Height: rectangle.Height);
    }

#endif
}
