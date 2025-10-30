using System;
using System.Collections.Generic;
using Fractal.Abstractions;
using Fractal.ValueObjects;

namespace Fractal.Entities.ColoredImages;


public class Ocean : IColoredImage
{
    public Image Create(FractalData value)
    {
        var counts = value.Counts;
        var colored = new List<List<Pixel>>(counts.Count);

        for (int i = 0; i < counts.Count; i++)
        {
            var row = new List<Pixel>(counts[i].Count);
            for (int j = 0; j < counts[i].Count; j++)
            {
                double t = (value.MaxIteration > 1)
                    ? (double)counts[i][j] / (value.MaxIteration - 1)
                    : 1.0;

                // гамма для мягкости
                t = Math.Pow(Math.Clamp(t, 0.0, 1.0), 0.85);

                // точки градиента
                var c0 = (r: 0.0,   g: 10.0,  b: 40.0);   // #000A28 (тёмный синий)
                var c1 = (r: 0.0,   g: 72.0,  b: 255.0);  // #0048FF (синий)
                var c2 = (r: 0.0,   g: 229.0, b: 255.0);  // #00E5FF (бирюзовый)
                var c3 = (r: 255.0, g: 255.0, b: 255.0);  // белый

                (double r, double g, double b) Lerp((double r, double g, double b) a,
                                                     (double r, double g, double b) b_, double u)
                    => (a.r + (b_.r - a.r) * u,
                        a.g + (b_.g - a.g) * u,
                        a.b + (b_.b - a.b) * u);

                (double r, double g, double b) col;
                if (t < 0.33)
                    col = Lerp(c0, c1, t / 0.33);
                else if (t < 0.66)
                    col = Lerp(c1, c2, (t - 0.33) / 0.33);
                else
                    col = Lerp(c2, c3, (t - 0.66) / 0.34);

                byte r = (byte)Math.Clamp((int)Math.Round(col.r), 0, 255);
                byte g = (byte)Math.Clamp((int)Math.Round(col.g), 0, 255);
                byte b = (byte)Math.Clamp((int)Math.Round(col.b), 0, 255);

                row.Add(new Pixel(r, g, b));
            }
            colored.Add(row);
        }

        return new Image(colored);
    }
}