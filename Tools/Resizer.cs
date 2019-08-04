using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Tools
{
    public static class Resizer
    {
        public static async Task<MagickImage[]> Percent(DirectoryInfo origin, double percent)
        {
            return await Task.WhenAll(origin.GetFiles().Where(file => ImageSaver.ParseFormat(file.Extension) != ImageSaver.SupportedFormats.Unknown)
                .Select(file => new MagickImage(file)).Select(image => Task.Run(() =>
                {
                    image.Resize(new Percentage(percent));

                    return image;
                })));
        }
    }
}