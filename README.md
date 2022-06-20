## H.NotifyIcon

** This project is a continuation of the inactive base project to 
other platforms (WinUI/Uno.Skia.Wpf/Console) - ❤️ https://github.com/hardcodet/wpf-notifyicon ❤️ **

This is an implementation of a NotifyIcon (aka system tray icon or taskbar icon)
for .Net Core 3.1/.Net 5/.Net 6 WPF/WinUI/Uno.Skia.WPF/Console platforms.
It does not just rely on the Windows Forms NotifyIcon component, 
but is a purely independent control which leverages several features of the WPF/WinUI framework 
in order to display rich tooltips, popups, context menus, and balloon messages. 
It can be used directly in code or embedded in any XAML file.

### Features

- Notifications
- [Context menus](#winui-context-menu)
- ICommand support
- [Design-time access](#design-time-access)
- [Efficiency Mode](#efficiency-mode-)
- [Dynamic icon generation](#generated-icons)

### Nuget

[![Nuget](https://img.shields.io/nuget/dt/H.NotifyIcon.Wpf.svg?style=flat-square&label=H.NotifyIcon.Wpf)](https://www.nuget.org/packages/H.NotifyIcon.Wpf/)
[![Nuget](https://img.shields.io/nuget/dt/H.NotifyIcon.WinUI.svg?style=flat-square&label=H.NotifyIcon.WinUI)](https://www.nuget.org/packages/H.NotifyIcon.WinUI/)
[![Nuget](https://img.shields.io/nuget/dt/H.NotifyIcon.Uno.svg?style=flat-square&label=H.NotifyIcon.Uno)](https://www.nuget.org/packages/H.NotifyIcon.Uno/)
[![Nuget](https://img.shields.io/nuget/dt/H.NotifyIcon.Uno.WinUI.svg?style=flat-square&label=H.NotifyIcon.Uno.WinUI)](https://www.nuget.org/packages/H.NotifyIcon.Uno.WinUI/)

```
Install-Package H.NotifyIcon.Wpf
Install-Package H.NotifyIcon.WinUI
Install-Package H.NotifyIcon.Uno
Install-Package H.NotifyIcon.Uno.WinUI
// If you need other platforms, you can use this Core library - 
// it allows you to make NotifyIcon even in a console application.
Install-Package H.NotifyIcon
```

### Usage

```xml
<Window
    xmlns:tb="http://www.hardcodet.net/taskbar" // WPF
    xmlns:tb="using:H.NotifyIcon" // WinUI
    >
    <tb:TaskbarIcon
        ToolTipText="ToolTip"
        IconSource="/Images/TrayIcons/Logo.ico"
        ContextMenu="{StaticResource TrayMenu}"
        MenuActivation="LeftOrRightClick"
        TrayPopup="{StaticResoure TrayStatusPopup}"
        PopupActivation="DoubleClick"
        TrayToolTip="{StaticResource TrayToolTip}"
        />
</Window>
```

### Efficiency Mode ![image](https://user-images.githubusercontent.com/3002068/164691691-5baf5210-5b5e-417e-99d3-d0f19006d997.png)

Windows 11 introduces a new concept called [Efficiency Mode](https://devblogs.microsoft.com/performance-diagnostics/reduce-process-interference-with-task-manager-efficiency-mode/).  
Since, basically, this library is intended for applications to exist in the background with the ability to interact through TrayIcon,
the library implements an API for this mode:
```cs
EfficiencyModeUtilities.SetEfficiencyMode(bool value)
WindowExtensions.Hide(this Window window, enableEfficiencyMode: true) // default value
WindowExtensions.Show(this Window window, disableEfficiencyMode: true) // default value
TaskbarIcon.ForceCreate(bool enablesEfficiencyMode = true) // default value
```

### Generated icons
Example 1: <img width="15" alt="image" src="https://user-images.githubusercontent.com/3002068/163721411-1388f2b4-a039-4b4a-8114-f74bfc8835ba.png">
```xml
<tb:TaskbarIcon GeneratedIconText="❤️" GeneratedIconForeground="Red">
```
Example 2: <img width="14" alt="image" src="https://user-images.githubusercontent.com/3002068/163721399-cbfd0286-d2d4-4b40-b3f3-388c9613f535.png">
```xml
<tb:TaskbarIcon
    IconSource="/Icons/Error.ico"
    GeneratedIconText="5"
    GeneratedIconForeground="Black"
    GeneratedIconFontSize="36"
    GeneratedIconFontWeight="Bold"
    >
```
Example 3: <img width="19" alt="image" src="https://user-images.githubusercontent.com/3002068/163721367-dc6878df-3ec2-4288-b699-cf664894e1b1.png">
```xml
<tb:TaskbarIcon
    GeneratedIconText="❤️"
    GeneratedIconForeground="Red"
    GeneratedIconFontFamily="Segoe UI Emoji"
    GeneratedIconBackground="AliceBlue"
    GeneratedIconFontWeight="Bold"
    GeneratedIconFontSize="38"
    >
```
Example 4: <img width="18" alt="image" src="https://user-images.githubusercontent.com/3002068/163723782-8b135584-8b35-401e-926e-0fe0e7aa801e.png">
```xml
<tb:TaskbarIcon
    GeneratedIconText="❤️"
    GeneratedIconBorderThickness="5"
    GeneratedIconFontSize="46"
    >
    <tb:TaskbarIcon.GeneratedIconForeground>
        <LinearGradientBrush StartPoint="0,0" EndPoint="128,128">
            <GradientStop Color="White" />
            <GradientStop Color="Red" />
        </LinearGradientBrush>
    </tb:TaskbarIcon.GeneratedIconForeground>
    <tb:TaskbarIcon.GeneratedIconBorderBrush>
        <LinearGradientBrush StartPoint="0,0" EndPoint="128,128">
            <GradientStop Color="White" />
            <GradientStop Color="Red" />
        </LinearGradientBrush>
    </tb:TaskbarIcon.GeneratedIconBorderBrush>
```

### Design-Time Access
It is recommended to pin the designer icon for easy viewing. To do this, go to Taskbar Settings -> Other system tray icons and enable this icon:  
<img width="412" alt="image" src="https://user-images.githubusercontent.com/3002068/163700588-eb2ad5f2-45d0-4b6f-ad39-c66f96202cb5.png">

### WinUI Context menu
At the moment, three modes are implemented, each with its own pros and cons.
1. Based on your MenuFlyout, a Win32 PopupMenu will be created that will call the commands attached to your MenuFlyoutItem. This is the default.  <img width="174" alt="image" src="https://user-images.githubusercontent.com/3002068/164977047-e8497047-0c6d-4f99-b160-bc1c1a1a6c3f.png">
3. The menu will be created in your open window, in the corner of the screen.
4. A second transparent window will be created and used to render the native menu.. At the moment it is in the preview stage. To do this you need to explicitly set ContextMenuMode="SecondWindow"  
![image](https://user-images.githubusercontent.com/3002068/164977343-fab0ef4d-d1bd-4ff0-a1af-1d87f32c6400.png)

Availability of various options(depends on the version of `WindowsAppSDK` you are using):
Option       | Packaged App                 | Unpackaged App 
-------------|------------------------------|-----------------------------------
Transparency | 🟢 from 1.1.0-preview        | 🟢 from 1.1.0-preview
Borderless   | 🔷                           | 🟢 from 1.1.0-preview
Animations   | 🟢, but with borders         | 🟢 from 1.1.0-preview
Submenus     | 🔷                           | 🔷

### Behavior that needs attention
1. This implementation currently uses the Guid associated with each TrayIcon. 
The default is a hash function that creates a unique Guid based on the path to your file, 
because Windows associates the guid with the current path when TrayIcon is registered. 
The only way to keep the settings when changing the file path is to use [Authenticode](https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/ms537359(v=vs.85)). 
Read more here: https://docs.microsoft.com/en-us/windows/win32/api/shellapi/ns-shellapi-notifyicondataa#troubleshooting

### [Sample Apps](https://github.com/HavenDV/H.NotifyIcon/tree/master/src/apps)
The minimum supported version of the .Net Framework is 4.5.1.  
So in some cases to build the project you will need to install this -  
https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net451-developer-pack-offline-installer

### Support
You can get answers to your questions in my discord support channel:  
https://discord.gg/g8u2t9dKgE
