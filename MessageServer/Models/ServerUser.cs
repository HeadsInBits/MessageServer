using System.Transactions;
using LibObjects;

namespace MessageServer.Models
{
    public class ServerUser: User
    {
        private int WebSocketID;
        
        public ServerUser(string userName, bool isValidated, int webSocketId)
        {
            WebSocketID = webSocketId;
            _isValidated = isValidated;
            _userName = userName;
    
        }
        public int GetWebSocketID()
        {
            return WebSocketID;
        }

        public virtual bool Equals(ServerUser other)
        {
            return base.Equals(other) && other.WebSocketID == WebSocketID;
        }
    }
}

