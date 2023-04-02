namespace LibObjects
{
    public enum CommunicationTypeEnum
    {
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
        
        
        
        ClientReceiveUsernameOfUsersInRoom, // implemented on server
        ClientReceiveCommunicationToAllButSender, // implemented on server
        ClientReceiveCommunicationToAll, // implemented on server
        ClientReceiveYourWebsocketId, // implemented on server
        ClientReceiveRoomGuidListPaginated, //implemented on server
        ClientReceiveRoomGuidList, //implemented on server
        
        
        ServerReceiveAuthenticate,
        ServerReceiveGuid,
        ServerReceiveUserList,
        ClientReceiveUserList,
        ClientReceiveUserListPaginated,
        ServerReceiveUserListJson,
        ServerReceiveRoomList,
        ClientReceiveRoomList,
        ClientReceiveRoomListPaginated,
        ServerReceiveRoomListJson,

    }
}