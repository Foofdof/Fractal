using Fractal.ValueObjects;

namespace Fractal.Entities;

/// <summary>
/// TODO: дописать!
/// </summary>
public class ImageBox
{
    public (int Nx, int Ny) Screen;
    public Box2D Box;

    public ImageBox((int Nx, int Ny) screen, Box2D box)
    {
        Screen = screen;
        Box = box;
    }
}