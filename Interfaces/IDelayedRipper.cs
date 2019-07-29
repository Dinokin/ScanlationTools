using System;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Interfaces
{
    public interface IDelayedRipper
    {
        Task<MagickImage[]> GetImages(Uri uri, TimeSpan waitTimeInSeconds);
    }
}