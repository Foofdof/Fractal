using System;

namespace Fractal.Numerics;

public readonly struct DecimalComplex
{
    public decimal Real { get; }
    public decimal Imag { get; }

    public static readonly DecimalComplex Zero = new(0m, 0m);

    public DecimalComplex(decimal real, decimal imag)
    {
        Real = real;
        Imag = imag;
    }

    public decimal MagnitudeSquared => Real * Real + Imag * Imag;

    public DecimalComplex Square()
    {
        // (x + i y)^2 = (x^2 - y^2) + i(2xy)
        var xx = Real * Real;
        var yy = Imag * Imag;
        var xy = Real * Imag;
        return new DecimalComplex(xx - yy, 2m * xy);
    }
    
    public DecimalComplex AbsComponents()
        => new(Math.Abs(Real), Math.Abs(Imag));
    
    public static DecimalComplex operator +(in DecimalComplex a, in DecimalComplex b)
        => new(a.Real + b.Real, a.Imag + b.Imag);

    public static DecimalComplex operator -(in DecimalComplex a, in DecimalComplex b)
        => new(a.Real - b.Real, a.Imag - b.Imag);

    public static DecimalComplex operator *(in DecimalComplex a, in DecimalComplex b)
        => new(
            a.Real * b.Real - a.Imag * b.Imag,
            a.Real * b.Imag + a.Imag * b.Real
        );

    public override string ToString() => $"{Real} {(Imag < 0 ? '-' : '+')} {decimal.Abs(Imag)}i";
}