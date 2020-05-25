using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Others;
using ImageMagick;

namespace Dinokin.ScanlationTools.Functions
{
    public static class ImageDealer
    {
        public static async Task<IEnumerable<MagickImage>> FetchImages(IEnumerable<FileInfo> files) => 
            await Task.WhenAll(files.OrderBy(file => file.Name).Where(file => ParseFormat(file.Extension) != OutputFormats.None)
                .Select(file => Task.Run(() => new MagickImage(file))));
        
        public static async Task SaveImages(IEnumerable<MagickImage> images, OutputFormats format, DirectoryInfo location) =>
            await Task.WhenAll(images.Select((image, iterator) => Task.Run(() =>
            {
                string fileName;
                MagickFormat finalFormat;

                image.Quality = 100;
                image.Alpha(AlphaOption.Remove);
                image.BackgroundColor = MagickColor.FromRgb(255, 255, 255);
                if (image.DetermineColorType() == ColorType.Grayscale) image.Grayscale();

                if (iterator < 9)
                    fileName = $"00{iterator + 1}";
                else if (iterator < 99)
                    fileName = $"0{iterator + 1}";
                else
                    fileName = $"{iterator + 1}";

                switch (format)
                {
                    case OutputFormats.JPG:
                        finalFormat = MagickFormat.Jpg;
                        fileName += ".jpg";
                        break;
                    case OutputFormats.PNG:
                        finalFormat = image.ColorType == ColorType.Grayscale ? MagickFormat.Png8 : MagickFormat.Png24;
                        fileName += ".png";
                        break;
                    case OutputFormats.PSD:
                        finalFormat = MagickFormat.Psd;
                        fileName += ".psd";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(format), format, null);
                }

                image.Write($"{location}{Path.DirectorySeparatorChar}{fileName}", finalFormat);
            })));

        private static OutputFormats ParseFormat(string format) => format.ToLower() switch
        {
            ".jpeg" => OutputFormats.JPG,
            ".jpg" => OutputFormats.JPG,
            ".png" => OutputFormats.PNG,
            ".psd" => OutputFormats.PSD,
            _ => OutputFormats.None
        };
    }
}