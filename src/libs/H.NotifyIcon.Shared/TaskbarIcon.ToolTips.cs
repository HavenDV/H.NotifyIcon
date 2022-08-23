namespace H.NotifyIcon;

[DependencyProperty<string>("ToolTipText", DefaultValue = "",
    Description = "A tooltip text that is being displayed if no custom ToolTip was set or if custom tooltips are not supported.", Category = CategoryName)]
[DependencyProperty<UIElement>("TrayToolTip",
    Description = "A custom UI element that is displayed as a tooltip if the user hovers over the taskbar icon. Works only with Vista and above. Accordingly, you should make sure that the ToolTipText property is set as well.", Category = CategoryName)]
[DependencyProperty<ToolTip>("TrayToolTipResolved", IsReadOnly = true,
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

    partial void OnToolTipTextChanged(string? oldValue, string? newValue)
    {
        //do not touch tooltips if we have a custom tooltip element
        if (TrayToolTip == null)
        {
            var currentToolTip = TrayToolTipResolved;
            if (currentToolTip == null)
            {
                //if we don't have a wrapper tooltip for the tooltip text, create it now
                CreateCustomToolTip();
            }
            else
            {
                //if we have a wrapper tooltip that shows the old tooltip text, just update content
                currentToolTip.Content = newValue;
            }
        }

        WriteToolTipSettings();
    }

    #endregion

    #region TrayToolTip

    partial void OnTrayToolTipChanged(UIElement? oldValue, UIElement? newValue)
    {
        //recreate tooltip control
        CreateCustomToolTip();

#if HAS_WPF
        if (oldValue != null)
        {
            //remove the taskbar icon reference from the previously used element
            SetParentTaskbarIcon(oldValue, null);
        }

        if (newValue != null)
        {
            //set this taskbar icon as a reference to the new tooltip element
            SetParentTaskbarIcon(newValue, this);
        }
#endif

        //update tooltip settings - needed to make sure a string is set, even
        //if the ToolTipText property is not set. Otherwise, the event that
        //triggers tooltip display is never fired.
        WriteToolTipSettings();
    }

    #endregion

    /// <summary>
    /// Indicates whether custom tooltips are supported, which depends
    /// on the OS. Windows Vista or higher is required in order to
    /// support this feature.
    /// </summary>
    public bool SupportsCustomToolTips => TrayIcon.SupportsCustomToolTips;

    /// <summary>
    /// Checks whether a non-tooltip popup is currently opened.
    /// </summary>
    private bool IsPopupOpen
    {
        get
        {
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

#pragma warning disable CA1508 // Avoid dead conditional code
            return (popup != null && popup.IsOpen) ||
                   (menu != null && menu.IsOpen) ||
                   (balloon != null && balloon.IsOpen);
#pragma warning restore CA1508 // Avoid dead conditional code
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a <see cref="ToolTip"/> control that either
    /// wraps the currently set <see cref="TrayToolTip"/>
    /// control or the <see cref="ToolTipText"/> string.<br/>
    /// If <see cref="TrayToolTip"/> itself is already
    /// a <see cref="ToolTip"/> instance, it will be used directly.
    /// </summary>
    /// <remarks>We use a <see cref="ToolTip"/> rather than
    /// <see cref="Popup"/> because there was no way to prevent a
    /// popup from causing cyclic open/close commands if it was
    /// placed under the mouse. ToolTip internally uses a Popup of
    /// its own, but takes advance of Popup's internal <see cref="UIElement.IsHitTestVisible"/>
    /// property which prevents this issue.</remarks>
    private void CreateCustomToolTip()
    {
        // check if the item itself is a tooltip
        var tt = TrayToolTip as ToolTip;

        if (tt == null && TrayToolTip != null)
        {
            // create an invisible wrapper tooltip that hosts the UIElement
            tt = new ToolTip
            {
                Placement = PlacementMode.Mouse,
                // do *not* set the placement target, as it causes the popup to become hidden if the
                // TaskbarIcon's parent is hidden, too. At runtime, the parent can be resolved through
                // the ParentTaskbarIcon attached dependency property:
                // PlacementTarget = this;

                // make sure the tooltip is invisible
#if HAS_WPF
                HasDropShadow = false,
                Background = System.Windows.Media.Brushes.Transparent,
                // setting the
                StaysOpen = true,
#endif
                BorderThickness = new Thickness(0),
                Content = TrayToolTip,
            };
        }
        else if (tt == null && !string.IsNullOrEmpty(ToolTipText))
        {
            // create a simple tooltip for the ToolTipText string
            tt = new ToolTip
            {
                Content = ToolTipText
            };
        }

        // the tooltip explicitly gets the DataContext of this instance.
        // If there is no DataContext, the TaskbarIcon assigns itself
        if (tt != null)
        {
            UpdateDataContext(tt, null, DataContext);
        }

        // store a reference to the used tooltip
        TrayToolTipResolved = tt;
    }

    /// <summary>
    /// Sets tooltip settings for the class depending on defined
    /// dependency properties and OS support.
    /// </summary>
    private void WriteToolTipSettings()
    {
        var text = ToolTipText;
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

        TrayIcon.UpdateToolTip(text ?? string.Empty);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Displays a custom tooltip, if available. This method is only
    /// invoked for Windows Vista and above.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="visible">Whether to show or hide the tooltip.</param>
    private void OnToolTipChange(object? sender, bool visible)
    {
        // if we don't have a tooltip, there's nothing to do here...
        if (TrayToolTipResolved == null)
        {
            return;
        }

        if (visible)
        {
            if (IsPopupOpen)
            {
                // ignore if we are already displaying something down there
                return;
            }

            var args = OnPreviewTrayToolTipOpen();
#if HAS_WPF
            if (args.Handled)
            {
                return;
            }
#endif

            TrayToolTipResolved.IsOpen = true;

#if HAS_WPF
            // raise attached event first
            if (TrayToolTip != null)
            {
                TrayToolTip.RaiseEvent(new RoutedEventArgs(ToolTipOpenedEvent));
            }
#endif

            // bubble routed event
            OnTrayToolTipOpen();
        }
        else
        {
            var args = OnPreviewTrayToolTipClose();
#if HAS_WPF
            if (args.Handled)
            {
                return;
            }

            // raise attached event first
            if (TrayToolTip != null)
            {
                TrayToolTip.RaiseEvent(new RoutedEventArgs(ToolTipCloseEvent));
            }
#endif

            TrayToolTipResolved.IsOpen = false;

            // bubble event
            OnTrayToolTipClose();
        }
    }

    #endregion
}
