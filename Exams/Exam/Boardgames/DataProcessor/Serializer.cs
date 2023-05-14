namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Boardgames.Utilities;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            XmlHelper xmlHelper = new XmlHelper();

            ExportCreatorDto[] creators = context.Creators
                .Where(c => c.Boardgames.Any())
                .Select(c => new ExportCreatorDto()
                {
                    CreatorName = $"{c.FirstName} {c.LastName}",
                    BoardgamesCount = c.Boardgames.Count,
                    Boardgames = c.Boardgames.Select(b => new ExportBoardgameDto()
                    {
                        BoardgameName = b.Name,
                        BoardgameYearPublished = b.YearPublished
                    })
                    .OrderBy(b => b.BoardgameName)
                    .ToArray()
                })
                .ToArray()
                .OrderByDescending(c => c.BoardgamesCount)
                .ThenBy(c => c.CreatorName)
                .ToArray();

            string result = xmlHelper.Serialize<ExportCreatorDto[]>(creators, "Creators");

            return result;
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellers = context.Sellers
                    .AsNoTracking()
                    .Where(s => s.BoardgamesSellers.Any(bs => bs.Boardgame.YearPublished >= year && bs.Boardgame.Rating <= rating))
                    .Select(s => new
                    {
                        Name = s.Name,
                        Website = s.Website,
                        Boardgames = s.BoardgamesSellers
                        .Where(bs => bs.Boardgame.YearPublished >= year && bs.Boardgame.Rating <= rating)
                        .Select(bs => new
                        {
                            Name = bs.Boardgame.Name,
                            Rating = bs.Boardgame.Rating,
                            Mechanics = bs.Boardgame.Mechanics,
                            Category = bs.Boardgame.CategoryType.ToString()
                        })
                        .OrderByDescending(b => b.Rating)
                        .ThenBy(b => b.Name)
                        .ToArray()
                    })
                    .OrderByDescending(s => s.Boardgames.Length)
                    .ThenBy(s => s.Name)
                    .Take(5)
                    .ToArray();

            string result = JsonConvert.SerializeObject(sellers, Formatting.Indented);

            return result;
        }
    }
}