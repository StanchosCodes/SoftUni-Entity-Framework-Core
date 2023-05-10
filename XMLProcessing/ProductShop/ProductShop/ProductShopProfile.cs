using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            // IMPORT
            // User
            this.CreateMap<ImportUserDto, User>();

            // Product
            this.CreateMap<ImportProductDto, Product>();

            // Category
            this.CreateMap<ImportCategoryDto, Category>();

            // CategoryProduct
            this.CreateMap<ImportCategoryProductDto, CategoryProduct>();

            // EXPORT
            // Product
            this.CreateMap<Product, ExportProductDto>()
                    .ForMember(d => d.Buyer, opt => opt.MapFrom(s => $"{s.Buyer.FirstName} {s.Buyer.LastName}"));

            // User with Products
            this.CreateMap<Product, ExportUserProductDto>();
            this.CreateMap<User, ExportUserWithProductsDto>()
                    .ForMember(d => d.Products, opt => opt.MapFrom(s => s.ProductsSold));

            // Category
            this.CreateMap<Category, ExportCategoryDto>()
                    .ForMember(d => d.Count, opt => opt.MapFrom(s => s.CategoryProducts.Count()))
                    .ForMember(d => d.AveragePrice, opt => opt.MapFrom(s => s.CategoryProducts.Average(cp => cp.Product.Price)))
                    .ForMember(d => d.TotalPrice, opt => opt.MapFrom(s => s.CategoryProducts.Sum(cp => cp.Product.Price)));

            // Users, Products and count - Not working fully properly
            //this.CreateMap<Product, ExportUserProductDto>();
            //this.CreateMap<User, ExportUserInfoDto>()
            //        .ForMember(d => d.SoldProducts, opt => opt.MapFrom(s => s.ProductsSold));
            //this.CreateMap<Product, ExportSoldProductsAndCountDto>()
            //        .ForMember(d => d.Count, opt => opt.MapFrom(s => s.CategoryProducts.Count()))
            //        .ForMember(d => d.Products, opt => opt.MapFrom(s => s.CategoryProducts.Select(cp => cp.Product)));
        }
    }
}
