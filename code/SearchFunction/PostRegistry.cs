using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SearchFunction
{
    public static class PostRegistry
    {
        private class Payload
        {
            public string[] Url { get; set; }
            public string[] Title { get; set; }
            public string[] Published { get; set; }
            public string[] Content { get; set; }
            public string[] Excerpt { get; set; }
            public string[][] Categories { get; set; }
            public string[][] Tags { get; set; }
        }

        public static async Task<Post[]> LoadPostListAsync()
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync("https://vplauzon.github.io/search-source.json"))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = JsonSerializer.Create();
                var payload = serializer.Deserialize<Payload>(jsonReader);

                ValidateLengths(payload);

                var posts = from i in Enumerable.Range(0, payload.Url.Length)
                            select new Post
                            {
                                Id = payload.Url[i].Replace('/', '-'),
                                Url = payload.Url[i],
                                Title = payload.Title[i],
                                Published = DateTime.Parse(payload.Published[i]),
                                Content = CapLength(payload.Content[i], Int16.MaxValue / 2),
                                Excerpt = CapLength(payload.Excerpt[i], 1200),
                                Categories = payload.Categories[i],
                                Tags = payload.Tags[i],
                            };

                return posts.ToArray();
            }
        }

        private static string CapLength(string text, int maxLength)
        {
            if (text.Length <= maxLength)
            {
                return text;
            }
            else
            {
                return text.Substring(0, maxLength) + "...";
            }
        }

        private static void ValidateLengths(Payload payload)
        {
            if (payload.Url.Length != payload.Title.Length)
            {
                throw new IndexOutOfRangeException(
                    "Url doesn't match title size:  "
                    + $"{payload.Url.Length} != {payload.Title.Length}");
            }
            if (payload.Url.Length != payload.Published.Length)
            {
                throw new IndexOutOfRangeException(
                    "Url doesn't match published size:  "
                    + $"{payload.Url.Length} != {payload.Published.Length}");
            }
            if (payload.Url.Length != payload.Content.Length)
            {
                throw new IndexOutOfRangeException(
                    "Url doesn't match content size:  "
                    + $"{payload.Url.Length} != {payload.Content.Length}");
            }
            if (payload.Url.Length != payload.Excerpt.Length)
            {
                throw new IndexOutOfRangeException(
                    "Url doesn't match excerpt size:  "
                    + $"{payload.Url.Length} != {payload.Excerpt.Length}");
            }
            if (payload.Url.Length != payload.Categories.Length)
            {
                throw new IndexOutOfRangeException(
                    "Url doesn't match categories size:  "
                    + $"{payload.Url.Length} != {payload.Categories.Length}");
            }
            if (payload.Url.Length != payload.Tags.Length)
            {
                throw new IndexOutOfRangeException(
                    "Url doesn't match tags size:  "
                    + $"{payload.Url.Length} != {payload.Tags.Length}");
            }
        }
    }
}