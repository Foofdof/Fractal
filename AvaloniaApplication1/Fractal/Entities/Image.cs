using System.Collections.Generic;
using System.Linq;
using Fractal.ValueObjects;

namespace Fractal.Entities;

public class Image
{
    public List<List<Pixel>> Pixels;
    
    public Image(List<List<Pixel>> pixels)
    {
        Pixels = pixels.Select(row => new List<Pixel>(row)).ToList();
    }
}