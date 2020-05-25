using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Functions
{
    public static class Resizer
    {
        public static async Task<IEnumerable<MagickImage>> Percent(IEnumerable<MagickImage> images, double percent) =>
            await Task.WhenAll(images.Select(image => Task.Run(() =>
            {
                image.Resize(new Percentage(percent));

                return image;
            })));
    }
}