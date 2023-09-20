using Microsoft.Maui.LifecycleEvents;

namespace H.NotifyIcon;

/// <summary>
/// 
/// </summary>
[CLSCompliant(false)]
[WeakEvent("WindowClosed")]
public static partial class MauiAppBuilderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static MauiAppBuilder UseNotifyIcon(this MauiAppBuilder builder)
    {
        return builder
            .ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                    events.AddWindows(windows => windows
                           .OnClosed((window, _) => RaiseWindowClosedEvent(window)));
#endif
            });
    }
}
