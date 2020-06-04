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
    public partial class ComicRide : IRipper
    {
        public string Name { get; } = "ComicRIDE";

        private readonly HttpClient _httpClient;
        private readonly HtmlWeb _htmlWeb;

        public ComicRide(IHttpClientFactory httpClientFactory, HtmlWeb htmlWeb)
        {
            _httpClient = httpClientFactory.CreateClient();
            _htmlWeb = htmlWeb;
        }

        public async Task<IEnumerable<MagickImage>> RipPages(IEnumerable<Uri> pagesAddresses, IProgress<ushort>? progressReporter = null) => await Task.WhenAll(
            pagesAddresses.Select(async pagesAddress =>
            {
                var dataAddress = Regex.Replace(pagesAddress.AbsoluteUri, "[0-9]{4}\\.ptimg\\.json$", string.Empty);
                var imageRecipe = JsonConvert.DeserializeObject<ImageRecipe>(await _httpClient.GetStringAsync(pagesAddress));
                var image = new MagickImage(await _httpClient.GetByteArrayAsync($"{dataAddress}{imageRecipe.Resources.Image.Source}"));
                var rebuiltImage = new MagickImage(MagickColors.White, imageRecipe.Views[0].Width, imageRecipe.Views[0].Height);

                foreach (var block in imageRecipe.Views[0].Coordinates.Select(coordinateSet => new ImageBlock(coordinateSet)))
                    rebuiltImage.Composite(image.Clone(block.StartX, block.StartY, block.CropSizeX, block.CropSizeY), block.DestinationX, block.DestinationY);
                
                progressReporter?.Report(1);

                return rebuiltImage;
            }));

        public async Task<IEnumerable<Uri>> GetPagesAddresses(Uri address, IProgress<ushort>? progressReporter = null) =>
            (await _htmlWeb.LoadFromWebAsync(address.AbsoluteUri)).DocumentNode.Descendants("div").Where(element => element.Attributes.Contains("data-ptimg"))
            .Select(attribute =>
            {
                var pagesAddress = new Uri($"{address.AbsoluteUri}{attribute.GetAttributeValue("data-ptimg", string.Empty)}");
                progressReporter?.Report(1);

                return pagesAddress;
            });

        public bool IsValidURI(Uri address) => Regex.IsMatch(address.AbsoluteUri, @"^http:\/\/comicride\.jp\/viewer\/[a-z]+\/[a-z]+_[0-9]{3}\/$");
    }
}