namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;
    using Trucks.Utilities;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            XmlHelper xmlHelper = new XmlHelper();
            StringBuilder result = new StringBuilder();

            ImportDespatcherDto[] despatcherDtos = xmlHelper.Deserialize<ImportDespatcherDto[]>(xmlString, "Despatchers");

            ICollection<Despatcher> validDespatchers = new HashSet<Despatcher>();

            foreach (ImportDespatcherDto despatcherDto in despatcherDtos)
            {
                if (!IsValid(despatcherDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                ICollection<Truck> validTrucks = new HashSet<Truck>();

                foreach (ImportTruckDto truckDto in despatcherDto.Trucks)
                {
                    if (!IsValid(truckDto))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    Truck truck = new Truck()
                    {
                        RegistrationNumber = truckDto.RegistrationNumber,
                        VinNumber = truckDto.VinNumber,
                        TankCapacity = truckDto.TankCapacity,
                        CargoCapacity = truckDto.CargoCapacity,
                        CategoryType = (CategoryType)truckDto.CategoryType,
                        MakeType = (MakeType)truckDto.MakeType
                    };

                    validTrucks.Add(truck);
                }

                Despatcher despatcher = new Despatcher()
                {
                    Name = despatcherDto.Name,
                    Position = despatcherDto.Position,
                    Trucks = validTrucks
                };

                validDespatchers.Add(despatcher);

                result.AppendLine(string.Format(SuccessfullyImportedDespatcher, despatcher.Name, despatcher.Trucks.Count));
            }

            context.Despatchers.AddRange(validDespatchers);
            context.SaveChanges();

            return result.ToString().TrimEnd();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            StringBuilder result = new StringBuilder();

            ImportClientDto[] clientDtos = JsonConvert.DeserializeObject<ImportClientDto[]>(jsonString);

            ICollection<Client> validClients = new HashSet<Client>();

            ICollection<int> existingTruckIds = context.Trucks.Select(t => t.Id).ToArray();

            foreach (ImportClientDto clientDto in clientDtos)
            {
                if (!IsValid(clientDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                if (clientDto.Type == "usual")
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                Client client = new Client()
                {
                    Name = clientDto.Name,
                    Nationality = clientDto.Nationality,
                    Type = clientDto.Type
                };

                foreach (int truckId in clientDto.TruckIds.Distinct())
                {
                    if (!existingTruckIds.Contains(truckId))
                    {
                        result.AppendLine(ErrorMessage);
                        continue;
                    }

                    ClientTruck clientTruck = new ClientTruck()
                    {
                        Client = client,
                        TruckId = truckId
                    };

                    // To get only the unique truckIds there are two ways:
                    // The first is to make the TruckIds prop in ImportClientDto a HashSet<int> because the hashset takes only unique values
                    // The second is in the foreach after clientDto.TruckIds to say .Distinct()

                    client.ClientsTrucks.Add(clientTruck);
                }

                result.AppendLine(string.Format(SuccessfullyImportedClient, client.Name, client.ClientsTrucks.Count));
                validClients.Add(client);
            }

            context.Clients.AddRange(validClients);
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