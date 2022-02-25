global using System.Drawing;
global using System.Diagnostics;
global using System.ComponentModel;
global using System.Diagnostics.Contracts;
global using Hardcodet.Wpf.TaskbarNotification.Interop;
global using Point = Hardcodet.Wpf.TaskbarNotification.Interop.Point;
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
global using System.Windows.Interop;
global using System.Windows.Resources;
global using System.Windows.Threading;
#elif HAS_WINUI
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
#else
global using Windows.UI.Xaml;
global using Windows.UI.Xaml.Controls;
#endif