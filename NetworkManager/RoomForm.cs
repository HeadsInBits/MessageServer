using MessageServer.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkManager
{
	public partial class RoomForm : Form
	{
		public Room thisRoom;
		public int RoomID;
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
			myClient.SendMessageToRoomAsync(RoomID, MessageInput.Text);
		}
	}
}
