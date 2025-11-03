using System.Collections.Generic;
using System.Threading.Tasks;
using Fractal.Entities.Base;
using Fractal.ValueObjects;
using Fractal.Constants;

namespace Fractal.Entities.Fractals
{
    public sealed class FractalNewton3 : BaseFractal
    {
        // Тонкая настройка безопасности:
        public decimal Eps       { get; set; } = 1e-6m;   // критерий сходимости по шагу
        public decimal MinDenom  { get; set; } = 1e-28m;  // нижняя граница для |f'|^2
        public decimal BailoutR2 { get; set; } = 1e12m;   // (1e6)^2 — «слишком далеко» → прерываем
        public decimal MaxStep2  { get; set; } = 1e10m;   // ограничение величины шага (опция)

        public override FractalData Generate(ImageBox? box, int? maxIterations)
        {
            int maxIter = maxIterations ?? FractalConstants.MaxIteration;
            var imageBox = box ?? DefaultGeneratingBox;
            var b = imageBox.Box;

            int w = imageBox.Screen.Nx;
            int h = imageBox.Screen.Ny;

            decimal xStep = (b.Xmax - b.Xmin) / System.Math.Max(1, w - 1);
            decimal yStep = (b.Ymax - b.Ymin) / System.Math.Max(1, h - 1);

            var rows = new int[h][];

            Parallel.For(0, h, py =>
            {
                decimal yi = b.Ymax - py * yStep; // верх = Ymax
                var row = new int[w];

                decimal xi = b.Xmin;
                for (int px = 0; px < w; px++)
                {
                    int iter = 0;
                    decimal x = xi, y = yi;

                    try
                    {
                        while (iter < maxIter)
                        {
                            // 0) Bailout по модулю (защита от переполнений)
                            decimal r2 = x * x + y * y;
                            if (r2 > BailoutR2) { iter = maxIter; break; }

                            // 1) f(z) = z^3 - 1
                            decimal x2 = x * x, y2 = y * y;
                            decimal fx = x * (x2 - 3m * y2) - 1m;
                            decimal fy = y * (3m * x2 - y2);

                            // 2) f'(z) = 3 z^2
                            decimal dfx = 3m * (x2 - y2);
                            decimal dfy = 6m * x * y;

                            // 3) q = f / f'
                            decimal denom = dfx * dfx + dfy * dfy;
                            if (denom < MinDenom) { iter = maxIter; break; } // почти нулевая производная

                            decimal qx = (fx * dfx + fy * dfy) / denom;
                            decimal qy = (fy * dfx - fx * dfy) / denom;

                            decimal nx = x - qx;
                            decimal ny = y - qy;

                            // 4) критерий сходимости и ограничение размера шага
                            decimal dx = nx - x, dy = ny - y;
                            decimal step2 = dx * dx + dy * dy;
                            if (step2 < Eps * Eps) break;
                            if (step2 > MaxStep2) { iter = maxIter; break; }

                            x = nx; y = ny; iter++;
                        }
                    }
                    catch (System.OverflowException)
                    {
                        iter = maxIter; // страхуем любой редкий overflow
                    }

                    row[px] = iter;
                    xi += xStep;
                }

                rows[py] = row;
            });

            var counts = new List<List<int>>(h);
            for (int py = 0; py < h; py++) counts.Add(new List<int>(rows[py]));
            return new FractalData { MaxIteration = maxIter, Counts = counts };
        }
    }
}