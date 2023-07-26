global using System.IO;
global using System.ComponentModel;
global using System.Diagnostics;
global using DependencyPropertyGenerator;
global using EventGenerator;
global using H.NotifyIcon.Core;
global using System.Runtime.Versioning;
#if HAS_AVALONIA
global using Avalonia;
global using Avalonia.Data;
global using Avalonia.Controls;
global using DependencyObject = Avalonia.IAvaloniaObject;
#elif HAS_WPF
global using System.Windows;
global using System.Windows.Controls;
global using System.Windows.Controls.Primitives;
global using System.Windows.Input;
global using System.Windows.Media;
global using System.Windows.Media.Imaging;
global using System.Windows.Interop;
global using System.Windows.Resources;
global using System.Windows.Threading;
#elif HAS_WINUI || HAS_UNO_WINUI
global using Windows.Foundation;
global using Windows.UI;
global using Microsoft.UI;
global using Windows.UI.Text;
global using Microsoft.UI.Text;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Controls.Primitives;
global using Microsoft.UI.Xaml.Input;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Media.Imaging;
global using Microsoft.UI.Xaml.Interop;
global using Microsoft.UI.Xaml.Resources;
global using System.Windows.Input;
global using Windows.Storage;
global using FontStyles = Windows.UI.Text.FontStyle;
global using WinRT.Interop;
global using Microsoft.UI.Windowing;
global using Windows.Graphics;
#if HAS_UNO
global using WindowActivationState = Windows.UI.Core.CoreWindowActivationState;
#endif
#elif HAS_UNO
global using Windows.UI.Core;
global using Windows.System;
global using Windows.UI;
global using Windows.UI.Text;
global using Windows.UI.Xaml;
global using Windows.UI.Xaml.Controls;
global using Windows.UI.Xaml.Controls.Primitives;
global using Windows.UI.Xaml.Input;
global using Windows.UI.Xaml.Media;
global using Windows.UI.Xaml.Media.Imaging;
global using System.Windows.Input;
global using Windows.Storage;
global using Windows.Foundation;
global using FontStyles = Windows.UI.Text.FontStyle;
#else
global using System.Windows.Input;
global using Microsoft.Maui;
global using Microsoft.Maui.Controls;
global using Microsoft.Maui.Media;
global using Microsoft.Maui.Graphics;
global using Microsoft.Maui.Primitives;
global using FontStyle = Microsoft.Maui.Controls.FontAttributes;
global using FontStyles = Microsoft.Maui.Controls.FontAttributes;
global using FontWeights = Microsoft.Maui.FontWeight;
global using FontFamily = System.String;
global using FrameworkElement = Microsoft.Maui.Controls.TemplatedView;
global using MenuFlyoutItemBase = Microsoft.Maui.IMenuElement;
#endif
#if HAS_MAUI_WINUI
global using Windows.Graphics;
global using WinRT.Interop;
global using Microsoft.UI.Windowing;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Media.Imaging;
global using MenuFlyout = Microsoft.Maui.Controls.MenuFlyout;
global using MenuFlyoutItem = Microsoft.Maui.Controls.MenuFlyoutItem;
global using MenuFlyoutSeparator = Microsoft.Maui.Controls.MenuFlyoutSeparator;
global using MenuFlyoutSubItem = Microsoft.Maui.Controls.MenuFlyoutSubItem;
#endif
#if HAS_SYSTEM_DRAWING
global using Icon = System.Drawing.Icon;
global using Bitmap = System.Drawing.Bitmap;
#elif HAS_SKIA_SHARP
global using Icon = SkiaSharp.SKBitmap;
global using Bitmap = SkiaSharp.SKBitmap;
#endif
