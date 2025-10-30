using System;
using System.IO;
using System.Linq;

namespace Fractal.Entities.Writers;

public class PPMWriter
{
    public static void Save(Image image, string path)
    {
        if (image == null) throw new ArgumentNullException(nameof(image));
        if (image.Pixels == null || image.Pixels.Count == 0)
            throw new ArgumentException("Image.Pixels пуст.", nameof(image));

        int height = image.Pixels.Count;
        int width  = image.Pixels[0].Count;
        if (width == 0) throw new ArgumentException("Image.Pixels содержит пустые строки.", nameof(image));
        if (image.Pixels.Any(row => row == null || row.Count != width))
            throw new ArgumentException("Image.Pixels имеет строки разной длины.", nameof(image));

        var fullPath = Path.GetFullPath(path);
        var dir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        using var bw = new BinaryWriter(fs);

        // Заголовок PPM (P6)
        var header = $"P6\n{width} {height}\n255\n";
        bw.Write(System.Text.Encoding.ASCII.GetBytes(header));

        // Данные RGB: сверху вниз, слева направо
        for (int y = 0; y < height; y++)
        {
            var row = image.Pixels[y];
            for (int x = 0; x < width; x++)
            {
                // Ожидается Pixel с свойствами R,G,B (byte)
                var p = row[x];
                bw.Write(p.R);
                bw.Write(p.G);
                bw.Write(p.B);
            }
        }
    }
}