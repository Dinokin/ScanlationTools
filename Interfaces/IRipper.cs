using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Interfaces
{
    public interface IRipper
    {
        public string Name { get; }
        
        public Task<IEnumerable<MagickImage>> RipPages(IEnumerable<Uri> pagesAddresses, IProgress<ushort>? progressReporter = null);

        public Task<IEnumerable<Uri>> GetPagesAddresses(Uri address, IProgress<ushort>? progressReporter = null);
        
        public bool IsValidURI(Uri address);
    }
}