namespace devSplain.Server.Services
{
    public interface ICosmosDbService
    {
        Task<List<T>> GetItemsAsync<T>();
        Task<T> GetItemAsync<T>(string id);
        Task<T> GetItemByAuthIdAsync<T>(string authId);
        Task AddItemAsync<T>(T user, string id);
        Task UpdateItemAsync<T>(string id, T user);
        Task DeleteItemAsync(string id);
    }
}
