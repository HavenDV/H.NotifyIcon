# H.NotifyIcon.WPF - NotifyIcon for .Net Core 3.1 and .Net 5 WPF

**This fork is aimed only at developing the .Net Core version of the original project - https://github.com/hardcodet/wpf-notifyicon**

Current version: 

[![Nuget](https://img.shields.io/nuget/v/Hardcodet.NotifyIcon.Wpf.NetCore.svg)](https://www.nuget.org/packages/Hardcodet.NotifyIcon.Wpf.NetCore/)
[![Language](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)](https://github.com/HavenDV/Hardcodet.NotifyIcon.Wpf.NetCore/search?l=C%23&o=desc&s=&type=Code) 
[![License](https://img.shields.io/github/license/HavenDV/H.NotifyIcon.WPF.svg?label=License&maxAge=86400)](LICENSE.md) 
[![Requirements](https://img.shields.io/badge/Requirements-.NET%20Core%203.1-blue.svg)](https://github.com/dotnet/core/blob/master/release-notes/3.1/3.1-supported-os.md)
[![Requirements](https://img.shields.io/badge/Requirements-.NET%205.0-blue.svg)](https://github.com/dotnet/core/blob/master/release-notes/5.0/5.0-supported-os.md)
[![Build Status](https://github.com/HavenDV/H.NotifyIcon.WPF/workflows/.NET/badge.svg?branch=master)](https://github.com/HavenDV/H.NotifyIcon.WPF/actions?query=workflow%3A%22.NET%22)

## Description

This is an implementation of a NotifyIcon (aka system tray icon or taskbar icon) for the WPF platform. It does not just rely on the Windows Forms NotifyIcon component, but is a purely independent control which leverages several features of the WPF framework in order to display rich tooltips, popups, context menus, and balloon messages. It can be used directly in code or embedded in any XAML file.

### Features at a glance

- Custom Popups (interactive controls) on mouse clicks.
- Customized ToolTips (Vista and above) with fallback mechanism for xp/2003.
- Rich event model including attached events to trigger animations in Popups, ToolTips, and balloon messages. I just love that.
- Full support for standard Windows balloons, including custom icons.
- Custom balloons that pop up in the tray area. Go wild with styles and animations 🙂
- Support for WPF context menus.
- You can define whether to show Popups on left-, right-, double-clicks etc. The same goes for context menus.
- Simple data binding for Popups, ToolTips and custom balloons through attached properties and derived data context.
- Command support for single / double clicks on the tray icon.

### Tutorial and Support

A comprehensive tutorial that complements the attached sample application can be found on the Code Project: 
http://www.codeproject.com/KB/WPF/wpf_notifyicon.aspx

## XAML Declaration Sample

The sample below shows some of the properties of the control. For a more comprehensive sample, have a look at the sample application that comes with the download.

``` XML
<Window
  x:Class="Hardcodet.NetDrives.UI.SystemTray.Sample"
  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  xmlns:tb="http://www.hardcodet.net/taskbar">

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

