using System.Windows;
using System.Windows.Controls;
using DependencyPropertyGenerator;

namespace NotifyIconWpf.Sample.ShowCases.Showcase;

[DependencyProperty<int>("ClickCount", Description = "The number of clicks on the popup button.")]
public partial class FancyPopup : UserControl
{
    public FancyPopup()
    {
        InitializeComponent();
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        //just increment a counter - will be displayed on screen
        ClickCount++;
    }
}
