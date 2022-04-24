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

#if !HAS_WPF

    #region ContextMenuMode

    /// <summary>Identifies the <see cref="ContextMenuMode"/> dependency property.</summary>
    public static readonly DependencyProperty ContextMenuModeProperty =
        DependencyProperty.Register(
            nameof(ContextMenuMode),
            typeof(ContextMenuMode),
            typeof(TaskbarIcon),
            new PropertyMetadata(
                ContextMenuMode.PopupMenu,
                (d, e) => ((TaskbarIcon)d).OnContextMenuModeChanged(d, e)));

    /// <summary>
    /// A property wrapper for the <see cref="ContextMenuModeProperty"/>
    /// dependency property:<br/>
    /// Defines the context menu mode.
    /// Defaults to <see cref="ContextMenuMode.PopupMenu"/>.
    /// </summary>
    [Category(CategoryName)]
    [Description("Defines the context menu mode.")]
    public ContextMenuMode ContextMenuMode
    {
        get { return (ContextMenuMode)GetValue(ContextMenuModeProperty); }
        set { SetValue(ContextMenuModeProperty, value); }
    }

#pragma warning disable CA1822 // Mark members as static
    private void OnContextMenuModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
#pragma warning restore CA1822 // Mark members as static
    {
        if (args.NewValue is ContextMenuMode.SecondWindow)
        {
#if !HAS_UNO
            PrepareContextMenuWindow();
#endif
        }
    }

    #endregion

#endif

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

#if HAS_WINUI && !HAS_UNO
    private bool IsContextMenuVisible { get; set; }
    private Window? ContextMenuWindow { get; set; }
    private nint? ContextMenuWindowHandle { get; set; }
    private AppWindow? ContextMenuAppWindow { get; set; }
#endif

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

        var handle = (nint)0;

        // try to get a handle on the context itself
        var source = (HwndSource)PresentationSource.FromVisual(ContextMenu);
        if (source != null)
        {
            handle = source.Handle;
        }

        // if we don't have a handle for the popup, fall back to the message sink
        if (handle == 0)
        {
            handle = TrayIcon.WindowHandle;
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

        switch (ContextMenuMode)
        {
            case ContextMenuMode.PopupMenu:
                {
                    using var menu = new PopupMenu();
                    foreach (var flyoutItemBase in ((MenuFlyout)ContextFlyout).Items)
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

                    var handle = TrayIcon.WindowHandle;

                    WindowUtilities.SetForegroundWindow(handle);
                    menu.Show(
                        ownerHandle: handle,
                        x: cursorPosition.X,
                        y: cursorPosition.Y);
                }
                break;

#if !HAS_UNO
            case ContextMenuMode.SecondWindow:
                {
                    if (ContextMenuWindowHandle == null)
                    {
                        break;
                    }

                    var size = MeasureFlyout(ContextFlyout, availableSize: new Size(10000.0, 10000.0));
                    ContextMenuAppWindow?.MoveAndResize(new RectInt32(
                        _X: cursorPosition.X - (int)size.Width,
                        _Y: cursorPosition.Y - (int)size.Height,
                        _Width: (int)size.Width,
                        _Height: (int)size.Height));
                    WindowUtilities.ShowWindow(ContextMenuWindowHandle.Value);
                    WindowUtilities.SetForegroundWindow(ContextMenuWindowHandle.Value);
                }
                break;
#endif

            case ContextMenuMode.ActiveWindow:
                {
                    ContextFlyout.Hide();
                    ContextFlyout.ShowAt(this, new FlyoutShowOptions
                    {
                        Placement = FlyoutPlacementMode.Auto,
                        Position = new Point(cursorPosition.X, cursorPosition.Y),
                        ShowMode = FlyoutShowMode.Auto,
                    });
                }
                break;

            default:
                throw new NotImplementedException($"ContextMenuMode: {ContextMenuMode} is not implemented.");
        }
#endif
    }

#if HAS_WINUI && !HAS_UNO

    private void PrepareContextMenuWindow()
    {
        if (ContextFlyout == null ||
            ContextMenuMode != ContextMenuMode.SecondWindow)
        {
            return;
        }

        var frame = new Frame
        {
            Background = new SolidColorBrush(Colors.Transparent),
        };
        var window = new Window()
        {
            Content = frame,
        };
        var handle = WindowNative.GetWindowHandle(window);
        WindowUtilities.MakeTransparent(handle);

        var id = Win32Interop.GetWindowIdFromWindow(handle);
        var appWindow = AppWindow.GetFromWindowId(id);
        appWindow.IsShownInSwitchers = false;

        var presenter = (OverlappedPresenter)appWindow.Presenter;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        presenter.IsResizable = false;
        presenter.IsAlwaysOnTop = true;
        presenter.SetBorderAndTitleBar(false, false);

        var flyout = new MenuFlyout
        {
            AreOpenCloseAnimationsEnabled = ContextFlyout.AreOpenCloseAnimationsEnabled,
            Placement = FlyoutPlacementMode.Full,
        };
        flyout.Closed += async (_, _) =>
        {
            if (!flyout.AreOpenCloseAnimationsEnabled ||
                !IsContextMenuVisible)
            {
                _ = WindowUtilities.HideWindow(handle);
                return;
            }

            await Task.Delay(1).ConfigureAwait(true);
            flyout.ShowAt(window.Content, new FlyoutShowOptions
            {
                ShowMode = FlyoutShowMode.Transient,
            });
        };
        foreach (var flyoutItemBase in ((MenuFlyout)ContextFlyout).Items)
        {
            flyout.Items.Add(flyoutItemBase);
            flyoutItemBase.Tapped += (_, _) =>
            {
                IsContextMenuVisible = false;
                flyout.Hide();
                _ = WindowUtilities.HideWindow(handle);
            };
        }

        frame.Loaded += (_, _) =>
        {
            flyout.ShowAt(window.Content, new FlyoutShowOptions
            {
                ShowMode = FlyoutShowMode.Transient,
            });
            flyout.Hide();
        };
        window.Activated += (_, args) =>
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                IsContextMenuVisible = false;
                flyout.Hide();
                _ = WindowUtilities.HideWindow(handle);
                return;
            }

            if (ContextMenuWindow == null)
            {
                return;
            }

            IsContextMenuVisible = true;
            flyout.ShowAt(window.Content, new FlyoutShowOptions
            {
                ShowMode = FlyoutShowMode.Transient,
            });
        };

        ContextMenuWindow = window;
        ContextMenuWindowHandle = handle;
        ContextMenuAppWindow = appWindow;
    }

    private static Size MeasureFlyout(FlyoutBase flyout, Size availableSize)
    {
        var width = 0.0;
        var height = 16.0;
        flyout.Placement = FlyoutPlacementMode.Auto;
        flyout.ShowMode = FlyoutShowMode.Transient;
        foreach (var flyoutItemBase in ((MenuFlyout)flyout).Items)
        {
            flyoutItemBase.Measure(availableSize);

            width = Math.Max(
                width,
                flyoutItemBase.DesiredSize.Width +
                flyoutItemBase.Padding.Left +
                flyoutItemBase.Padding.Right +
                16.0);
            height +=
                flyoutItemBase.DesiredSize.Height +
                flyoutItemBase.Padding.Top +
                flyoutItemBase.Padding.Bottom;
        }

        return new Size(
            width: 2 * Math.Round(width),
            height: Math.Round(height));
    }

#endif

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
