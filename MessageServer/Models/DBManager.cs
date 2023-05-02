using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using LibObjects;

namespace MessageServer.Models
{
    public class DBManager : IDisposable
    {
        private MySqlConnection connection;
        private readonly string connectionString;
        private bool logging = true;

        public DBManager(string server, string database, string username, string password)
        {
            connectionString = $"Server={server};Database={database};User Id={username};Password={password};";

            int retries = 0;
            while (true)
            {
                try
                {
                    connection = new MySqlConnection(connectionString);
                    connection.Open();
                    break;
                }
                catch (MySqlException)
                {
                    if (retries < 3)
                    {
                        retries++;
                        Console.WriteLine($"Failed to connect to database. Retrying in 5 seconds... ({retries}/3)");
                        Thread.Sleep(5000);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
        public bool ValidateAccount(string userName, string passWord)
        {
            int numberOfResults;

            using (var command = new MySqlCommand("SELECT COUNT(*) FROM Users WHERE UserName = @UserName AND Password = @Password COLLATE utf8mb4_general_ci", connection))
            {
                command.Parameters.AddWithValue("@UserName", userName);
                command.Parameters.AddWithValue("@Password", passWord);

                numberOfResults = Convert.ToInt32(command.ExecuteScalar());
            }

            if (logging)
            {
                Console.WriteLine($"Validate Account Found {numberOfResults} Accounts");
            }

            return numberOfResults > 0;
        }

        public void PersistMessageToUser(User user, string message)
        {
            throw new NotImplementedException();
        }

        public void CloseConnection()
        {
            try
            {
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error closing connection: " + ex.ToString());
            }
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}