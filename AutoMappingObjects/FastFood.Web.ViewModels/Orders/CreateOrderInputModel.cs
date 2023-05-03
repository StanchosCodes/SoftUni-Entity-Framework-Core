namespace FastFood.Web.ViewModels.Orders
{
    public class CreateOrderInputModel
    {
        public string Customer { get; set; } = null!;

        public string ItemId { get; set; } = null!;

        public string EmployeeId { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
