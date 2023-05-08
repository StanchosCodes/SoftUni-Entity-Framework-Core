using Newtonsoft.Json;

namespace ProductShop.DTOs.Import
{
    public class ImportUserDto
    {
        [JsonProperty("firstName")] // not recomended (just to be sure everything will pass)
        public string? FirstName { get; set; }

        [JsonProperty("lastName")] // not recomended (just to be sure everything will pass)
        public string LastName { get; set; } = null!;

        [JsonProperty("age")] // not recomended (just to be sure everything will pass)
        public int? Age { get; set; }
    }
}
