using System.Diagnostics.CodeAnalysis;

namespace H.NotifyIcon;

[DependencyProperty<string>("ToolTipText", DefaultValue = "",
    Description = "A tooltip text that is being displayed if no custom ToolTip was set or if custom tooltips are not supported.", Category = CategoryName)]
[DependencyProperty<UIElement>("TrayToolTip",
    Description = "A custom UI element that is displayed as a tooltip if the user hovers over the taskbar icon. Works only with Vista and above. Accordingly, you should make sure that the ToolTipText property is set as well.", Category = CategoryName)]
[DependencyProperty<ToolTip2>("TrayToolTipResolved", IsReadOnly = true,
    Description = "Returns a ToolTip control that was created in order to display either TrayToolTip or ToolTipText", Category = CategoryName)]
[RoutedEvent("TrayToolTipOpen", RoutedEventStrategy.Bubble,
    Description = "Bubbled event that occurs when the custom ToolTip is being displayed.", Category = CategoryName)]
[RoutedEvent("PreviewTrayToolTipOpen", RoutedEventStrategy.Tunnel,
    Description = "Tunneled event that occurs when the custom ToolTip is being displayed.", Category = CategoryName)]
[RoutedEvent("TrayToolTipClose", RoutedEventStrategy.Bubble,
    Description = "Bubbled event that occurs when a custom tooltip is being closed.", Category = CategoryName)]
[RoutedEvent("PreviewTrayToolTipClose", RoutedEventStrategy.Tunnel,
    Description = "Tunneled event that occurs when a custom tooltip is being closed.", Category = CategoryName)]
[RoutedEvent("ToolTipOpened", RoutedEventStrategy.Bubble, IsAttached = true, Category = CategoryName)]
[RoutedEvent("ToolTipClose", RoutedEventStrategy.Bubble, IsAttached = true, Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Properties

    #region ToolTipText

    [SupportedOSPlatform("windows5.1.2600")]
    partial void OnToolTipTextChanged()
    {
        WriteToolTipSettings();
    }

    #endregion

    #region TrayToolTip

    [SupportedOSPlatform("windows5.1.2600")]
    partial void OnTrayToolTipChanged(UIElement? oldValue, UIElement? newValue)
    {
        CreateCustomToolTip();

#if HAS_WPF
        if (oldValue != null)
        {
            SetParentTaskbarIcon(oldValue, null);
        }
        if (newValue != null)
        {
            SetParentTaskbarIcon(newValue, this);
        }
#endif

        WriteToolTipSettings();
    }

    #endregion

#if !MACOS
    
    /// <summary>
    /// Indicates whether custom tooltips are supported, which depends
    /// on the OS. Windows Vista or higher is required in order to
    /// support this feature.
    /// </summary>
    [SupportedOSPlatform("windows5.1.2600")]
    public bool SupportsCustomToolTips => TrayIcon.SupportsCustomToolTips;

#endif
    
    /// <summary>
    /// Checks whether a non-tooltip popup is currently opened.
    /// </summary>
    private bool IsPopupOpen
    {
        get
        {
#if HAS_MAUI
            return false;
#else
            var popup = TrayPopupResolved;
#if HAS_WPF
            var menu = ContextMenu;
#else
            var menu = ContextFlyout;
#endif
#if HAS_WPF
            var balloon = CustomBalloon;
#else
            var balloon = (Popup?)null;
#endif

            return popup is { IsOpen: true } ||
                   menu is { IsOpen: true } ||
                   balloon is { IsOpen: true };
#endif
        }
    }

    #endregion

    #region Methods

    /// <remarks>We use a ToolTip rather than
    /// Popup because there was no way to prevent a
    /// popup from causing cyclic open/close commands if it was
    /// placed under the mouse. ToolTip internally uses a Popup of
    /// its own, but takes advance of Popup's internal IsHitTestVisible
    /// property which prevents this issue.</remarks>
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(ToolTip2))]
    private void CreateCustomToolTip()
    {
#if !MACOS && !HAS_MAUI
        var toolTip = TrayToolTip as ToolTip2 ?? new ToolTip2();
        if (TrayToolTip != null &&
            TrayToolTip is not ToolTip2)
        {
            toolTip.Content = TrayToolTip;
        }

        UpdateDataContext(toolTip, DataContext);

        TrayToolTipResolved = toolTip;
#endif
    }

    [SupportedOSPlatform("windows5.1.2600")]
    private void WriteToolTipSettings()
    {
        var text = ToolTipText;
#if !MACOS
        if (TrayIcon.Version == IconVersion.Vista)
        {
            // we need to set a tooltip text to get tooltip events from the
            // taskbar icon
            if (string.IsNullOrEmpty(text) && TrayToolTipResolved != null)
            {
                // if we have not tooltip text but a custom tooltip, we
                // need to set a dummy value (we're displaying the ToolTip control, not the string)
                text = "ToolTip";
            }
        }
#endif

        TrayIcon.UpdateToolTip(text ?? string.Empty);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Displays a custom tooltip, if available. This method is only
    /// invoked for Windows Vista and above.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args">Whether to show or hide the tooltip.</param>
    private void OnToolTipChange(object? sender, MessageWindow.ChangeToolTipStateRequestEventArgs args)
    {
#if !HAS_MAUI        
        if (TrayToolTipResolved == null)
        {
            return;
        }

        if (args.IsVisible)
        {
            if (IsPopupOpen)
            {
                return;
            }

            var previewArgs = OnPreviewTrayToolTipOpen();
#if HAS_WPF
            if (previewArgs.Handled)
            {
                return;
            }
#endif

#if HAS_WINUI
            // https://github.com/HavenDV/H.NotifyIcon/issues/47
            if (Environment.OSVersion.Version.Major > 10)
            {
                TrayToolTipResolved.IsOpen = true;
            }
#else
            TrayToolTipResolved.IsOpen = true;
#endif

#if HAS_WPF
            TrayToolTip?.RaiseEvent(new RoutedEventArgs(ToolTipOpenedEvent));
#endif

            OnTrayToolTipOpen();
        }
        else
        {
            var previewArgs = OnPreviewTrayToolTipClose();
#if HAS_WPF
            if (previewArgs.Handled)
            {
                return;
            }

            TrayToolTip?.RaiseEvent(new RoutedEventArgs(ToolTipCloseEvent));
#endif

#if HAS_WINUI
            // https://github.com/HavenDV/H.NotifyIcon/issues/47
            if (Environment.OSVersion.Version.Major > 10)
            {
                TrayToolTipResolved.IsOpen = false;
            }
#else
            TrayToolTipResolved.IsOpen = false;
#endif

            OnTrayToolTipClose();
        }
#endif
    }

    #endregion
}
