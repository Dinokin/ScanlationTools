using System;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Interfaces
{
    public interface IRipper
    {
        Task<MagickImage[]> GetImages(Uri uri);
    }
}