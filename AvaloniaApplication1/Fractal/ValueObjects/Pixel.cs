namespace Fractal.ValueObjects;

public struct Pixel
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }

    public Pixel(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }
}