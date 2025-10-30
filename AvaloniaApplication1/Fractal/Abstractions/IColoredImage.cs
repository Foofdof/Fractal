using Fractal.Entities;
using Fractal.ValueObjects;

namespace Fractal.Abstractions;

public interface IColoredImage
{
    Image Create(FractalData value);
}
