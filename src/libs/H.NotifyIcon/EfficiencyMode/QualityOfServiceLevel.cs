namespace H.NotifyIcon.EfficiencyMode;

/// <summary>
/// The system maintains multiple QoS levels, each with differentiated performance and power efficiency.  <br/>
/// Windows provides standard default settings for scheduling and processor power management for each QoS level.  <br/>
/// The precise tuning of each QoS level for processor power management and heterogeneous scheduling can be modified through Windows Provisioning. <br/>
/// For more information on performance tuning and provisioning, see: <br/>
/// <see href="https://docs.microsoft.com/en-us/windows/win32/procthread/quality-of-service#quality-of-service-levels"/>
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows8.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
public enum QualityOfServiceLevel
{
    /// <summary>
    /// Default system managed behavior. Let system manage all power throttling. <br/>
    /// </summary>
    Default,

    /// <summary>
    /// Description: Windowed applications that are in the foreground and in focus, or audible, 
    /// and explicitly tag processes with SetProcessInformation or threads with SetThreadInformation. <br/>
    /// Performance and power: Standard high performance. <br/>
    /// Release: 1709 <br/>
    /// </summary>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0.16299.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
    High,

    /// <summary>
    /// Description: Windowed applications that may be visible to the end user but are not in focus. <br/>
    /// Performance and power: Varies by platform, between High and Low. <br/>
    /// Release: 1709 <br/>
    /// </summary>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0.16299.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
    Medium,

    /// <summary>
    /// Description: Windowed applications that are not visible or audible to the end user. <br/>
    /// Performance and power: On battery, selects most efficient CPU frequency and schedules to efficient core. <br/>
    /// Release: 1709 <br/>
    /// </summary>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0.16299.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
    Low,

    /// <summary>
    /// Description: Background services. <br/>
    /// Performance and power: On battery, selects most efficient CPU frequency and schedules to efficient cores. <br/>
    /// Release: Windows 11 22H2 <br/>
    /// </summary>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows11.0.22621.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
    Utility,

    /// <summary>
    /// Description: Applications that explicitly tag processes with SetProcessInformation or threads with SetThreadInformation. <br/>
    /// Performance and power: Always selects most efficient CPU frequency and schedules to efficient cores. <br/>
    /// Release: Windows 11 <br/>
    /// </summary>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows11.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
    Eco,

    /// <summary>
    /// Description: Threads explicitly tagged by the Multimedia Class Scheduler Service to denote multimedia batch buffering. <br/>
    /// Performance and power: CPU frequency reduced for efficient batch processing. <br/>
    /// Release: 2004 <br/>
    /// </summary>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0.19041.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
    Media,

    /// <summary>
    /// Description: Threads explicitly tagged by Multimedia Class Scheduler Service to denote that audio threads require performance to meet deadlines. <br/>
    /// Performance and power: High performance to meet media deadlines. <br/>
    /// Release: 2004 <br/>
    /// </summary>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows10.0.19041.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
    Deadline,
}
