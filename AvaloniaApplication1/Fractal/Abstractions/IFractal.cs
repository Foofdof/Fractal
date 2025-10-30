using Fractal.Entities;
using Fractal.ValueObjects;

namespace Fractal.Abstractions;

public interface IFractal
{
    FractalData Generate(ImageBox? box, int? maxIterations);
}
