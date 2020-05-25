using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Functions
{
    public static class WebtoonPageJoiner
    {
        public static async Task<IEnumerable<MagickImage>> JoinPages(IEnumerable<MagickImage> images, ushort imagesPerPage)
        {
            var imageArray = images.ToArray();
            var pageGroup = new List<MagickImage>();
            var finishedPages = new List<Task<MagickImage>>();

            for (var i = 1; i <= imageArray.Length; i++)
            {
                pageGroup.Add(imageArray[i - 1]);

                if (i % imagesPerPage == 0 || i == imageArray.Length)
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