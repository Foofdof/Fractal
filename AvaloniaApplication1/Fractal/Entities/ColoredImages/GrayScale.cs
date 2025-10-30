using System;
using System.Collections.Generic;
using Fractal.Abstractions;
using Fractal.ValueObjects;

namespace Fractal.Entities.ColoredImages;

public class GrayScale: IColoredImage
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
                
                byte r = (byte)Math.Clamp((int)Math.Round(255.0 * t), 0, 255);
                byte g = r;
                byte b = r;

                row.Add(new Pixel(r, g, b));
            }
            colored.Add(row);
        }
        
        return new Image(colored);
    }
}