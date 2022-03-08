using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devSplain.Shared.Models
{
    public class PostModel
    {
        [JsonProperty(PropertyName = "id")]
        public string PostId { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "author_id")]
        public string AuthorId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "image")]
        public byte[]? Image { get; set; }

        [JsonProperty(PropertyName = "tag")]
        public string Tag { get; set; }

        [JsonProperty(PropertyName = "created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "rating")]
        public int Rating { get; set; }

        public PostModel(string postId, string author, string authorId, string title, string text, 
            byte[] image, string tag, DateTime createdOn, string type, int rating)
        {
            PostId = postId ?? throw new ArgumentNullException(nameof(postId));
            Author = author ?? throw new ArgumentNullException(nameof(author));
            AuthorId = authorId ?? throw new ArgumentNullException(nameof(authorId));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Image = image ?? throw new ArgumentNullException(nameof(image));
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            CreatedOn = createdOn;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Rating = rating;
        }

        public PostModel()
        {
            PostId = "";
            Author = "";
            AuthorId = "";
            Title = "";
            Text = "";
            Image = null;
            Tag = "";
            CreatedOn = DateTime.Now;
            Type = "";
            Rating = 0;
        }
    }
}
