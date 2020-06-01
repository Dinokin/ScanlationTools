using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Interfaces;
using ImageMagick;

namespace Dinokin.ScanlationTools.Functions
{
    public class Resizer : IFunction
    {
        public string Name { get; } = "Resizer";

        public async Task<IEnumerable<MagickImage>> DoWork(IEnumerable<MagickImage> images, IEnumerable<(string name, string value)>? arguments = null, IProgress<ushort>? progressReporter = null) =>
            await Task.WhenAll(images.Select(image => Task.Run(() =>
            {
                if (arguments == null || !arguments.Any(arg => arg.name == "percentage" && arg.value != null) || !ushort.TryParse(arguments.Single(arg => arg.name == "percentage").value, out var percentage))
                    throw new ArgumentException();
                
                image.Resize(new Percentage(percentage));

                progressReporter?.Report(1);
                return image;
            })));
    }
}