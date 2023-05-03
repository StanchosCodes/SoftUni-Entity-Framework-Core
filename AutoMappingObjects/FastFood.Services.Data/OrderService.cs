using AutoMapper;
using AutoMapper.QueryableExtensions;
using FastFood.Data;
using FastFood.Models;
using FastFood.Web.ViewModels.Orders;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Services.Data
{
    public class OrderService : IOrderService
    {
        private readonly IMapper mapper;
        private readonly FastFoodContext context;

        public OrderService(IMapper mapper, FastFoodContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public async Task CreateAsync(CreateOrderInputModel model)
        {
            Order order = this.mapper.Map<Order>(model);

            await this.context.AddAsync(order);
            await this.context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderAllViewModel>> GetAllAsync()
            => await this.context.Orders
                            .ProjectTo<OrderAllViewModel>(this.mapper.ConfigurationProvider)
                            .ToArrayAsync();

        public async Task<IEnumerable<CreateOrderInputModel>> GetEmployeesAsync()
            => await this.context.Employees
                                    .ProjectTo<CreateOrderInputModel>(this.mapper.ConfigurationProvider)
                                    .ToArrayAsync();

        public async Task<IEnumerable<CreateOrderInputModel>> GetItemsAsync()
            => await this.context.Items
                                    .ProjectTo<CreateOrderInputModel>(this.mapper.ConfigurationProvider)
                                    .ToArrayAsync();
    }
}
