namespace Footballers.DataProcessor
{
    using Data;
    using Footballers.DataProcessor.ExportDto;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {
            var coaches = context.Coaches
                    .Where(c => c.Footballers.Any())
                    .Select(c => new ExportCoachDbo()
                    {
                        CoachName = c.Name,
                        FootballersCount = c.Footballers.Count,
                        Footballers = c.Footballers.Select(f => new ExportFootballerDbo()
                        {
                            Name = f.Name,
                            Position = f.PositionType.ToString()
                        })
                        .OrderBy(f => f.Name)
                        .ToArray()
                    })
                    .OrderByDescending(c => c.FootballersCount)
                    .ThenBy(c => c.CoachName)
                    .ToArray();

            XmlRootAttribute root = new XmlRootAttribute("Coaches");
            XmlSerializer serializer = new XmlSerializer(typeof(ExportCoachDbo[]), root);
            StringBuilder result = new StringBuilder();

            using StringWriter writer = new StringWriter(result);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, coaches, namespaces);

            return result.ToString().TrimEnd();
        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {
            var teams = context.Teams
                    .Where(t => t.TeamsFootballers.Any(tf => tf.Footballer.ContractStartDate >= date))
                    .ToArray()
                    .Select(t => new
                    {
                        Name = t.Name,
                        Footballers = t.TeamsFootballers
                            .Where(tf => tf.Footballer.ContractStartDate >= date)
                            .OrderByDescending(f => f.Footballer.ContractEndDate) // The Order shoud be here because after that we are making the date to string and we cant order by it any more as expected
                            .ThenBy(f => f.Footballer.Name)
                            .Select(tf => new
                            {
                                FootballerName = tf.Footballer.Name,
                                ContractStartDate = tf.Footballer.ContractStartDate.ToString("d", CultureInfo.InvariantCulture),
                                ContractEndDate = tf.Footballer.ContractEndDate.ToString("d", CultureInfo.InvariantCulture),
                                BestSkillType = tf.Footballer.BestSkillType.ToString(),
                                PositionType = tf.Footballer.PositionType.ToString()
                            })
                            .ToArray()
                    })
                    .OrderByDescending(t => t.Footballers.Length)
                    .ThenBy(t => t.Name)
                    .Take(5)
                    .ToArray();

            string result = JsonConvert.SerializeObject(teams, Formatting.Indented);

            return result;
        }
    }
}
