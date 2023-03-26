using MessageServer.Data;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;

namespace NetworkManager;

public partial class Form1 : Form
{
	NetClient.Client netClient = new NetClient.Client();


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

	private async void RefreshRoomsButton_Click(object sender, EventArgs e)
	{
		
		await Task.FromResult(
		netClient.RefreshRoomList());

		MessageBox.Show("Room List Refresh Sent");
	}

	private async void CreateRoomButton_Click(object sender, EventArgs e)
	{
		await netClient.CreateRoom("Manic");
		MessageBox.Show("Sent Room Request");

	}

	private void Form1_Load(object sender, EventArgs e)
	{
		netClient.onAuthenticateEvent += NetClient_onAuthenticateEvent;
		netClient.onMessageRecievedEvent += NetClient_onMessageRecievedEvent;
		netClient.onRoomCreatedEvent += NetClient_onRoomCreatedEvent;
		netClient.onRoomJoinedEvent += NetClient_onRoomJoinedEvent;
		netClient.onRoomListRecievedEvent += NetClient_onRoomListRecievedEvent;
		netClient.onUserListRecievedEvent += NetClient_onUserListRecievedEvent;
	}

	private void NetClient_onUserListRecievedEvent(List<User> obj)
	{
		UserList.Items.Clear();

		foreach(var item in obj) {
			UserList.Items.Add(item);
		}
	}

	private void NetClient_onRoomListRecievedEvent(List<Guid> obj)
	{
		MessageBox.Show("Room List Reiceved");
		foreach(var room in netClient.roomList) { 
			RoomList.Items.Add(room);
		}
	}

	private void NetClient_onRoomJoinedEvent(bool obj)
	{
		throw new NotImplementedException();
	}

	private void NetClient_onRoomCreatedEvent(bool obj)
	{
		MessageBox.Show("Room Created: "+  obj);
		throw new NotImplementedException();
	}

	private void NetClient_onMessageRecievedEvent(string obj)
	{
		MessageBox.Show("Message Recieved: ", obj);
	}

	private void NetClient_onAuthenticateEvent(bool obj)
	{

		if (obj) {
			LoginButton.BackColor= Color.Green;	
			LoginStatusStrip.Text = "Login OK + Authenticated";

		} else { 
			LoginButton.BackColor= Color.Red;
			LoginStatusStrip.Text="Login Failed!";
		}

	}
}