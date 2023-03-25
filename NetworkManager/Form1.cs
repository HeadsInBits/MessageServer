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

		if (netClient.IsClientValidated()) {
			LoginButton.BackColor = Color.Green;
			LoginButton.ForeColor = Color.Red;

			LoginStatusStrip.Text = "Login OK And Validated";

		}

	}

	private async void RefreshRoomsButton_Click(object sender, EventArgs e)
	{
		try {
			await netClient.RefreshRoomList();

			RoomList.Items.Clear();
			foreach (var roomId in netClient.GetRoomList()) {
				RoomList.Items.Add(roomId);
			}
		} catch (Exception ex) {
			Console.WriteLine(ex.Message);
		}
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
	}

	private void NetClient_onUserListRecievedEvent(ObservableCollection<string> obj)
	{
		UserList.Items.Clear();

		foreach (var client in netClient.networkUsers) {
			UserList.Items.Add(client);
		}
	}

	private void NetClient_onRoomListRecievedEvent(List<Guid> obj)
	{
		foreach (var roomId in obj) {
			RoomList.Items.Add(roomId.ToString());
		}
	}

	private void NetClient_onRoomJoinedEvent(bool obj)
	{
		throw new NotImplementedException();
	}

	private void NetClient_onRoomCreatedEvent(bool obj)
	{
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
			LoginStatusStrip.Text = "Login OK + Authenticated as " + netClient.GetClientName();

		} else { 
			LoginButton.BackColor= Color.Red;
			LoginStatusStrip.Text="Login Failed!";
		}

	}
}