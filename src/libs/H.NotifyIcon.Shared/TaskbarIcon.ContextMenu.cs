namespace H.NotifyIcon;

/// <inheritdoc/>
public partial class TaskbarIcon
{
    #region Properties

    #region MenuActivation

    /// <summary>Identifies the <see cref="MenuActivation"/> dependency property.</summary>
    public static readonly DependencyProperty MenuActivationProperty =
        DependencyProperty.Register(
            nameof(MenuActivation),
            typeof(PopupActivationMode),
            typeof(TaskbarIcon),
            new PropertyMetadata(PopupActivationMode.RightClick));

    /// <summary>
    /// A property wrapper for the <see cref="MenuActivationProperty"/>
    /// dependency property:<br/>
    /// Defines what mouse events display the context menu.
    /// Defaults to <see cref="PopupActivationMode.RightClick"/>.
    /// </summary>
    [Category(CategoryName)]
    [Description("Defines what mouse events display the context menu.")]
#if !HAS_WPF
    [CLSCompliant(false)]
#endif
    public PopupActivationMode MenuActivation
    {
        get { return (PopupActivationMode)GetValue(MenuActivationProperty); }
        set { SetValue(MenuActivationProperty, value); }
    }

    #endregion

    #endregion

    #region Events

#if HAS_WPF

    /// <summary>Identifies the <see cref="TrayContextMenuOpen"/> routed event.</summary>
    public static readonly RoutedEvent TrayContextMenuOpenEvent =
        EventManager.RegisterRoutedEvent(
            nameof(TrayContextMenuOpen),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TaskbarIcon));

    /// <summary>
    /// Bubbled event that occurs when the context menu of the taskbar icon is being displayed.
    /// </summary>
    public event RoutedEventHandler TrayContextMenuOpen
    {
        add { AddHandler(TrayContextMenuOpenEvent, value); }
        remove { RemoveHandler(TrayContextMenuOpenEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the TrayContextMenuOpen event.
    /// </summary>
    protected RoutedEventArgs RaiseTrayContextMenuOpenEvent()
    {
        return this.RaiseRoutedEvent(new RoutedEventArgs(TrayContextMenuOpenEvent));
    }

    /// <summary>Identifies the <see cref="PreviewTrayContextMenuOpen"/> routed event.</summary>
    public static readonly RoutedEvent PreviewTrayContextMenuOpenEvent =
        EventManager.RegisterRoutedEvent(
            nameof(PreviewTrayContextMenuOpen),
            RoutingStrategy.Tunnel,
            typeof(RoutedEventHandler),
            typeof(TaskbarIcon));

    /// <summary>
    /// Tunneled event that occurs when the context menu of the taskbar icon is being displayed.
    /// </summary>
    public event RoutedEventHandler PreviewTrayContextMenuOpen
    {
        add { AddHandler(PreviewTrayContextMenuOpenEvent, value); }
        remove { RemoveHandler(PreviewTrayContextMenuOpenEvent, value); }
    }

    /// <summary>
    /// A helper method to raise the PreviewTrayContextMenuOpen event.
    /// </summary>
    protected RoutedEventArgs RaisePreviewTrayContextMenuOpenEvent()
    {
        return this.RaiseRoutedEvent(new RoutedEventArgs(PreviewTrayContextMenuOpenEvent));
    }

#endif

    #endregion

    #region Methods

    /// <summary>
    /// Displays the ContextMenu/ContextFlyout if it was set.
    /// </summary>
    private void ShowContextMenu(System.Drawing.Point cursorPosition)
    {
        if (IsDisposed)
        {
            return;
        }

#if HAS_WPF
        // raise preview event no matter whether context menu is currently set
        // or not (enables client to set it on demand)
        var args = RaisePreviewTrayContextMenuOpenEvent();
        if (args.Handled)
        {
            return;
        }

        if (ContextMenu == null)
        {
            return;
        }

        // use absolute positioning. We need to set the coordinates, or a delayed opening
        // (e.g. when left-clicked) opens the context menu at the wrong place if the mouse
        // is moved!
        ContextMenu.Placement = PlacementMode.AbsolutePoint;
        ContextMenu.HorizontalOffset = cursorPosition.X;
        ContextMenu.VerticalOffset = cursorPosition.Y;
        ContextMenu.IsOpen = true;

        var handle = IntPtr.Zero;

        // try to get a handle on the context itself
        var source = (HwndSource)PresentationSource.FromVisual(ContextMenu);
        if (source != null)
        {
            handle = source.Handle;
        }

        // if we don't have a handle for the popup, fall back to the message sink
        if (handle == IntPtr.Zero)
        {
            handle = MessageWindow.Handle;
        }

        // activate the context menu or the message window to track deactivation - otherwise, the context menu
        // does not close if the user clicks somewhere else. With the message window
        // fallback, the context menu can't receive keyboard events - should not happen though
        WindowUtilities.SetForegroundWindow(handle);

        // bubble event
        RaiseTrayContextMenuOpenEvent();
#else
        if (ContextFlyout == null)
        {
            return;
        }

        // use absolute positioning. We need to set the coordinates, or a delayed opening
        // (e.g. when left-clicked) opens the context menu at the wrong place if the mouse
        // is moved!
        //ContextFlyout.Placement = FlyoutPlacementMode.Auto;
        //var window = new Window()
        //{
        //};
        //window.Activated += (_, args) =>
        //{
        //    if (args.WindowActivationState == WindowActivationState.Deactivated)
        //    {
        //        window.Close();
        //    }
        //};
        //var handle = WindowNative.GetWindowHandle(window);
        //PInvoke.SetLayeredWindowAttributes(new HWND(handle), 0U, 255, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_ALPHA);

        //var id = Win32Interop.GetWindowIdFromWindow(handle);
        //var appWindow = AppWindow.GetFromWindowId(id);
        //appWindow.MoveAndResize(new RectInt32(cursorPosition.X - 100, cursorPosition.Y - 100, 100, 100));

        //var presenter = appWindow.Presenter as OverlappedPresenter;
        //presenter.IsMaximizable = false;
        //presenter.IsMinimizable = false;
        //presenter.IsResizable = false;
        //presenter.IsAlwaysOnTop = true;
        //presenter.SetBorderAndTitleBar(false, false);

        //var flyout = new MenuFlyout
        //{
        //    Items =
        //    {
        //        new MenuFlyoutItem
        //        {
        //            Text = "Show/Hide Window",
        //        },
        //        new MenuFlyoutSeparator(),
        //        new MenuFlyoutItem
        //        {
        //            Text = "Exit",
        //        },
        //    },
        //};
        //var grid = new Grid
        //{
        //    ContextFlyout = flyout,
        //};
        //window.Content = new Frame
        //{
        //    Content = new Page
        //    {
        //        Content = grid,
        //    },
        //};
        //window.Activate();

        //flyout.Hide();
        //flyout.ShowAt(grid, new FlyoutShowOptions
        //{
        //    Placement = FlyoutPlacementMode.Auto,
        //    Position = new Windows.Foundation.Point(0, 0),
        //    ShowMode = FlyoutShowMode.Auto,
        //});

        using var menu = new PopupMenu();
        foreach(var flyoutItemBase in ((MenuFlyout)ContextFlyout).Items)
        {
            switch (flyoutItemBase)
            {
                case MenuFlyoutItem flyoutItem:
                    {
                        var item = new PopupMenuItem()
                        {   
                            Text = flyoutItem.Text,
                        };
                        item.Click += (_, args) =>
                        {
                            flyoutItem.Command?.TryExecute(flyoutItem.CommandParameter);
                        };
                        menu.Add(item);
                        break;
                    }
                case MenuFlyoutSeparator separator:
                    {
                        menu.Add(new PopupMenuSeparator());
                        break;
                    }
            }
        }

        var handle = MessageWindow.Handle;

        WindowUtilities.SetForegroundWindow(handle);
        menu.Show(
            ownerHandle: handle,
            x: cursorPosition.X,
            y: cursorPosition.Y);
        
        //ContextFlyout.Hide();
        //ContextFlyout.ShowAt(this, new FlyoutShowOptions
        //{
        //    Placement = FlyoutPlacementMode.Auto,
        //    Position = new Windows.Foundation.Point(cursorPosition.X, cursorPosition.Y),
        //    ShowMode = FlyoutShowMode.Auto,
        //});
#endif
    }

    #endregion

    #region Event Handlers

#if HAS_WPF

    /// <summary>
    /// A static callback listener which is being invoked if the
    /// <see cref="FrameworkElement.ContextMenuProperty"/> dependency property has
    /// been changed. Invokes the <see cref="OnContextMenuPropertyChanged"/>
    /// instance method of the changed instance.
    /// </summary>
    /// <param name="d">The currently processed owner of the property.</param>
    /// <param name="e">Provides information about the updated property.</param>
    private static void ContextMenuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (TaskbarIcon)d;
        owner.OnContextMenuPropertyChanged(e);
    }


    /// <summary>
    /// Releases the old and updates the new <see cref="ContextMenu"/> property
    /// in order to reflect both the NotifyIcon's <see cref="FrameworkElement.DataContext"/>
    /// property and have the <see cref="ParentTaskbarIconProperty"/> assigned.
    /// </summary>
    /// <param name="e">Provides information about the updated property.</param>
    private void OnContextMenuPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        var contextMenu = (ContextMenu)e.NewValue;
        if (e.OldValue != null)
        {
            //remove the taskbar icon reference from the previously used element
            SetParentTaskbarIcon((DependencyObject)e.OldValue, null);
        }

        if (e.NewValue != null)
        {
            //set this taskbar icon as a reference to the new tooltip element
            SetParentTaskbarIcon((DependencyObject)e.NewValue, this);
        }

        UpdateDataContext(contextMenu, null, DataContext);
    }

#else

    private void UpdateContextFlyoutDataContext(
        FlyoutBase flyout,
        object? oldValue,
        object? newValue)
    {
        void UpdateMenuFlyoutDataContext(MenuFlyoutItemBase item)
        {
            UpdateDataContext(item, oldValue, newValue);

            if (item is MenuFlyoutSubItem subItem)
            {
                foreach (var innerItem in subItem.Items)
                {
                    UpdateMenuFlyoutDataContext(innerItem);
                }
            }
        }

        if (flyout is MenuFlyout menuFlyout)
        {
            foreach (var item in menuFlyout.Items)
            {
                UpdateMenuFlyoutDataContext(item);
            }
        }
    }

#endif

    #endregion
}
