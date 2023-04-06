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
        if (disposing && (components != null))
        {
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
        tabControl1 = new TabControl();
        NetworkTab = new TabPage();
        groupBox1 = new GroupBox();
        statusStrip1 = new StatusStrip();
        LoginStatusStrip = new ToolStripStatusLabel();
        PasswordInput = new TextBox();
        UserInput = new TextBox();
        label2 = new Label();
        label1 = new Label();
        LoginButton = new Button();
        UsersTab = new TabPage();
        groupBox2 = new GroupBox();
        RefreshUsersButton = new Button();
        UserList = new ListBox();
        RoomTab = new TabPage();
        groupBox3 = new GroupBox();
        JoinRoomButton = new Button();
        CreateRoomButton = new Button();
        RefreshRoomsButton = new Button();
        RoomList = new ListBox();
        tabControl1.SuspendLayout();
        NetworkTab.SuspendLayout();
        groupBox1.SuspendLayout();
        statusStrip1.SuspendLayout();
        UsersTab.SuspendLayout();
        groupBox2.SuspendLayout();
        RoomTab.SuspendLayout();
        groupBox3.SuspendLayout();
        SuspendLayout();
        // 
        // tabControl1
        // 
        tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tabControl1.Controls.Add(NetworkTab);
        tabControl1.Controls.Add(UsersTab);
        tabControl1.Controls.Add(RoomTab);
        tabControl1.Location = new Point(17, 20);
        tabControl1.Margin = new Padding(4, 5, 4, 5);
        tabControl1.Name = "tabControl1";
        tabControl1.SelectedIndex = 0;
        tabControl1.Size = new Size(1009, 1232);
        tabControl1.TabIndex = 3;
        // 
        // NetworkTab
        // 
        NetworkTab.Controls.Add(groupBox1);
        NetworkTab.Location = new Point(4, 34);
        NetworkTab.Margin = new Padding(4, 5, 4, 5);
        NetworkTab.Name = "NetworkTab";
        NetworkTab.Padding = new Padding(4, 5, 4, 5);
        NetworkTab.Size = new Size(1001, 1194);
        NetworkTab.TabIndex = 0;
        NetworkTab.Text = "Network";
        NetworkTab.UseVisualStyleBackColor = true;
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(statusStrip1);
        groupBox1.Controls.Add(PasswordInput);
        groupBox1.Controls.Add(UserInput);
        groupBox1.Controls.Add(label2);
        groupBox1.Controls.Add(label1);
        groupBox1.Controls.Add(LoginButton);
        groupBox1.Location = new Point(24, 28);
        groupBox1.Margin = new Padding(4, 5, 4, 5);
        groupBox1.Name = "groupBox1";
        groupBox1.Padding = new Padding(4, 5, 4, 5);
        groupBox1.Size = new Size(660, 177);
        groupBox1.TabIndex = 1;
        groupBox1.TabStop = false;
        groupBox1.Text = "User Details";
        // 
        // statusStrip1
        // 
        statusStrip1.ImageScalingSize = new Size(24, 24);
        statusStrip1.Items.AddRange(new ToolStripItem[] { LoginStatusStrip });
        statusStrip1.Location = new Point(4, 140);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Padding = new Padding(1, 0, 20, 0);
        statusStrip1.Size = new Size(652, 32);
        statusStrip1.SizingGrip = false;
        statusStrip1.TabIndex = 5;
        statusStrip1.Text = "statusStrip1";
        // 
        // LoginStatusStrip
        // 
        LoginStatusStrip.Name = "LoginStatusStrip";
        LoginStatusStrip.Size = new Size(113, 25);
        LoginStatusStrip.Text = "Login Status:";
        // 
        // PasswordInput
        // 
        PasswordInput.Location = new Point(133, 85);
        PasswordInput.Margin = new Padding(4, 5, 4, 5);
        PasswordInput.Name = "PasswordInput";
        PasswordInput.Size = new Size(310, 31);
        PasswordInput.TabIndex = 4;
        // 
        // UserInput
        // 
        UserInput.Location = new Point(133, 37);
        UserInput.Margin = new Padding(4, 5, 4, 5);
        UserInput.Name = "UserInput";
        UserInput.Size = new Size(310, 31);
        UserInput.TabIndex = 3;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(21, 85);
        label2.Margin = new Padding(4, 0, 4, 0);
        label2.Name = "label2";
        label2.Size = new Size(87, 25);
        label2.TabIndex = 2;
        label2.Text = "Password";
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(21, 38);
        label1.Margin = new Padding(4, 0, 4, 0);
        label1.Name = "label1";
        label1.Size = new Size(94, 25);
        label1.TabIndex = 1;
        label1.Text = "UserName";
        // 
        // LoginButton
        // 
        LoginButton.Location = new Point(484, 37);
        LoginButton.Margin = new Padding(4, 5, 4, 5);
        LoginButton.Name = "LoginButton";
        LoginButton.Size = new Size(160, 87);
        LoginButton.TabIndex = 0;
        LoginButton.Text = "Login";
        LoginButton.UseVisualStyleBackColor = true;
        LoginButton.Click += LoginButton_Click;
        // 
        // UsersTab
        // 
        UsersTab.Controls.Add(groupBox2);
        UsersTab.Location = new Point(4, 34);
        UsersTab.Margin = new Padding(4, 5, 4, 5);
        UsersTab.Name = "UsersTab";
        UsersTab.Padding = new Padding(4, 5, 4, 5);
        UsersTab.Size = new Size(1001, 1194);
        UsersTab.TabIndex = 1;
        UsersTab.Text = "Users";
        UsersTab.UseVisualStyleBackColor = true;
        // 
        // groupBox2
        // 
        groupBox2.Controls.Add(RefreshUsersButton);
        groupBox2.Controls.Add(UserList);
        groupBox2.Location = new Point(9, 10);
        groupBox2.Margin = new Padding(4, 5, 4, 5);
        groupBox2.Name = "groupBox2";
        groupBox2.Padding = new Padding(4, 5, 4, 5);
        groupBox2.Size = new Size(900, 1020);
        groupBox2.TabIndex = 2;
        groupBox2.TabStop = false;
        groupBox2.Text = "User List";
        // 
        // RefreshUsersButton
        // 
        RefreshUsersButton.Location = new Point(23, 768);
        RefreshUsersButton.Margin = new Padding(4, 5, 4, 5);
        RefreshUsersButton.Name = "RefreshUsersButton";
        RefreshUsersButton.Size = new Size(364, 38);
        RefreshUsersButton.TabIndex = 2;
        RefreshUsersButton.Text = "Refresh Users";
        RefreshUsersButton.UseVisualStyleBackColor = true;
        RefreshUsersButton.Click += RefreshUsersButton_Click;
        // 
        // UserList
        // 
        UserList.FormattingEnabled = true;
        UserList.ItemHeight = 25;
        UserList.Location = new Point(21, 37);
        UserList.Margin = new Padding(4, 5, 4, 5);
        UserList.Name = "UserList";
        UserList.Size = new Size(388, 704);
        UserList.TabIndex = 0;

        // 
        // RoomTab
        // 
        RoomTab.Controls.Add(groupBox3);
        RoomTab.Location = new Point(4, 34);
        RoomTab.Margin = new Padding(4, 5, 4, 5);
        RoomTab.Name = "RoomTab";
        RoomTab.Size = new Size(1001, 1194);
        RoomTab.TabIndex = 2;
        RoomTab.Text = "Rooms";
        RoomTab.UseVisualStyleBackColor = true;
        // 
        // groupBox3
        // 
        groupBox3.Controls.Add(JoinRoomButton);
        groupBox3.Controls.Add(CreateRoomButton);
        groupBox3.Controls.Add(RefreshRoomsButton);
        groupBox3.Controls.Add(RoomList);
        groupBox3.Location = new Point(4, 5);
        groupBox3.Margin = new Padding(4, 5, 4, 5);
        groupBox3.Name = "groupBox3";
        groupBox3.Padding = new Padding(4, 5, 4, 5);
        groupBox3.Size = new Size(373, 1013);
        groupBox3.TabIndex = 3;
        groupBox3.TabStop = false;
        groupBox3.Text = "Room List";
        // 
        // JoinRoomButton
        // 
        JoinRoomButton.Location = new Point(10, 817);
        JoinRoomButton.Margin = new Padding(4, 5, 4, 5);
        JoinRoomButton.Name = "JoinRoomButton";
        JoinRoomButton.RightToLeft = RightToLeft.Yes;
        JoinRoomButton.Size = new Size(174, 38);
        JoinRoomButton.TabIndex = 3;
        JoinRoomButton.Text = "Join Room";
        JoinRoomButton.UseVisualStyleBackColor = true;
        // 
        // CreateRoomButton
        // 
        CreateRoomButton.Location = new Point(216, 817);
        CreateRoomButton.Margin = new Padding(4, 5, 4, 5);
        CreateRoomButton.Name = "CreateRoomButton";
        CreateRoomButton.Size = new Size(149, 38);
        CreateRoomButton.TabIndex = 2;
        CreateRoomButton.Text = "Create Room";
        CreateRoomButton.UseVisualStyleBackColor = true;
        CreateRoomButton.Click += CreateRoomButton_Click;
        // 
        // RefreshRoomsButton
        // 
        RefreshRoomsButton.Location = new Point(9, 768);
        RefreshRoomsButton.Margin = new Padding(4, 5, 4, 5);
        RefreshRoomsButton.Name = "RefreshRoomsButton";
        RefreshRoomsButton.Size = new Size(356, 38);
        RefreshRoomsButton.TabIndex = 1;
        RefreshRoomsButton.Text = "Refresh Rooms";
        RefreshRoomsButton.UseVisualStyleBackColor = true;
        RefreshRoomsButton.Click += RefreshRoomsButton_Click;
        // 
        // RoomList
        // 
        RoomList.FormattingEnabled = true;
        RoomList.ItemHeight = 25;
        RoomList.Location = new Point(9, 37);
        RoomList.Margin = new Padding(4, 5, 4, 5);
        RoomList.Name = "RoomList";
        RoomList.Size = new Size(354, 704);
        RoomList.TabIndex = 0;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1043, 1258);
        Controls.Add(tabControl1);
        Margin = new Padding(4, 5, 4, 5);
        Name = "Form1";
        Text = "Network Manager";
        Load += Form1_Load;
        tabControl1.ResumeLayout(false);
        NetworkTab.ResumeLayout(false);
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
        UsersTab.ResumeLayout(false);
        groupBox2.ResumeLayout(false);
        RoomTab.ResumeLayout(false);
        groupBox3.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TabControl tabControl1;
    private TabPage NetworkTab;
    private GroupBox groupBox1;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel LoginStatusStrip;
    private TextBox PasswordInput;
    private TextBox UserInput;
    private Label label2;
    private Label label1;
    private Button LoginButton;
    private TabPage UsersTab;
    private GroupBox groupBox2;
    private Button RefreshUsersButton;
    private ListBox UserList;
    private TabPage RoomTab;
    private GroupBox groupBox3;
    private Button CreateRoomButton;
    private Button RefreshRoomsButton;
    private ListBox RoomList;
    private Button JoinRoomButton;
}