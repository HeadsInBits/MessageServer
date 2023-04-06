namespace LibObjects
{
    public enum CommunicationTypeEnum
    {
        /// <summary>
        /// MessageFromServer
        /// </summary>
        ClientReceiveRoomMessage, //implemented on client & server
        ClientReceiveUserInfo, //implemented on client & server
        ClientReceiveClientGuid, //implemented on client & server
        ClientReceiveRoomListJson, //implemented on client & server
        ClientReceiveAuthenticated, //implemented on client & server
        ClientReceiveMessageFromUser, //implemented on client & server
        ClientReceiveUserListJson, //implemented on client & server
        ClientReceiveUserListJsonPaginated, //implemented on client & server
        ClientReceiveRoomListJsonPaginated, //implemented on client & server
        ClientReceiveRoomDestroyed, //implemented on client & server
        ClientReceiveRoomCreated, //implemented on client & server
        ClientReceiveJoinedRoom, //implemented on client & server
        ClientReceiveLeftRoom, //implemented on client & server
        ClientReceiveUserJoinedRoom, //implemented on client & server
        ClientReceiveUserLeftRoom, //implemented on client & server
        ClientReceiveMessageSentSuccessful, //implemented on client & server
        ClientReceiveUsersListJsonInRoom, // implemented on client & server
        ClientReceiveUsersListJsonInRoomPaginated, // implemented on client & server
        ClientReceiveCommunicationToAll, // implemented on client & server
        ClientReceiveErrorResponseFromServer, // implemented on client & server
        ClientReceiveRemovedFromTheRoom, // implemented on client & server
        ClientReceiveBannedFromRoom, // implemented on client & server
        ClientReceiveNoLongerBannedFromRoom, // implemented on client & server
        ClientReceiveApprovedForRoom, // implemented on client & server
        ClientReceiveNoLongerApprovedForRoom, // implemented on client & server
        ClientReceiveUserDisconnected, // implemented on client & server
        /// <summary>
        /// MessageFromClient
        /// </summary>
        ServerReceiveAuthenticate, //implemented on client & server
        ServerReceiveRequestSendMessageToUser, //implemented on client & server
        ServerReceiveRequestUserFromGuid, //implemented on client & server
        ServerReceiveRequestClientGuid, //implemented on client & server
        ServerReceiveRequestCreateRoom, //implemented on client & server
        ServerReceiveRequestAddUserToRoom, //implemented on client & server
        ServerReceiveSendMessageToRoom, //implemented on client & server
        ServerReceiveRequestUserListJson, //implemented on client & server
        ServerReceiveRequestRoomListJson, //implemented on client  & server
        ServerReceiveMessageReceivedSuccessfully, //implemented on client & server
        ServerReceiveRequestSendMessageToAll, //implemented on client & server 
        ServerReceiveRequestUsersListJsonInRoom, //implemented on client & server
        ServerReceiveRequestLockRoom, //implemented on client & server
        ServerReceiveRequestUnlockRoom, //implemented on client & server
        ServerReceiveRequestRemoveUserFromRoom, // implemented on server
        ServerReceiveRequestBanUserFromRoom, //implemented on client & server
        ServerReceiveRequestRemoveBanFromUserInRoom, //implemented on client & server
        ServerReceiveRequestApproveUserFromRoom, //implemented on client & server
        ServerReceiveRequestRemoveApproveFromUserInRoom, //implemented on client & server
    }
}