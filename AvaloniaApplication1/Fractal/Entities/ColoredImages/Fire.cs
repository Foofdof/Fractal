using System;
using System.Collections.Generic;
using Fractal.Abstractions;
using Fractal.ValueObjects;

namespace Fractal.Entities.ColoredImages;

public class Fire: IColoredImage
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
                double t = (value.MaxIteration > 1) ? (double)counts[i][j] / (value.MaxIteration - 1) : 1.0;
                t = Math.Pow(t, 0.6);
                
                byte r = (byte)Math.Round(255 * Math.Min(1.0, t * 3.0));
                byte g = (byte)Math.Round(Math.Min(1.0, Math.Max(0.0, (t - 0.33) * 3.0)));
                byte b = (byte)Math.Round(Math.Max(0.0, (t - 0.66) * 3.0 * 0.5));

                row.Add(new Pixel(r, g, b));
            }
            colored.Add(row);
        }
        
        return new Image(colored);
    }
}