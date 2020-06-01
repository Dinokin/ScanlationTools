using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Interfaces;
using ImageMagick;

namespace Dinokin.ScanlationTools.Functions
{
    public class Converter : IFunction
    {
        public string Name { get; } = "Converter";

        public Task<IEnumerable<MagickImage>> DoWork(IEnumerable<MagickImage> images, IEnumerable<(string name, string value)>? arguments = null, IProgress<ushort>? progressReporter = null) => Task.FromResult(
            images.Select(image =>
            {
                progressReporter?.Report(1);
                return image;
            }));
    }
}