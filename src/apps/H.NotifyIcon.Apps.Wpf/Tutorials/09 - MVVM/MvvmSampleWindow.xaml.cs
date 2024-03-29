﻿using System.Windows;

namespace NotifyIconWpf.Sample.ShowCases.Tutorials;

/// <summary>
/// Interaction logic for MvvmSampleWindow.xaml
/// </summary>
public partial class MvvmSampleWindow : Window
{
    public MvvmSampleWindow()
    {
        InitializeComponent();

        Closed += (_, _) =>
        {
            var viewModel = (MvvmSampleViewModel)DataContext;
            viewModel.Dispose();

            TaskbarIcon.Dispose();
        };
    }
}
