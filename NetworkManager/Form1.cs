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
		Task.FromResult(netClient.UpdateUserList());
	}

	private void LoginButton_Click(object sender, EventArgs e)
	{
		RunLogin();
		RefreshUsersButton_Click(null, null);
	}

	private async void RunLogin()
	{

		await netClient.Connect("localhost","8080");
		await Task.FromResult(netClient.Listen());
		await Task.FromResult(netClient.Authenticate(UserInput.Text, PasswordInput.Text));
	}

	private void RefreshRoomsButton_Click(object sender, EventArgs e)
	{
		netClient.RequestRoomList();
	}

	private void CreateRoomButton_Click(object sender, EventArgs e)
	{
		netClient.CreateRoom("Manic", 50, true);
	}

	private void Form1_Load(object sender, EventArgs e)
	{
		netClient.onAuthenticateEvent += NetClient_onAuthenticateEvent;
		netClient.onMessageRecievedEvent += NetClient_onMessageRecievedEvent;
		netClient.onRoomCreatedEvent += NetClient_onRoomCreatedEvent;
		netClient.onRoomJoinedEvent += NetClient_onRoomJoinedEvent;
		netClient.onRoomListRecievedEvent += NetClient_onRoomListRecievedEvent;
		netClient.onUserListRecievedEvent += NetClient_onUserListRecievedEvent;
		netClient.onRoomMessageRecievedEvent += NetClient_onRoomMessageRecievedEvent;
	}

	private void NetClient_onRoomMessageRecievedEvent((Room room, User user, string Message) obj)
	{
		MessageBox.Show($"Got Message from Room{obj.room.RoomId} :- {obj.Message}");
	}

	private void NetClient_onRoomJoinedEvent(Room obj)
	{
		//TODO
	}

	private async void NetClient_onRoomCreatedEvent(Room obj)
	{
		netClient.RequestRoomList();

		RoomForm roomForm = new RoomForm(netClient);
		roomForm.RoomID = obj.GetGuid();
	//	roomForm.thisRoom = netClient.roomList [obj];
	    roomForm.Show();

		//throw new NotImplementedException();
	}

	private void NetClient_onMessageRecievedEvent((User user, string message) obj)
	{
		MessageBox.Show($"Message Recieved: {obj.message}", obj.user.GetUserName());
	}

	private void NetClient_onUserListRecievedEvent(List<User> obj)
	{
		UserList.Items.Clear();

		foreach (var item in obj) {
			UserList.Items.Add(item);
		}
	}

	private void NetClient_onRoomListRecievedEvent(List<Room> obj)
	{
		RoomList.Items.Clear();
		foreach (var room in netClient.GetLocalClientRoomList()) {
			RoomList.Items.Add(room.RoomId);
		}
	}



	private void NetClient_onAuthenticateEvent(bool obj)
	{

		if (obj) {
			LoginButton.BackColor = Color.Green;
			LoginStatusStrip.Text = "Login OK + Authenticated";

		}
		else {
			LoginButton.BackColor = Color.Red;
			LoginStatusStrip.Text = "Login Failed!";
		}

	}
}