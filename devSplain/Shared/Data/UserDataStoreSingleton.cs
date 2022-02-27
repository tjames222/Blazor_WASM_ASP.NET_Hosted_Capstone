using devSplain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devSplain.Shared.Data
{
    public class UserDataStoreSingleton
    {
        public List<UserModel> UsersList { get; set; } = new List<UserModel>();
    }
}
