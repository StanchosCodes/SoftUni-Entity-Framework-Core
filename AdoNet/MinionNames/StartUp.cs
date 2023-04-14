using Microsoft.Data.SqlClient;
using System.Text;

namespace MinionNames
{
    internal class StartUp
    {
        static async Task Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());

            await using SqlConnection connection = new SqlConnection(Config.ConnectionString);

            await connection.OpenAsync();

            string result = await GetMinionAndVillainInfo(connection, villainId);

            Console.WriteLine(result);
        }

        private static async Task<string> GetMinionAndVillainInfo(SqlConnection sqlConnection, int villainId)
        {
            string villainByIdQuery = @"SELECT Name FROM Villains WHERE Id = @Id";
            string villainsAndMinionsCount = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) AS RowNum,
                                                      m.Name, 
                                                      m.Age
                                                 FROM MinionsVillains AS mv
                                                 JOIN Minions As m ON mv.MinionId = m.Id
                                                WHERE mv.VillainId = @Id
                                             ORDER BY m.Name";

            StringBuilder result = new StringBuilder();

            SqlCommand getVillainNameCmd = new SqlCommand(villainByIdQuery, sqlConnection);
            // The way to prevent sql injection attack is to use parameters on the command
            getVillainNameCmd.Parameters.AddWithValue("@Id", villainId);

            // The result will be one column with one row so we can use ExecuteScalar()
            object? villainNameObj = await getVillainNameCmd.ExecuteScalarAsync();

            if(villainNameObj == null)
            {
                return $"No villain with ID {villainId} exists in the database.";
            }

            string villainName = (string)villainNameObj;

            result.AppendLine($"Villain: {villainName}");

            SqlCommand getMinionsCmd = new SqlCommand(villainsAndMinionsCount, sqlConnection);
            getMinionsCmd.Parameters.AddWithValue("@Id", villainId);

            // The result wil be many column with many rows so we must use SqlDataReader
            SqlDataReader minionsReader = await getMinionsCmd.ExecuteReaderAsync();

            if (!minionsReader.HasRows)
            {
                result.AppendLine("(no minions)");
            }
            else
            {
                while(minionsReader.Read())
                {
                    long rowNum = (long)minionsReader["RowNum"];
                    string name = (string)minionsReader["Name"];
                    int age = (int)minionsReader["Age"];

                    result.AppendLine($"{rowNum}. {name} {age}");
                }
            }

            return result.ToString();
        }
    }
}