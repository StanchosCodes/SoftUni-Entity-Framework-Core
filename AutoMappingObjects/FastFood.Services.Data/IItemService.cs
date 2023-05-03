using FastFood.Web.ViewModels.Categories;
using FastFood.Web.ViewModels.Items;

namespace FastFood.Services.Data
{
    public interface IItemService
    {
        Task<IEnumerable<CreateItemViewModel>> GetAllCategoriesAsync();
        Task CreateAsync(CreateItemInputModel model);
        Task<IEnumerable<ItemsAllViewModels>> GetAllAsync();
    }
}
