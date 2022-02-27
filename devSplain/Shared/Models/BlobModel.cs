using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devSplain.Shared.Models
{
    public class BlobModel
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public string? Uri { get; set; }

        [JsonProperty(PropertyName = "file_name")]
        public string? FileName { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string? UserId { get; set; }
    }
}
