using Fractal.Entities;

namespace Fractal.ValueObjects;

public struct BuildingFractalParamsDto
{
    public ImageBox? Box { get; init; }
    public int? MaxIterations { get; init; }
}
