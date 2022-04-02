namespace H.NotifyIcon;

internal static class RoutedEventHelper
{
    #region RoutedEvent Helper Methods

#if HAS_WPF

    internal static void RaiseEvent(DependencyObject target, RoutedEventArgs args)
    {
        if (target is UIElement uiElement)
        {
            uiElement.RaiseEvent(args);
        }
        else if (target is ContentElement contentElement)
        {
            contentElement.RaiseEvent(args);
        }
    }
    
#endif

    internal static void AddHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
    {
        if (element is UIElement uie)
        {
            uie.AddHandler(routedEvent, handler, handledEventsToo: false);
        }
#if HAS_WPF
        else if (element is ContentElement ce)
        {
            ce.AddHandler(routedEvent, handler, handledEventsToo: false);
        }
#endif
    }

    internal static void RemoveHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
    {
        if (element is UIElement uie)
        {
            uie.RemoveHandler(routedEvent, handler);
        }
#if HAS_WPF
        else if (element is ContentElement ce)
        {
            ce.RemoveHandler(routedEvent, handler);
        }
#endif
    }

    #endregion
}
