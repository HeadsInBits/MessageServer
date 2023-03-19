using System.Data.SqlClient;

namespace MessageServer;

public class User
{
    private string _userName;
    private string _passWord;
    private bool _isValidated;
    public int WebSocketID;
    
    public User(String userName, String passWord, bool isValidated)
    {
        _userName = userName;
        _passWord = passWord;
    }

    public String GetUserName()
    {
        return _userName;
    }
    
    public bool isValidateAccount()
    {
        return _isValidated;
    }
    
}