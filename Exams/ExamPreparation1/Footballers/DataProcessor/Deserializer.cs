namespace Footballers.DataProcessor
{
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            StringBuilder result = new StringBuilder();

            XmlRootAttribute coachXmlRoot = new XmlRootAttribute("Coaches");
            XmlSerializer coachSerializer = new XmlSerializer(typeof(ImportCoachDto[]), coachXmlRoot);

            using StringReader reader = new StringReader(xmlString);

            ImportCoachDto[] coachDtos = (ImportCoachDto[])coachSerializer.Deserialize(reader);
            ICollection<Coach> validCoaches = new HashSet<Coach>();

            foreach (ImportCoachDto coachDto in coachDtos)
            {
                Coach coach = new Coach()
                {
                    Name = coachDto.Name,
                    Nationality = coachDto.Nationality
                };

                if (!IsValid(coach))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                foreach (ImportFootballerDto footballerDto in coachDto.Footballers)
                {
                    try
                    {
                        Footballer footballer = new Footballer()
                        {
                            Name = footballerDto.Name,
                            ContractStartDate = DateTime.ParseExact(footballerDto.ContractStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            ContractEndDate = DateTime.ParseExact(footballerDto.ContractEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            PositionType = (PositionType)footballerDto.PositionType,
                            BestSkillType = (BestSkillType)footballerDto.BestSkillType
                        };

                        if (!IsValid(footballer) || footballer.ContractStartDate > footballer.ContractEndDate)
                        {
                            result.AppendLine(ErrorMessage);
                            continue;
                        }

                        coach.Footballers.Add(footballer);
                    }
                    catch (Exception)
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }
                }

                result.AppendLine(string.Format(SuccessfullyImportedCoach, coach.Name, coach.Footballers.Count));
                validCoaches.Add(coach);
            }

            context.Coaches.AddRange(validCoaches);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportTeamDto[] teamDtos = JsonConvert.DeserializeObject<ImportTeamDto[]>(jsonString);
            ICollection<Team> validTeams = new HashSet<Team>();

            foreach (ImportTeamDto teamDto in teamDtos)
            {
                try
                {
                    Team team = new Team()
                    {
                        Name = teamDto.Name,
                        Nationality = teamDto.Nationality,
                        Trophies = int.Parse(teamDto.Trophies)
                    };

                    if (!IsValid(team) || team.Trophies <= 0)
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    // First variant
                    //int[] footballersIds = context.Footballers
                    //        .Select(f => f.Id)
                    //        .ToArray();

                    foreach (int footballerId in teamDto.Footballers)
                    {
                        // First variant
                        //if (!footballersIds.Contains(footballerId))
                        //{
                        //    result.AppendLine(ErrorMessage);
                        //    continue;
                        //}

                        // Second variant
                        Footballer? searchedFootballer = context.Footballers.Find(footballerId); // may return the footballer with the given Id or null

                        if (searchedFootballer == null)
                        {
                            result.AppendLine(ErrorMessage);
                            continue;
                        }

                        TeamFootballer teamFootballer = new TeamFootballer()
                        {
                            Footballer = searchedFootballer,
                            FootballerId = footballerId
                            
                        };

                        team.TeamsFootballers.Add(teamFootballer);
                    }

                    validTeams.Add(team);

                    result.AppendLine(string.Format(SuccessfullyImportedTeam, team.Name, team.TeamsFootballers.Count));
                }
                catch (Exception)
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }
            }

            context.Teams.AddRange(validTeams);
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
