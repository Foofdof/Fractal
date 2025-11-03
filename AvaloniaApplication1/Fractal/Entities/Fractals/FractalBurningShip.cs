using Fractal.Entities.Base;
using Fractal.Numerics;

namespace Fractal.Entities.Fractals;

public sealed class FractalBurningShip : BaseFractal
{
    protected override DecimalComplex Iterate(in DecimalComplex z, in DecimalComplex c)
        => z.AbsComponents().Square() + c;
}