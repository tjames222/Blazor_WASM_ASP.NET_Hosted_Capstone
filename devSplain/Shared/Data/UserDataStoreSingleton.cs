using devSplain.Shared.Models;

namespace devSplain.Shared.Data
{
    public class UserDataStoreSingleton
    {
        public List<UserModel> UsersList { get; set; } = new List<UserModel>();
    }
}
