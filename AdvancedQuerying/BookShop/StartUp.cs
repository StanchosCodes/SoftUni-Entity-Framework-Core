namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var dbContext = new BookShopContext();
            // DbInitializer.ResetDatabase(dbContext);

            // string ageRestriction = Console.ReadLine(); // Task 2
            // string resultTitles = GetBooksByAgeRestriction(dbContext, ageRestriction); // Task 2

            // string goldenBooksTitles = GetGoldenBooks(dbContext); // Task 3

            // string booksAndPrices = GetBooksByPrice(dbContext); // Task 4

            // int year = int.Parse(Console.ReadLine()); // Task 5
            // string bookTitles = GetBooksNotReleasedIn(dbContext, year); // Task 5

            // string categoriesString = Console.ReadLine(); // Task 6
            // string bookTitles = GetBooksByCategory(dbContext, categoriesString); // Task 6

            // string date = Console.ReadLine(); // Task 7
            // string bookTitles = GetBooksReleasedBefore(dbContext, date); // Task 7

            // string input = Console.ReadLine(); // Task 8, 9, 10
            // string authorsNames = GetAuthorNamesEndingIn(dbContext, input); // Task 8

            // string bookTitles = GetBookTitlesContaining(dbContext, input); // Task 9

            // string booksAuthors = GetBooksByAuthor(dbContext, input); // Task 10

            // int length = int.Parse(Console.ReadLine()); // Task 11
            // int countOfBooks = CountBooks(dbContext, length); // Task 11
            // string countOfBooksString = $"There are {countOfBooks} books with longer title than {length} symbols"; // Task 11

            // string authorsCopies = CountCopiesByAuthor(dbContext); // Task 12

            // string categorysPrices = GetTotalProfitByCategory(dbContext); // Task 13

            // string recentBooks = GetMostRecentBooks(dbContext); // Task 14

            // IncreasePrices(dbContext); // Task 15

            int removedBooksCount = RemoveBooks(dbContext); // Task 16

            Console.WriteLine(removedBooksCount);
        }

        // Task 2
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            try
            {
                AgeRestriction givenRestriction = Enum.Parse<AgeRestriction>(command, true);
                // parsing the string command to Enum so we can compare it (parse takes as arguments the command and if we want to ignore casing or not)

                string[] bookTitles = context.Books
                .Where(b => b.AgeRestriction == givenRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

                return string.Join(Environment.NewLine, bookTitles);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // Task 3
        public static string GetGoldenBooks(BookShopContext context)
        {
            string[] goldenBooksTitles = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, goldenBooksTitles);
        }

        // Task 4
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder result = new StringBuilder();

            var booksAndPrice = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .ToArray();

            foreach (var b in booksAndPrice)
            {
                result.AppendLine($"{b.Title} - ${b.Price:f2}");
            }

            return result.ToString().TrimEnd();
        }

        // Task 5
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            string[] bookTitles = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        // Task 6
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            string[] bookTitles = context.Books
                .Where(b => b.BookCategories.Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        // Task 7
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder result = new StringBuilder();

            DateTime parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate < parsedDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToArray();

            foreach (var b in books)
            {
                result.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:f2}");
            }

            return result.ToString().TrimEnd();
        }

        // Task 8
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            string[] authorsNames = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => $"{a.FirstName} {a.LastName}")
                .ToArray();

            return string.Join(Environment.NewLine, authorsNames);
        }

        // Task 9
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string[] bookTitles = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        // Task 10
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder result = new StringBuilder();

            var booksAuthors = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    AuthorFirstName = b.Author.FirstName,
                    AuthorLastName = b.Author.LastName
                })
                .ToArray();

            foreach (var b in booksAuthors)
            {
                result.AppendLine($"{b.Title} ({b.AuthorFirstName} {b.AuthorLastName})");
            }

            return result.ToString().TrimEnd();
        }

        // Task 11
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int count = context.Books
                .Where(b => b.Title.Length > lengthCheck).Count();

            return count;
        }

        // Task 12
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder result = new StringBuilder();

            var authorsCopies = context.Authors
                .Select(a => new
                {
                    FullName = $"{a.FirstName} {a.LastName}",
                    BookCopies = a.Books.Select(b => b.Copies).Sum()
                })
                .OrderByDescending(a => a.BookCopies)
                .ToArray();

            foreach (var a in authorsCopies)
            {
                result.AppendLine($"{a.FullName} - {a.BookCopies}");
            }

            return result.ToString().TrimEnd();
        }

        // Task 13
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder result = new StringBuilder();

            var categoriesWithPrices = context.Categories
                .Select(c => new
                {
                    Category = c.Name,
                    Profit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(c => c.Profit)
                .ThenBy(c => c.Category)
                .ToArray();

            foreach (var category in categoriesWithPrices)
            {
                result.AppendLine($"{category.Category} ${category.Profit:f2}");
            }

            return result.ToString().TrimEnd();
        }

        // Task 14
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder result = new StringBuilder();

            var recentBooks = context.Categories
                .Select(c => new
                {
                    Category = c.Name,
                    Books = c.CategoryBooks
                      .OrderByDescending(cb => cb.Book.ReleaseDate)
                      .Take(3)
                      .Select(cb => new
                      {
                          BookTitle = cb.Book.Title,
                          BookYear = cb.Book.ReleaseDate.Value.Year
                      })
                      .ToArray()
                })
                .OrderBy(c => c.Category)
                .ToArray();

            foreach (var c in recentBooks)
            {
                result.AppendLine($"--{c.Category}");

                foreach (var b in c.Books)
                {
                    result.AppendLine($"{b.BookTitle} ({b.BookYear})");
                }
            }

            return result.ToString().TrimEnd();
        }

        // Task 15
        public static void IncreasePrices(BookShopContext context)
        {
            int priceIncrease = 5;

            Book[] books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010)
                .ToArray(); // Materializing the query but the entities are still atached to the change tracer so we can modify them 

            foreach (Book book in books)
            {
                book.Price += priceIncrease;
            }

            context.SaveChanges();
        }

        // Task 16
        public static int RemoveBooks(BookShopContext context)
        {
            Book[] booksToRemove = context.Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            int removedBooks = booksToRemove.Length;

            context.Books.RemoveRange(booksToRemove);
            context.SaveChanges();

            return removedBooks;
        }
    }
}


