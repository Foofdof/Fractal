using System;
using System.Collections.Generic;
using Fractal.Abstractions;
using Fractal.Constants;
using Fractal.Entities;
using Fractal.ValueObjects;

namespace Fractal.Entities.Fractals;


public sealed class FractalBurningShip : IFractal
{
    public FractalData Generate(ImageBox? box, int? maxIterations)
    {
        // Дефолты (как в Mandelbrot)
        box ??= new ImageBox(FractalConstants.FullHD, FractalConstants.MandelbrotConstants.DefaultGeneratingBox);
        int maxIter = maxIterations ?? FractalConstants.MaxIteration;

        // Экран и мировая область
        int w = box!.Screen.Nx;
        int h = box!.Screen.Ny;

        double xMin = (double)box.Box.Xmin;
        double xMax = (double)box.Box.Xmax;
        double yMin = (double)box.Box.Ymin;
        double yMax = (double)box.Box.Ymax;

        double worldW = xMax - xMin;
        double worldH = yMax - yMin;

        var counts = new List<List<int>>(h);

        double r = FractalConstants.MandelbrotConstants.DefaultThreshold;
        double r2 = r * r;

        for (int py = 0; py < h; py++)
        {
            double cy = yMin + (py / Math.Max(1.0, h - 1.0)) * worldH;
            var row = new List<int>(w);

            for (int px = 0; px < w; px++)
            {
                double cx = xMin + (px / Math.Max(1.0, w - 1.0)) * worldW;

                double x = 0.0, y = 0.0;
                int iter = 0;

                // Burning Ship: берём модуль координат перед шагом
                while (x * x + y * y <= r2 && iter < maxIter)
                {
                    double ax = Math.Abs(x);
                    double ay = Math.Abs(y);

                    double xNew = ax * ax - ay * ay + cx;
                    double yNew = 2.0 * ax * ay + cy;

                    x = xNew;
                    y = yNew;
                    iter++;
                }

                row.Add(iter);
            }

            counts.Add(row);
        }

        return new FractalData
        {
            MaxIteration = maxIter,
            Counts = counts
        };
    }
}