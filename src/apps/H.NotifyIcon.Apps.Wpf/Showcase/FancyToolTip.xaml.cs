using DependencyPropertyGenerator;

namespace NotifyIconWpf.Sample.ShowCases.Showcase;

[DependencyProperty<string>("InfoText", DefaultValue = "", Description = "The tooltip details.")]
public partial class FancyToolTip
{
    public FancyToolTip()
    {
        InitializeComponent();
    }
}
