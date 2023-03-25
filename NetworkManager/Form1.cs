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
		UserList.Items.Clear();

		foreach (var client in netClient.networkUsers) {
			UserList.Items.Add(client);
		}

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
}