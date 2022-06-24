namespace H.NotifyIcon;

[DependencyProperty<PopupActivationMode>("PopupActivation", DefaultValue = PopupActivationMode.LeftClick,
    Description = "Defines what mouse events display the TaskbarIconPopup.", Category = CategoryName, CLSCompliant = false)]
[DependencyProperty<UIElement>("TrayPopup",
    Description = "Displayed as a Popup if the user clicks on the taskbar icon.", Category = CategoryName)]
[DependencyProperty<Popup>("TrayPopupResolved", IsReadOnly = true,
    Description = "Returns a Popup which is either the TrayPopup control itself or a Popup control that contains the TrayPopup.", Category = CategoryName)]
[DependencyProperty<PlacementMode>("PopupPlacement",
#if HAS_WPF
    DefaultValue = PlacementMode.AbsolutePoint,
#else
    DefaultValue = PlacementMode.Mouse,
#endif
    Description = "Defines popup placement mode of TaskbarIconPopup.", Category = CategoryName)]
[DependencyProperty<Thickness>("PopupOffset",
    Description = "Defines popup offset of TaskbarIconPopup.", Category = CategoryName)]
[RoutedEvent("TrayPopupOpen", RoutedEventStrategy.Bubble,
    Description = "Bubbled event that occurs when the custom popup is being opened.", Category = CategoryName)]
[RoutedEvent("PreviewTrayPopupOpen", RoutedEventStrategy.Tunnel,
    Description = "Tunneled event that occurs when the custom popup is being opened.", Category = CategoryName)]
[RoutedEvent("PopupOpened", RoutedEventStrategy.Bubble, IsAttached = true, Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Properties

    #region TrayPopup

    partial void OnTrayPopupChanged(UIElement? oldValue, UIElement? newValue)
    {
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

        //create a pop
        CreatePopup();
    }

    #endregion

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
    private void CreatePopup()
    {
        // check if the item itself is a popup
        var popup = TrayPopup as Popup;

        if (popup == null && TrayPopup != null)
        {
            // create an invisible popup that hosts the UIElement
            popup = new Popup
            {
#if HAS_WPF
                AllowsTransparency = true,
                // don't animate by default - developers can use attached events or override
                PopupAnimation = PopupAnimation.None,
                // the CreateRootPopup method outputs binding errors in the debug window because
                // it tries to bind to "Popup-specific" properties in case they are provided by the child.
                // We don't need that so just assign the control as the child.
                // do *not* set the placement target, as it causes the popup to become hidden if the
                // TaskbarIcon's parent is hidden, too. At runtime, the parent can be resolved through
                // the ParentTaskbarIcon attached dependency property:
                // PlacementTarget = this;

                Placement = PopupPlacement,
                StaysOpen = false,
#endif
                Child = TrayPopup,
            };
        }

        // the popup explicitly gets the DataContext of this instance.
        // If there is no DataContext, the TaskbarIcon assigns itself
        if (popup != null)
        {
            UpdateDataContext(popup, null, DataContext);
        }

        // store a reference to the used tooltip
        TrayPopupResolved = popup;
    }

    /// <summary>
    /// Hide the <see cref="TrayPopup"/> control if it was visible.
    /// </summary>
    public void CloseTrayPopup()
    {
        if (IsDisposed)
        {
            return;
        }

#if HAS_WPF
        var args = RaisePreviewTrayPopupOpenEvent();
        if (args.Handled)
        {
            return;
        }
#endif

        if (TrayPopup == null)
        {
            return;
        }

        if (TrayPopupResolved != null)
        {
            TrayPopupResolved.IsOpen = false;
        }
    }

    /// <summary>
    /// Displays the <see cref="TrayPopup"/> control if it was set.
    /// </summary>
    public void ShowTrayPopup(System.Drawing.Point cursorPosition)
    {
        if (IsDisposed)
        {
            return;
        }

#if HAS_WPF
        // raise preview event no matter whether popup is currently set
        // or not (enables client to set it on demand)
        var args = RaisePreviewTrayPopupOpenEvent();
        if (args.Handled)
        {
            return;
        }
#endif

        if (TrayPopup == null)
        {
            return;
        }
        if (TrayPopupResolved == null)
        {
            return;
        }

        PlacePopup(cursorPosition);

        // open popup
        TrayPopupResolved.IsOpen = true;

#if HAS_WPF
        var handle = (nint)0;
        if (TrayPopupResolved.Child != null)
        {
            // try to get a handle on the popup itself (via its child)
            if (PresentationSource.FromVisual(TrayPopupResolved.Child) is HwndSource source)
            {
                handle = source.Handle;
            }
        }

        // if we don't have a handle for the popup, fall back to the message sink
        if (handle == 0)
        {
            handle = TrayIcon.WindowHandle;
        }

        // activate either popup or message sink to track deactivation.
        // otherwise, the popup does not close if the user clicks somewhere else
        WindowUtilities.SetForegroundWindow(handle);

        // raise attached event - item should never be null unless developers
        // changed the CustomPopup directly...
        TrayPopup?.RaiseEvent(new RoutedEventArgs(PopupOpenedEvent));

        // bubble routed event
        RaiseTrayPopupOpenEvent();
#endif
    }

    private void PlacePopup(System.Drawing.Point cursorPosition)
    {
        if (TrayPopupResolved == null)
        {
            return;
        }

#if HAS_WPF
        TrayPopupResolved.Placement = PopupPlacement;
#endif
        if (PopupPlacement == PlacementMode.Bottom)
        {
            // place popup above system taskbar
            var point = TrayInfo.GetTrayLocation(0);
#if HAS_WPF
            TrayPopupResolved.Placement = PlacementMode.AbsolutePoint;
#endif
            TrayPopupResolved.HorizontalOffset = point.X;
            TrayPopupResolved.VerticalOffset = point.Y;
        }
#if HAS_WPF
        else if (PopupPlacement == PlacementMode.AbsolutePoint)
#else
        else if (PopupPlacement == PlacementMode.Mouse)
#endif
        {
            // place popup near mouse cursor
            TrayPopupResolved.HorizontalOffset = cursorPosition.X;
            TrayPopupResolved.VerticalOffset = cursorPosition.Y;
        }

        TrayPopupResolved.HorizontalOffset += PopupOffset.Left;
        TrayPopupResolved.VerticalOffset += PopupOffset.Top;
    }

    #endregion
}
