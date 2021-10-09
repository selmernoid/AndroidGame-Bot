using System;
using System.Drawing;
using System.Threading.Tasks;

namespace InfinitodeBot
{
    public class ImageProcessor
    {
        public readonly int width, height;
        protected Pixel[][] Pixels { get; set; }

        public ImageProcessor(Image template)
        {
            using var bitmap = new Bitmap(template);
            width = bitmap.Width;
            height = bitmap.Height;
            Pixels = ImageToColorMatrix(bitmap);
        }

        static object lockObj = new object();
        public Point FindPosition(Image original, Point? minPoint = null, Point? maxPoint = null) => GetBestPosition(original, minPoint, maxPoint).Value;
        public Point? TryFindPosition(Image original, Point? minPoint = null, Point? maxPoint = null, decimal tolerancePercent = 1) => GetBestPosition(original, minPoint, maxPoint, Decimal.ToInt32(width * height * 255 * tolerancePercent / 100));
        
        protected Point? GetBestPosition(Image original, Point? minPoint = null, Point? maxPoint = null, int tolerance = int.MaxValue)
        {

            using var bmp = new Bitmap(original);
            var originalPixels = ImageToColorMatrix(bmp);

            Point? pos = null;
            var min = tolerance;

            var maxX = original.Width;
            var maxY = original.Height;
            if (maxPoint.HasValue)
            {
                maxX = Math.Min(maxX, maxPoint.Value.X);
                maxY = Math.Min(maxY, maxPoint.Value.Y);
            }
            maxX -= width;
            maxY -= height;
            for (int x = minPoint?.X ?? 0; x < maxX; x++)
            {
                Parallel.For(minPoint?.Y ?? 0, maxY, (y, state) =>
                //for (int y = 0; y < bmp.Height - height; y++)
                {
                    if (state.IsStopped)
                        return;
                    var diff = GetMatch(originalPixels, x, y, min);
                    if (diff == 0 || min > diff)
                    {
                        lock (lockObj)
                        {
                            if (diff == 0 || min > diff)
                            {
                                min = diff;
                                pos = new Point(x, y);
                                if (min == 0)
                                    state.Stop();
                            }
                        }
                    }
                }
                );
            }

            return !pos.HasValue ?
                (Point?)null :
                new Point(pos.Value.X + width / 2, pos.Value.Y + height / 2);
        }

        protected int GetMatch(Pixel[][] originalPixels, int x_, int y_, int min)
        {
            var diff = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Pixel pxl = originalPixels[x + x_][y + y_];
                    Pixel pxl2 = Pixels[x][y];
                    diff += pxl.GetDifference(pxl2);
                }
                if (min < diff)
                    return int.MaxValue;
            }
            return diff;
        }

        public static Pixel[][] ImageToColorMatrix(Bitmap bmp)
        {
            var result = new Pixel[bmp.Width][];

            for (int x = 0; x < bmp.Width; x++)
            {
                result[x] = new Pixel[bmp.Height];
                for (int y = 0; y < bmp.Height; y++)
                {
                    result[x][y] = new Pixel(bmp.GetPixel(x, y));
                }
            }

            return result;
        }

        public struct Pixel
        {
            byte Value;
            public Pixel(Color original)
            {
                Value = (byte)((original.R * 299 + original.G * 587 + original.B * 114) / 1000);
            }
            public int GetDifference(Pixel other) => Math.Abs(Value - other.Value);

            public static bool operator ==(Pixel one, Pixel two) => one.Value == two.Value;
            public static bool operator !=(Pixel one, Pixel two) => !(one == two);
        }
    }
}
