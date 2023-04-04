namespace LibObjects
{
    public enum CommunicationTypeEnum
    {
        /// <summary>
        /// MessageFromServer
        /// </summary>
        ClientReceiveRoomMessage = 0, //implemented on client & server
        ClientReceiveUserInfo = 1, //implemented on client & server
        ClientReceiveYourGuid = 2, //implemented on client & server
        ClientReceiveRoomListJson = 3, //implemented on client & server
        ClientReceiveAuthenticated = 4, //implemented on client & server
        ClientReceiveMessageFromUser = 5, //implemented on client & server
        ClientReceiveUserListJson = 6, //implemented on client & server
        ClientReceiveUserListJsonPaginated = 7, //implemented on client & server
        ClientReceiveRoomListJsonPaginated = 8, //implemented on client & server
        ClientReceiveRoomDestroyed = 9, //implemented on client & server
        ClientReceiveRoomCreated = 10, //implemented on client
        ClientReceiveJoinedRoom = 11, //implemented on client & server
        ClientReceiveUserJoinedRoom = 12, //implemented on client & server
        ClientReceiveUserLeftRoom = 13, //implemented on client & server
        ClientReceiveMessageSentSuccessful = 14, //implemented on client & server
        ClientReceiveUsersListJsonInRoom = 15, // implemented on client & server
        ClientReceiveCommunicationToAllButSender = 16, // implemented on client & server
        ClientReceiveCommunicationToAll = 17, // implemented on client & server
        ClientReceiveErrorResponseFromServer = 18, // implemented on client
        
        
        //TODO:
        ClientReceiveYourWebsocketId = 19, // implemented on server


        /// <summary>
        /// MessageFromClient
        /// </summary>
        ServerReceiveAuthenticate = 20, //implemented on client & server
        ServerReceiveRequestSendMessageToUser = 21, //implemented on client & server
        ServerReceiveRequestUserFromGuid = 22, //implemented on client & server
        ServerReceiveRequestClientGuid = 23, //implemented on client & server
        ServerReceiveRequestCreateRoom = 24, //implemented on client & server
        ServerReceiveRequestAddUserRoom = 25, //implemented on client & server
        ServerReceiveSendMessageToRoom = 26, //implemented on client & server
        ServerReceiveRequestUserListJson = 27, //implemented on client & server
        ServerReceiveRequestRoomListJson = 28, //implemented on client  & server
        ServerReceiveMessageReceivedSuccessfully = 29, //implemented on client & server
        ServerReceiveRequestSendMessageToAll = 30, //implemented on client & server 
        ServerReceiveRequestUsersListJsonInRoom = 31, //implemented on client & server

        //TODO:
        ServerReceiveRequestClientWebSocketId = 32, //implemented on server 

    }
}