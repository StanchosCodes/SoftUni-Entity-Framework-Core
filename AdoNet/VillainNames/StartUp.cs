using Microsoft.Data.SqlClient;
using System.Text;

namespace VillainNames
{
    public class StartUp
    {
        static async Task Main(string[] args)
        {
            await using SqlConnection connection = new SqlConnection(Config.ConnectionString);

            await connection.OpenAsync();

            string result = await GetVillainNamesAndCountMinions(connection);

            Console.WriteLine(result);
        }

        static async Task<string> GetVillainNamesAndCountMinions(SqlConnection sqlConnection)
        {
            string sqlQuery = @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                  FROM Villains AS v 
                                  JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                              GROUP BY v.Id, v.Name 
                                HAVING COUNT(mv.VillainId) > 3 
                              ORDER BY COUNT(mv.VillainId)";

            SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
            SqlDataReader reader = await command.ExecuteReaderAsync();

            // the reader hasnt loade'd any data before we call Read(), that is because in sql the rows start from one but here starts from 0 so we dont have data on zero position, then in a while cicle we iterate through the data, when Read() return false it means no data is left in the reader

            StringBuilder result = new StringBuilder();

            while (reader.Read())
            {
                string villainName = (string)reader["Name"]; // we cast because reader gives us an object
                int minionsCount = (int)reader["MinionsCount"];

                result.AppendLine($"{villainName} - {minionsCount}");
            }

                return result.ToString();
        }
    }
}