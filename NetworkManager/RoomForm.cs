using LibObjects;
using System.Windows.Forms;

namespace NetworkManager
{
    public partial class RoomForm : Form
    {
        public Room thisRoom;
        public NetClient.Client myClient;

        private bool roomActive = true;

        public RoomForm(Room room, NetClient.Client client)
        {
            thisRoom = room;
            myClient = client;

            InitializeComponent();
        }

        private void RoomForm_Load(object sender, EventArgs e)
        {
            this.Text = "Room Loaded:" + thisRoom.GetRoomName();
            myClient.RequestGetUsersInRoomAsync(thisRoom.GetGuid());
        }

        public void SetIfRoomActive(bool value)
        {
            roomActive = value;
        }

        private void SendMessageButton_Click(object sender, EventArgs e)
        {
            if (MessageInput.Text == "")
            {
                MessageBox.Show("Enter a message");
                return;
            }

            myClient.RequestSendMessageToRoomAsync(thisRoom.GetGuid(), MessageInput.Text);
            MessageInput.Text = "";

        }

        public void ProcessIncomingMessage(string message)
        {
            RecievedMessagesRoomList.Items.Add(message);
            RecievedMessagesRoomList.TopIndex = RecievedMessagesRoomList.Items.Count - 1;

        }

        public void ProcessIncomingUserList(List<User> users)
        {
            RoomUsersList.Items.Clear();
            foreach (User user in users)
            {
                RoomUsersList.Items.Add((User)user);
            }
        }

        public void UpdateUserList()
        {
            RefreshUsersButton_Click(null, null);
        }

        private void RoomForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (roomActive)
                myClient.RequestRemoveUserFromRoom(thisRoom, myClient.GetUser());
        }

        private void RefreshUsersButton_Click(object sender, EventArgs e)
        {
            myClient.RequestGetUsersInRoomAsync(thisRoom.GetGuid());
        }
    }
}
