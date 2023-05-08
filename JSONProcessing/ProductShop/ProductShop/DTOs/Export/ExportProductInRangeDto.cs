using Newtonsoft.Json;

namespace ProductShop.DTOs.Export
{
    public class ExportProductInRangeDto
    {
        [JsonProperty("name")] // here we put the name of the property in the json file we will create
        public string ProductName { get; set; } = null!;

        [JsonProperty("price")] // here we put the name of the property in the json file we will create
        public decimal ProductPrice { get; set; }

        [JsonProperty("seller")] // here we put the name of the property in the json file we will create
        public string SellerName { get; set; } = null!;
    }
}
