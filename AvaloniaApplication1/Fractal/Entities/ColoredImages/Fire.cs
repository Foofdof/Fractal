using Fractal.Entities.Base;

namespace Fractal.Entities.ColoredImages;

public class Fire : ColoredImageBase
{
    protected override double Gamma => 0.85;

    protected override Pixel FromT(double t)
    {
        // градиент: чёрный -> тёмно-красный -> оранжевый -> белый
        var c0 = (0.0,   0.0,   0.0);
        var c1 = (139.0, 0.0,   0.0);
        var c2 = (255.0, 165.0, 0.0);
        var c3 = (255.0, 255.0, 255.0);

        (double r, double g, double b) col =
            t < 0.33 ? Lerp(c0, c1, t / 0.33) :
            t < 0.66 ? Lerp(c1, c2, (t - 0.33) / 0.33) :
            Lerp(c2, c3, (t - 0.66) / 0.34);

        return ToPixel(col);
    }
}