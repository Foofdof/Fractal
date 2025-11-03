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
            case FractalType.Multibrot3:
                return new FractalMultibrot3();
            case FractalType.Newton3:
                return new FractalNewton3();
            default:
                throw new ArgumentOutOfRangeException(nameof(fractalType), $"Неизвестный тип фрактала: {fractalType}");
        }
    }
}