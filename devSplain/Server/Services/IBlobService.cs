using Azure.Storage.Blobs;
using devSplain.Shared.Models;

namespace devSplain.Server.Services
{
    public interface IBlobService
    {
        Task<List<BlobModel>> GetUrls();
        Task<List<BlobModel>> GetUrlsByTag();
        Task AddBlobMetadataAsync(BlobClient blob, string key, string value);
        Task<List<KeyValuePair<string, string>>> ReadBlobMetadataAsync(BlobClient blob);
        Task UploadUserImageToContainer(string userId);
    }
}
