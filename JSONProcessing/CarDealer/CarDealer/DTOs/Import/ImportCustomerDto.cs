namespace CarDealer.DTOs.Import
{
    public class ImportCustomerDto
    {
        public string Name { get; set; } = null!;

        public DateTime Birthdate { get; set; }

        public bool IsYoungDriver { get; set; }
    }
}
