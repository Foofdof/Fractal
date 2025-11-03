using System.Collections.Generic;
using System.Threading.Tasks;
using Fractal.Abstractions;
using Fractal.Constants;
using Fractal.ValueObjects;
using Fractal.Entities; // ImageBox

namespace Fractal.Entities.Base
{
    public abstract class BaseFractal : IFractal
    {
        protected decimal Threshold { get; set; } = FractalConstants.MandelbrotConstants.DefaultThreshold;

        protected ImageBox DefaultGeneratingBox { get; set; } =
            FractalConstants.MandelbrotConstants.DefaultMandelbrotImageBox;
        
        protected virtual void IterateCore(ref decimal zx, ref decimal zy, decimal cx, decimal cy)
        {
            decimal xx = zx * zx;
            decimal yy = zy * zy;
            decimal xy = zx * zy;
            zx = xx - yy + cx;
            zy = 2m * xy + cy;
        }

        public virtual FractalData Generate(ImageBox? box, int? maxIterations)
        {
            int maxIter = maxIterations ?? FractalConstants.MaxIteration;
            var imageBox = box ?? DefaultGeneratingBox;
            var cb = imageBox.Box;

            int w = imageBox.Screen.Nx;
            int h = imageBox.Screen.Ny;

            decimal xStep = (cb.Xmax - cb.Xmin) / System.Math.Max(1, w - 1);
            decimal yStep = (cb.Ymax - cb.Ymin) / System.Math.Max(1, h - 1);
            decimal r2 = Threshold * Threshold;

            var rows = new int[h][];

            Parallel.For(0, h, py =>
            {
                decimal cy = cb.Ymax - py * yStep; // верх экрана соответствует Ymax
                var row = new int[w];

                decimal cx = cb.Xmin; // инкрементируем без умножения
                for (int px = 0; px < w; px++)
                {
                    decimal zx = 0m, zy = 0m;
                    int iter = 0;

                    while (iter < maxIter && (zx * zx + zy * zy) <= r2)
                    {
                        IterateCore(ref zx, ref zy, cx, cy);
                        iter++;
                    }

                    row[px] = iter;
                    cx += xStep;
                }

                rows[py] = row;
            });

            var counts = new List<List<int>>(h);
            for (int py = 0; py < h; py++)
                counts.Add(new List<int>(rows[py]));

            return new FractalData
            {
                MaxIteration = maxIter,
                Counts = counts
            };
        }
    }
}