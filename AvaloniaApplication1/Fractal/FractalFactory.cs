using Fractal.Abstractions;
using Fractal.Entities.Fractals;
using System;
using Fractal.Types;

namespace Fractal.Factories;

/// <summary>
/// TODO: дописать!
/// </summary>
public class FractalFactory
{
    public static IFractal Generate(FractalType fractalType)
    {
        switch (fractalType)
        {
            case FractalType.Mandelbrot:
                return new FractalMandelbrot();

            default:
                throw new ArgumentOutOfRangeException(nameof(fractalType), $"Неизвестный тип фрактала: {fractalType}");
        }
    }

    //private Box2D ConvertScreenToBox2D(Box2D defaultBox)
    //{

    //}
}
