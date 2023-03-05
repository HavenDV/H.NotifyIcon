#if HAS_SYSTEM_DRAWING
using H.NotifyIcon.Interop;

namespace H.NotifyIcon;

internal static partial class ImageExtensions
{
    internal static System.Drawing.Icon ToSmallIcon(this Stream stream)
    {
        var iconSize = IconUtilities.GetRequiredCustomIconSize(largeIcon: false).ScaleWithDpi();

        return new System.Drawing.Icon(stream, iconSize);
    }
}
#endif
