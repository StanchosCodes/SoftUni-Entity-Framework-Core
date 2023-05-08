using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            // IMPORT
            // string inputJson = File.ReadAllText(@"../../../Datasets/users.json"); // Task 1
            // string inputJson = File.ReadAllText(@"../../../Datasets/products.json"); // Task 2
            // string inputJson = File.ReadAllText(@"../../../Datasets/categories.json"); // Task 3
            // string inputJson = File.ReadAllText(@"../../../Datasets/categories-products.json"); // Task 4

            // string result = ImportUsers(context, inputJson); // Task 1
            // string result = ImportProducts(context, inputJson); // Task 2
            // string result = ImportCategories(context, inputJson); // Task 3
            // string result = ImportCategoryProducts(context, inputJson); // Task 4

            // EXPORT
            // string result = GetProductsInRange(context); // Task 5
            // string result = GetSoldProducts(context); // Task 6
            // string result = GetCategoriesByProductsCount(context); // Task 7
            string result = GetUsersWithProducts(context); // Task 8

            Console.WriteLine(result);
        }
        // IMPORT
        // Task 1
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            // Declare a Mapper
            // The Mapper needs configuration so we declare a MapperConfiguration
            // The MapperCofiguration needs Profile so we pass the Profile we created
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            // we deserialize the json input to many ImportUserDto objects because the input is an array of Dtos
            ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            ICollection<User> validUsers = new HashSet<User>();

            foreach (ImportUserDto userDto in userDtos)
            {
                User user = mapper.Map<User>(userDto);

                validUsers.Add(user);
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }

        // Task 2
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(config =>
            {
                config.AddProfile<ProductShopProfile>();
            }));

            ImportProductDto[] productDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson);

            ICollection<Product> validProducts = new HashSet<Product>();

            foreach (ImportProductDto productDto in productDtos)
            {
                Product product = mapper.Map<Product>(productDto);

                validProducts.Add(product);
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return $"Successfully imported {validProducts.Count}";
        }

        // Task 3
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            ImportCategoryDto[] categoryDtos = JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson);

            ICollection<Category> validCategories = new HashSet<Category>();

            foreach (ImportCategoryDto categoryDto in categoryDtos)
            {
                if (categoryDto.Name != null)
                {
                    Category category = mapper.Map<Category>(categoryDto);

                    validCategories.Add(category);
                }
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        // Task 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(config =>
            {
                config.AddProfile<ProductShopProfile>();
            }));

            ImportCategoryProductDto[] categoryProductDtos = JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson);

            ICollection<CategoryProduct> validCategoriesProducts = new HashSet<CategoryProduct>();

            foreach (ImportCategoryProductDto categoryProductDto in categoryProductDtos)
            {
                CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(categoryProductDto);

                validCategoriesProducts.Add(categoryProduct);
            }

            context.CategoriesProducts.AddRange(validCategoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {validCategoriesProducts.Count}";
        }

        // EXPORT
        // Task 5
        public static string GetProductsInRange(ProductShopContext context)
        {
            // WITH ANONYMOUS OBJECT
            //var products = context.Products
            //        .Where(p => p.Price >= 500 && p.Price <= 1000)
            //        .OrderBy(p => p.Price)
            //        .Select(p => new
            //        {
            //            name = p.Name,
            //            price = p.Price,
            //            seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
            //        })
            //        .AsNoTracking()
            //        .ToArray();

            //return JsonConvert.SerializeObject(products, Formatting.Indented);

            // WITH DTO
            IMapper mapper = new Mapper(new MapperConfiguration(config =>
            {
                config.AddProfile<ProductShopProfile>();
            }));

            ExportProductInRangeDto[] productsInRange = context.Products
                 .Where(p => p.Price >= 500 && p.Price <= 1000)
                 .OrderBy(p => p.Price)
                 .AsNoTracking()
                 .ProjectTo<ExportProductInRangeDto>(mapper.ConfigurationProvider)
                 .ToArray();

            return JsonConvert.SerializeObject(productsInRange, Formatting.Indented);
        }

        // Task 6
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                    .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new
                    {
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        soldProducts = u.ProductsSold.Select(p => new
                        {
                            name = p.Name,
                            price = p.Price,
                            buyerFirstName = p.Buyer.FirstName,
                            buyerLastName = p.Buyer.LastName
                        })
                        .ToArray()
                    })
                    .AsNoTracking()
                    .ToArray();

            return JsonConvert.SerializeObject(users, Formatting.Indented);
        }

        // Task 7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                   .OrderByDescending(c => c.CategoriesProducts.Count)
                   .Select(c => new
                   {
                       category = c.Name,
                       productsCount = c.CategoriesProducts.Count,
                       averagePrice = (c.CategoriesProducts.Any() ? c.CategoriesProducts.Average(cp => cp.Product.Price) : 0).ToString("f2"),
                       totalRevenue = c.CategoriesProducts.Sum(cp => cp.Product.Price).ToString("f2")
                   })
                   .AsNoTracking()
                   .ToArray();

            string result = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return result;
        }

        // Task 8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                    .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                    .OrderByDescending(u => u.ProductsSold.Count(p => p.Buyer != null))
                    .Select(u => new
                    { // UserDto
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        age = u.Age,
                        soldProducts = new
                        { // ProductWrapperDto
                            count = u.ProductsSold.Count(p => p.Buyer != null),
                            products = u.ProductsSold
                            .Where(p => p.Buyer != null)
                            .Select(p => new
                            { // ProductDto
                                name = p.Name,
                                price = p.Price
                            })
                            .ToArray()
                        }
                    })
                    .AsNoTracking()
                    .ToArray();

            var usersWrapper = new
            { // UserWrapperDto
                usersCount = users.Length,
                users = users
            };

            return JsonConvert.SerializeObject(usersWrapper, Formatting.Indented, new JsonSerializerSettings
            {
                // in this case firstName can be null so we want to ignore it at all if it is and display it if it is not
                NullValueHandling = NullValueHandling.Ignore // ignores properties with null value
            });
        }
    }
}