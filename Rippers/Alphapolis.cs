using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Interfaces;
using ImageMagick;

namespace Dinokin.ScanlationTools.Rippers
{
    public class Alphapolis : IRipper
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<MagickImage[]> GetImages(Uri uri)
        {
            var downloadTasks = Regex.Matches(await _httpClient.GetStringAsync(uri), "_pages[.]push.*[.]jpg")
                .Select(match => match.Value.Substring(match.Value.IndexOf("\"", StringComparison.Ordinal) + 1,
                    match.Value.LastIndexOf("/", StringComparison.Ordinal) - match.Value.IndexOf("\"", StringComparison.Ordinal)) + "1080x1536.jpg")
                .Select(data => Task.Run(async () => new MagickImage(await _httpClient.GetByteArrayAsync(data)))).ToArray();

            Task.WaitAll(downloadTasks);

            return downloadTasks.Select(result => result.Result).ToArray();
        }
    }
}