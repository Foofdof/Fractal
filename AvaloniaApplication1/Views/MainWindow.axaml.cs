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
        if (PreviewHost == null || CanvasLayer == null) return;

        _vpW = Math.Max(1, (int)PreviewHost.Bounds.Width);
        _vpH = Math.Max(1, (int)PreviewHost.Bounds.Height);

        CanvasLayer.Width = _vpW;
        CanvasLayer.Height = _vpH;

        if (DataContext is MainWindowViewModel vm)
        {
            vm.RenderViewport(_vpW, _vpH); // Пересчёт с текущими координатами
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
            var start = _dragStart.Value;
            var cur = e.GetPosition(CanvasLayer);

            // целевое соотношение сторон (ширина/высота) под холст
            double target = _vpW / (double)_vpH;

            double dx = cur.X - start.X;
            double dy = cur.Y - start.Y;

            if (Math.Abs(dy) < 1e-6)
            {
                dy = Math.Sign(dy == 0 ? 1 : dy) * Math.Abs(dx) / target;
            }
            else
            {
                double current = Math.Abs(dx / dy);
                if (current > target)
                {
                    // слишком широкий — подгоняем высоту
                    dy = Math.Sign(dy) * Math.Abs(dx) / target;
                }
                else
                {
                    // слишком высокий — подгоняем ширину
                    dx = Math.Sign(dx) * Math.Abs(dy) * target;
                }
            }

            var end = new Point(start.X + dx, start.Y + dy);

            // ограничим в пределах холста
            end = new Point(
                Math.Clamp(end.X, 0, _vpW),
                Math.Clamp(end.Y, 0, _vpH)
            );

            _selection = new Rect(start, end).Normalize();
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

        if (rect.Width < 4 || rect.Height < 4) return;
        if (DataContext is not MainWindowViewModel vm) return;
        
        decimal xMin = vm.Xmin;
        decimal xMax = vm.Xmax;
        decimal yMin = vm.Ymin;
        decimal yMax = vm.Ymax;

        decimal worldW = xMax - xMin;
        decimal worldH = yMax - yMin;

        decimal denomX = (decimal)Math.Max(1.0, _vpW - 1.0);
        decimal denomY = (decimal)Math.Max(1.0, _vpH - 1.0);
        
        decimal x0 = xMin + ((decimal)rect.Left  / denomX) * worldW;
        decimal x1 = xMin + ((decimal)rect.Right / denomX) * worldW;

        // Y на канве сверху-вниз
        decimal y0 = yMax - ((decimal)rect.Top    / denomY) * worldH;
        decimal y1 = yMax - ((decimal)rect.Bottom / denomY) * worldH;
        
        vm.Xmin = Math.Min(x0, x1);
        vm.Xmax = Math.Max(x0, x1);
        vm.Ymin = Math.Min(y0, y1);
        vm.Ymax = Math.Max(y0, y1);

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