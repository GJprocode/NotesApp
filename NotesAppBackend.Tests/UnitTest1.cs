using System;
using Microsoft.Data.SqlClient;
using Xunit;

namespace NotesAppBackend.Tests
{
    public class DatabaseHelperTests
    {
        private readonly string _connectionString = 
            "Server=GERT,1433;Database=NotesProjectDB;User Id=NotesAppUser;Password=Success100%;TrustServerCertificate=True;";

        [Fact]
        public void TestDatabaseConnection_ShouldOpenSuccessfully()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    Assert.Equal(System.Data.ConnectionState.Open, connection.State);
                    Console.WriteLine("Connected to SQL Server successfully!");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"Database connection failed: {ex.Message}");
            }
        }
    }
}
