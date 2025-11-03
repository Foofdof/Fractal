using Fractal.Entities.Base;

namespace Fractal.Entities.Fractals
{
    public sealed class FractalMultibrot3 : BaseFractal
    {
        protected override void IterateCore(ref decimal zx, ref decimal zy, decimal cx, decimal cy)
        {
            decimal x = zx, y = zy;
            decimal x2 = x * x;
            decimal y2 = y * y;
            
            zx = x * (x2 - 3m * y2) + cx;
            zy = y * (3m * x2 - y2) + cy;
        }
    }
}