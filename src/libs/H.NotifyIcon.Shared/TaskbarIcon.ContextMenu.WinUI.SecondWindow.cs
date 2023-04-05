using H.NotifyIcon.Interop;

namespace H.NotifyIcon;

public partial class TaskbarIcon
{
    #region Properties

    private bool IsContextMenuVisible { get; set; }
    private Window? ContextMenuWindow { get; set; }
    private nint? ContextMenuWindowHandle { get; set; }
    private AppWindow? ContextMenuAppWindow { get; set; }
    private MenuFlyout? ContextMenuFlyout { get; set; }

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
            ContextMenuFlyout == null)
        {
            return;
        }

        var size = MeasureFlyout(ContextMenuFlyout, new Size(10000.0, 10000.0));
        var rectangle = CursorUtilities.CalculatePopupWindowPosition(
            cursorPosition.X,
            cursorPosition.Y,
            (int)size.Width,
            (int)size.Height);

        ContextMenuAppWindow?.MoveAndResize(rectangle.ToRectInt32());
        _ = WindowUtilities.ShowWindow(ContextMenuWindowHandle.Value);
        _ = WindowUtilities.SetForegroundWindow(ContextMenuWindowHandle.Value);
    }

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

        ActualThemeChanged += (_, _) => frame.RequestedTheme = ActualTheme;

        var handle = WindowNative.GetWindowHandle(window);
        DesktopWindowsManagerMethods.SetRoundedCorners(handle);
        WindowUtilities.MakeTransparent(handle);

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
            if (flyoutItemBase is not MenuFlyoutSeparator)
            {
                flyoutItemBase.Height = 32;
                flyoutItemBase.Padding = new Thickness(11, 0, 11, 0);
            }
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

        ContextMenuWindow = window;
        ContextMenuWindowHandle = handle;
#if !HAS_UNO
        ContextMenuAppWindow = appWindow;
#endif
        ContextMenuFlyout = flyout;
    }

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
}
