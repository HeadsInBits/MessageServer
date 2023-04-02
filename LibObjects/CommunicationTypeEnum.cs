namespace LibObjects
{
    public enum CommunicationTypeEnum
    {
        /// <summary>
        /// MessageFromServer
        /// </summary>
        ClientReceiveRoomMessage, //implemented on client & server
        ClientReceiveUserInfo, //implemented on client & server
        ClientReceiveYourGuid, //implemented on client & server
        ClientReceiveRoomListJson, //implemented on client & server
        ClientReceiveAuthenticated, //implemented on client & server
        ClientReceiveMessageFromUser, //implemented on client & server
        ClientReceiveUserListJson, //implemented on client & server
        ClientReceiveUserListJsonPaginated, //implemented on client & server
        ClientReceiveRoomListJsonPaginated, //implemented on client & server
        ClientReceiveRoomDestroyed, //implemented on client & server
        ClientReceiveRoomCreated, //implemented on client
        ClientReceiveJoinedRoom, //implemented on client & server
        ClientReceiveUserJoinedRoom, //implemented on client & server
        ClientReceiveUserLeftRoom, //implemented on client & server
        ClientReceiveMessageSentSuccessful, //implemented on client & server
        
        //TODO:
        ClientReceiveUsernameOfUsersInRoom, // implemented on server
        ClientReceiveCommunicationToAllButSender, // implemented on server
        ClientReceiveCommunicationToAll, // implemented on server
        ClientReceiveYourWebsocketId, // implemented on server
        ClientReceiveRoomGuidListPaginated, //implemented on server
        ClientReceiveRoomGuidList, //implemented on server
        ClientReceiveMessageToUserInRoomFromRoomHost,
        ClientReceiveMessageToAllUsersInRoomFromRoomHost,
        ClientReceiveMessageFromUserInRoomToRoomHost,
        
        
        /// <summary>
        /// MessageFromClient
        /// </summary>
        ServerReceiveAuthenticate, //implemented on client & server
        ServerReceiveRequestSendMessageToUser, //implemented on client & server
        ServerReceiveRequestUserFromGuid, //implemented on client
        ServerReceiveRequestClientGuid, //implemented on client & server
        ServerReceiveRequestCreateRoom, //implemented on client & server
        ServerReceiveRequestAddUserRoom, //implemented on client & server
        ServerReceiveSendMessageToRoom, //implemented on client & server
        ServerReceiveRequestUserListJson, //implemented on client & server
        ServerReceiveRequestRoomListJson, //implemented on client  & server
        ServerReceiveMessageReceivedSuccessfully, //implemented on client & server

        //TODO:
        ServerReceiveRequestClientWebSocketId, //implemented on server 
        ServerReceiveRequestSendMessageToAll, //implemented on server 
        ServerReceiveRequestUsernamesInRoom, //implemented on server
        ServerReceiveRequestRoomGuidList, //implemented on server
        ServerReceiveMessageToUserInRoomFromRoomHost,
        ServerReceiveMessageToAllUsersInRoomFromRoomHost,
        ServerReceiveMessageFromUserInRoomToRoomHost,
    }
}