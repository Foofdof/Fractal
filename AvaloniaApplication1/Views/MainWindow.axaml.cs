using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Views;

public partial class MainWindow : Window
{
    private Point? _dragStart;
    private Rect _selection;
    private int _vpW;
    private int _vpH;

    public MainWindow()
    {
        InitializeComponent();
        // Когда окно отрисовалось — попросим VM пересчитать
        this.AttachedToVisualTree += (_, __) =>
        {
            UpdateViewportFromHost();
        };
    }

    // Подгоняем размеры канвы и картинки под доступную область
    private void PreviewHost_OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateViewportFromHost();
    }

    private void UpdateViewportFromHost()
    {
        if (PreviewHost == null || CanvasLayer == null || FractalImage == null) return;

        _vpW = Math.Max(1, (int)PreviewHost.Bounds.Width);
        _vpH = Math.Max(1, (int)PreviewHost.Bounds.Height);

        CanvasLayer.Width = _vpW;
        CanvasLayer.Height = _vpH;

        FractalImage.Width = _vpW;
        FractalImage.Height = _vpH;

        if (DataContext is MainWindowViewModel vm)
        {
            vm.RenderViewport(_vpW, _vpH);
        }
    }

    // --- Выделение мышью ---

    private void CanvasLayer_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _dragStart = e.GetPosition(CanvasLayer);
        _selection = new Rect(_dragStart.Value, _dragStart.Value);
        ShowSelection();
        e.Pointer.Capture(CanvasLayer);
    }

    private void CanvasLayer_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragStart.HasValue && e.GetCurrentPoint(CanvasLayer).Properties.IsLeftButtonPressed)
        {
            var p = e.GetPosition(CanvasLayer);
            _selection = new Rect(_dragStart.Value, p).Normalize();
            UpdateSelectionRect();
        }
    }

    private void CanvasLayer_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_dragStart.HasValue) return;

        e.Pointer.Capture(null);
        HideSelection();

        var rect = _selection;
        _dragStart = null;

        // Минимальный размер выделения
        if (rect.Width < 4 || rect.Height < 4) return;

        if (DataContext is not MainWindowViewModel vm) return;

        // Пересчёт px -> координаты комплексной плоскости
        double xMin = (double)vm.Xmin;
        double xMax = (double)vm.Xmax;
        double yMin = (double)vm.Ymin;
        double yMax = (double)vm.Ymax;

        double x0 = xMin + (rect.Left  / Math.Max(1, _vpW - 1)) * (xMax - xMin);
        double x1 = xMin + (rect.Right / Math.Max(1, _vpW - 1)) * (xMax - xMin);
        double y0 = yMin + (rect.Top   / Math.Max(1, _vpH - 1)) * (yMax - yMin);
        double y1 = yMin + (rect.Bottom/ Math.Max(1, _vpH - 1)) * (yMax - yMin);

        // Нормализуем
        var nxmin = Math.Min(x0, x1);
        var nxmax = Math.Max(x0, x1);
        var nymin = Math.Min(y0, y1);
        var nymax = Math.Max(y0, y1);

        vm.Xmin = (decimal)nxmin;
        vm.Xmax = (decimal)nxmax;
        vm.Ymin = (decimal)nymin;
        vm.Ymax = (decimal)nymax;

        vm.RenderViewport(_vpW, _vpH);
    }

    // --- Вспомогательные методы для рамки выделения ---

    private void ShowSelection()
    {
        SelectionRect.IsVisible = true;
        UpdateSelectionRect();
    }

    private void HideSelection()
    {
        SelectionRect.IsVisible = false;
    }

    private void UpdateSelectionRect()
    {
        Canvas.SetLeft(SelectionRect, _selection.X);
        Canvas.SetTop(SelectionRect, _selection.Y);
        SelectionRect.Width = _selection.Width;
        SelectionRect.Height = _selection.Height;
    }
}