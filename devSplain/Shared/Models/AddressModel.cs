using Newtonsoft.Json;

namespace devSplain.Shared.Models
{
    public class AddressModel
    {
        [JsonProperty(PropertyName = "id")]
        public string AddressId { get; set; }

        [JsonProperty(PropertyName = "street")]
        public string Street { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        public AddressModel(string addressId, string street, string city, string state, string postalCode, string country)
        {
            AddressId = addressId ?? throw new ArgumentNullException(nameof(addressId));
            Street = street ?? throw new ArgumentNullException(nameof(street));
            City = city ?? throw new ArgumentNullException(nameof(city));
            State = state ?? throw new ArgumentNullException(nameof(state));
            PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
            Country = country ?? throw new ArgumentNullException(nameof(country));
        }

        public AddressModel()
        {
            AddressId = "";
            Street = "";
            City = "";
            State = "";
            PostalCode = "";
            Country = "";
        }
    }
}
