using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Interfaces;
using ImageMagick;

namespace Dinokin.ScanlationTools.Functions
{
    public class PageJoiner : IFunction
    {
        public string Name { get; } = "PageJoiner";
        
        public async Task<IEnumerable<MagickImage>> DoWork(IEnumerable<MagickImage> images, IEnumerable<(string name, string value)>? arguments = null, IProgress<ushort>? progressReporter = null)
        {
            if (arguments == null || !arguments.Any(arg => arg.name == "pagesPerPage" && arg.value != null) || !ushort.TryParse(arguments.Single(arg => arg.name == "pagesPerPage").value, out var pagesPerPage))
                throw new ArgumentException();

            var imageArray = images.ToArray();
            var pageGroup = new List<MagickImage>();
            var finishedPages = new List<Task<MagickImage>>();

            for (var i = 1; i <= imageArray.Length; i++)
            {
                pageGroup.Add(imageArray[i - 1]);

                if (i % pagesPerPage == 0 || i == imageArray.Length)
                {
                    var pageCopy = pageGroup.ToArray();

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

                        progressReporter?.Report(1);
                        return finalImage;
                    }));

                    pageGroup.Clear();
                }
            }

            return await Task.WhenAll(finishedPages);
        }
    }
}