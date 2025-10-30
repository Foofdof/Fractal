namespace Fractal.ValueObjects;

/// <summary>
/// Задаёт область точек для расчётов в терминах (xMin, xMax, yMin, yMax).
/// </summary>
public struct Box2D
{
    public decimal Xmin { get; init; }
    public decimal Xmax { get; init; }
    public decimal Ymin { get; init; }
    public decimal Ymax { get; init; }
}