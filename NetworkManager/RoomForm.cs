using LibObjects;

namespace NetworkManager
{
	public partial class RoomForm : Form
	{
		public Room thisRoom;
		public Guid RoomID;
		public NetClient.Client myClient;

		public RoomForm(Room room)
		{
			thisRoom = room;
			InitializeComponent();
		}

		public RoomForm(NetClient.Client myClient)
		{
			InitializeComponent();
			this.myClient = myClient;
		}

		private void RoomForm_Load(object sender, EventArgs e)
		{
			this.Text = "Room Loaded:" + RoomID;

		}

		private void SendMessageButton_Click(object sender, EventArgs e)
		{
			myClient.RequestSendMessageToRoomAsync(RoomID, MessageInput.Text);
		}
	}
}
