using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Interfaces
{
    public interface IFunction : INameable
    {
        public Task<IEnumerable<MagickImage>> DoWork(IEnumerable<MagickImage> images, IEnumerable<(string name, string value)>? arguments = null, IProgress<ushort>? progressReporter = null);
    }
}