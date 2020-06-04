using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dinokin.ScanlationTools.Rippers
{
    public partial class ComicBorder
    {
        private struct Episode
        {
            [JsonProperty("readableProduct")]
            public ReadableProduct ReadableProduct { get; set; }
        }

        private struct ReadableProduct
        {
            [JsonProperty("publishedAt")]
            public DateTimeOffset PublishedAt { get; set; }

            [JsonProperty("series")]
            public Series Series { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("toc")]
            public object Toc { get; set; }

            [JsonProperty("pageStructure")]
            public PageStructure PageStructure { get; set; }

            [JsonProperty("imageUrisDigest")]
            public string ImageUrisDigest { get; set; }

            [JsonProperty("typeName")]
            public string TypeName { get; set; }

            [JsonProperty("prevReadableProductUri")]
            public object PrevReadableProductUri { get; set; }

            [JsonProperty("number")]
            public long Number { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("isPublic")]
            public bool IsPublic { get; set; }

            [JsonProperty("nextReadableProductUri")]
            public Uri NextReadableProductUri { get; set; }

            [JsonProperty("hasPurchased")]
            public bool HasPurchased { get; set; }

            [JsonProperty("permalink")]
            public Uri Permalink { get; set; }
        }

        private struct PageStructure
        {
            [JsonProperty("readingDirection")]
            public string ReadingDirection { get; set; }

            [JsonProperty("pages")]
            public IEnumerable<Page> Pages { get; set; }

            [JsonProperty("startPosition")]
            public string StartPosition { get; set; } 

            [JsonProperty("choJuGiga")]
            public string ChoJuGiga { get; set; }
        }

        private struct Page
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("contentStart")]
            public string ContentStart { get; set; }

            [JsonProperty("src")]
            public Uri Src { get; set; }

            [JsonProperty("width")]
            public long? Width { get; set; }

            [JsonProperty("height")]
            public long? Height { get; set; }

            [JsonProperty("contentEnd")]
            public string ContentEnd { get; set; }
        }

        private struct Series
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("thumbnailUri")]
            public Uri ThumbnailUri { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }
    }
}