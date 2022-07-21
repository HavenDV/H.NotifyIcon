namespace H.NotifyIcon;

[DependencyProperty<PopupActivationMode>("MenuActivation", DefaultValue = PopupActivationMode.RightClick,
    Description = "Defines what mouse events display the context menu.", Category = CategoryName, CLSCompliant = false)]
#if !HAS_WPF
[DependencyProperty<ContextMenuMode>("ContextMenuMode", DefaultValue = ContextMenuMode.PopupMenu,
    Description = "Defines the context menu mode.", Category = CategoryName)]
#endif
[RoutedEvent("TrayContextMenuOpen", RoutedEventStrategy.Bubble,
    Description = "Bubbled event that occurs when the context menu of the taskbar icon is being displayed.", Category = CategoryName)]
[RoutedEvent("PreviewTrayContextMenuOpen", RoutedEventStrategy.Tunnel,
    Description = "Tunneled event that occurs when the context menu of the taskbar icon is being displayed.", Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Properties

#if !HAS_WPF

    #region ContextMenuMode

#pragma warning disable CA1822 // Mark members as static
    partial void OnContextMenuModeChanged(ContextMenuMode oldValue, ContextMenuMode newValue)
#pragma warning restore CA1822 // Mark members as static
    {
        if (newValue is ContextMenuMode.SecondWindow)
        {
#if !HAS_UNO
            PrepareContextMenuWindow();
#endif
        }
    }

    #endregion

#endif

    #endregion

    #region Methods

#if HAS_WINUI && !HAS_UNO
    private bool IsContextMenuVisible { get; set; }
    private Window? ContextMenuWindow { get; set; }
    private nint? ContextMenuWindowHandle { get; set; }
    private AppWindow? ContextMenuAppWindow { get; set; }
    private MenuFlyout? ContextMenuFlyout { get; set; }
    private bool FirstMeasure { get; set; } = true;
    private bool PointerActionInContextMenuWindow { get; set; }
    private bool SecondMeasureAfterPointerAction { get; set; }
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

        // raise preview event no matter whether context menu is currently set
        // or not (enables client to set it on demand)
        var args = OnPreviewTrayContextMenuOpen();
#if HAS_WPF
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
        if (PresentationSource.FromVisual(ContextMenu) is HwndSource source)
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
                                    item.Click += (_, _) =>
                                    {
                                        flyoutItem.Command?.TryExecute(flyoutItem.CommandParameter);
                                    };
                                    menu.Add(item);
                                    break;
                                }
                            case MenuFlyoutSeparator:
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
                    if (ContextMenuWindowHandle == null ||
                        ContextMenuFlyout == null)
                    {
                        break;
                    }

                    var size = MeasureFlyout(
                        flyout: ContextMenuFlyout,
                        availableSize: new Size(10000.0, 10000.0),
                        firstMeasure: FirstMeasure,
                        pointerActionInContextMenuWindow: PointerActionInContextMenuWindow,
                        secondMeasureAfterPointerAction: SecondMeasureAfterPointerAction);
                    FirstMeasure = false;
                    if (PointerActionInContextMenuWindow)
                    {
                        SecondMeasureAfterPointerAction = true;
                    }
                    var rectangle = CursorUtilities.CalculatePopupWindowPosition(cursorPosition, size.ToSystemDrawingSize());

                    ContextMenuAppWindow?.MoveAndResize(rectangle.ToRectInt32());
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

        // bubble event
        OnTrayContextMenuOpen();
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
            flyoutItemBase.PointerMoved += (_, _) => PointerActionInContextMenuWindow = true;
            flyoutItemBase.PointerPressed += (_, _) => PointerActionInContextMenuWindow = true;
        }
        
        frame.Loaded += (_, _) =>
        {
            flyout.ShowAt(window.Content, new FlyoutShowOptions
            {
                ShowMode = FlyoutShowMode.Transient,
            });
            flyout.Hide();
        };
        window.Activated += (sender, args) =>
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
    
        this.ActualThemeChanged += (_, _) => {
            ((FrameworkElement)window.Content).RequestedTheme = this.ActualTheme;
        };

        ContextMenuWindow = window;
        ContextMenuWindowHandle = handle;
        ContextMenuAppWindow = appWindow;
        ContextMenuFlyout = flyout;
    }

    private static Size MeasureFlyout(
        MenuFlyout flyout,
        Size availableSize,
        bool firstMeasure,
        bool pointerActionInContextMenuWindow,
        bool secondMeasureAfterPointerAction)
    {
        var width = 0.0;
        var height = 4.0; // top and bottom margin

        foreach (var item in flyout.Items)
        {
            item.Measure(availableSize);

            var additionalHeight = item is MenuFlyoutItem && firstMeasure
                ? 2.0
                : 0.0;
            // https://github.com/microsoft/microsoft-ui-xaml/issues/7374
            additionalHeight += item is MenuFlyoutItem &&
                (pointerActionInContextMenuWindow && !secondMeasureAfterPointerAction)
                ? -8.0
                : 0.0;
            width = Math.Max(width, item.DesiredSize.Width);
            height += item.DesiredSize.Height + additionalHeight;
        }

        var scale = flyout.XamlRoot.RasterizationScale;
        return new Size(
            width: Math.Round(scale * width
                + (firstMeasure ? 128.0 : 0.0) // ??
                + 4.0),  // borders
            height: Math.Round(scale * height + 4.0)); // borders
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
