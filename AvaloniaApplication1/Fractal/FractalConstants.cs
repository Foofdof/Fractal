using Fractal.Entities;
using Fractal.ValueObjects;

namespace Fractal.Constants;

public static class FractalConstants
{
    public static class MandelbrotConstants
    {
        public static readonly Box2D DefaultGeneratingBox = new Box2D()
        {
            Xmax = 1.25M,
            Xmin = -2.25M,

            Ymax = 1.75M,
            Ymin = -1.75M,
        };

        public static readonly ImageBox DefaultMandelbrotImageBox = new ImageBox(FullHD, DefaultGeneratingBox);
        public static double DefaultThreshold = 2.0;
    }

    public static (int Nx, int Ny) FullHD = (1920, 1080);
    public static readonly int MaxIteration = 300;
}
