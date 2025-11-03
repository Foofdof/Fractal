using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Fractal;

namespace AvaloniaApplication1.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // -------- Статус
    [ObservableProperty] private string status = "Готово";

    // -------- Координатная область (редактируемые, а также меняются при выделении мышью)
    [ObservableProperty] private decimal xmin = FractalConstants.MandelbrotConstants.DefaultGeneratingBox.Xmin;
    [ObservableProperty] private decimal xmax = FractalConstants.MandelbrotConstants.DefaultGeneratingBox.Xmax;
    [ObservableProperty] private decimal ymin = FractalConstants.MandelbrotConstants.DefaultGeneratingBox.Ymin;
    [ObservableProperty] private decimal ymax = FractalConstants.MandelbrotConstants.DefaultGeneratingBox.Ymax;

    // -------- Итерации
    [ObservableProperty] private int iterations = FractalConstants.MaxIteration;

    // -------- Списки выбора
    public IEnumerable<Fractal.Types.FractalType> FractalTypes { get; } = Enum.GetValues<Fractal.Types.FractalType>();
    public IEnumerable<Fractal.Enums.ColorImageType> ColorMapTypes { get; } = Enum.GetValues<Fractal.Enums.ColorImageType>();

    [ObservableProperty] private Fractal.Types.FractalType selectedFractalType = Fractal.Types.FractalType.Mandelbrot;
    [ObservableProperty] private Fractal.Enums.ColorImageType selectedColorMap = Fractal.Enums.ColorImageType.Fire;

    // -------- Bitmap для UI
    [ObservableProperty] private Bitmap? renderedBitmap;

    // -------- Текущий размер области вывода (задаётся из code-behind)
    private int _viewportWidth;
    private int _viewportHeight;

    public MainWindowViewModel()
    {
        // Ничего не рендерим до получения размеров из окна
    }

    [RelayCommand]
    private void Render()
    {
        if (_viewportWidth <= 0 || _viewportHeight <= 0) return;
        RenderViewport(_viewportWidth, _viewportHeight);
    }

    [RelayCommand]
    private void ResetBox()
    {
        Xmin = FractalConstants.MandelbrotConstants.DefaultGeneratingBox.Xmin;
        Xmax = FractalConstants.MandelbrotConstants.DefaultGeneratingBox.Xmax;
        Ymin = FractalConstants.MandelbrotConstants.DefaultGeneratingBox.Ymin;
        Ymax = FractalConstants.MandelbrotConstants.DefaultGeneratingBox.Ymax;

        Render();
    }

    /// <summary>
    /// Вызывается из окна при изменении размера правой панели и после выделения.
    /// </summary>
    public void RenderViewport(int width, int height)
    {
        _viewportWidth = width;
        _viewportHeight = height;

        try
        {
            Status = "Рисую...";

            var fractal = Fractal.Factories.FractalFactory.Generate(SelectedFractalType);

            var box2D = new Fractal.ValueObjects.Box2D
            {
                Xmin = Xmin,
                Xmax = Xmax,
                Ymin = Ymin,
                Ymax = Ymax
            };
            var imageBox = new Fractal.Entities.ImageBox((width, height), box2D);

            var data = fractal.Generate(imageBox, Iterations);

            var palette = Fractal.Factories.ColoredImageFactory.Create(SelectedColorMap);
            var img = palette.Create(data);

            RenderedBitmap = ToBitmap(img);
            Status = "Готово";
        }
        catch (Exception ex)
        {
            Status = "Ошибка: " + ex.Message;
            RenderedBitmap = null;
        }
    }

    private static WriteableBitmap ToBitmap(Fractal.Entities.Image image)
    {
        if (image?.Pixels == null || image.Pixels.Count == 0)
            throw new ArgumentException("Image is empty.");

        int h = image.Pixels.Count;
        int w = image.Pixels[0].Count;

        var bmp = new WriteableBitmap(
            new PixelSize(w, h),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Opaque);

        using var fb = bmp.Lock();
        int stride = fb.RowBytes;

        // Копируем только видимые байты строки (w * 4), padding заполняет сам framebuffer
        var line = new byte[w * 4];

        for (int y = 0; y < h; y++)
        {
            var src = image.Pixels[y];
            int off = 0;

            for (int x = 0; x < w; x++)
            {
                var p = src[x]; // Pixel с полями R,G,B (byte)
                line[off + 0] = p.B;     // B
                line[off + 1] = p.G;     // G
                line[off + 2] = p.R;     // R
                line[off + 3] = 255;     // A (opaque)
                off += 4;
            }

            // адрес начала строки в framebuffer
            var dst = IntPtr.Add(fb.Address, y * stride);
            Marshal.Copy(line, 0, dst, line.Length);
        }

        return bmp;
    }
}