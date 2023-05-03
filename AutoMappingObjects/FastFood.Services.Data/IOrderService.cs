using FastFood.Web.ViewModels.Orders;

namespace FastFood.Services.Data
{
    public interface IOrderService
    {
        Task<IEnumerable<CreateOrderInputModel>> GetEmployeesAsync();
        Task<IEnumerable<CreateOrderInputModel>> GetItemsAsync();

        Task CreateAsync(CreateOrderInputModel model);

        Task<IEnumerable<OrderAllViewModel>> GetAllAsync();
    }
}
