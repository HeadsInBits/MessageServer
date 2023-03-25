namespace NetworkManager;

partial class Form1
{
	/// <summary>
	///  Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	///  Clean up any resources being used.
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
	///  Required method for Designer support - do not modify
	///  the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		groupBox1 = new GroupBox();
		statusStrip1 = new StatusStrip();
		LoginStatusStrip = new ToolStripStatusLabel();
		PasswordInput = new TextBox();
		UserInput = new TextBox();
		label2 = new Label();
		label1 = new Label();
		LoginButton = new Button();
		groupBox3 = new GroupBox();
		CreateRoomButton = new Button();
		RefreshRoomsButton = new Button();
		RoomList = new ListBox();
		UserList = new ListBox();
		RefreshUsersButton = new Button();
		groupBox2 = new GroupBox();
		groupBox1.SuspendLayout();
		statusStrip1.SuspendLayout();
		groupBox3.SuspendLayout();
		groupBox2.SuspendLayout();
		SuspendLayout();
		// 
		// groupBox1
		// 
		groupBox1.Controls.Add(statusStrip1);
		groupBox1.Controls.Add(PasswordInput);
		groupBox1.Controls.Add(UserInput);
		groupBox1.Controls.Add(label2);
		groupBox1.Controls.Add(label1);
		groupBox1.Controls.Add(LoginButton);
		groupBox1.Location = new Point(10, 8);
		groupBox1.Name = "groupBox1";
		groupBox1.Size = new Size(462, 106);
		groupBox1.TabIndex = 0;
		groupBox1.TabStop = false;
		groupBox1.Text = "User Details";
		// 
		// statusStrip1
		// 
		statusStrip1.Items.AddRange(new ToolStripItem [] { LoginStatusStrip });
		statusStrip1.Location = new Point(3, 81);
		statusStrip1.Name = "statusStrip1";
		statusStrip1.Size = new Size(456, 22);
		statusStrip1.SizingGrip = false;
		statusStrip1.TabIndex = 5;
		statusStrip1.Text = "statusStrip1";
		// 
		// LoginStatusStrip
		// 
		LoginStatusStrip.Name = "LoginStatusStrip";
		LoginStatusStrip.Size = new Size(75, 17);
		LoginStatusStrip.Text = "Login Status:";
		// 
		// PasswordInput
		// 
		PasswordInput.Location = new Point(93, 51);
		PasswordInput.Name = "PasswordInput";
		PasswordInput.Size = new Size(218, 23);
		PasswordInput.TabIndex = 4;
		// 
		// UserInput
		// 
		UserInput.Location = new Point(93, 22);
		UserInput.Name = "UserInput";
		UserInput.Size = new Size(218, 23);
		UserInput.TabIndex = 3;
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Location = new Point(15, 51);
		label2.Name = "label2";
		label2.Size = new Size(57, 15);
		label2.TabIndex = 2;
		label2.Text = "Password";
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new Point(15, 23);
		label1.Name = "label1";
		label1.Size = new Size(62, 15);
		label1.TabIndex = 1;
		label1.Text = "UserName";
		// 
		// LoginButton
		// 
		LoginButton.Location = new Point(339, 22);
		LoginButton.Name = "LoginButton";
		LoginButton.Size = new Size(112, 52);
		LoginButton.TabIndex = 0;
		LoginButton.Text = "Login";
		LoginButton.UseVisualStyleBackColor = true;
		LoginButton.Click += LoginButton_Click;
		// 
		// groupBox3
		// 
		groupBox3.Controls.Add(CreateRoomButton);
		groupBox3.Controls.Add(RefreshRoomsButton);
		groupBox3.Controls.Add(RoomList);
		groupBox3.Location = new Point(327, 122);
		groupBox3.Name = "groupBox3";
		groupBox3.Size = new Size(261, 529);
		groupBox3.TabIndex = 2;
		groupBox3.TabStop = false;
		groupBox3.Text = "Room List";
		// 
		// CreateRoomButton
		// 
		CreateRoomButton.Location = new Point(151, 490);
		CreateRoomButton.Name = "CreateRoomButton";
		CreateRoomButton.Size = new Size(104, 23);
		CreateRoomButton.TabIndex = 2;
		CreateRoomButton.Text = "Create Room";
		CreateRoomButton.UseVisualStyleBackColor = true;
		CreateRoomButton.Click += CreateRoomButton_Click;
		// 
		// RefreshRoomsButton
		// 
		RefreshRoomsButton.Location = new Point(6, 461);
		RefreshRoomsButton.Name = "RefreshRoomsButton";
		RefreshRoomsButton.Size = new Size(249, 23);
		RefreshRoomsButton.TabIndex = 1;
		RefreshRoomsButton.Text = "Refresh Rooms";
		RefreshRoomsButton.UseVisualStyleBackColor = true;
		RefreshRoomsButton.Click += RefreshRoomsButton_Click;
		// 
		// RoomList
		// 
		RoomList.FormattingEnabled = true;
		RoomList.ItemHeight = 15;
		RoomList.Location = new Point(6, 22);
		RoomList.Name = "RoomList";
		RoomList.Size = new Size(249, 424);
		RoomList.TabIndex = 0;
		// 
		// UserList
		// 
		UserList.FormattingEnabled = true;
		UserList.ItemHeight = 15;
		UserList.Location = new Point(15, 22);
		UserList.Name = "UserList";
		UserList.Size = new Size(273, 424);
		UserList.TabIndex = 0;
		// 
		// RefreshUsersButton
		// 
		RefreshUsersButton.Location = new Point(16, 461);
		RefreshUsersButton.Name = "RefreshUsersButton";
		RefreshUsersButton.Size = new Size(255, 23);
		RefreshUsersButton.TabIndex = 2;
		RefreshUsersButton.Text = "Refresh Users";
		RefreshUsersButton.UseVisualStyleBackColor = true;
		RefreshUsersButton.Click += RefreshUsersButton_Click;
		// 
		// groupBox2
		// 
		groupBox2.Controls.Add(RefreshUsersButton);
		groupBox2.Controls.Add(UserList);
		groupBox2.Location = new Point(8, 122);
		groupBox2.Name = "groupBox2";
		groupBox2.Size = new Size(294, 529);
		groupBox2.TabIndex = 1;
		groupBox2.TabStop = false;
		groupBox2.Text = "User List";
		// 
		// Form1
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(634, 680);
		Controls.Add(groupBox3);
		Controls.Add(groupBox2);
		Controls.Add(groupBox1);
		Name = "Form1";
		Text = "Network Manager";
		groupBox1.ResumeLayout(false);
		groupBox1.PerformLayout();
		statusStrip1.ResumeLayout(false);
		statusStrip1.PerformLayout();
		groupBox3.ResumeLayout(false);
		groupBox2.ResumeLayout(false);
		ResumeLayout(false);
	}

	#endregion

	private GroupBox groupBox1;
	private StatusStrip statusStrip1;
	private TextBox PasswordInput;
	private TextBox UserInput;
	private Label label2;
	private Label label1;
	private Button LoginButton;
	private ToolStripStatusLabel LoginStatusStrip;
	private GroupBox groupBox3;
	private Button CreateRoomButton;
	private Button RefreshRoomsButton;
	private ListBox RoomList;
	private ListBox UserList;
	private Button RefreshUsersButton;
	private GroupBox groupBox2;
}