using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Others
{
    public static class ImageIO
    {
        public static Task<IEnumerable<MagickImage>> FetchImages(IEnumerable<FileInfo> files, IProgress<ushort>? progressReporter = null) => Task.FromResult(
            files.OrderBy(file => file.Name).Where(file => ParseFormat(file.Extension) != SelectableOutputFormats.None)
                .Select(file => 
                { 
                    var image = new MagickImage(file);
                    progressReporter?.Report(1);

                    return image;
                }));
        
        public static async Task SaveImages(IEnumerable<MagickImage> images, SelectableOutputFormats format, DirectoryInfo location, IProgress<ushort>? progressReporter = null) => await Task.WhenAll(
            images.Select((image, iterator) => Task.Run(() =>
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
                    case SelectableOutputFormats.JPG:
                        finalFormat = MagickFormat.Jpg;
                        fileName += ".jpg";
                        break;
                    case SelectableOutputFormats.PNG:
                        finalFormat = image.ColorType == ColorType.Grayscale ? MagickFormat.Png8 : MagickFormat.Png24;
                        fileName += ".png";
                        break;
                    case SelectableOutputFormats.PSD:
                        finalFormat = MagickFormat.Psd;
                        fileName += ".psd";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(format), format, null);
                }

                image.Write($"{location}{Path.DirectorySeparatorChar}{fileName}", finalFormat);
                progressReporter?.Report(1);
            })));
        
        private static SelectableOutputFormats ParseFormat(string format) => format.ToLower() switch
        {
            ".jpeg" => SelectableOutputFormats.JPG,
            ".jpg" => SelectableOutputFormats.JPG,
            ".png" => SelectableOutputFormats.PNG,
            ".psd" => SelectableOutputFormats.PSD,
            _ => SelectableOutputFormats.None
        };
    }
}