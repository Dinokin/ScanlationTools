using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Tools
{
    public static class WebtoonJoiner
    {
        public static async Task<IList<MagickImage>> Join(IList<MagickImage> images, ushort imagesPerPage)
        {
            var pageGroup = new List<MagickImage>();
            var finishedPages = new List<Task<MagickImage>>();

            for (var i = 1; i <= images.Count; i++)
            {
                pageGroup.Add(images[i - 1]);

                if (i % imagesPerPage == 0 || i == images.Count)
                {
                    var pageCopy = pageGroup.ToList();

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

                    pageGroup.Clear();
                }
            }

            return await Task.WhenAll(finishedPages);
        }
    }
}