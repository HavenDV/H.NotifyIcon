using System.Diagnostics.CodeAnalysis;
using H.NotifyIcon.Interop;
using Microsoft.UI.Xaml.Data;

namespace H.NotifyIcon;

public partial class TaskbarIcon
{
#if !HAS_MAUI

    #region Properties

    private bool IsContextMenuVisible { get; set; }
    private Window? ContextMenuWindow { get; set; }
    private nint? ContextMenuWindowHandle { get; set; }
    private AppWindow? ContextMenuAppWindow { get; set; }
    private MenuFlyout? ContextMenuFlyout { get; set; }
    public event EventHandler? SecondWindowContextMenuOpened;

#pragma warning disable CA1822 // Mark members as static
    partial void OnContextMenuModeChanged(ContextMenuMode oldValue, ContextMenuMode newValue)
#pragma warning restore CA1822 // Mark members as static
    {
        if (newValue is ContextMenuMode.SecondWindow)
        {
            PrepareContextMenuWindow();
        }
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

        var size = MeasureFlyout(ContextMenuFlyout, new Size(10000.0, 10000.0));
        var excludeRect = CreateTrayCursorExcludeRect(cursorPosition);
        var rectangle = CursorUtilities.CalculatePopupWindowPosition(
            cursorPosition.X,
            cursorPosition.Y,
            (int)size.Width,
            (int)size.Height,
            excludeRect);

        IsContextMenuVisible = true;
        ContextMenuAppWindow?.MoveAndResize(rectangle.ToRectInt32());
        _ = WindowUtilities.ShowWindow(ContextMenuWindowHandle.Value);
        _ = WindowUtilities.SetForegroundWindow(ContextMenuWindowHandle.Value);
        ShowSecondWindowFlyout(ContextMenuFlyout, ContextMenuWindow.Content);
    }

    private static System.Drawing.Rectangle CreateTrayCursorExcludeRect(System.Drawing.Point cursorPosition)
    {
        // Native tray menus avoid overlapping the icon/taskbar affordance itself.
        // Give CalculatePopupWindowPosition a small tray-sized exclusion box so it
        // picks a position adjacent to the cursor instead of overlapping the taskbar.
        const int width = 36;
        const int height = 36;

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

    private void RaiseSecondWindowContextMenuOpened()
    {
        SecondWindowContextMenuOpened?.Invoke(this, EventArgs.Empty);
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

        ActualThemeChanged += (_, _) => frame.RequestedTheme = ActualTheme;

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
        flyout.Opened += (_, _) =>
        {
            if (IsContextMenuVisible)
            {
                RaiseSecondWindowContextMenuOpened();
            }
        };
        flyout.Closed += (_, _) =>
        {
            if (!flyout.AreOpenCloseAnimationsEnabled ||
                !IsContextMenuVisible)
            {
                _ = WindowUtilities.HideWindow(handle);
                return;
            }

            flyout.ShowAt(window.Content, new FlyoutShowOptions
            {
                ShowMode = FlyoutShowMode.Transient,
            });
        };
        foreach (var flyoutItemBase in ((MenuFlyout)ContextFlyout).Items)
        {
            if (flyoutItemBase is not MenuFlyoutSeparator)
            {
                flyoutItemBase.Height = 32;
                flyoutItemBase.Padding = new Thickness(11, 0, 11, 0);
            }
            flyout.Items.Add(flyoutItemBase);

            // MenuFlyoutSubItem should not be clickable and should not close the context menu
            if (flyoutItemBase is not MenuFlyoutSubItem)
            {
                flyoutItemBase.Tapped += (_, _) =>
                {
                    IsContextMenuVisible = false;
                    flyout.Hide();
                    _ = WindowUtilities.HideWindow(handle);
                };
            }
        }

        frame.Loaded += (_, _) =>
        {
            // Set the window style to PopupWindow to make the title bar invisible
            if (ContextMenuWindowHandle != null)
            {
                HwndUtilities.SetWindowStyleAsPopupWindow(ContextMenuWindowHandle.Value);
            }
            
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

            if (!IsContextMenuVisible)
            {
                return;
            }

            ShowSecondWindowFlyout(flyout, window.Content);
        };

        ContextMenuWindow = window;
        ContextMenuWindowHandle = handle;
#if !HAS_UNO
        ContextMenuAppWindow = appWindow;
#endif
        ContextMenuFlyout = flyout;
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(MenuFlyoutSeparator))]
    private static Size MeasureFlyout(MenuFlyout flyout, Size availableSize)
    {
        var width = 0.0;
        var height = 4.0; // top and bottom margin

        foreach (var item in flyout.Items)
        {
            // https://github.com/microsoft/microsoft-ui-xaml/issues/7374
            if (item is not MenuFlyoutSeparator)
            {
                item.Height = 32;
                item.Padding = new Thickness(11, 0, 11, 0);
            }
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
