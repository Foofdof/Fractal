using Fractal.Entities.Base;

namespace Fractal.Entities.ColoredImages;

public class Ocean : ColoredImageBase
{
    protected override double Gamma => 0.85;

    protected override Pixel FromT(double t)
    {
        // тёмно-синий -> синий -> бирюзовый -> белый
        var c0 = (0.0,   10.0,  40.0);  // #000A28
        var c1 = (0.0,   72.0,  255.0); // #0048FF
        var c2 = (0.0,   229.0, 255.0); // #00E5FF
        var c3 = (255.0, 255.0, 255.0); // белый

        (double r, double g, double b) col =
            t < 0.33 ? Lerp(c0, c1, t / 0.33) :
            t < 0.66 ? Lerp(c1, c2, (t - 0.33) / 0.33) :
            Lerp(c2, c3, (t - 0.66) / 0.34);

        return ToPixel(col);
    }
}