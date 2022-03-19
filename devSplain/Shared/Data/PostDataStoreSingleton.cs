using devSplain.Shared.Models;

namespace devSplain.Shared.Data
{
    public class PostDataStoreSingleton
    {
        public List<PostModel> PostsList { get; set; } = new List<PostModel>();
    }
}
