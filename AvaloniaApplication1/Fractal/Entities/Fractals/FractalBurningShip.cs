using System;
using Fractal.Entities.Base;

namespace Fractal.Entities.Fractals;

public sealed class FractalBurningShip : BaseFractal
{
    protected override void IterateCore(ref decimal zx, ref decimal zy, decimal cx, decimal cy)
    {
        decimal ax = Math.Abs(zx);
        decimal ay = Math.Abs(zy);

        decimal ax2 = ax * ax;
        decimal ay2 = ay * ay;
        decimal axy = ax * ay;

        zx = ax2 - ay2 + cx;
        zy = 2m * axy + cy;
    }
}