﻿global using System.Drawing;
global using System.ComponentModel;
global using System.Diagnostics.Contracts;
global using H.NotifyIcon.Interop;
global using Point = System.Drawing.Point;
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
global using Windows.UI.Core;
global using Windows.System;
global using Windows.UI.Xaml;
global using Windows.UI.Xaml.Controls;
global using Windows.UI.Xaml.Controls.Primitives;
global using Windows.UI.Xaml.Input;
global using Windows.UI.Xaml.Media;
global using Windows.UI.Xaml.Media.Imaging;
global using System.Windows.Input;
global using Windows.Storage;
#endif