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
    public class ComicRider : IRipper
    {
        private struct BlockCoordinates
        {
            public int StartX { get; }
            public int StartY { get; }
            public int CropSizeX { get; }
            public int CropSizeY { get; }
            public int DestinationX { get; }
            public int DestinationY { get; }

            public BlockCoordinates(string info)
            {
                info = info.Remove(0, 2);

                StartX = int.Parse(Regex.Match(info, @"^[0-9]+").Value);
                info = Regex.Replace(info, @"^[0-9]+,", string.Empty);

                StartY = int.Parse(Regex.Match(info, @"^[0-9]+").Value);
                info = Regex.Replace(info, @"^[0-9]+\+", string.Empty);

                CropSizeX = int.Parse(Regex.Match(info, @"^[0-9]+").Value);
                info = Regex.Replace(info, @"^[0-9]+,", string.Empty);

                CropSizeY = int.Parse(Regex.Match(info, @"^[0-9]+").Value);
                info = Regex.Replace(info, @"^[0-9]+>", string.Empty);

                DestinationX = int.Parse(Regex.Match(info, @"^[0-9]+").Value);
                info = Regex.Replace(info, @"^[0-9]+,", string.Empty);

                DestinationY = int.Parse(Regex.Match(info, @"^[0-9]+").Value);
            }
        }

        private struct ImageRecipe
        {
            [JsonProperty("resources")]
            public Contents Resources { get; set; }
            [JsonProperty("views")]
            public List<View> Views { get; set; }

            public struct Image
            {
                [JsonProperty("src")]
                public string Source { get; set; }
                [JsonProperty("width")]
                public int Width { get; set; }
                [JsonProperty("height")]
                public int Height { get; set; }
            }

            public struct Contents
            {
                [JsonProperty("i")]
                public Image Image { get; set; }
            }

            public struct View
            {
                [JsonProperty("width")]
                public int Width { get; set; }
                [JsonProperty("height")]
                public int Height { get; set; }
                [JsonProperty("coords")]
                public List<string> Coordinates { get; set; }
            }
        }
        
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly HtmlWeb _htmlWeb = new HtmlWeb();

        public async Task<MagickImage[]> GetImages(Uri uri)
        {
            var downloadTasks = (await _htmlWeb.LoadFromWebAsync(uri.AbsoluteUri)).DocumentNode.Descendants("div").Where(element => element.Attributes.Contains("data-ptimg"))
                .Select(attribute => attribute.GetAttributeValue("data-ptimg", string.Empty)).Select(value => Task.Run(async () =>
                {
                    var imageRecipe = JsonConvert.DeserializeObject<ImageRecipe>(await _httpClient.GetStringAsync($"{uri.AbsoluteUri}{value}"));
                    var image = new MagickImage(await _httpClient.GetByteArrayAsync($"{uri.AbsoluteUri}data/{imageRecipe.Resources.Image.Source}"));
                    var reImage = new MagickImage(MagickColors.White, imageRecipe.Views[0].Width,  imageRecipe.Views[0].Height);

                    foreach (var coordinateSet in imageRecipe.Views[0].Coordinates)
                    {
                        var block = new BlockCoordinates(coordinateSet);
                        
                        reImage.Composite(image.Clone(block.StartX, block.StartY, block.CropSizeX, block.CropSizeY), block.DestinationX, block.DestinationY);
                    }

                    return reImage;
                })).ToArray();

            Task.WaitAll(downloadTasks);
            
            return downloadTasks.Select(result => result.Result).ToArray();
        }
    }
}