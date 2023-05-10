using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            // IMPORT
            // string inputXml = File.ReadAllText("../../../Datasets/users.xml"); // Task 1
            // string inputXml = File.ReadAllText("../../../Datasets/products.xml"); // Task 2
            // string inputXml = File.ReadAllText("../../../Datasets/categories.xml"); // Task 3
            // string inputXml = File.ReadAllText("../../../Datasets/categories-products.xml"); // Task 4

            // IMPORT
            // string result = ImportUsers(context, inputXml); // Task 1
            // string result = ImportProducts(context, inputXml); // Task 2
            // string result = ImportCategories(context, inputXml); // Task 3
            // string result = ImportCategoryProducts(context, inputXml); // Task 4

            // EXPORT
            // string result = GetProductsInRange(context); // Task 5
            // string result = GetSoldProducts(context); // Task 6
            // string result = GetCategoriesByProductsCount(context); // Task 7
            string result = GetUsersWithProducts(context); // Task 8

            Console.WriteLine(result);
        }

        // IMPORT
        // Task 1
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportUserDto[] userDtos = xmlHelper.Deserialize<ImportUserDto[]>(inputXml, "Users");

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
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportProductDto[] productDtos = xmlHelper.Deserialize<ImportProductDto[]>(inputXml, "Products");

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
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportCategoryDto[] categoryDtos = xmlHelper.Deserialize<ImportCategoryDto[]>(inputXml, "Categories");

            ICollection<Category> validCategories = new HashSet<Category>();

            foreach (ImportCategoryDto categoryDto in categoryDtos)
            {
                if (string.IsNullOrEmpty(categoryDto.Name))
                {
                    continue;
                }

                Category category = mapper.Map<Category>(categoryDto);

                validCategories.Add(category);
            }

            context.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        // Task 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportCategoryProductDto[] categoryProductDtos = xmlHelper.Deserialize<ImportCategoryProductDto[]>(inputXml, "CategoryProducts");

            ICollection<CategoryProduct> validCategoryProducts = new HashSet<CategoryProduct>();

            foreach (ImportCategoryProductDto categoryProductDto in categoryProductDtos)
            {
                if (!context.Categories.Any(c => c.Id == categoryProductDto.CategoryId) || !context.Products.Any(p => p.Id == categoryProductDto.ProductId))
                {
                    continue;
                }

                CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(categoryProductDto);

                validCategoryProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(validCategoryProducts);
            context.SaveChanges();

            return $"Successfully imported {validCategoryProducts.Count}";
        }

        // EXPORT
        // Task 5
        public static string GetProductsInRange(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportProductDto[] products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ProjectTo<ExportProductDto>(mapper.ConfigurationProvider)
                .ToArray();

            string result = xmlHelper.Serialize<ExportProductDto[]>(products, "Products");

            return result;
        }

        // Task 6
        public static string GetSoldProducts(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportUserWithProductsDto[] usersWithProducts = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ProjectTo<ExportUserWithProductsDto>(mapper.ConfigurationProvider)
                .ToArray();

            string result = xmlHelper.Serialize<ExportUserWithProductsDto[]>(usersWithProducts, "Users");

            return result;
        }

        // Task 7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportCategoryDto[] categories = context.Categories
                .ProjectTo<ExportCategoryDto>(mapper.ConfigurationProvider)
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalPrice)
                .ToArray();

            string result = xmlHelper.Serialize<ExportCategoryDto[]>(categories, "Categories");

            return result;
        }

        // Task 8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            XmlHelper xmlHelper = new XmlHelper();

            ExportUserInfoDto[] usersInfo = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderByDescending(u => u.ProductsSold.Count())
                .Select(u => new ExportUserInfoDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ExportSoldProductsAndCountDto()
                    {
                        Count = u.ProductsSold.Count(),
                        Products = u.ProductsSold.Select(p => new ExportUserProductDto()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .Take(10)
                .ToArray();

            ExportUsersAndCountDto usersAndCount = new ExportUsersAndCountDto()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = usersInfo
            };

            string result = xmlHelper.Serialize<ExportUsersAndCountDto>(usersAndCount, "Users");

            return result;
        }

        private static IMapper InitializeAutoMapper()
        {
            IMapper mapper = new Mapper(new MapperConfiguration(config =>
            {
                config.AddProfile<ProductShopProfile>();
            }));

            return mapper;
        }
    }
}