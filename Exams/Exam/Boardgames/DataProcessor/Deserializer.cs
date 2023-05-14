namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Boardgames.Utilities;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            XmlHelper xmlHelper = new XmlHelper();
            StringBuilder result = new StringBuilder();

            ImportCreatorDto[] creatorDtos = xmlHelper.Deserialize<ImportCreatorDto[]>(xmlString, "Creators");

            ICollection<Creator> validCreators = new HashSet<Creator>();

            foreach (ImportCreatorDto creatorDto in creatorDtos)
            {
                if (!IsValid(creatorDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                ICollection<Boardgame> validBoardgames = new HashSet<Boardgame>();

                foreach (ImportBoardgameDto boardgameDto in creatorDto.Boardgames)
                {
                    if (!IsValid(boardgameDto))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    Boardgame boardgame = new Boardgame()
                    {
                        Name = boardgameDto.Name,
                        Rating = boardgameDto.Rating,
                        YearPublished = boardgameDto.YearPublished,
                        CategoryType = (CategoryType)boardgameDto.CategoryType,
                        Mechanics = boardgameDto.Mechanics
                    };

                    validBoardgames.Add(boardgame);
                }

                Creator creator = new Creator()
                {
                    FirstName = creatorDto.FirstName,
                    LastName = creatorDto.LastName,
                    Boardgames = validBoardgames
                };

                validCreators.Add(creator);

                result.AppendLine(string.Format(SuccessfullyImportedCreator, creator.FirstName, creator.LastName, creator.Boardgames.Count));
            }

            context.Creators.AddRange(validCreators);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportSellerDto[] sellerDtos = JsonConvert.DeserializeObject<ImportSellerDto[]>(jsonString);

            ICollection<Seller> validSellers = new HashSet<Seller>();

            ICollection<int> existingBoardgameIds = context.Boardgames.Select(b => b.Id).ToArray();

            foreach (ImportSellerDto sellerDto in sellerDtos)
            {
                if (!IsValid(sellerDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Seller seller = new Seller()
                {
                    Name = sellerDto.Name,
                    Address = sellerDto.Address,
                    Country = sellerDto.Country,
                    Website = sellerDto.Website
                };

                foreach (int boardgameId in sellerDto.BoardgameIds.Distinct())
                {
                    if (!existingBoardgameIds.Contains(boardgameId))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    BoardgameSeller boardGameSeller = new BoardgameSeller()
                    {
                        Seller = seller,
                        BoardgameId = boardgameId
                    };

                    seller.BoardgamesSellers.Add(boardGameSeller);
                }

                validSellers.Add(seller);
                result.AppendLine(string.Format(SuccessfullyImportedSeller, seller.Name, seller.BoardgamesSellers.Count));
            }

            context.Sellers.AddRange(validSellers);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
