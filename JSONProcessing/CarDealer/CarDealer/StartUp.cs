using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            // For Imports
            // 1 - Creating DTO
            // 2 - Adding Mapping Configuration in the Profile
            // 3 - Creating a Context Instance
            // 4 - Reading the text from the json File
            // 5 - Calling the Method

            // For the Import Methods
            // 1 - Declaring a Mapper (Declaring a MapperComfiguration, Passing the Profile to the Configuration)
            // 2 - Deserializing the inputJson to the ImportDto (in array if needed)
            // 3 - Creating a collection with the valid entities (optional - can be passed with Mapping in an array directly)
            // EXAMPLE - Supplier[] suppliers = mapper.Map<Supplier[]>(supplierDtos)
            // 4 - If using collection then foreach to map one by one the entities and add them in the collection
            // 5 - Add the entities to the context
            // 6 - SaveChanges

            CarDealerContext context = new CarDealerContext();

            // IMPORT
            // string inputJson = File.ReadAllText(@"../../../Datasets/suppliers.json"); // Task 9
            // string inputJson = File.ReadAllText(@"../../../Datasets/parts.json"); // Task 10
            // string inputJson = File.ReadAllText(@"../../../Datasets/cars.json"); // Task 11
            // string inputJson = File.ReadAllText(@"../../../Datasets/customers.json"); // Task 12
            // string inputJson = File.ReadAllText(@"../../../Datasets/sales.json"); // Task 13

            // string result = ImportSuppliers(context, inputJson); // Task 9
            // string result = ImportParts(context, inputJson); // Task 10
            // string result = ImportCars(context, inputJson); // Task 11
            // string result = ImportCustomers(context, inputJson); // Task 12
            // string result = ImportSales(context, inputJson); // Task 13

            // EXPORT
            // string result = GetOrderedCustomers(context); // Task 14
            // string result = GetCarsFromMakeToyota(context); // Task 15
            // string result = GetLocalSuppliers(context); // Task 16
            // string result = GetCarsWithTheirListOfParts(context); // Task 17
            // string result = GetTotalSalesByCustomer(context); // Task 18
            string result = GetSalesWithAppliedDiscount(context); // Task 19

            Console.WriteLine(result);
        }

        // IMPORT
        // Task 9
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            // Declare a Mapper
            // The Mapper needs configuration so we declare a MapperConfiguration
            // The MapperCofiguration needs Profile so we pass the Profile we created
            IMapper mapper = new Mapper(new MapperConfiguration(config =>
            {
                config.AddProfile<CarDealerProfile>();
            }));

            ImportSuplierDto[] supliersDtos = JsonConvert.DeserializeObject<ImportSuplierDto[]>(inputJson);

            ICollection<Supplier> validSuppliers = new HashSet<Supplier>();

            foreach (ImportSuplierDto supplierDto in supliersDtos)
            {
                Supplier supplier = mapper.Map<Supplier>(supplierDto);

                validSuppliers.Add(supplier);
            }

            context.Suppliers.AddRange(validSuppliers);
            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Count}.";
        }

        // Task 10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(config =>
            {
                config.AddProfile<CarDealerProfile>();
            }));

            ImportPartDto[] partsDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

            ICollection<Part> validParts = new HashSet<Part>();

            foreach (ImportPartDto partDto in partsDtos)
            {
                if (context.Suppliers.Any(s => s.Id == partDto.SupplierId))
                {
                    Part part = mapper.Map<Part>(partDto);

                    validParts.Add(part);
                }
            }

            context.Parts.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}.";
        }

        // Task 11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(config =>
            {
                config.AddProfile<CarDealerProfile>();
            }));

            ImportCarDto[] carsDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            ICollection<Car> validCars = new HashSet<Car>();

            foreach (ImportCarDto carDto in carsDtos)
            {
                Car car = mapper.Map<Car>(carDto);

                // We have to check if every part of the car is contained in the PartsCars collection and if not to add it
                foreach (var partId in carDto.PartsId)
                {
                    bool isNotContained = car.PartsCars.FirstOrDefault(pc => pc.PartId == partId) == null;
                    bool isValidPart = context.Parts.Any(p => p.Id == partId);

                    if (isNotContained && isValidPart)
                    {
                        car.PartsCars.Add(new PartCar
                        {
                            PartId = partId
                        });
                    }
                }

                validCars.Add(car);
            }

            context.Cars.AddRange(validCars);
            context.SaveChanges();

            return $"Successfully imported {validCars.Count}.";
        }

        // Task 12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(config =>
            {
                config.AddProfile<CarDealerProfile>();
            }));

            ImportCustomerDto[] customersDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);

            ICollection<Customer> validCustomers = new HashSet<Customer>();

            foreach (ImportCustomerDto customerDto in customersDtos)
            {
                Customer customer = mapper.Map<Customer>(customerDto);

                validCustomers.Add(customer);
            }

            context.Customers.AddRange(validCustomers);
            context.SaveChanges();

            return $"Successfully imported {validCustomers.Count}.";
        }

        // Task 13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(config =>
            {
                config.AddProfile<CarDealerProfile>();
            }));

            ImportSaleDto[] salesDtos = JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson);

            ICollection<Sale> validSales = new HashSet<Sale>();

            foreach (ImportSaleDto saleDto in salesDtos)
            {
                Sale sale = mapper.Map<Sale>(saleDto);

                validSales.Add(sale);
            }

            context.Sales.AddRange(validSales);
            context.SaveChanges();

            return $"Successfully imported {validSales.Count}.";
        }

        // EXPORT
        // Task 14
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                    .OrderBy(c => c.BirthDate)
                    .ThenBy(c => c.IsYoungDriver)
                    .Select(c => new
                    {
                        c.Name,
                        BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                        c.IsYoungDriver
                    })
                    .AsNoTracking()
                    .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        // Task 15
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                    .Where(c => c.Make == "Toyota")
                    .OrderBy(c => c.Model)
                    .ThenByDescending(c => c.TraveledDistance)
                    .Select(c => new
                    {
                        c.Id,
                        c.Make,
                        c.Model,
                        c.TraveledDistance
                    })
                    .AsNoTracking()
                    .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        // Task 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                    .Where(s => !s.IsImporter)
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        PartsCount = s.Parts.Count
                    })
                    .AsNoTracking()
                    .ToArray();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        // Task 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                    .Select(c => new
                    {
                        car = new
                        {
                            c.Make,
                            c.Model,
                            c.TraveledDistance
                        },
                        parts = c.PartsCars.Select(pc => new
                        {
                            Name = pc.Part.Name,
                            Price = pc.Part.Price.ToString("f2")
                        })
                    })
                    .AsNoTracking()
                    .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        // Task 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customersSales = context.Customers
                    .Where(c => c.Sales.Any())
                    .Select(c => new
                    {
                        fullName = c.Name,
                        boughtCars = c.Sales.Count,
                        totalSales = c.Sales.SelectMany(s => s.Car.PartsCars.Select(pc => pc.Part.Price))
                    })
                    .ToArray();

            var totalSalesByCustomer = customersSales
                    .Select(c => new
                    {
                        c.fullName,
                        c.boughtCars,
                        spentMoney = c.totalSales.Sum()
                    })
                    .OrderByDescending(c => c.spentMoney)
                    .ThenByDescending(c => c.boughtCars)
                    .ToArray();

            return JsonConvert.SerializeObject(totalSalesByCustomer, Formatting.Indented);
        }

        // Task 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales.Take(10)
                    .Select(s => new
                    {
                        car = new
                        {
                            Make = s.Car.Make,
                            Model = s.Car.Model,
                            TraveledDistance = s.Car.TraveledDistance
                        },
                        customerName = s.Customer.Name,
                        discount = s.Discount.ToString("f2"),
                        price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("f2"),
                        priceWithDiscount = (s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - (s.Discount / 100))).ToString("f2")
                    })
                    .AsNoTracking()
                    .ToArray();

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}