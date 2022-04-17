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
- Context menus
- ICommand support
- Design-time access
- Dynamic icon generation

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
Example 4: <img width="17" alt="image" src="https://user-images.githubusercontent.com/3002068/163723467-0e019f54-8e3e-4517-8177-d2800fa91583.png">
```xml
<tb:TaskbarIcon
    GeneratedIconText="❤️"
    GeneratedIconBorderThickness="5"
    GeneratedIconMargin="2"
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


### [Sample Apps](https://github.com/HavenDV/H.NotifyIcon/tree/master/src/apps)
The minimum supported version of the .Net Framework is 4.5.1.  
So in some cases to build the project you will need to install this -  
https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net451-developer-pack-offline-installer

### Contacts
* [mail](mailto:havendv@gmail.com)
* Discord: Haven#5924
