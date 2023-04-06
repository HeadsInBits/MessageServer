using MessageServer.Data;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using LibObjects;

namespace NetworkManager;

public partial class Form1 : Form
{
    public NetClient.Client netClient = new NetClient.Client();


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

        await netClient.Connect("localhost", "8080");
        await Task.FromResult(netClient.Listen());
        await Task.FromResult(netClient.RequestAuthenticate(UserInput.Text, PasswordInput.Text));
    }

    private void RefreshRoomsButton_Click(object sender, EventArgs e)
    {
        netClient.RequestRoomList();
    }

    private void CreateRoomButton_Click(object sender, EventArgs e)
    {
        netClient.RequestCreateRoom("ChatApp", 50, true, "MyRoom");
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        netClient.onReceivedAuthenticateEvent += NetClientOnReceivedAuthenticateEvent;
        netClient.onRecievedMessageFromUserEvent += NetClientOnRecievedMessageReceivedEvent;
        netClient.onRecievedRoomCreatedEvent += NetClientOnRecievedRoomCreatedEvent;
        netClient.onRecievedRoomJoinedEvent += NetClientOnRecievedRoomJoinedEvent;
        netClient.onRecievedRoomListEvent += NetClientOnRecievedRoomListReceivedEvent;
        netClient.onRecievedUserListEvent += NetClientOnRecievedUserListReceivedEvent;
        netClient.onRecievedRoomMessageEvent += NetClientOnRecievedRoomMessageEvent;
    }

    private void NetClientOnRecievedRoomMessageEvent((Room room, User user, string Message) obj)
    {
        MessageBox.Show($"Got Message from Room{obj.room.GetGuid()} :- {obj.Message}");
    }

    private void NetClientOnRecievedRoomJoinedEvent(Room obj)
    {
        //TODO
    }

    private async void NetClientOnRecievedRoomCreatedEvent(Room obj)
    {
        netClient.RequestRoomList();

        RoomForm roomForm = new RoomForm(netClient);
        roomForm.RoomID = obj.GetGuid();
        //	roomForm.thisRoom = netClient.roomList [obj];
        roomForm.Show();

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
        foreach (var room in netClient.GetLocalClientRoomList())
        {
            RoomList.Items.Add(room.GetRoomName());
            Task.FromResult(netClient.RequestGetUsersInRoomAsync(room.GetGuid()));
        }
    }



    private void NetClientOnReceivedAuthenticateEvent(bool obj)
    {

        if (obj)
        {
            LoginButton.BackColor = Color.Green;
            LoginStatusStrip.Text = "Login OK + Authenticated";

        }
        else
        {
            LoginButton.BackColor = Color.Red;
            LoginStatusStrip.Text = "Login Failed!";
        }

    }


}