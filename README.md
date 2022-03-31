## H.NotifyIcon - NotifyIcon for .Net Core 3.1/.Net 5/.Net 6 WPF and WinUI

** This is a fork, if you see any activity in the official version - 
it's better to use them - https://github.com/hardcodet/wpf-notifyicon **

[![Language](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)](https://github.com/HavenDV/H.NotifyIcon/search?l=C%23&o=desc&s=&type=Code) 
[![License](https://img.shields.io/github/license/HavenDV/H.NotifyIcon.svg?label=License&maxAge=86400)](LICENSE.md) 
[![Build Status](https://github.com/HavenDV/H.NotifyIcon/workflows/.NET/badge.svg?branch=master)](https://github.com/HavenDV/H.NotifyIcon/actions?query=workflow%3A%22.NET%22)

This is an implementation of a NotifyIcon (aka system tray icon or taskbar icon) for the WPF/WinUI platform. 
It does not just rely on the Windows Forms NotifyIcon component, 
but is a purely independent control which leverages several features of the WPF/WinUI framework 
in order to display rich tooltips, popups, context menus, and balloon messages. 
It can be used directly in code or embedded in any XAML file.

### Nuget

[![Nuget](https://img.shields.io/nuget/dt/H.NotifyIcon.Wpf.svg?style=flat-square&label=H.NotifyIcon.Wpf)](https://www.nuget.org/packages/H.NotifyIcon.Wpf/)
[![Nuget](https://img.shields.io/nuget/dt/H.NotifyIcon.WinUI.svg?style=flat-square&label=H.NotifyIcon.WinUI)](https://www.nuget.org/packages/H.NotifyIcon.WinUI/)

```
Install-Package H.NotifyIcon.Wpf
Install-Package H.NotifyIcon.WinUI
```

### Usage

```xml
<Window
  x:Class="Hardcodet.NetDrives.UI.SystemTray.Sample"
  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  xmlns:tb="http://www.hardcodet.net/taskbar" // WPF
  xmlns:tb="using:H.NotifyIcon" // WinUI
  >

    <tb:TaskbarIcon x:Name="myNotifyIcon"
                    Visibility="Visible"
                    ToolTipText="Fallback ToolTip for Windows xp"
                    IconSource="/Images/TrayIcons/Logo.ico"
                    ContextMenu="{StaticResource TrayMenu}"
                    MenuActivation="LeftOrRightClick"
                    TrayPopup="{StaticResoure TrayStatusPopup}"
                    PopupActivation="DoubleClick"
                    TrayToolTip="{StaticResource TrayToolTip}"
      />

</Window>
```

### Features

- Custom Popups (interactive controls) on mouse clicks.
- Customized ToolTips (Vista and above) with fallback mechanism for xp/2003.
- Rich event model including attached events to trigger animations in Popups, ToolTips, and balloon messages. I just love that.
- Full support for standard Windows balloons, including custom icons.
- Custom balloons that pop up in the tray area. Go wild with styles and animations 🙂
- Support for WPF context menus.
- You can define whether to show Popups on left-, right-, double-clicks etc. The same goes for context menus.
- Simple data binding for Popups, ToolTips and custom balloons through attached properties and derived data context.
- Command support for single / double clicks on the tray icon.

### [Sample Apps](https://github.com/HavenDV/H.NotifyIcon/tree/master/samples)

