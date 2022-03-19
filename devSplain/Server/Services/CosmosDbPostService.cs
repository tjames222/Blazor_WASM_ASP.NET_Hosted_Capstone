using devSplain.Shared.Models;
using Microsoft.Azure.Cosmos;
using Serilog;

namespace devSplain.Server.Services
{
    public class CosmosDbPostService : ICosmosDbService
    {
        private readonly Container _container;

        public CosmosDbPostService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            Log.Information("CosmosDbPostService: Establishing connection with post container");
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddItemAsync<T>(T post, string id)
        {
            try
            {
                Log.Information("CosmosDbUserService: AddItemAsync");
                await this._container.CreateItemAsync<T?>(post, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                Log.Error("CosmosDbPostService: Exception thrown -> {0}", ex);
            }
        }

        public async Task DeleteItemAsync(string id)
        {
            try
            {
                Log.Information("CosmosDbPostService: DeleteItemAsync");
                await this._container.DeleteItemAsync<PostModel?>(id, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                Log.Error("CosmosDbPostService: Exception thrown -> {0}", ex);
            }
        }

        public async Task<T?> GetItemAsync<T>(string id)
        {
            try
            {
                Log.Information("CosmosDbPostService: GetItemAsync");
                ItemResponse<T> response = await this._container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Log.Information("CosmosDbPostService: Exception thrown -> {0}", ex);
                return default;
            }
        }

        public Task<T> GetItemByAuthIdAsync<T>(string authId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<T>> GetItemsAsync<T>()
        {
            Log.Information("CosmosDbPostService: AddItemsAsync");
            string queryString = "SELECT * FROM c";

            try
            {
                var query = this._container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
                List<T> results = new();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }

                return results;
            }
            catch (Exception ex)
            {
                Log.Error("CosmosDbPostService: Exception thrown -> {0}", ex);
                return default;
            }
        }

        public async Task UpdateItemAsync<T>(string id, T post)
        {
            try
            {
                Log.Information("CosmosDbPostService: UpdateItemAsync");
                await this._container.UpsertItemAsync<T?>(post, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                Log.Error("CosmosDbPostService: Exception thrown -> {0}", ex);
            }
        }
    }
}
