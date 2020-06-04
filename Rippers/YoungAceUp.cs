using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dinokin.ScanlationTools.Interfaces;
using ImageMagick;
using Newtonsoft.Json;

namespace Dinokin.ScanlationTools.Rippers
{
    public class YoungAceUp : IRipper
    {
        public string Name { get; } = "Young Ace UP";
        
        private readonly HttpClient _httpClient;
        
        public YoungAceUp(IHttpClientFactory httpClientFactory) => _httpClient = httpClientFactory.CreateClient();

        public async Task<IEnumerable<MagickImage>> RipPages(IEnumerable<Uri> pagesAddresses, IProgress<ushort>? progressReporter = null) => await Task.WhenAll(
            pagesAddresses.Select(async address =>
            {
                var image = new MagickImage(await _httpClient.GetByteArrayAsync(address));
                progressReporter?.Report(1);

                return image;
            }));

        public async Task<IEnumerable<Uri>> GetPagesAddresses(Uri address, IProgress<ushort>? progressReporter = null) =>
            JsonConvert.DeserializeObject<string[]>(await _httpClient.GetStringAsync($"{address.AbsoluteUri}/json/"))
                .Select(result =>
                {
                    var uri = new Uri($"https://web-ace.jp{result}");
                    progressReporter?.Report(1);

                    return uri;
                });

        public bool IsValidURI(Uri address) => Regex.IsMatch(address.AbsoluteUri, @"^https:\/\/web-ace\.jp\/youngaceup\/contents\/1000[0-9]{3}\/episode\/[0-9]{1,4}\/$");
    }
}