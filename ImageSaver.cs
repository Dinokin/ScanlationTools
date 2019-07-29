using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools
{
    public static class ImageSaver
    {
        public static void SaveImages(MagickImage[] images, MagickFormat format, DirectoryInfo location)
        {
            var writeTasks = new List<Task>();

            for (var i = 0; i < images.Length; i++)
            {
                string fileName;
                MagickFormat finalFormat = format;
                
                images[i].Alpha(AlphaOption.Remove);
                images[i].BackgroundColor = MagickColor.FromRgb(255, 255, 255);
                if (images[i].DetermineColorType() == ColorType.Grayscale)
                    images[i].Grayscale();
                
                if (i < 9)
                    fileName = $"00{i + 1}";
                else if (i < 99)
                    fileName = $"0{i + 1}";
                else
                    fileName = $"{i + 1}";

                switch (format)
                {
                    case MagickFormat.Jpg:
                        fileName += ".jpg";
                        break;
                    case MagickFormat.Png8:
                        finalFormat = images[i].ColorType == ColorType.Grayscale ? MagickFormat.Png8 : MagickFormat.Png24;
                        fileName += ".png";
                        break;
                    case MagickFormat.Psd:
                        fileName += ".psd";
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                
                var iterator = i;
                writeTasks.Add(Task.Run(() => images[iterator].Write($"{location.FullName}{Path.DirectorySeparatorChar}{fileName}", finalFormat)));
            }

            Task.WaitAll(writeTasks.ToArray());
        }
    }
}