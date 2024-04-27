namespace NetworkManager
{
    partial class RoomForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            RoomUsersList = new ListBox();
            RecievedMessagesRoomList = new ListBox();
            SendMessageButton = new Button();
            RefreshUsersButton = new Button();
            MessageInput = new RichTextBox();
            menuStrip1 = new MenuStrip();
            roomToolStripMenuItem = new ToolStripMenuItem();
            updateRoomMetaDataToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // RoomUsersList
            // 
            RoomUsersList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            RoomUsersList.FormattingEnabled = true;
            RoomUsersList.Location = new Point(10, 32);
            RoomUsersList.Name = "RoomUsersList";
            RoomUsersList.Size = new Size(121, 472);
            RoomUsersList.TabIndex = 0;
            // 
            // RecievedMessagesRoomList
            // 
            RecievedMessagesRoomList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RecievedMessagesRoomList.BorderStyle = BorderStyle.FixedSingle;
            RecievedMessagesRoomList.FormattingEnabled = true;
            RecievedMessagesRoomList.Location = new Point(135, 32);
            RecievedMessagesRoomList.Name = "RecievedMessagesRoomList";
            RecievedMessagesRoomList.Size = new Size(507, 470);
            RecievedMessagesRoomList.TabIndex = 1;
            // 
            // SendMessageButton
            // 
            SendMessageButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            SendMessageButton.Font = new Font("InputMono", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            SendMessageButton.Location = new Point(517, 515);
            SendMessageButton.Name = "SendMessageButton";
            SendMessageButton.Size = new Size(125, 41);
            SendMessageButton.TabIndex = 3;
            SendMessageButton.Text = "Send Msg";
            SendMessageButton.UseVisualStyleBackColor = true;
            SendMessageButton.Click += SendMessageButton_Click;
            // 
            // RefreshUsersButton
            // 
            RefreshUsersButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            RefreshUsersButton.Location = new Point(10, 515);
            RefreshUsersButton.Name = "RefreshUsersButton";
            RefreshUsersButton.Size = new Size(120, 41);
            RefreshUsersButton.TabIndex = 4;
            RefreshUsersButton.Text = "Refresh Users";
            RefreshUsersButton.UseVisualStyleBackColor = true;
            RefreshUsersButton.Click += RefreshUsersButton_Click;
            // 
            // MessageInput
            // 
            MessageInput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MessageInput.Font = new Font("Anka/Coder Narrow", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            MessageInput.Location = new Point(136, 515);
            MessageInput.Name = "MessageInput";
            MessageInput.Size = new Size(375, 41);
            MessageInput.TabIndex = 5;
            MessageInput.Text = "";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { roomToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(653, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // roomToolStripMenuItem
            // 
            roomToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { updateRoomMetaDataToolStripMenuItem });
            roomToolStripMenuItem.Name = "roomToolStripMenuItem";
            roomToolStripMenuItem.Size = new Size(51, 20);
            roomToolStripMenuItem.Text = "Room";
            // 
            // updateRoomMetaDataToolStripMenuItem
            // 
            updateRoomMetaDataToolStripMenuItem.Name = "updateRoomMetaDataToolStripMenuItem";
            updateRoomMetaDataToolStripMenuItem.Size = new Size(201, 22);
            updateRoomMetaDataToolStripMenuItem.Text = "Update Room MetaData";
            updateRoomMetaDataToolStripMenuItem.Click += updateRoomMetaDataToolStripMenuItem_Click;
            // 
            // RoomForm
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.SeaShell;
            ClientSize = new Size(653, 567);
            Controls.Add(MessageInput);
            Controls.Add(RefreshUsersButton);
            Controls.Add(SendMessageButton);
            Controls.Add(RecievedMessagesRoomList);
            Controls.Add(RoomUsersList);
            Controls.Add(menuStrip1);
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(139, 155);
            Name = "RoomForm";
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += RoomForm_FormClosing;
            Load += RoomForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox RoomUsersList;
        private ListBox RecievedMessagesRoomList;
        private Button SendMessageButton;
        private Button RefreshUsersButton;
        private RichTextBox MessageInput;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem roomToolStripMenuItem;
        private ToolStripMenuItem updateRoomMetaDataToolStripMenuItem;
    }
}