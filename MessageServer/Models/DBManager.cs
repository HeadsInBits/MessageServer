using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;  // MariaDB .NET connector
using System.Threading;
using LibObjects;
using Microsoft.Data.Sqlite;

namespace MessageServer.Models
{
    public class DBManager : IDisposable
    {
        private DbConnection connection;
        private readonly string connectionString;
        private DbConnectionType connectionType;
        private bool logging = true;

        public DBManager(string server, string database, string username, string password, DbConnectionType type)
        {
            connectionType = type;

            switch (connectionType)
            {
                case DbConnectionType.AzureSql:
                    connectionString = $"Server=tcp:{server}.database.windows.net,1433;Database={database};User ID={username}@{server};Password={password};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                    connection = new System.Data.SqlClient.SqlConnection(connectionString);
                    break;
                case DbConnectionType.MariaDb:
                    connectionString = $"Server={server};Database={database};User Id={username};Password={password};";
                    connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
                    break;
                case DbConnectionType.SqLite:
                    connectionString = $"Data Source={server}";
                    connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
                    break;
                case DbConnectionType.InMemory: 
                    connectionString = $"Data Source={server};Mode=Memory;Cache=Shared";
                    connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            int retries = 0;
            Console.WriteLine("Starting Database Connection.." + "\n");

            while (true)
            {
                try
                {
                    connection.Open();
                    break;
                }
                catch (DbException e)
                {
                    if (retries < 3)
                    {
                        retries++;
                        Console.WriteLine($"Failed to connect to database. Retrying in 5 seconds... ({retries}/3)");
                        Console.WriteLine(e.Message);
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
            using (var command = connection.CreateCommand())
            {
               // command.CommandText = "SELECT COUNT(*) FROM Users WHERE UserName = @UserName AND Password = @Password COLLATE SQL_Latin1_General_CP1_CI_AS";
               command.CommandText = "SELECT COUNT(*) FROM Users WHERE UserName = @UserName AND Password = @Password COLLATE utf8mb4_general_ci";
                var usernameParam = command.CreateParameter();
                usernameParam.ParameterName = "@UserName";
                usernameParam.Value = userName;
                command.Parameters.Add(usernameParam);

                var passwordParam = command.CreateParameter();
                passwordParam.ParameterName = "@Password";
                passwordParam.Value = passWord;
                command.Parameters.Add(passwordParam);

                try {
                    numberOfResults = Convert.ToInt32(command.ExecuteScalar());
                    if (logging) {
                        Console.WriteLine($"Validate Account Found {numberOfResults} Accounts");
                    }

                    return numberOfResults > 0;
                }
                catch (SqlException ex) {
                    Console.WriteLine($"==== *** SQL EXCEPTION: {ex.Message} *** ====");
                }
            }

            return false;
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
