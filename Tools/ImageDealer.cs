using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Tools
{
    public static class ImageDealer
    {
        public enum SupportedFormats
        {
            Jpg,
            Png,
            Psd,
            Unknown
        }

        public static async Task<IList<MagickImage>> FetchImages(DirectoryInfo origin) => 
            await Task.WhenAll(origin.GetFiles().Where(file => ParseFormat(file.Extension) != SupportedFormats.Unknown)
                .Select(file => Task.Run(() => new MagickImage(file))));
        
        public static async Task SaveImages(IList<MagickImage> images, SupportedFormats format, DirectoryInfo location) =>
            await Task.WhenAll(images.Select((image, iterator) => iterator).Select(iterator => Task.Run(() =>
            {
                string fileName;
                MagickFormat finalFormat;

                images[iterator].Alpha(AlphaOption.Remove);
                images[iterator].BackgroundColor = MagickColor.FromRgb(255, 255, 255);
                if (images[iterator].DetermineColorType() == ColorType.Grayscale) images[iterator].Grayscale();

                if (iterator < 9)
                    fileName = $"00{iterator + 1}";
                else if (iterator < 99)
                    fileName = $"0{iterator + 1}";
                else
                    fileName = $"{iterator + 1}";

                switch (format)
                {
                    case SupportedFormats.Jpg:
                        finalFormat = MagickFormat.Jpg;
                        fileName += ".jpg";
                        break;
                    case SupportedFormats.Png:
                        finalFormat = images[iterator].ColorType == ColorType.Grayscale ? MagickFormat.Png8 : MagickFormat.Png24;
                        fileName += ".png";
                        break;
                    case SupportedFormats.Psd:
                        finalFormat = MagickFormat.Psd;
                        fileName += ".psd";
                        break;
                    default:
                        throw new FormatException("The selected format is not supported.");
                }

                images[iterator].Write($"{location.FullName}{Path.DirectorySeparatorChar}{fileName}", finalFormat);
            })));

        public static SupportedFormats ParseFormat(string format)
        {
            switch (format.ToLower())
            {
                case "jpg":
                    return SupportedFormats.Jpg;
                case "png":
                    return SupportedFormats.Png;
                case "psd":
                    return SupportedFormats.Psd;
                case ".jpg":
                    return SupportedFormats.Jpg;
                case ".png":
                    return SupportedFormats.Png;
                case ".psd":
                    return SupportedFormats.Psd;
                default:
                    return SupportedFormats.Unknown;
            }
        }
    }
}