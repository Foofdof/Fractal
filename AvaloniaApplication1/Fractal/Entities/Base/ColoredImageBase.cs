using System;
using System.Collections.Generic;
using System.Linq;
using Fractal.Abstractions;
using Fractal.Entities;        // Image
using Fractal.ValueObjects;    // Pixel, FractalData

namespace Fractal.Entities.ColoredImages;

public abstract class ColoredImageBase : IColoredImage
{
    /// Цвет для точек внутри множества (count >= MaxIteration).
    protected virtual Pixel InsidePixel => new Pixel(0, 0, 0);

    /// Гамма-коррекция для t (1.0 — без коррекции).
    protected virtual double Gamma => 1.0;

    /// Перцентили для автоконтраста
    protected virtual double LowPercentile  => 0.01;
    protected virtual double HighPercentile => 0.99;

    /// Перевод нормализованного t∈[0,1] в цвет.
    protected abstract Pixel FromT(double t);

    public Image Create(FractalData value)
    {
        var counts = value.Counts;
        int h = counts.Count;
        int w = counts[0].Count;

        // ---------- 1) Окно нормализации по гистограмме ----------
        // Берём только "вышедшие" (escape) точки: c < MaxIteration
        var outside = counts.SelectMany(r => r).Where(c => c < value.MaxIteration).ToArray();

        int minC, maxC;
        if (outside.Length == 0)
        {
            // Вся область внутри множества — вернём сплошной InsidePixel
            var allInside = new List<List<Pixel>>(h);
            for (int y = 0; y < h; y++)
            {
                var row = new List<Pixel>(w);
                for (int x = 0; x < w; x++) row.Add(InsidePixel);
                allInside.Add(row);
            }
            return new Image(allInside);
        }
        else
        {
            // По массиву outside строим квантили (перцентили)
            Array.Sort(outside);
            minC = Percentile(outside, LowPercentile);
            maxC = Percentile(outside, HighPercentile);

            // если окно выродилось — расширим до наблюдаемого диапазона
            if (maxC <= minC)
            {
                minC = outside[0];
                maxC = outside[outside.Length - 1];
                if (maxC <= minC) maxC = minC + 1; // последняя защита
            }
        }

        double denom = Math.Max(1, maxC - minC);

        // ---------- 2) Рисуем ----------
        var colored = new List<List<Pixel>>(h);
        for (int y = 0; y < h; y++)
        {
            var row = new List<Pixel>(w);
            for (int x = 0; x < w; x++)
            {
                int c = counts[y][x];

                if (c >= value.MaxIteration)
                {
                    row.Add(InsidePixel);
                    continue;
                }

                // t в окне [minC, maxC]
                double t = (c - minC) / denom;
                t = Math.Clamp(t, 0.0, 1.0);

                if (Math.Abs(Gamma - 1.0) > 1e-9)
                    t = Math.Pow(t, Gamma);

                row.Add(FromT(t));
            }
            colored.Add(row);
        }

        return new Image(colored);
    }

    // ---------- helpers ----------

    /// Быстрый перцентиль по отсортированному массиву (0..1).
    private static int Percentile(int[] sorted, double p)
    {
        p = Math.Clamp(p, 0.0, 1.0);
        double idx = p * (sorted.Length - 1);
        int i = (int)Math.Floor(idx);
        int j = Math.Min(i + 1, sorted.Length - 1);
        double w = idx - i;
        // линейная интерполяция между соседями
        return (int)Math.Round(sorted[i] * (1 - w) + sorted[j] * w);
    }

    protected static (double r, double g, double b) Lerp((double r, double g, double b) a,
                                                         (double r, double g, double b) b, double u)
        => (a.r + (b.r - a.r) * u,
            a.g + (b.g - a.g) * u,
            a.b + (b.b - a.b) * u);

    protected static Pixel ToPixel((double r, double g, double b) col)
        => new Pixel(
            (byte)Math.Clamp((int)Math.Round(col.r), 0, 255),
            (byte)Math.Clamp((int)Math.Round(col.g), 0, 255),
            (byte)Math.Clamp((int)Math.Round(col.b), 0, 255));

    /// h: 0..360, s/v: 0..1
    protected static (double r, double g, double b) HsvToRgb(double h, double s, double v)
    {
        h = (h % 360 + 360) % 360;
        double c = v * s;
        double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
        double m = v - c;

        double r1, g1, b1;
        if (h < 60)       (r1, g1, b1) = (c, x, 0);
        else if (h < 120) (r1, g1, b1) = (x, c, 0);
        else if (h < 180) (r1, g1, b1) = (0, c, x);
        else if (h < 240) (r1, g1, b1) = (0, x, c);
        else if (h < 300) (r1, g1, b1) = (x, 0, c);
        else              (r1, g1, b1) = (c, 0, x);

        return ((r1 + m) * 255.0, (g1 + m) * 255.0, (b1 + m) * 255.0);
    }
}