using System;
using System.Collections.Generic;
using Fractal.Abstractions;
using Fractal.ValueObjects;

namespace Fractal.Entities.ColoredImages;

/// <summary>
/// Радужная окраска: меняем оттенок (HSV) от 0 до ~270°.
/// </summary>
public class Rainbow : IColoredImage
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

                t = Math.Clamp(t, 0.0, 1.0);

                // больше контраста, чтобы "внутри множества" было темнее
                double v = Math.Pow(t, 0.7);   // value
                double s = 1.0;                // saturation
                double h = 270.0 * t;          // hue 0..270°

                (byte r, byte g, byte b) = HsvToRgb(h, s, v);
                row.Add(new Pixel(r, g, b));
            }
            colored.Add(row);
        }

        return new Image(colored);
    }

    // h: 0..360, s/v: 0..1
    private static (byte r, byte g, byte b) HsvToRgb(double h, double s, double v)
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

        byte r = (byte)Math.Clamp((int)Math.Round((r1 + m) * 255.0), 0, 255);
        byte g = (byte)Math.Clamp((int)Math.Round((g1 + m) * 255.0), 0, 255);
        byte b = (byte)Math.Clamp((int)Math.Round((b1 + m) * 255.0), 0, 255);
        return (r, g, b);
    }
}