using Microsoft.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace AddMinion
{
    internal class StartUp
    {
        static async Task Main(string[] args)
        {
            string[] minionArgs = Console.ReadLine().Split(": ", StringSplitOptions.RemoveEmptyEntries).ToArray();
            string[] villainArgs = Console.ReadLine().Split(": ", StringSplitOptions.RemoveEmptyEntries).ToArray();

            // minionArgs[1] = Name Age TownName

            // villainArgs[1] = Name

            await using SqlConnection connection = new SqlConnection(Config.ConnectionString);
            await connection.OpenAsync();

            string result = await AddMinionAsync(connection, minionArgs[1], villainArgs[1]);

            Console.WriteLine(result);
        }

        private static async Task<string> AddMinionAsync(SqlConnection connection, string minionInfo, string villainName)
        {
            string[] minionArgs = minionInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();

            string minionName = minionArgs[0];
            int minionAge = int.Parse(minionArgs[1]);
            string minionTown = minionArgs[2];

            StringBuilder result = new StringBuilder();

            // Making a transaction so if one operation is not successfull none of them will take affect

            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                int townId = await GetTownIdByNameAsync(connection, transaction, result, minionTown);
                int villainId = await GetVillainIdByNameAsync(connection, transaction, result, villainName);
                int minionId = await AddMinionAndGetIdAsync(connection, transaction, minionName, minionAge, townId);

                await SetMinionAServentOfAVillainAsync(connection, transaction, minionId, villainId);

                result.AppendLine($"Successfully added {minionName} to be minion of {villainName}");

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                result.AppendLine("Operation Failed");
            }

            return result.ToString();
        }

        private static async Task<int> GetTownIdByNameAsync(SqlConnection connection, SqlTransaction transaction, StringBuilder result, string minionTown)
        {
            string getTownIdQuery = @"SELECT Id FROM Towns WHERE Name = @townName";
            string addTownQuery = @"INSERT INTO Towns (Name) VALUES (@townName)";

            SqlCommand getTownIdCmd = new SqlCommand(getTownIdQuery, connection, transaction);
            getTownIdCmd.Parameters.AddWithValue("@townName", minionTown);

            int? townId = (int?)await getTownIdCmd.ExecuteScalarAsync(); // gtting the town id

            if (!townId.HasValue) // checking if the town exists
            {
                // if it does not then we add it
                SqlCommand addTownCmd = new SqlCommand(addTownQuery, connection, transaction);
                addTownCmd.Parameters.AddWithValue("@townName", minionTown);

                await addTownCmd.ExecuteNonQueryAsync();

                // taking the id of the added town
                townId = (int?)await getTownIdCmd.ExecuteScalarAsync();

                result.AppendLine($"Town {minionTown} was added to the database.");
            }

            return townId.Value;
        }

        private static async Task<int> GetVillainIdByNameAsync(SqlConnection connection, SqlTransaction transaction, StringBuilder result, string villainName)
        {
            string getIdQuery = @"SELECT Id FROM Villains WHERE Name = @Name";
            string addVillainQuery = @"INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";

            SqlCommand getVillainIdCmd = new SqlCommand(getIdQuery, connection, transaction);
            getVillainIdCmd.Parameters.AddWithValue("@Name", villainName);

            int? villainId = (int?)await getVillainIdCmd.ExecuteScalarAsync();

            if (!villainId.HasValue)
            {
                SqlCommand addVillainCmd = new SqlCommand(addVillainQuery, connection, transaction);
                addVillainCmd.Parameters.AddWithValue("@villainName", villainName);

                await addVillainCmd.ExecuteNonQueryAsync(); // using nonQuery because we dont need result (afected rows is enough)

                villainId = (int?)await getVillainIdCmd.ExecuteScalarAsync();

                result.AppendLine($"Villain {villainName} was added to the database.");
            }

            return villainId.Value;
        }

        private static async Task<int> AddMinionAndGetIdAsync(SqlConnection connection, SqlTransaction transaction, string minionName, int minionAge, int minionTownId)
        {
            string addMinionQuery = @"INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";
            string getMinionIdQuery = @"SELECT Id FROM Minions WHERE Name = @Name";

            SqlCommand addMinionCmd = new SqlCommand(addMinionQuery, connection, transaction);
            addMinionCmd.Parameters.AddWithValue("@name", minionName);
            addMinionCmd.Parameters.AddWithValue("@age", minionAge);
            addMinionCmd.Parameters.AddWithValue("@townId", minionTownId);

            await addMinionCmd.ExecuteNonQueryAsync();

            SqlCommand getMinionId = new SqlCommand(getMinionIdQuery, connection, transaction);
            getMinionId.Parameters.AddWithValue("@Name", minionName);

            int minionId = (int)await getMinionId.ExecuteScalarAsync();

            return minionId;
        }

        private static async Task SetMinionAServentOfAVillainAsync(SqlConnection connection, SqlTransaction transaction, int minionId, int villainId)
        {
            string setMinionQuery = @"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";

            SqlCommand setMinionCmd = new SqlCommand(setMinionQuery, connection, transaction);
            setMinionCmd.Parameters.AddWithValue("@minionId", minionId);
            setMinionCmd.Parameters.AddWithValue("@villainId", villainId);

            await setMinionCmd.ExecuteNonQueryAsync();
        }
    }
}