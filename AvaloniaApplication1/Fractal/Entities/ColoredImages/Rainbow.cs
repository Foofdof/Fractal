using Fractal.Entities.Base;

namespace Fractal.Entities.ColoredImages;

public class Rainbow : ColoredImageBase
{
    // чуть мягче по тону
    protected override double Gamma => 0.8;

    protected override Pixel FromT(double t)
    {
        // Радуга: идём по оттенку от 0 до 270°
        double h = 270.0 * t;
        double s = 1.0;
        double v = t;           // чтобы центр был светлее
        var rgb = HsvToRgb(h, s, v);
        return ToPixel(rgb);
    }
}