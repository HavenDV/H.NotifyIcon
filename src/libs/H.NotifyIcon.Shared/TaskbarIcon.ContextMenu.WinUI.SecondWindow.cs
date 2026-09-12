using System.Diagnostics.CodeAnalysis;
using H.NotifyIcon.Interop;
using Microsoft.UI.Xaml.Data;

namespace H.NotifyIcon;

[Event("SecondWindowContextMenuOpened",
    Description = "Occurs when the second-window context menu is opened.")]
public partial class TaskbarIcon
{
#if !HAS_MAUI

    #region Properties

    private bool IsContextMenuVisible { get; set; }
    private bool IsSecondWindowContextMenuOpenEventRaised { get; set; }
    private bool IsSecondWindowContextMenuLoaded { get; set; }
    private Window? ContextMenuWindow { get; set; }
    private Frame? ContextMenuWindowRoot { get; set; }
    private nint? ContextMenuWindowHandle { get; set; }
    private AppWindow? ContextMenuAppWindow { get; set; }
    private MenuFlyout? ContextMenuFlyout { get; set; }

#pragma warning disable CA1822 // Mark members as static
    partial void OnContextMenuModeChanged(ContextMenuMode oldValue, ContextMenuMode newValue)
#pragma warning restore CA1822 // Mark members as static
    {
        if (oldValue is ContextMenuMode.SecondWindow &&
            newValue is not ContextMenuMode.SecondWindow)
        {
            DisposeSecondWindowContextMenu();
        }

        if (newValue is ContextMenuMode.SecondWindow)
        {
            PrepareContextMenuWindow();
        }
    }

    partial void OnContextMenuThemeModeChanged(PopupMenuThemeMode oldValue, PopupMenuThemeMode newValue)
    {
        ApplySecondWindowContextMenuTheme(ContextMenuWindowRoot);
    }

    #endregion
    
    #region Methods

    private void ShowContextMenuInSecondWindowMode(System.Drawing.Point cursorPosition)
    {
        if (ContextMenuWindowHandle == null ||
            ContextMenuFlyout == null ||
            ContextMenuWindow?.Content == null)
        {
            return;
        }

        SynchronizeSecondWindowContextMenuItems();
        EnsureSecondWindowContextMenuLoaded();

        var size = MeasureFlyout(ContextMenuFlyout, new Size(10000.0, 10000.0));
        var rasterizationScale = ContextMenuWindow.Content.XamlRoot?.RasterizationScale ?? 1.0;
        var excludeRect = CreateTrayCursorExcludeRect(cursorPosition, rasterizationScale);
        var rectangle = CursorUtilities.CalculatePopupWindowPosition(
            cursorPosition.X,
            cursorPosition.Y,
            (int)size.Width,
            (int)size.Height,
            excludeRect);

        // Choose placement so WinUI's MenuPopupThemeTransition runs the open
        // animation from the correct direction: only Top placement produces
        // the bottom-up slide. Offset the rectangle by its own height to
        // compensate for Top/Bottom placing the flyout adjacent to the anchor
        // rather than overlapping it.
        if (rectangle.Y + rectangle.Height <= cursorPosition.Y)
        {
            // Menu extends above the cursor (bottom-taskbar case).
            ContextMenuFlyout.Placement = FlyoutPlacementMode.Top;
            rectangle.Y += rectangle.Height;
        }
        else if (rectangle.Y >= cursorPosition.Y)
        {
            // Menu extends below the cursor (top-taskbar case).
            ContextMenuFlyout.Placement = FlyoutPlacementMode.Bottom;
            rectangle.Y -= rectangle.Height;
        }
        else
        {
            // Horizontal taskbar — preserve existing Full placement.
            ContextMenuFlyout.Placement = FlyoutPlacementMode.Full;
        }

        IsContextMenuVisible = true;
        IsSecondWindowContextMenuOpenEventRaised = false;
        ContextMenuAppWindow?.MoveAndResize(rectangle.ToRectInt32());
        _ = WindowUtilities.ShowWindow(ContextMenuWindowHandle.Value);
        _ = WindowUtilities.SetForegroundWindow(ContextMenuWindowHandle.Value);
        ShowSecondWindowFlyout(ContextMenuFlyout, ContextMenuWindow.Content);
    }

    private void CloseSecondWindowContextMenu()
    {
        IsContextMenuVisible = false;
        IsSecondWindowContextMenuOpenEventRaised = false;
        ContextMenuFlyout?.Hide();

        if (ContextMenuWindowHandle is { } handle)
        {
            _ = WindowUtilities.HideWindow(handle);
        }
    }

    private void DisposeSecondWindowContextMenu()
    {
        CloseSecondWindowContextMenu();

        ActualThemeChanged -= OnSecondWindowActualThemeChanged;

        if (ContextMenuFlyout is { } flyout)
        {
            flyout.Opened -= OnSecondWindowFlyoutOpened;
            flyout.Closing -= OnSecondWindowFlyoutClosing;
            flyout.Closed -= OnSecondWindowFlyoutClosed;

            var items = flyout.Items.ToList();
            foreach (var item in items)
            {
                item.Tapped -= OnSecondWindowContextMenuItemTapped;
                _ = flyout.Items.Remove(item);

                if (ContextFlyout is MenuFlyout sourceFlyout &&
                    !sourceFlyout.Items.Contains(item))
                {
                    sourceFlyout.Items.Add(item);
                }
            }
        }

        if (ContextMenuWindowRoot is { } root)
        {
            root.Loaded -= OnSecondWindowRootLoaded;
            root.ClearValue(FlowDirectionProperty);
        }

        if (ContextMenuWindow is { } window)
        {
            window.Activated -= OnSecondWindowActivated;
            window.Content = null;
            window.Close();
        }

        IsSecondWindowContextMenuLoaded = false;
        ContextMenuFlyout = null;
        ContextMenuWindow = null;
        ContextMenuWindowRoot = null;
        ContextMenuWindowHandle = null;
        ContextMenuAppWindow = null;
    }

    private void EnsureSecondWindowContextMenuLoaded()
    {
        if (IsSecondWindowContextMenuLoaded ||
            ContextMenuWindow == null ||
            ContextMenuWindowHandle == null)
        {
            return;
        }

        ContextMenuWindow.Activate();
        _ = WindowUtilities.HideWindow(ContextMenuWindowHandle.Value);
    }

    private static System.Drawing.Rectangle CreateTrayCursorExcludeRect(
        System.Drawing.Point cursorPosition,
        double rasterizationScale)
    {
        // Native tray menus avoid overlapping the icon/taskbar affordance itself.
        // Give CalculatePopupWindowPosition a small tray-sized exclusion box so it
        // picks a position adjacent to the cursor instead of overlapping the taskbar.
        var width = Math.Max(1, (int)Math.Round(36 * rasterizationScale));
        var height = Math.Max(1, (int)Math.Round(36 * rasterizationScale));

        return new System.Drawing.Rectangle(
            x: cursorPosition.X - (width / 2),
            y: cursorPosition.Y - (height / 2),
            width: width,
            height: height);
    }

    private static void ShowSecondWindowFlyout(MenuFlyout flyout, UIElement target)
    {
        if (!flyout.IsOpen)
        {
            flyout.ShowAt(target, new FlyoutShowOptions
            {
                ShowMode = FlyoutShowMode.Transient,
            });
        }
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(OverlappedPresenter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(MenuFlyout))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(MenuFlyoutSeparator))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(MenuFlyoutSubItem))]
    private void PrepareContextMenuWindow()
    {
        if (ContextFlyout == null ||
            ContextMenuMode != ContextMenuMode.SecondWindow)
        {
            return;
        }

        DisposeSecondWindowContextMenu();
        IsSecondWindowContextMenuLoaded = false;

        var frame = new Frame
        {
            Background = new SolidColorBrush(Colors.Transparent),
        };

        var flowDirectionBinding = new Binding
        {
            Source = this,
            Path = new PropertyPath(nameof(FlowDirection)),
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(frame, FlowDirectionProperty, flowDirectionBinding);

        var window = new Window()
        {
            Content = frame,
        };

        ApplySecondWindowContextMenuTheme(frame);
        ActualThemeChanged += OnSecondWindowActualThemeChanged;

        var handle = WindowNative.GetWindowHandle(window);
        DesktopWindowsManagerMethods.SetRoundedCorners(handle);
        WindowUtilities.MakeTransparent(handle);

        if (TrayIcon.WindowHandle != 0)
        {
            // Keep the second-window popup in the tray icon's window family so it
            // participates in the same z-order/activation stack as the tray host.
            HwndUtilities.SetOwnerWindow(handle, TrayIcon.WindowHandle);
        }

#if !HAS_UNO
        var id = Win32Interop.GetWindowIdFromWindow(handle);
        var appWindow = AppWindow.GetFromWindowId(id);
        appWindow.IsShownInSwitchers = false;

        var presenter = (OverlappedPresenter)appWindow.Presenter;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        presenter.IsResizable = false;
        presenter.IsAlwaysOnTop = true;
        presenter.SetBorderAndTitleBar(false, false);
#endif

        var flyout = new MenuFlyout
        {
            AreOpenCloseAnimationsEnabled = ContextFlyout.AreOpenCloseAnimationsEnabled,
            Placement = FlyoutPlacementMode.Full,
        };
        flyout.Opened += OnSecondWindowFlyoutOpened;
        flyout.Closing += OnSecondWindowFlyoutClosing;
        flyout.Closed += OnSecondWindowFlyoutClosed;
        ContextMenuFlyout = flyout;
        SynchronizeSecondWindowContextMenuItems();

        frame.Loaded += OnSecondWindowRootLoaded;
        window.Activated += OnSecondWindowActivated;

        ContextMenuWindow = window;
        ContextMenuWindowRoot = frame;
        ContextMenuWindowHandle = handle;
#if !HAS_UNO
        ContextMenuAppWindow = appWindow;
#endif

        EnsureSecondWindowContextMenuLoaded();
    }

    private void OnSecondWindowActualThemeChanged(FrameworkElement sender, object args)
    {
        if (ContextMenuThemeMode == PopupMenuThemeMode.System)
        {
            ApplySecondWindowContextMenuTheme(ContextMenuWindowRoot);
        }
    }

    private void OnSecondWindowFlyoutOpened(object? sender, object args)
    {
        if (IsContextMenuVisible &&
            !IsSecondWindowContextMenuOpenEventRaised)
        {
            IsSecondWindowContextMenuOpenEventRaised = true;
            _ = OnSecondWindowContextMenuOpened();
        }
    }

    private void OnSecondWindowFlyoutClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
    {
        if (!CloseContextMenuOnItemClick &&
            IsContextMenuVisible)
        {
            args.Cancel = true;
        }
    }

    private void OnSecondWindowFlyoutClosed(object? sender, object args)
    {
        if (sender is not MenuFlyout flyout ||
            !flyout.AreOpenCloseAnimationsEnabled ||
            !IsContextMenuVisible)
        {
            if (ContextMenuWindowHandle is { } handle)
            {
                _ = WindowUtilities.HideWindow(handle);
            }

            return;
        }

        if (ContextMenuWindowRoot is { } root)
        {
            ShowSecondWindowFlyout(flyout, root);
        }
    }

    private void OnSecondWindowRootLoaded(object sender, RoutedEventArgs args)
    {
        if (sender is not Frame root ||
            ContextMenuFlyout is not { } flyout ||
            ContextMenuWindowHandle is not { } handle)
        {
            return;
        }

        IsSecondWindowContextMenuLoaded = true;

        // Set the window style to PopupWindow to make the title bar invisible
        HwndUtilities.SetWindowStyleAsPopupWindow(handle);

        flyout.ShowAt(root, new FlyoutShowOptions
        {
            ShowMode = FlyoutShowMode.Transient,
        });
        flyout.Hide();
        _ = WindowUtilities.HideWindow(handle);
    }

    private void OnSecondWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
        {
            CloseSecondWindowContextMenu();
            return;
        }

        if (!IsContextMenuVisible ||
            ContextMenuFlyout is not { } flyout ||
            ContextMenuWindowRoot is not { } root)
        {
            return;
        }

        ShowSecondWindowFlyout(flyout, root);
    }

    private void ApplySecondWindowContextMenuTheme(FrameworkElement? target)
    {
        if (target == null)
        {
            return;
        }

        target.RequestedTheme = ContextMenuThemeMode switch
        {
            PopupMenuThemeMode.System => ActualTheme,
            PopupMenuThemeMode.Light => ElementTheme.Light,
            PopupMenuThemeMode.Dark => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };
    }

    private void SynchronizeSecondWindowContextMenuItems()
    {
        if (ContextFlyout is not MenuFlyout sourceFlyout ||
            ContextMenuFlyout == null)
        {
            return;
        }

        var sourceItems = sourceFlyout.Items.ToList();
        foreach (var flyoutItemBase in sourceItems)
        {
            if (ContextMenuFlyout.Items.Contains(flyoutItemBase))
            {
                continue;
            }

            AddSecondWindowContextMenuItem(flyoutItemBase);
        }
    }

    private void AddSecondWindowContextMenuItem(MenuFlyoutItemBase flyoutItemBase)
    {
        PrepareSecondWindowContextMenuItem(flyoutItemBase);
        ContextMenuFlyout!.Items.Add(flyoutItemBase);

        // MenuFlyoutSubItem should not be clickable and should not close the context menu.
        if (flyoutItemBase is not MenuFlyoutSubItem)
        {
            flyoutItemBase.Tapped += OnSecondWindowContextMenuItemTapped;
        }
    }

    private void OnSecondWindowContextMenuItemTapped(object sender, TappedRoutedEventArgs args)
    {
        if (CloseContextMenuOnItemClick)
        {
            CloseSecondWindowContextMenu();
        }
    }

    private static void PrepareSecondWindowContextMenuItem(MenuFlyoutItemBase item)
    {
        // https://github.com/microsoft/microsoft-ui-xaml/issues/7374
        if (item is not MenuFlyoutSeparator)
        {
            item.Height = 32;
            item.Padding = new Thickness(11, 0, 11, 0);
        }
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(MenuFlyoutSeparator))]
    private static Size MeasureFlyout(MenuFlyout flyout, Size availableSize)
    {
        var width = 0.0;
        var height = 4.0; // top and bottom margin

        foreach (var item in flyout.Items)
        {
            PrepareSecondWindowContextMenuItem(item);
            item.Measure(availableSize);

            width = Math.Max(width, item.DesiredSize.Width);
            height += item.DesiredSize.Height;
        }

        var scale = flyout.XamlRoot?.RasterizationScale ?? 1.0;

        return new Size(
            width: Math.Round(scale * width + 4.0),  // borders
            height: Math.Round(scale * height + 4.0)); // borders
    }

    #endregion

#endif
}
