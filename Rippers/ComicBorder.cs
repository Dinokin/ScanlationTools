using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Interfaces;
using HtmlAgilityPack;
using ImageMagick;
using Newtonsoft.Json;

namespace Dinokin.ScanlationTools.Rippers
{
    public partial class ComicBorder : IRipper
    {
        public string Name { get; } = "Comic Border";

        private readonly HttpClient _httpClient;

        public ComicBorder(IHttpClientFactory httpClientFactory) => _httpClient = httpClientFactory.CreateClient();

        public async Task<IEnumerable<MagickImage>> RipPages(IEnumerable<Uri> pagesAddresses, IProgress<ushort>? progressReporter = null) => await Task.WhenAll(
            pagesAddresses.Select(async address =>
            {
                const int numberOfVerticalBlocks = 4, numberOfHorizontalBlocks = 4, horizontalBlockSize = 224, verticalBlockSize = 320;

                var inputImage = new MagickImage(await _httpClient.GetByteArrayAsync(address));
                var outputImage = new MagickImage(inputImage);

                for (var i = 0; i < numberOfVerticalBlocks; i++)
                for (var j = 0; j < numberOfHorizontalBlocks; j++)
                {
                    var blockWidth = j == numberOfHorizontalBlocks - 1 ? inputImage.Width - horizontalBlockSize * j : horizontalBlockSize;
                
                    outputImage.Composite(inputImage.Clone(horizontalBlockSize * j, verticalBlockSize * i, blockWidth, verticalBlockSize), horizontalBlockSize * i, verticalBlockSize * j);
                }
                
                progressReporter?.Report(1);
                return outputImage;
            }));

        public async Task<IEnumerable<Uri>> GetPagesAddresses(Uri address, IProgress<ushort>? progressReporter = null)
        {
            var episode = JsonConvert.DeserializeObject<Episode>(await _httpClient.GetStringAsync($"{address.AbsoluteUri}.json"));

            return episode.ReadableProduct.PageStructure.Pages.Where(page => page.Src != null).Select(page => page.Src);
        }

        public bool IsValidURI(Uri address) => Regex.IsMatch(address.AbsoluteUri, @"^https:\/\/comicborder\.com\/episode\/[0-9]{20}$");
    }
}