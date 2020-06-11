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
    public class SeigaNicoNico : IAuthenticatedRipper
    {
        public string Name { get; } = "Seiga Niconico";
        
        private readonly HttpClient _httpClient;

        public SeigaNicoNico(IHttpClientFactory httpClientFactory) => _httpClient = httpClientFactory.CreateClient();
        
        public async Task<IEnumerable<MagickImage>> RipPages(IEnumerable<Uri> pagesAddresses, IProgress<ushort>? progressReporter = null) => await Task.WhenAll(
            pagesAddresses.Select(async address =>
            {
                var image = new MagickImage(await _httpClient.GetByteArrayAsync(address));

                progressReporter?.Report(1);
                return image;
            }));

        public async Task<IEnumerable<Uri>> GetPagesAddresses(Uri address, IProgress<ushort>? progressReporter = null) =>
            Regex.Matches(await _httpClient.GetStringAsync(address), "\"image_id\":\"([0-9]+)\"").Select(match => ulong.Parse(match.Groups.Values.Last().Value)).Distinct().Select(value => new Uri($"https://seiga.nicovideo.jp/image/source/{value}"));

        public bool IsValidURI(Uri address) => Regex.IsMatch(address.AbsoluteUri, @"^https:\/\/seiga\.nicovideo\.jp\/watch\/mg[0-9]{6}\?track=ct_episode$");

        public async Task Authenticate(string user, string password, string? twoFactorCode = null)
        {
            var fields = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("mail_tel", user),
                new KeyValuePair<string, string>("password", password)
            };

            await _httpClient.PostAsync("https://account.nicovideo.jp/login/redirector", new FormUrlEncodedContent(fields));
        }
    }
}