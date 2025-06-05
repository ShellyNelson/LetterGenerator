using LetterGeneration.DataAccess;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LetterGeneration.Tests.TestHelpers
{
    public static class DatabaseTestHelper
    {
        public static async Task InitializeTestDatabase(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Create test database if it doesn't exist
            var createDbCommand = new SqlCommand(
                @"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LetterGenerationTest')
                BEGIN
                    CREATE DATABASE LetterGenerationTest;
                END", connection);
            await createDbCommand.ExecuteNonQueryAsync();

            // Switch to test database
            connection.ChangeDatabase("LetterGenerationTest");

            // Create Letters table if it doesn't exist
            var createTableCommand = new SqlCommand(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Letters')
                BEGIN
                    CREATE TABLE Letters (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Title NVARCHAR(200) NOT NULL,
                        Content NVARCHAR(MAX) NOT NULL,
                        Recipient NVARCHAR(200) NOT NULL,
                        CreatedDate DATETIME2 NOT NULL,
                        SentDate DATETIME2 NULL,
                        Status NVARCHAR(50) NOT NULL
                    );
                END", connection);
            await createTableCommand.ExecuteNonQueryAsync();
        }

        public static async Task CleanupTestDatabase(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Switch to test database
            connection.ChangeDatabase("LetterGenerationTest");

            // Delete all records from Letters table
            var deleteCommand = new SqlCommand("DELETE FROM Letters", connection);
            await deleteCommand.ExecuteNonQueryAsync();
        }

        public static DatabaseContext CreateTestContext(string connectionString)
        {
            return new DatabaseContext(connectionString);
        }
    }
} 