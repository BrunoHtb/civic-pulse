using Npgsql;

namespace CivicPulse.IntegrationTests
{
    public static class DbReset
    {
        public static async Task ResetAsync(string connString)
        {
            await using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync();

            var sql = """
                TRUNCATE TABLE
                    public."Measurements",
                    public."IngestionRuns",
                    public."Stations",
                    public."Sources"
                RESTART IDENTITY CASCADE;
                """;
            
            await using var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
        }

        public static string GetTestDbConnectionString() => 
            "Host=localhost;Port=5433;Database=civicpulse;Username=postgres;Password=postgres";
    }
}
