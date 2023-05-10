using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
using Castle.Core.Resource;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            // IMPORT
            // string inputXml = File.ReadAllText("../../../Datasets/suppliers.xml"); // Task 9
            // string inputXml = File.ReadAllText("../../../Datasets/parts.xml"); // Task 10
            // string inputXml = File.ReadAllText("../../../Datasets/cars.xml"); // Task 11
            // string inputXml = File.ReadAllText("../../../Datasets/customers.xml"); // Task 12
            // string inputXml = File.ReadAllText("../../../Datasets/sales.xml"); // Task 13

            // IMPORT
            // string result = ImportSuppliers(context, inputXml); // Task 9
            // string result = ImportParts(context, inputXml); // Task 10
            // string result = ImportCars(context, inputXml); // Task 11
            // string result = ImportCustomers(context, inputXml); // Task 12
            // string result = ImportSales(context, inputXml); // Task 13

            // EXPORT
            // string result = GetCarsWithDistance(context); // Task 14
            // string result = GetCarsFromMakeBmw(context); // Task 15
            // string result = GetLocalSuppliers(context); // Task 16
            // string result = GetCarsWithTheirListOfParts(context); // Task 17
            // string result = GetTotalSalesByCustomer(context); // Task 18
            string result = GetSalesWithAppliedDiscount(context); // Task 19

            Console.WriteLine(result);
        }

        // IMPORT
        // Task 9
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            // With the created XmlHelper we can use it easier to deserialize with less code
            XmlHelper xmlHelper = new XmlHelper();

            ImportSupplierDto[] supplierDtos = xmlHelper.Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");

            // Creating ICollection to store the valid entities just like with JSON
            ICollection<Supplier> validSuppliers = new HashSet<Supplier>();

            // Itterating throw the supplierDtos to validate them and add the to the validSuppliers collection
            foreach (ImportSupplierDto supplierDto in supplierDtos)
            {
                // Checking if the dto has name
                if (string.IsNullOrEmpty(supplierDto.Name))
                {
                    continue;
                }

                // Mapping the entity

                // Manual mapping
                //Supplier supplier = new Supplier()
                //{
                //    Name = supplierDto.Name,
                //    IsImporter = supplierDto.IsImporter
                //};

                // Auto Mapping
                IMapper mapper = InitiaizeAutoMapper();
                Supplier supplier = mapper.Map<Supplier>(supplierDto);

                validSuppliers.Add(supplier);
            }

            context.Suppliers.AddRange(validSuppliers);
            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Count}";
        }

        // Task 10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitiaizeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportPartDto[] partDtos = xmlHelper.Deserialize<ImportPartDto[]>(inputXml, "Parts");

            ICollection<Part> validParts = new HashSet<Part>();

            foreach (ImportPartDto partDto in partDtos)
            {
                if (!context.Suppliers.Any(s => s.Id == partDto.SupplierId))
                {
                    continue;
                }

                Part part = mapper.Map<Part>(partDto);

                validParts.Add(part);
            }

            context.Parts.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}";
        }

        // Task 11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitiaizeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportCarDto[] carDtos = xmlHelper.Deserialize<ImportCarDto[]>(inputXml, "Cars");

            ICollection<Car> validCars = new HashSet<Car>();

            foreach (ImportCarDto carDto in carDtos)
            {
                if (string.IsNullOrEmpty(carDto.Make) || string.IsNullOrEmpty(carDto.Model))
                {
                    continue;
                }

                Car car = mapper.Map<Car>(carDto);

                // When we use .Value is is because the element is nullable type (int?) and .Value returns the int value of the element
                // DistinctBy takes only the unique elements by the given condition (in this case the PartId)
                foreach (ImportCarPartDto carPartDto in carDto.Parts.DistinctBy(cp => cp.PartId))
                {
                    if (!context.Parts.Any(p => p.Id == carPartDto.PartId))
                    {
                        continue;
                    }

                    PartCar partCar = new PartCar()
                    {
                        CarId = car.Id,
                        PartId = carPartDto.PartId
                    };

                    car.PartsCars.Add(partCar);
                }

                validCars.Add(car);
            }

            context.AddRange(validCars);
            context.SaveChanges();

            return $"Successfully imported {validCars.Count}";
        }

        // Task 12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlHelper xmlHelper = new XmlHelper();
            IMapper mapper = InitiaizeAutoMapper();

            ImportCustomerDto[] customerDtos = xmlHelper.Deserialize<ImportCustomerDto[]>(inputXml, "Customers");

            ICollection<Customer> validCustomers = new HashSet<Customer>();

            foreach (ImportCustomerDto customerDto in customerDtos)
            {
                Customer customer = mapper.Map<Customer>(customerDto);

                validCustomers.Add(customer);
            }

            context.AddRange(validCustomers);
            context.SaveChanges();

            return $"Successfully imported {validCustomers.Count}";
        }

        // Task 13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlHelper xmlHelper = new XmlHelper();
            IMapper mapper = InitiaizeAutoMapper();

            ImportSaleDto[] saleDtos = xmlHelper.Deserialize<ImportSaleDto[]>(inputXml, "Sales");

            ICollection<Sale> validSales = new HashSet<Sale>();

            foreach (ImportSaleDto saleDto in saleDtos)
            {
                if (!context.Cars.Any(c => c.Id == saleDto.CarId))
                {
                    continue;
                }

                Sale sale = mapper.Map<Sale>(saleDto);

                validSales.Add(sale);
            }

            context.Sales.AddRange(validSales);
            context.SaveChanges();

            return $"Successfully imported {validSales.Count}";
        }

        // Task 14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            IMapper mapper = InitiaizeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportCarDto[] carDtos = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarDto>(mapper.ConfigurationProvider)
                .ToArray();

            string result = xmlHelper.Serialize<ExportCarDto[]>(carDtos, "cars");

            return result;
        }

        // Task 15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            IMapper mapper = InitiaizeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportCarFromMakeBmwDto[] cars = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ProjectTo<ExportCarFromMakeBmwDto>(mapper.ConfigurationProvider)
                .ToArray();

            string result = xmlHelper.Serialize(cars, "cars");

            return result;
        }

        // Task 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            IMapper mapper = InitiaizeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportSupplierDto[] suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .ProjectTo<ExportSupplierDto>(mapper.ConfigurationProvider)
                .ToArray();

            string result = xmlHelper.Serialize<ExportSupplierDto[]>(suppliers, "suppliers");

            return result;
        }

        // Task 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            IMapper mapper = InitiaizeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportCarWithPartsDto[] carsWithParts = context.Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ProjectTo<ExportCarWithPartsDto>(mapper.ConfigurationProvider)
                .ToArray();

            string result = xmlHelper.Serialize<ExportCarWithPartsDto[]>(carsWithParts, "cars");

            return result;
        }

        // Task 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            //IMapper mapper = InitiaizeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            // Not working with AutoMapper
            //ExportCustomerDto[] customers = context.Customers
            //    .Where(c => c.Sales.Any())
            //    .ProjectTo<ExportCustomerDto>(mapper.ConfigurationProvider)
            //    .OrderByDescending(c => c.SpentMoney)
            //    .ToArray();

            ExportCustomerDto[] customers = context.Customers
                .Where(c => c.Sales.Any())
                .ToArray()
                .Select(c => new ExportCustomerDto
                {
                    Name = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SpentMoney = c.IsYoungDriver ?
                                decimal.Parse(c.Sales.Sum(s => s.Car.PartsCars.Sum(pc => Math.Round((double)pc.Part.Price * 0.95, 2))).ToString("f2")) :
                                decimal.Parse(c.Sales.Sum(s => s.Car.PartsCars.Sum(pc => pc.Part.Price)).ToString("f2"))
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            string result = xmlHelper.Serialize<ExportCustomerDto[]>(customers, "customers");

            return result;
        }

        // Task 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            IMapper mapper = InitiaizeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportSaleDto[] sales = context.Sales
                .ProjectTo<ExportSaleDto>(mapper.ConfigurationProvider)
                .ToArray();

            string result = xmlHelper.Serialize<ExportSaleDto[]>(sales, "sales");

            return result;
        }

        private static IMapper InitiaizeAutoMapper()
        {
            IMapper mapper = new Mapper(new MapperConfiguration(config =>
            {
                config.AddProfile<CarDealerProfile>();
            }));

            return mapper;
        }
    }
}