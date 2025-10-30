using Fractal.Entities.Base;
using Fractal.Entities.ColoredImages;

namespace Fractal.Entities.ColoredImages;

public class GrayScale : ColoredImageBase
{
    // немного усилим тёмные детали
    protected override double Gamma => 0.8;

    protected override Pixel FromT(double t)
    {
        byte v = (byte)System.Math.Round(255 * t);
        return new Pixel(v, v, v);
    }
}