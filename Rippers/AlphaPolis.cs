using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Interfaces;
using ImageMagick;

namespace Dinokin.ScanlationTools.Rippers
{
    public class AlphaPolis : IRipper
    {
        public string Name { get; } = "AlphaPolis";

        private readonly HttpClient _httpClient;

        public AlphaPolis(IHttpClientFactory httpClientFactory) => _httpClient = httpClientFactory.CreateClient();

        public async Task<IEnumerable<MagickImage>> RipPages(IEnumerable<Uri> pagesAddresses, IProgress<ushort>? progressReporter = null) => await Task.WhenAll(
            pagesAddresses.Select(async address =>
            {
                var image = new MagickImage(await _httpClient.GetByteArrayAsync(address));
                progressReporter?.Report(1);

                return image;
            }));
        
        public async Task<IEnumerable<Uri>> GetPagesAddresses(Uri address, IProgress<ushort>? progressReporter = null) =>
            Regex.Matches(await _httpClient.GetStringAsync(address), "_pages[.]push.*[.]jpg").Select(match =>
            {
                var startIndex = match.Value.IndexOf("\"", StringComparison.InvariantCulture) + 1;
                var length = match.Value.LastIndexOf("/", StringComparison.InvariantCulture) - (startIndex - 1);
                progressReporter?.Report(1);

                return new Uri($"{match.Value.Substring(startIndex, length)}1080x1536.jpg");
            });

        public bool IsValidURI(Uri address) => Regex.IsMatch(address.AbsoluteUri, "^https://www.alphapolis.co.jp/manga/official/[0-9]{9}/[0-9]{1,4}$");
    }
}