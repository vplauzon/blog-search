using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace SearchFunction
{
    public static class RefreshIndex
    {
        private const string INDEX_NAME = "posts";
        
        [FunctionName("refresh-search-index")]
        public async static Task Run(
            [TimerTrigger("0 0 0 * * FRI")]TimerInfo timer,
            ILogger log)
        {
            var searchAccount = Environment.GetEnvironmentVariable("search-account");
            var searchKey = Environment.GetEnvironmentVariable("search-key");
            
            log.LogInformation($"Load at: {DateTime.Now}");
            log.LogInformation(
                $"Search account status:  {!string.IsNullOrWhiteSpace(searchAccount)}");
            log.LogInformation(
                $"Search key status:  {!string.IsNullOrWhiteSpace(searchKey)}");

            var serviceClient = new SearchServiceClient(
                searchAccount,
                new SearchCredentials(searchKey));
            var posts = await PostRegistry.LoadPostListAsync();

            log.LogInformation($"Load {posts.Length} posts list");

            await RefreshIndexAsync(serviceClient);
            await LoadDocumentsAsync(serviceClient.Indexes.GetClient(INDEX_NAME), posts);
        }

        private static SearchServiceClient GetService(string searchKey)
        {
            var serviceClient = new SearchServiceClient("vplmvp", new SearchCredentials(searchKey));

            return serviceClient;
        }

        private static async Task RefreshIndexAsync(SearchServiceClient serviceClient)
        {
            var indexList = await serviceClient.Indexes.ListAsync();
            var deletingTasks = from i in indexList.Indexes
                                select serviceClient.Indexes.DeleteAsync(i.Name);

            await Task.WhenAll(deletingTasks);

            var definition = new Microsoft.Azure.Search.Models.Index()
            {
                Name = INDEX_NAME,
                Fields = FieldBuilder.BuildForType<Post>()
            };

            await serviceClient.Indexes.CreateAsync(definition);
        }

        private static async Task LoadDocumentsAsync(ISearchIndexClient indexClient, Post[] posts)
        {
            var actions = from p in posts
                          select IndexAction.Upload(p);
            var batch = IndexBatch.New(actions);
            var result = await indexClient.Documents.IndexAsync(batch);
        }
    }
}