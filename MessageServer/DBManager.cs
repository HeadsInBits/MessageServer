using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace MessageServer;

public class DBManager
{
    private MySqlConnection connection;
    private string connectionString;

    public DBManager(string server, string database, string username, string password) {
        // Set up the connection string
        connectionString = $"Server={server};Database={database};User Id={username};Password={password};";

        // Create a new SqlConnection object
        connection = new MySqlConnection(connectionString);
        connection.Open();
    }

    public bool ValidateAccount(String userName, String passWord)
    { 
        String query = $"Select * From Users WHERE UserName like '{userName}' and Password like '{passWord}'";
        Console.WriteLine("Sending Query: " + query );        
       MySqlDataReader reader =  ExecuteQuery(query);

       MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
      
       DataTable dt = new DataTable();
       mySqlDataAdapter.Fill(dt);
       dt.Load(reader);
       int numberOfResults = dt.Rows.Count;
       Console.WriteLine("Validate Account Found "+ numberOfResults + " Accounts");
       return numberOfResults > 0;

    }

    
    public void CloseConnection() {
        try {
            // Close the connection
            connection.Close();
        } catch (Exception ex) {
            Console.WriteLine("Error closing connection: " + ex.ToString());
        }
    }

    public MySqlDataReader ExecuteQuery(string query) {
        MySqlDataReader reader = null;

        try {
            // Create a new SqlCommand object with the query and connection
            MySqlCommand command = new MySqlCommand(query, connection);

            // Execute the query and return the SqlDataReader
            reader = command.ExecuteReader();
        } catch (Exception ex) {
            Console.WriteLine("Error executing query: " + ex.ToString());
        }

        return reader;
    }
    
}