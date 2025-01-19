namespace NetworkObjects
{
    public enum CommunicationTypeEnum
    {
        /// <summary>
        /// MessageFromServer
        /// </summary>
        ClientReceiveRoomMessage = 0, //implemented on client & server
        ClientReceiveUserInfo = 1, //implemented on client & server
        ClientReceiveClientGuid = 2, //implemented on client & server
        ClientReceiveRoomListJson = 3, //implemented on client & server
        ClientReceiveAuthenticated = 4, //implemented on client & server
        ClientReceiveMessageFromUser = 5, //implemented on client & server
        ClientReceiveUserListJson = 6, //implemented on client & server
        ClientReceiveUserListJsonPaginated = 7, //implemented on client & server
        ClientReceiveRoomListJsonPaginated = 8, //implemented on client & server
        ClientReceiveRoomDestroyed = 9, //implemented on client & server
        ClientReceiveRoomCreated = 10, //implemented on client & server
        ClientReceiveJoinedRoom = 11, //implemented on client & server
        ClientReceiveLeftRoom = 12, //implemented on client & server
        ClientReceiveUserJoinedRoom = 13, //implemented on client & server
        ClientReceiveUserLeftRoom = 14, //implemented on client & server
        ClientReceiveMessageSentSuccessful = 15, //implemented on client & server
        ClientReceiveUsersListJsonInRoom = 16, // implemented on client & server
        ClientReceiveUsersListJsonInRoomPaginated = 17, // implemented on client & server
        ClientReceiveCommunicationToAll = 18, // implemented on client & server
        ClientReceiveErrorResponseFromServer = 19, // implemented on client & server
        ClientReceiveRemovedFromTheRoom = 20, // implemented on client & server
        ClientReceiveBannedFromRoom = 21, // implemented on client & server
        ClientReceiveNoLongerBannedFromRoom = 22, // implemented on client & server
        ClientReceiveApprovedForRoom = 23, // implemented on client & server
        ClientReceiveNoLongerApprovedForRoom = 24, // implemented on client & server
        ClientReceiveUserDisconnected = 25, // implemented on client & server
        ClientReceiveRoomBannedUserList = 26, //implemented on client & server
        ClientReceiveRoomApprovedUserList = 27, //implemented on client & server
        ClientReceiveRoomBannedUserListPaginated = 28, //implemented on client & server
        ClientReceiveRoomApprovedUserListPaginated = 29, //implemented on client & server
        ClientReceiveUserLoggedIn = 30, //implemented on client & server
        ClientReceivePrivateMessageInRoom = 31,
        ClientRecieveRoomMetaUpdate = 32,
        /// <summary>
        /// MessageFromClient
        /// </summary>
        ServerReceiveAuthenticate = 33, //implemented on client & server
        ServerReceiveRequestSendMessageToUser = 34, //implemented on client & server
        ServerReceiveRequestUserFromGuid = 35, //implemented on client & server
        ServerReceiveRequestClientGuid = 36, //implemented on client & server
        ServerReceiveRequestCreateRoom = 37, //implemented on client & server
        ServerReceiveRequestAddUserToRoom = 38, //implemented on client & server
        ServerReceiveSendMessageToRoom = 39, //implemented on client & server
        ServerReceiveRequestUserListJson = 40, //implemented on client & server
        ServerReceiveRequestRoomListJson = 41, //implemented on client  & server
        ServerReceiveMessageReceivedSuccessfully = 42, //implemented on client & server
        ServerReceiveRequestSendMessageToAll = 43, //implemented on client & server 
        ServerReceiveRequestUsersListJsonInRoom = 44, //implemented on client & server
        ServerReceiveRequestLockRoom = 45, //implemented on client & server
        ServerReceiveRequestUnlockRoom = 46, //implemented on client & server
        ServerReceiveRequestRemoveUserFromRoom = 47, // implemented on server
        ServerReceiveRequestBanUserFromRoom = 48, //implemented on client & server
        ServerReceiveRequestRemoveBanFromUserInRoom = 49, //implemented on client & server
        ServerReceiveRequestApproveUserFromRoom = 50, //implemented on client & server
        ServerReceiveRequestRemoveApproveFromUserInRoom = 51, //implemented on client & server
        ServerReceiveRequestRoomBannedUserList = 52, //implemented on client & server
        ServerReceiveRequestRoomApprovedUserList = 53, //implemented on client & server
        ServerReceiveRequestSendPrivateMessageToUserInRoom = 54,//implemented on client & server
        ServerReceiveRequestUpdateRoomMetaData = 55
    }
}