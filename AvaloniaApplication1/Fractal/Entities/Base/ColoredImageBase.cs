using System;
using System.Collections.Generic;
using System.Linq;
using Fractal.Abstractions;
using Fractal.Entities;        // Image
using Fractal.ValueObjects;    // Pixel, FractalData

namespace Fractal.Entities.Base;

public abstract class ColoredImageBase : IColoredImage
{
    /// <summary>Цвет для точек внутри множества (count >= MaxIteration).</summary>
    protected virtual Pixel InsidePixel => new Pixel(0, 0, 0);

    /// <summary>Гамма-коррекция для t (1.0 — без коррекции).</summary>
    protected virtual double Gamma => 1.0;

    /// <summary>Преобразование нормализованного t∈[0,1] в цвет.</summary>
    protected abstract Pixel FromT(double t);

    public Image Create(FractalData value)
    {
        var counts = value.Counts;
        int h = counts.Count;
        int w = counts[0].Count;

        // максимум среди "вышедших" (escape) точек
        int outsideMax = counts.SelectMany(r => r)
                               .Where(c => c < value.MaxIteration)
                               .DefaultIfEmpty(0)
                               .Max();
        int denom = Math.Max(1, outsideMax); // защита от 0

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

                double t = Math.Clamp(c / (double)denom, 0.0, 1.0);
                if (Math.Abs(Gamma - 1.0) > 1e-9)
                    t = Math.Pow(t, Gamma);

                row.Add(FromT(t));
            }
            colored.Add(row);
        }

        return new Image(colored);
    }

    // ---------- helpers ----------

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

    /// <summary>h: 0..360, s/v: 0..1</summary>
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