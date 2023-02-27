global using System.IO;
global using System.ComponentModel;
global using System.Diagnostics;
global using DependencyPropertyGenerator;
global using H.NotifyIcon.Core;
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
#if !HAS_UNO
global using WinRT.Interop;
global using Microsoft.UI.Windowing;
global using Windows.Graphics;
#endif
#else
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
#endif
#if HAS_SYSTEM_DRAWING
global using Icon = System.Drawing.Icon;
#elif HAS_SKIA_SHARP
global using Icon = SkiaSharp.SKBitmap;
#endif
