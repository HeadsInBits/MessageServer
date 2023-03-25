using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace MessageServer;

public class DBManager
{
    private MySqlConnection connection;
    private string connectionString;
    private bool logging = true;
    
    public DBManager(string server, string database, string username, string password) {
        // Set up the connection string
        connectionString = $"Server={server};Database={database};User Id={username};Password={password};";

        // Create a new SqlConnection object
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine("Server Execption!!::" + e.Message);
        }
    }

    public bool ValidateAccount(String userName, String passWord)
    { 
       /*String query = $"Select * From Users WHERE UserName like '{userName}' and Password like '{passWord}'";
       Console.WriteLine("Sending Query: " + query );        
       MySqlDataReader reader =  ExecuteQuery(query);

       MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(query, connection);
      
       DataTable dt = new DataTable();
       mySqlDataAdapter.Fill(dt, "Users");
       dt.Load(reader);*/

       int numberOfResults;
       
       using (MySqlCommand cmd = new MySqlCommand($"Select * From Users WHERE UserName = BINARY '{userName}' and Password = BINARY '{passWord}' COLLATE utf8mb4_general_ci", connection))
       {
           cmd.CommandType = CommandType.Text;
           using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
           {
               using (DataTable dt = new DataTable())
               {
                   sda.Fill(dt);
                   numberOfResults = dt.Rows.Count;
               }
           }
       }
       if(logging) 
           Console.WriteLine("Validate Account Found "+ numberOfResults + " Accounts");
       return numberOfResults > 0;
    }

    public void PersistMessageToUser(User usr, string message)
    {
        throw new NotImplementedException();
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