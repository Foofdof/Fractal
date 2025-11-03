using System;
using System.Collections.Generic;
using Fractal.Abstractions;
using Fractal.Constants;
using Fractal.Numerics;
using Fractal.ValueObjects;

namespace Fractal.Entities.Base;

public abstract class BaseFractal : IFractal
{
    protected double Threshold { get; set; } = FractalConstants.MandelbrotConstants.DefaultThreshold;
    protected ImageBox DefaultGeneratingBox { get; set; } = FractalConstants.MandelbrotConstants.DefaultMandelbrotImageBox;
    
    protected virtual DecimalComplex Iterate(in DecimalComplex z, in DecimalComplex c)
        => z.Square() + c;

    public virtual FractalData Generate(ImageBox? box, int? maxIterations)
    {
        int maxIter = maxIterations ?? FractalConstants.MaxIteration;
        var imageBox = box ?? DefaultGeneratingBox;
        var cb = imageBox.Box;

        int w = imageBox.Screen.Nx;
        int h = imageBox.Screen.Ny;

        decimal xStep = (cb.Xmax - cb.Xmin) / Math.Max(1, w - 1);
        decimal yStep = (cb.Ymax - cb.Ymin) / Math.Max(1, h - 1);
        double r2 = Threshold * Threshold;

        var counts = new List<List<int>>(h);

        for (int py = 0; py < h; py++)
        {
            // ориентация: верх экрана соответствует Ymax
            decimal cy = cb.Ymax - py * yStep;
            var row = new List<int>(w);

            for (int px = 0; px < w; px++)
            {
                decimal cx = cb.Xmin + px * xStep;
                var c = new DecimalComplex(cx, cy);
                var z = DecimalComplex.Zero;

                int iter = 0;
                while (iter < maxIter && z.MagnitudeSquared <= (decimal)r2)
                {
                    z = Iterate(z, c);
                    iter++;
                }

                row.Add(iter);
            }

            counts.Add(row);
        }

        return new FractalData { MaxIteration = maxIter, Counts = counts };
    }
}