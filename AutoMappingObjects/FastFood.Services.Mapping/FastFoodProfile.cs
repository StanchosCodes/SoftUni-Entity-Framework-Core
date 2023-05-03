namespace FastFood.Services.Mapping
{
    using AutoMapper;
    using FastFood.Models;
    using FastFood.Web.ViewModels.Categories;
    using FastFood.Web.ViewModels.Employees;
    using FastFood.Web.ViewModels.Items;
    using FastFood.Web.ViewModels.Orders;
    using FastFood.Web.ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions mapping configuration

            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            // Categories mapping configuration

            // we want from the INPUT from the form to create (map) a CATEGORY
            // then we say which is the member in the CATEGORY (cat.Name) that we want to map
            // then we say from where we want to map it (cat.MapFrom( here we say which is the prop we want to map from in the INPUT ( ccim.CategoryName ) ))

            this.CreateMap<CreateCategoryInputModel, Category>()
                .ForMember(cat => cat.Name, cat => cat.MapFrom(ccim => ccim.CategoryName));

            // we want from CATEGORY to create (map) a CategoryAllViewModel (<Category, CategoryAllViewModel>)
            // we want the CategoryAllViewModel.Name to be mapped from the Category.Name

            this.CreateMap<Category, CategoryAllViewModel>();

            // if we do not have different prop names in the category and in the categoryAllViewModel we can skip this row
            // .ForMember(cavm => cavm.Name, cavm => cavm.MapFrom(cat => cat.Name));

            // Items mapping configuration
            this.CreateMap<Category, CreateItemViewModel>()
                   .ForMember(civm => civm.CategoryId, civm => civm.MapFrom(cat => cat.Id))
                   .ForMember(civm => civm.CategoryName, civm => civm.MapFrom(cat => cat.Name));

            this.CreateMap<CreateItemInputModel, Item>();
            this.CreateMap<Item, ItemsAllViewModels>()
                .ForMember(iavm => iavm.Category, iavm => iavm.MapFrom(item => item.Category.Name));

            // Orders mapping configuration
            
            //this.CreateMap<Item, CreateOrderViewModel>()
            //    .ForMember(covm => covm.Items, covm => covm.MapFrom(item => item.Id));

            //this.CreateMap<Employee, CreateOrderViewModel>()
            //    .ForMember(covm => covm.Employees, covm => covm.MapFrom(employee => employee.Id));

            this.CreateMap<Item, CreateOrderInputModel>()
                .ForMember(coim => coim.ItemId, covm => covm.MapFrom(item => item.Id));

            this.CreateMap<Employee, CreateOrderInputModel>()
                .ForMember(coim => coim.EmployeeId, covm => covm.MapFrom(employee => employee.Id));

            this.CreateMap<CreateOrderInputModel, Order>();

            this.CreateMap<Order, OrderAllViewModel>()
                .ForMember(oavm => oavm.OrderId, oavm => oavm.MapFrom(order => order.Id));

            // Employees mapping configuration

            this.CreateMap<Position, RegisterEmployeeViewModel>()
                    .ForMember(revm => revm.PositionId, revm => revm.MapFrom(pos => pos.Id));

            this.CreateMap<Employee, RegisterEmployeeInputModel>()
                    .ForMember(reim => reim.PositionName, reim => reim.MapFrom(emp => emp.Position.Name));

            this.CreateMap<Employee, EmployeesAllViewModel>()
                    .ForMember(eavm => eavm.Position, eavm => eavm.MapFrom(emp => emp.Position.Name));
        }
    }
}
