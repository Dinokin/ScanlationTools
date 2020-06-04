using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Dinokin.ScanlationTools.Rippers
{
    public partial class ComicRide
    {
        private readonly struct ImageBlock
        {
            public int StartX { get; }
            public int StartY { get; }
            public int CropSizeX { get; }
            public int CropSizeY { get; }
            public int DestinationX { get; }
            public int DestinationY { get; }

            public ImageBlock(string info)
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
            public struct Resource
            {
                [JsonProperty("i")] public Image Image { get; set; }
            }

            public struct Image
            {
                [JsonProperty("src")] public string Source { get; set; }
                [JsonProperty("width")] public int Width { get; set; }
                [JsonProperty("height")] public int Height { get; set; }
            }

            public struct View
            {
                [JsonProperty("width")] public int Width { get; set; }
                [JsonProperty("height")] public int Height { get; set; }
                [JsonProperty("coords")] public List<string> Coordinates { get; set; }
            }
            
            [JsonProperty("resources")] public Resource Resources { get; set; }
            [JsonProperty("views")] public List<View> Views { get; set; }
        }
    }
}