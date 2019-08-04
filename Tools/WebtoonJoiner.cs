using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Tools
{
    public static class WebtoonJoiner
    {
        public static async Task<MagickImage[]> Join(DirectoryInfo origin, ushort imagesPerPage)
        {
            var images = origin.GetFiles().Where(file => ImageSaver.ParseFormat(file.Extension) != ImageSaver.SupportedFormats.Unknown).Select(file => new MagickImage(file)).ToArray();
            var page = new List<MagickImage>();
            var finishedPages = new List<Task<MagickImage>>();

            for (var i = 1; i <= images.Length; i++)
            {
                page.Add(images[i - 1]);

                if (i % imagesPerPage == 0 || i == images.Length)
                {
                    var pageCopy = page.ToArray();

                    finishedPages.Add(Task.Run(() =>
                    {
                        var imageX = pageCopy[0].Width;
                        var imageY = pageCopy.Sum(image => image.Height);

                        var currentHeight = 0;
                        var finalImage = new MagickImage(MagickColors.White, imageX, imageY);

                        foreach (var image in pageCopy)
                        {
                            finalImage.Composite(image, 0, currentHeight);
                            currentHeight += image.Height;
                        }

                        return finalImage;
                    }));

                    page.Clear();
                }
            }

            return await Task.WhenAll(finishedPages);
        }
    }
}