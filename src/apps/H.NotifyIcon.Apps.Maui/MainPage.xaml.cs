using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace H.NotifyIcon.Apps.Maui;

public partial class MainPage
{
	public MainPage()
	{
		InitializeComponent();
        
        CanvasView.PaintSurface += CanvasViewOnPaintSurface;
	}

    private static void CanvasViewOnPaintSurface(object? sender, SKPaintSurfaceEventArgs args)
    {
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;

        canvas.Clear();

        using var backgroundBrush = new SKPaint();
        backgroundBrush.Style = SKPaintStyle.Stroke;
        backgroundBrush.Color = SKColors.Black;
        backgroundBrush.StrokeWidth = 24;
        backgroundBrush.StrokeCap = SKStrokeCap.Round;
            
        using var foregroundBrush = new SKPaint();
        backgroundBrush.Style = SKPaintStyle.Stroke;
        backgroundBrush.Color = SKColors.White;
        backgroundBrush.StrokeWidth = 24;
        backgroundBrush.StrokeCap = SKStrokeCap.Round;
        
        canvas.DrawBitmap(IconGenerator.Generate(
            backgroundBrush: backgroundBrush,
            foregroundBrush: foregroundBrush), info.Rect);
    }
}

