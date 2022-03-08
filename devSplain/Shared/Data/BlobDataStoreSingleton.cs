using devSplain.Shared.Models;

namespace devSplain.Shared.Data
{
    public class BlobDataStoreSingleton
    {
        public List<BlobModel> BlobList { get; set; } = new List<BlobModel>();
    }
}
