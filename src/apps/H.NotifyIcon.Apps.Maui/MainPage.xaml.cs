﻿using CommunityToolkit.Mvvm.Input;

namespace H.NotifyIcon.Apps.Maui;

public partial class MainPage
{
    private bool IsWindowVisible { get; set; } = true;

	public MainPage()
	{
		InitializeComponent();

        BindingContext = this;
        //CanvasView.PaintSurface += CanvasViewOnPaintSurface;
    }

    [RelayCommand]
    public void ShowHideWindow()
    {
        var window = Application.Current?.Windows[0];
        if (window == null)
        {
            return;
        }

        if (IsWindowVisible)
        {
            window.Hide();
        }
        else
        {
            window.Show();
        }
        IsWindowVisible = !IsWindowVisible;
    }

    [RelayCommand]
    public void ExitApplication()
    {
        Application.Current?.Quit();
    }

    //
    // private static void CanvasViewOnPaintSurface(object? sender, SKPaintSurfaceEventArgs args)
    // {
    //     SKImageInfo info = args.Info;
    //     SKSurface surface = args.Surface;
    //     SKCanvas canvas = surface.Canvas;
    //
    //     canvas.Clear();
    //
    //     using var backgroundBrush = new SKPaint();
    //     backgroundBrush.Style = SKPaintStyle.Stroke;
    //     backgroundBrush.Color = SKColors.Black;
    //     backgroundBrush.StrokeWidth = 24;
    //     backgroundBrush.StrokeCap = SKStrokeCap.Round;
    //         
    //     using var foregroundBrush = new SKPaint();
    //     foregroundBrush.Style = SKPaintStyle.Stroke;
    //     foregroundBrush.Color = SKColors.White;
    //     foregroundBrush.StrokeWidth = 1;
    //     foregroundBrush.StrokeCap = SKStrokeCap.Round;
    //     foregroundBrush.TextSize = 48;
    //     
    //     canvas.DrawBitmap(SkiaSharpIconGenerator.Generate(
    //         backgroundBrush: backgroundBrush,
    //         foregroundBrush: foregroundBrush,
    //         text: "H",
    //         font: new SKFont(SKTypeface.Default, size: 48F),
    //         pen: new SKPaint
    //         {
    //             Style = SKPaintStyle.Stroke,
    //             StrokeWidth = 5,
    //             Color = SKColors.Aqua,
    //         }), info.Rect);
    // }
}

