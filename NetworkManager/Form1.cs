using MessageServer.Data;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;

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

		await Task.FromResult(netClient.Connect());
		await Task.FromResult(netClient.Authenticate(UserInput.Text, PasswordInput.Text));
	}

	private void RefreshRoomsButton_Click(object sender, EventArgs e)
	{

		
		netClient.RefreshRoomList();
	}

	private void CreateRoomButton_Click(object sender, EventArgs e)
	{
		netClient.CreateRoom("Manic");
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

	private void NetClient_onRoomMessageRecievedEvent((int RoomID, string Message) obj)
	{
		MessageBox.Show($"Got Message from Room{obj.RoomID} :- {obj.Message}");
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
		foreach (var room in netClient.GetRoomList()) {
			RoomList.Items.Add(room.RoomID);
		}
	}

	private void NetClient_onRoomJoinedEvent(bool obj)
	{
	

		//throw new NotImplementedException();
	}

	private void NetClient_onRoomCreatedEvent(int obj)
	{
		netClient.RefreshRoomList();

		RoomForm roomForm = new RoomForm(netClient);
		roomForm.RoomID = obj;
	//	roomForm.thisRoom = netClient.roomList [obj];
	    roomForm.Show();

		//throw new NotImplementedException();
	}

	private void NetClient_onMessageRecievedEvent(string obj)
	{
		MessageBox.Show("Message Recieved: ", obj);
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