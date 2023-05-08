using Newtonsoft.Json;

namespace CarDealer.DTOs.Import
{
    public class ImportCarDto
    {
        public string Make { get; set; } = null!;

        public string Model { get; set; } = null!;

        [JsonProperty("traveledDistance")]
        public int TarveledDistance { get; set; }

        public List<int> PartsId { get; set; } = new List<int>();
    }
}
