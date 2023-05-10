using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            // IMPORT
            // Supplier
            this.CreateMap<ImportSupplierDto, Supplier>();

            // Part
            this.CreateMap<ImportPartDto, Part>();

            // Car
            // First Variant
            //this.CreateMap<ImportCarDto, Car>()
            //        .ForMember(d => d.PartsCars, opt => opt.MapFrom(s => s.Parts.Select(p => new PartCar() { PartId = p.PartId })));

            // Second Variant
            //this.CreateMap<ImportCarPartDto, PartCar>();
            //this.CreateMap<ImportCarDto, Car>();

            // Third Variant (this is the right one because we have to validate and skip some parts which doesnt exist)
            this.CreateMap<ImportCarDto, Car>()
                    .ForSourceMember(s => s.Parts, opt => opt.DoNotValidate());
            // In ForSourceMember .DoNotValidate skips the mapping of the given source (in this case the parts)
            // In ForMember is .Ignore

            // Customer
            this.CreateMap<ImportCustomerDto, Customer>();

            // Sale
            this.CreateMap<ImportSaleDto, Sale>();

            // EXPORT
            // Car
            this.CreateMap<Car, ExportCarDto>();

            // Car from model
            this.CreateMap<Car, ExportCarFromMakeBmwDto>();

            // Supplier
            this.CreateMap<Supplier, ExportSupplierDto>()
                    .ForMember(d => d.PartsCount, opt => opt.MapFrom(s => s.Parts.Count()));

            // Car with Parts
            this.CreateMap<Part, ExportCarPartDto>();
            this.CreateMap<Car, ExportCarWithPartsDto>()
                    .ForMember(d => d.Parts, opt => opt.MapFrom(s => s.PartsCars.Select(cp => cp.Part)
                        .OrderByDescending(p => p.Price)
                        .ToArray()));

            // Customer - Not working with this mapping configuration
            //this.CreateMap<Customer, ExportCustomerDto>()
            //        .ForMember(d => d.BoughtCars, opt => opt.MapFrom(s => s.Sales.Count()))
            //        .ForMember(d => d.SpentMoney, opt => opt.MapFrom(s => s.Sales.Sum(s =>
            //                                                                             s.Car.PartsCars.Sum(pc =>
            //                                                                                                 pc.Part.Price))));

            // Sale
            this.CreateMap<Sale, ExportSaleDto>()
                    //.ForMember(d => d.Discount, opt => opt.MapFrom(s => decimal.Parse(s.Discount.ToString("f0"))))
                    .ForMember(d => d.Price, opt => opt.MapFrom(s => (double)s.Car.PartsCars.Sum(pc => pc.Part.Price)))
                    .ForMember(d => d.PriceWithDiscount, opt => opt.MapFrom(s => (double)(s.Car.PartsCars.Sum(pc => pc.Part.Price) - (s.Car.PartsCars.Sum(pc => pc.Part.Price) * (s.Discount / 100)))));
        }
    }
}
