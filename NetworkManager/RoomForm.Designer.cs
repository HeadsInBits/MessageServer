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
			if (disposing && (components != null)) {
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
			MessageInput = new TextBox();
			SendMessageButton = new Button();
			SuspendLayout();
			// 
			// RoomUsersList
			// 
			RoomUsersList.FormattingEnabled = true;
			RoomUsersList.ItemHeight = 15;
			RoomUsersList.Location = new Point(12, 22);
			RoomUsersList.Name = "RoomUsersList";
			RoomUsersList.Size = new Size(141, 649);
			RoomUsersList.TabIndex = 0;
			// 
			// RecievedMessagesRoomList
			// 
			RecievedMessagesRoomList.FormattingEnabled = true;
			RecievedMessagesRoomList.ItemHeight = 15;
			RecievedMessagesRoomList.Location = new Point(169, 22);
			RecievedMessagesRoomList.Name = "RecievedMessagesRoomList";
			RecievedMessagesRoomList.Size = new Size(619, 589);
			RecievedMessagesRoomList.TabIndex = 1;
			// 
			// MessageInput
			// 
			MessageInput.Location = new Point(169, 617);
			MessageInput.Multiline = true;
			MessageInput.Name = "MessageInput";
			MessageInput.Size = new Size(467, 48);
			MessageInput.TabIndex = 2;
			// 
			// SendMessageButton
			// 
			SendMessageButton.Location = new Point(642, 617);
			SendMessageButton.Name = "SendMessageButton";
			SendMessageButton.Size = new Size(146, 48);
			SendMessageButton.TabIndex = 3;
			SendMessageButton.Text = "Send Msg";
			SendMessageButton.UseVisualStyleBackColor = true;
			SendMessageButton.Click += SendMessageButton_Click;
			// 
			// RoomForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 677);
			Controls.Add(SendMessageButton);
			Controls.Add(MessageInput);
			Controls.Add(RecievedMessagesRoomList);
			Controls.Add(RoomUsersList);
			Name = "RoomForm";
			Text = "RoomForm";
			Load += RoomForm_Load;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private ListBox RoomUsersList;
		private ListBox RecievedMessagesRoomList;
		private TextBox MessageInput;
		private Button SendMessageButton;
	}
}