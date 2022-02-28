using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using devSplain.Shared.Models;
using Serilog;

namespace devSplain.Server.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private BlobClient _blobClient;

        public BlobService(
            BlobServiceClient blobServiceClient,
            BlobContainerClient blobContainerClient)
        {
            _blobServiceClient = blobServiceClient;
            _blobContainerClient = blobContainerClient;
            Log.Information("BlobService: Establishing connection with blob storage container");
        }

        public async Task<List<BlobModel>> GetUrls()
        {
            List<BlobModel> Urls = new();
            Log.Information("BlobService: GetUrls");

            try
            {
                await foreach (BlobItem blobItem in _blobContainerClient.GetBlobsAsync())
                {
                    _blobClient = _blobContainerClient.GetBlobClient(blobItem.Name);
                    Uri uri = GetServiceSasUriForBlob(_blobClient);
                    BlobProperties properties = await _blobClient.GetPropertiesAsync();
                    BlobModel blobModel = new();
                    blobModel.Uri = uri.AbsoluteUri;
                    blobModel.UserId = properties.Metadata.Values.First(); // Grabs the first meta data item. In this case it is the userId
                    Urls.Add(blobModel);
                }
            }
            catch (Exception e)
            {
                Log.Error("ERROR: {0}", e);
                throw;
            }

            return await Task.FromResult(Urls);
        }

        public async Task<List<BlobModel>> GetUrlsByTag()
        {
            List<BlobModel> Urls = new();
            Log.Information("BlobService: GetUrlsByTag");

            try
            {
                string query = @"""Category"" = 'userImage'";
                await foreach (TaggedBlobItem blobItem in _blobServiceClient.FindBlobsByTagsAsync(query))
                {
                    _blobClient = _blobContainerClient.GetBlobClient(blobItem.BlobName);
                    Uri uri = GetServiceSasUriForBlob(_blobClient);
                    BlobProperties properties = await _blobClient.GetPropertiesAsync();
                    BlobModel blobModel = new();
                    blobModel.Uri = uri.AbsoluteUri;
                    blobModel.UserId = properties.Metadata.Values.First(); // Grabs the first meta data item. In this case it is the userId
                    Urls.Add(blobModel);
                }
            }
            catch (Exception e)
            {
                Log.Error("ERROR: {0}", e);
                throw;
            }

            return await Task.FromResult(Urls);
        }

        private static Uri? GetServiceSasUriForBlob(BlobClient blobClient, string? storedPolicyName = null)
        {
            Log.Information("BlobService: GetServiceSasUriForBlob");

            // Check whether this BlobClient object has been authorized with Shared Key.
            if (blobClient.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read |
                        BlobSasPermissions.Write);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                Log.Information("SAS URI for blob created.");

                return sasUri;
            }
            else
            {
                Log.Error(@"BlobClient must be authorized with Shared Key 
                          credentials to create a service SAS.");

                return null;
            }
        }

        // Add Metadata on the blob. We will need to add user id to each uploaded image to keep them related for later use
        public async Task AddBlobMetadataAsync(BlobClient blob, string key, string value)
        {
            Log.Information("BlobService: Add BlobMetadataAsync");

            try
            {
                IDictionary<string, string> metadata = new Dictionary<string, string>
                {
                    { key, value }
                };

                Log.Information("Adding blob metadata...");

                await blob.SetMetadataAsync(metadata);
            }
            catch (RequestFailedException e)
            {
                Log.Error($"HTTP error code {e.Status}: {e.ErrorCode}");
                Log.Error(e.Message);
            }
        }

        public async Task<List<KeyValuePair<string, string>>> ReadBlobMetadataAsync(BlobClient blob)
        {
            Log.Information("Blob Service: ReadBlobMetadataAsync");
            List<KeyValuePair<string, string>> valuePairs = new();

            try
            {
                BlobProperties properties = await blob.GetPropertiesAsync();

                Log.Information("Reading blob metadata:");

                foreach (KeyValuePair<string, string> metadataItem in properties.Metadata)
                {
                    valuePairs.Add(metadataItem);
                }
            }
            catch (RequestFailedException e)
            {
                Log.Error($"HTTP error code {e.Status}: {e.ErrorCode}");
                Log.Error(e.Message);
            }
            return valuePairs;
        }

        public async Task UploadUserImageToContainer(string userId)
        {
            Log.Information("Blob Service: UploadUserImageToContainer");
            IDictionary<string, string> tagData = new Dictionary<string, string>();
            string imgName = userId;
            string imgPath = "Development/unsafe_uploads/";

            tagData.Add("Category", "userImage");

            try
            {
                _blobClient = _blobContainerClient.GetBlobClient(imgName + ".png");

                using FileStream uploadFileStream = File.OpenRead(imgPath + "userTemp.png");

                await _blobClient.UploadAsync(uploadFileStream, overwrite: true);
                await AddBlobMetadataAsync(_blobClient, "user_id", userId);
                await _blobClient.SetTagsAsync(tagData);
                Log.Information("Uploading Image to container...");
                uploadFileStream.Close();
            }
            catch (Exception ex)
            {
                Log.Error("ERROR: {0}", ex);
            }
        }
    }
}
