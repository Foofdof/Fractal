using Fractal.Abstractions;
using Fractal.Entities.Fractals;
using System;
using Fractal.Types;

namespace Fractal.Factories;

public class FractalFactory
{
    public static IFractal Generate(FractalType fractalType)
    {
        switch (fractalType)
        {
            case FractalType.Mandelbrot:
                return new FractalMandelbrot();
            case FractalType.BurningShip:
                return new FractalBurningShip();
            default:
                throw new ArgumentOutOfRangeException(nameof(fractalType), $"Неизвестный тип фрактала: {fractalType}");
        }
    }
}