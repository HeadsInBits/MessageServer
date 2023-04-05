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
        ClientReceiveRoomCreated = 10, //implemented on client & server
        ClientReceiveJoinedRoom = 11, //implemented on client & server
        ClientReceiveUserJoinedRoom = 12, //implemented on client & server
        ClientReceiveUserLeftRoom = 13, //implemented on client & server
        ClientReceiveMessageSentSuccessful = 14, //implemented on client & server
        ClientReceiveUsersListJsonInRoom = 15, // implemented on client & server
        ClientReceiveUsersListJsonInRoomPaginated = 16, // implemented on client & server
        ClientReceiveCommunicationToAllButSender = 17, // implemented on client & server
        ClientReceiveCommunicationToAll = 18, // implemented on client & server
        ClientReceiveErrorResponseFromServer = 19, // implemented on client & server
        /// <summary>
        /// MessageFromClient
        /// </summary>
        ServerReceiveAuthenticate = 20, //implemented on client & server
        ServerReceiveRequestSendMessageToUser = 21, //implemented on client & server
        ServerReceiveRequestUserFromGuid = 22, //implemented on client & server
        ServerReceiveRequestClientGuid = 23, //implemented on client & server
        ServerReceiveRequestCreateRoom = 24, //implemented on client & server
        ServerReceiveRequestAddUserToRoom = 25, //implemented on client & server
        ServerReceiveSendMessageToRoom = 26, //implemented on client & server
        ServerReceiveRequestUserListJson = 27, //implemented on client & server
        ServerReceiveRequestRoomListJson = 28, //implemented on client  & server
        ServerReceiveMessageReceivedSuccessfully = 29, //implemented on client & server
        ServerReceiveRequestSendMessageToAll = 30, //implemented on client & server 
        ServerReceiveRequestUsersListJsonInRoom = 31, //implemented on client & server
        ServerReceiveRequestLockRoom = 32, //implemented on client & server
        ServerReceiveRequestUnlockRoom = 33, //implemented on client & server
        ServerReceiveRequestRemoveUserFromRoom = 34, // implemented on server
        ServerReceiveRequestBanUserFromRoom = 35, //implemented on client & server
        ServerReceiveRequestRemoveBanFromUserInRoom = 36, //implemented on client & server
        ServerReceiveRequestApproveUserFromRoom = 37, //implemented on client & server
        ServerReceiveRequestRemoveApproveFromUserInRoom = 38, //implemented on client & server
    }
}