using devSplain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devSplain.Shared.Data
{
    public class BlobDataStoreSingleton
    {
        public List<BlobModel> BlobList { get; set; } = new List<BlobModel>();
    }
}
