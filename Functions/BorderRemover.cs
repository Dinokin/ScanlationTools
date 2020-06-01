using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Interfaces;
using ImageMagick;

namespace Dinokin.ScanlationTools.Functions
{
    public class BorderRemover : IFunction
    {
        public string Name { get; } = "BorderRemover";

        public async Task<IEnumerable<MagickImage>> DoWork(IEnumerable<MagickImage> images, IEnumerable<(string name, string value)>? arguments = null, IProgress<ushort>? progressReporter = null) => 
            await Task.WhenAll(images.Select(image => Task.Run(async () =>
            {
                if (image.HasAlpha)
                {
                    var tasks = new Task[2];
                    var pixels = image.GetPixels();
                    Pixel? firstPixelX = null;
                    Pixel? lastPixelX = null;
                    Pixel? firstPixelY = null;
                    Pixel? lastPixelY = null;


                    foreach (var pixel in pixels)
                    {
                        if (pixel.GetChannel(3) != 255)
                        {
                            for (var i = 0; i < pixel.Channels; i++)
                                pixel.SetChannel(i, 0);
                        }
                    }

                    tasks[0] = Task.Run(() =>
                    {
                        for (var i = 0; i < image.Width; i++)
                        {
                            var targetPixel = pixels.GetPixel(i, image.Height / 2);

                            if (targetPixel.GetChannel(3) == 255 && (i == 0 || pixels.GetPixel(i - 1, image.Height / 2).GetChannel(3) == 0))
                                firstPixelX = targetPixel;
                            else if (targetPixel.GetChannel(3) == 255 && (i == image.Width - 1 || pixels.GetPixel(i + 1, image.Height / 2).GetChannel(3) == 0))
                                lastPixelX = targetPixel;
                        }
                    });

                    tasks[1] = Task.Run(() =>
                    {
                        for (var i = 0; i < image.Height; i++)
                        {
                            var targetPixel = pixels.GetPixel(image.Width / 2, i);

                            if (targetPixel.GetChannel(3) == 255 && (i == 0 || pixels.GetPixel(image.Width / 2, i - 1).GetChannel(3) == 0))
                                firstPixelY = targetPixel;
                            else if (targetPixel.GetChannel(3) == 255 && (i == image.Height - 1 || pixels.GetPixel(image.Width / 2, i + 1).GetChannel(3) == 0))
                                lastPixelY = targetPixel;
                        }
                    });

                    await Task.WhenAll(tasks);

                    if (firstPixelX != null && lastPixelX != null && firstPixelY != null && lastPixelY != null)
                    {
                        image.Crop(new MagickGeometry(firstPixelX.X, firstPixelY.Y, lastPixelX.X - firstPixelX.X, lastPixelY.Y - firstPixelY.Y));
                        image.RePage();
                    }
                }

                progressReporter?.Report(1);
                return image;
            })));
    }
}