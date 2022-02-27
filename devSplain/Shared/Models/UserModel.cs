using Newtonsoft.Json;

namespace devSplain.Shared.Models
{
    public class UserModel
    {
        [JsonProperty(PropertyName = "id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty(PropertyName = "address")]
        public AddressModel Address { get; set; }

        [JsonProperty(PropertyName = "about")]
        public string About { get; set; }

        [JsonProperty(PropertyName = "use_biometrics")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = "dob")]
        public DateTime DOB { get; set; }

        [JsonProperty(PropertyName = "auth_id")]
        public string AuthId { get; set; }

        [JsonProperty(PropertyName = "saved_tutorials")]
        public string[] SavedTutorials { get; set; }

        [JsonProperty(PropertyName = "saved_articles")]
        public string[] SavedArticles { get; set; }

        [JsonProperty(PropertyName = "is_member")]
        public bool IsMember { get; set; }

        public UserModel(string userId, string firstName, string lastName, string emailAddress, 
            AddressModel address, string about, string role, DateTime dOB, string authId, 
            string[] savedTutorials, string[] savedArticles, bool isMember)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            About = about ?? throw new ArgumentNullException(nameof(about));
            Role = role ?? throw new ArgumentNullException(nameof(role));
            DOB = dOB;
            AuthId = authId ?? throw new ArgumentNullException(nameof(authId));
            SavedTutorials = savedTutorials ?? throw new ArgumentNullException(nameof(savedTutorials));
            SavedArticles = savedArticles ?? throw new ArgumentNullException(nameof(savedArticles));
            IsMember = isMember;
        }

        public UserModel()
        {
            UserId = "";
            FirstName = "";
            LastName = "";
            EmailAddress = "";
            Address = new AddressModel();
            About = "";
            Role = "";
            DOB = DateTime.Now;
            SavedTutorials = new string[5];
            SavedArticles = new string[5];
            AuthId = "";
            IsMember = false;
        }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
