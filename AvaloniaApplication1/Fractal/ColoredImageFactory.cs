using Fractal.Abstractions;
using Fractal.Entities.ColoredImages;
using Fractal.Enums;

namespace Fractal.Factories;

public class ColoredImageFactory
{
    public static IColoredImage Create(ColorImageType colorImage)
    {
        switch (colorImage)
        {
            case ColorImageType.Fire:
                return new Fire();
            case ColorImageType.GrayScale:
                return new GrayScale();
            case ColorImageType.Ocean:
                return new Ocean();
            case ColorImageType.Rainbow:
                return new Rainbow();
            default:
                return new GrayScale();
        }
    }
}