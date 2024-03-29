﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace NotifyIconWpf.Sample.ShowCases.Tutorials;

public class MvvmSampleViewModel : INotifyPropertyChanged, IDisposable
{
    private DispatcherTimer timer;

    public string Timestamp => DateTime.Now.ToLongTimeString();


    public MvvmSampleViewModel()
    {
        timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, OnTimerTick, Application.Current.Dispatcher);
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        //fire a property change event for the timestamp
        Application.Current.Dispatcher.BeginInvoke(new Action(() => OnPropertyChanged("Timestamp")));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public void Dispose()
    {
        timer.Stop();
    }
}
