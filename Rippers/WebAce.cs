using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ImageMagick;
using Newtonsoft.Json;

namespace Dinokin.ScanlationTools.Rippers
{
    public class WebAce
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<IList<MagickImage>> GetImages(Uri uri) =>
            await Task.WhenAll(JsonConvert.DeserializeObject<string[]>(await _httpClient.GetStringAsync($"{uri.AbsoluteUri}/json/"))
                .Select(image => Task.Run(async () => new MagickImage(await _httpClient.GetByteArrayAsync($"https://web-ace.jp{image}")))));
    }
}