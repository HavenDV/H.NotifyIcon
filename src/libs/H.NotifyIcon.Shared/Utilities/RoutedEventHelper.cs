namespace H.NotifyIcon;

internal static class RoutedEventHelper
{
#if HAS_WPF

    internal static RoutedEventArgs RaiseRoutedEvent(this UIElement target, RoutedEventArgs args)
    {
        target.RaiseEvent(args);

        return args;
    }

#endif

    internal static void AddHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
    {
        if (element is UIElement uIElement)
        {
            uIElement.AddHandler(routedEvent, handler, handledEventsToo: false);
        }
#if HAS_WPF
        else if (element is ContentElement contentElement)
        {
            contentElement.AddHandler(routedEvent, handler, handledEventsToo: false);
        }
#endif
    }

    internal static void RemoveHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
    {
        if (element is UIElement uIElement)
        {
            uIElement.RemoveHandler(routedEvent, handler);
        }
#if HAS_WPF
        else if (element is ContentElement contentElement)
        {
            contentElement.RemoveHandler(routedEvent, handler);
        }
#endif
    }
}
