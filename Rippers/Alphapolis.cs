using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ImageMagick;

namespace Dinokin.ScanlationTools.Rippers
{
    public class Alphapolis
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<MagickImage[]> GetImages(Uri uri) =>
            await Task.WhenAll(Regex.Matches(await _httpClient.GetStringAsync(uri), "_pages[.]push.*[.]jpg")
                .Select(match => match.Value.Substring(match.Value.IndexOf("\"", StringComparison.InvariantCulture) + 1,
                                     match.Value.LastIndexOf("/", StringComparison.InvariantCulture) - match.Value.IndexOf("\"", StringComparison.InvariantCulture)) + "1080x1536.jpg")
                .Select(data => Task.Run(async () => new MagickImage(await _httpClient.GetByteArrayAsync(data)))));
    }
}