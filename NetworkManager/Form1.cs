using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using LibObjects;
using System.Diagnostics.CodeAnalysis;

namespace NetworkManager;

public partial class Form1 : Form
{
    public NetClient.Client netClient = new NetClient.Client();
    private List<RoomForm> roomForms = new List<RoomForm>();

    public Form1()
    {
        InitializeComponent();

    }

    private void RefreshUsersButton_Click(object sender, EventArgs e)
    {
        Task.FromResult(netClient.RequestUserList());
    }

    private void LoginButton_Click(object sender, EventArgs e)
    {
        RunLogin();
        RefreshUsersButton_Click(null, null);
    }

    private async void RunLogin()
    {
        await netClient.Connect(AddressInput.Text, PortInput.Text);
        await Task.FromResult(netClient.Listen());
        await Task.FromResult(netClient.RequestAuthenticate(UserInput.Text, PasswordInput.Text));
    }

    private void RefreshRoomsButton_Click(object sender, EventArgs e)
    {
        netClient.RequestRoomList();
    }

    private void CreateRoomButton_Click(object sender, EventArgs e)
    {
        if (RoomNameInput.Text == "")
        {
            MessageBox.Show("Enter A Room Name");
            return;
        }

        if (MaxMembersInput.Value <= 0)
        {
            MessageBox.Show("Enter Max Members");
            return;
        }

        netClient.RequestCreateRoom("Network-Manager.ChatApp", (int)MaxMembersInput.Value, PublicRoomImput.Checked, RoomNameInput.Text);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        netClient.onAuthenticate += NetClientOnReceivedAuthenticateEvent;
        netClient.onMessageFromUser += NetClientOnRecievedMessageReceivedEvent;
        netClient.onRoomCreated += NetClientOnRecievedRoomCreatedEvent;
        netClient.onRoomJoined += NetClientOnRecievedRoomJoinedEvent;
        netClient.onRoomList += NetClientOnRecievedRoomListReceivedEvent;
        netClient.onUserList += NetClientOnRecievedUserListReceivedEvent;
        netClient.onRoomMessage += NetClientOnRecievedRoomMessageEvent;
        netClient.onUsersListInRoom += NetClient_onReceivedUsersListInRoomEvent;
        netClient.onUserJoinedRoom += NetClient_onRecievedUserJoinedRoomEvent;
        netClient.onUserLeftRoom += NetClient_onRecievedUserLeftRoomEvent;
        netClient.onRoomDestroyed += NetClient_onRecievedRoomDestroyedEvent;
        netClient.onUserDisconnected += NetClient_onRecievedUserDisconnectedEvent;
        netClient.onErrorResponseFromServer += NetClient_onErrorResponseFromServer;
    }
    private static bool IsNotNull([NotNullWhen(true)] object? obj) => obj != null;

    private void NetClient_onErrorResponseFromServer((CommunicationTypeEnum comEnum, string message) obj)
    {
        MessageBox.Show(obj.message);
    }

    private void NetClient_onRecievedUserDisconnectedEvent(User obj)
    {
        RefreshUsersButton_Click(null, null);
    }

    private void NetClient_onRecievedRoomDestroyedEvent(Room obj)
    {
        MessageBox.Show(obj.GetRoomName() + " is being Destroyed \n The window will now close");

        CloseChatWindow(obj.GetGuid());
    }

    private void CloseChatWindow(Guid roomGuid)
    {
        RoomForm? context = GetRoomFormByGUID(roomGuid);
        if (context != null)
        {
            roomForms.Remove(context);
            context.Close();
        }
    }

    private void NetClient_onRecievedUserLeftRoomEvent((User user, Guid roomGuid) obj)
    {
        if (obj.user.GetUserName() == netClient.GetClientName())
        {
            CloseChatWindow(obj.roomGuid);
        }

        GetRoomFormByGUID(obj.roomGuid).UpdateUserList();
        GetRoomFormByGUID(obj.roomGuid).ProcessIncomingMessage(obj.user.GetUserName() + " :" + DateTime.Now.ToString("h:mm:ss tt") + ": LEFT THE ROOM");

    }

    private void NetClient_onRecievedUserJoinedRoomEvent((User user, Guid roomGuid) obj)
    {
        GetRoomFormByGUID(obj.roomGuid).UpdateUserList();
        GetRoomFormByGUID(obj.roomGuid).ProcessIncomingMessage(obj.user.GetUserName() + " :" + DateTime.Now.ToString("h:mm:ss tt") + ": JOINED THE ROOM");
    }

    private RoomForm? GetRoomFormByGUID(Guid roomID)
    {
        foreach (var rForm in roomForms)
        {
            if (rForm.thisRoom.GetGuid() == roomID)
            {
                return rForm;
            }
        }

        return null;
    }

    private void NetClient_onReceivedUsersListInRoomEvent((Room room, List<User> users) obj)
    {

        GetRoomFormByGUID(obj.room.GetGuid())?.ProcessIncomingUserList(obj.users);
        //throw new NotImplementedException();
    }

    private void NetClientOnRecievedRoomMessageEvent((Room room, User user, string Message) obj)
    {
        //MessageBox.Show($"Got Message from Room{obj.room.GetGuid()} :- {obj.Message}");

        GetRoomFormByGUID(obj.room.GetGuid()).ProcessIncomingMessage(obj.Message);

        //foreach (var rForm in roomForms)
        //{
        //    if (rForm.thisRoom.GetGuid() == obj.room.GetGuid())
        //    {
        //        rForm.ProcessIncomingMessage(obj.user.GetUserName() + " :" + DateTime.Now.ToString("h:mm:ss tt") + ": " + obj.Message);
        //    }
        //}

    }

    private void NetClientOnRecievedRoomJoinedEvent(Room obj)
    {
        //TODO:
        roomForms.Add(new RoomForm(obj, netClient));
        roomForms.Last().Show();
    }

    private async void NetClientOnRecievedRoomCreatedEvent(Room obj)
    {
        netClient.RequestRoomList();

        //   RoomForm roomForm = new RoomForm(obj, netClient);
        //	roomForm.thisRoom = netClient.roomList [obj];
        //  roomForm.Show();

        //throw new NotImplementedException();
    }

    private void NetClientOnRecievedMessageReceivedEvent((User user, string message) obj)
    {
        MessageBox.Show($"Message Recieved: {obj.message}", obj.user.GetUserName());
    }

    private void NetClientOnRecievedUserListReceivedEvent(List<User> obj)
    {
        UserList.Items.Clear();

        foreach (var item in obj)
        {
            UserList.Items.Add(item);
        }
    }

    private void NetClientOnRecievedRoomListReceivedEvent(List<Room> obj)
    {
        RoomList.Items.Clear();
        foreach (var room in obj)
        {
            RoomList.Items.Add(room);
            //Task.FromResult(netClient.RequestGetUsersInRoomAsync(room.GetGuid()));
        }
    }



    private void NetClientOnReceivedAuthenticateEvent(bool obj)
    {

        if (obj)
        {
            LoginButton.BackColor = Color.Green;
            LoginStatusStrip.Text = "Login OK + Authenticated";
            RefreshUsersButton_Click(null,null);
            RefreshRoomsButton_Click(null, null);

        }
        else
        {
            LoginButton.BackColor = Color.Red;
            LoginStatusStrip.Text = "Login Failed!";
        }

    }

    private void JoinRoomButton_Click(object sender, EventArgs e)
    {
        Room? roomToJoin = RoomList.SelectedItem as Room;
        if (roomToJoin == null)
            return;

        netClient.RequestToAddUserToRoom(netClient.GetUser(), roomToJoin.GetGuid());

    }

    private void RoomLockedLabel_Click(object sender, EventArgs e)
    {

    }

    private void RoomList_SelectedIndexChanged(object sender, EventArgs e)
    {
        Room? roomInfo = RoomList.SelectedItem as Room;
        if (roomInfo == null) { return; }

        RoomNameLabel.Text = "Room Name: " + roomInfo.GetRoomName();
        RoomCreatorLabel.Text = "Room Creator: " + roomInfo.GetCreator();
        MetaDataLabel.Text = "Meta Data: " + roomInfo.GetMeta();
        AccessLevelLabel.Text = "Access Level: " + roomInfo.GetAccessLevel();
        RoomLockedLabel.Text = "Room Locked: " + roomInfo.GetIsRoomLocked();
        MaxMembersLabel.Text = "Maximum Members: " + roomInfo.GetRoomLimit();
        CreationDateLabel.Text = "Creation Date: " + roomInfo.GetCreationDate();
        GUIDLabel.Text = "GUID: " + roomInfo.GetGuid();

    }

    private void AccessLevelLabel_Click(object sender, EventArgs e)
    {

    }
}