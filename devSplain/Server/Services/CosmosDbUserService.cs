using devSplain.Shared.Models;
using Microsoft.Azure.Cosmos;
using Serilog;


namespace devSplain.Server.Services
{
    public class CosmosDbUserService : ICosmosDbService
    {
        private readonly Container _container;

        public CosmosDbUserService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            Log.Information("CosmosDbUserService: Establishing connection with user container");
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddItemAsync<T>(T user, string id)
        {
            try
            {
                Log.Information("CosmosDbUserService: AddItemAsync");
                await this._container.CreateItemAsync<T?>(user, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                Log.Error("CosmosDbUserService: Exception thrown -> {0}", ex);
            }
        }

        public async Task DeleteItemAsync(string id)
        {
            try
            {
                Log.Information("CosmosDbUserService: DeleteItemAsync");
                await this._container.DeleteItemAsync<UserModel?>(id, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                Log.Error("CosmosDbUserService: Exception thrown -> {0}", ex);
            }
        }

        public async Task<T?> GetItemAsync<T>(string id)
        {
            try
            {
                Log.Information("CosmosDbUserService: GetItemAsync");
                ItemResponse<T> response = await this._container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Log.Information("CosmosDbUserService: Exception thrown -> {0}", ex);
                return default;
            }

        }

        public async Task<T?> GetItemByAuthIdAsync<T>(string authId)
        {
            Log.Information("CosmosDbUserService: GetItemByAuthIdAsync");
            string queryString = "SELECT * FROM c WHERE c.auth_id=" + "'" + authId + "'";

            try
            {
                var query = _container.GetItemQueryIterator<T?>(new QueryDefinition(queryString));
                List<T?> results = new();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error("CosmosDbUserService: Exception thrown -> {0}", ex);
                return default;
            }
        }

        public async Task<List<T>?> GetItemsAsync<T>()
        {
            Log.Information("CosmosDbUserService: AddItemsAsync");
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
                Log.Error("CosmosDbUserService: Exception thrown -> {0}", ex);
                return default;
            }
        }

        public async Task UpdateItemAsync<T>(string id, T user)
        {
            try
            {
                Log.Information("CosmosDbUserService: UpdateItemAsync");
                await this._container.UpsertItemAsync<T?>(user, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                Log.Error("CosmosDbUserService: Exception thrown -> {0}", ex);
            }
        }
    }
}
