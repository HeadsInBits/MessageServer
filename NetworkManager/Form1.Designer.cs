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
        JoinRoomButton = new Button();
        groupBox4 = new GroupBox();
        label4 = new Label();
        RoomNameInput = new TextBox();
        PublicRoomImput = new CheckBox();
        label3 = new Label();
        MaxMembersInput = new NumericUpDown();
        CreateRoomButton = new Button();
        GUIDLabel = new Label();
        CreationDateLabel = new Label();
        RoomCreatorLabel = new Label();
        MetaDataLabel = new Label();
        MaxMembersLabel = new Label();
        RoomLockedLabel = new Label();
        AccessLevelLabel = new Label();
        RoomNameLabel = new Label();
        groupBox3 = new GroupBox();
        RefreshRoomsButton = new Button();
        RoomList = new ListBox();
        UserNameLabel = new Label();
        tabControl1.SuspendLayout();
        NetworkTab.SuspendLayout();
        groupBox1.SuspendLayout();
        statusStrip1.SuspendLayout();
        UsersTab.SuspendLayout();
        groupBox2.SuspendLayout();
        RoomTab.SuspendLayout();
        groupBox4.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)MaxMembersInput).BeginInit();
        groupBox3.SuspendLayout();
        SuspendLayout();
        // 
        // tabControl1
        // 
        tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tabControl1.Controls.Add(NetworkTab);
        tabControl1.Controls.Add(UsersTab);
        tabControl1.Controls.Add(RoomTab);
        tabControl1.Location = new Point(12, 12);
        tabControl1.Name = "tabControl1";
        tabControl1.SelectedIndex = 0;
        tabControl1.Size = new Size(594, 482);
        tabControl1.TabIndex = 3;
        // 
        // NetworkTab
        // 
        NetworkTab.Controls.Add(groupBox1);
        NetworkTab.Location = new Point(4, 24);
        NetworkTab.Name = "NetworkTab";
        NetworkTab.Padding = new Padding(3);
        NetworkTab.Size = new Size(586, 454);
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
        groupBox1.Dock = DockStyle.Top;
        groupBox1.Location = new Point(3, 3);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(580, 106);
        groupBox1.TabIndex = 1;
        groupBox1.TabStop = false;
        groupBox1.Text = "User Details";
        // 
        // statusStrip1
        // 
        statusStrip1.ImageScalingSize = new Size(24, 24);
        statusStrip1.Items.AddRange(new ToolStripItem[] { LoginStatusStrip });
        statusStrip1.Location = new Point(3, 81);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Size = new Size(574, 22);
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
        // UsersTab
        // 
        UsersTab.Controls.Add(groupBox2);
        UsersTab.Location = new Point(4, 24);
        UsersTab.Name = "UsersTab";
        UsersTab.Padding = new Padding(3);
        UsersTab.Size = new Size(586, 454);
        UsersTab.TabIndex = 1;
        UsersTab.Text = "Users";
        UsersTab.UseVisualStyleBackColor = true;
        // 
        // groupBox2
        // 
        groupBox2.AutoSize = true;
        groupBox2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        groupBox2.Controls.Add(UserNameLabel);
        groupBox2.Controls.Add(RefreshUsersButton);
        groupBox2.Controls.Add(UserList);
        groupBox2.Dock = DockStyle.Fill;
        groupBox2.Location = new Point(3, 3);
        groupBox2.Name = "groupBox2";
        groupBox2.Size = new Size(580, 448);
        groupBox2.TabIndex = 2;
        groupBox2.TabStop = false;
        groupBox2.Text = "User List";
        // 
        // RefreshUsersButton
        // 
        RefreshUsersButton.Location = new Point(15, 407);
        RefreshUsersButton.Name = "RefreshUsersButton";
        RefreshUsersButton.Size = new Size(204, 23);
        RefreshUsersButton.TabIndex = 2;
        RefreshUsersButton.Text = "Refresh Users";
        RefreshUsersButton.UseVisualStyleBackColor = true;
        RefreshUsersButton.Click += RefreshUsersButton_Click;
        // 
        // UserList
        // 
        UserList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        UserList.FormattingEnabled = true;
        UserList.ItemHeight = 15;
        UserList.Location = new Point(15, 22);
        UserList.Name = "UserList";
        UserList.Size = new Size(204, 379);
        UserList.TabIndex = 0;
        // 
        // RoomTab
        // 
        RoomTab.Controls.Add(JoinRoomButton);
        RoomTab.Controls.Add(groupBox4);
        RoomTab.Controls.Add(GUIDLabel);
        RoomTab.Controls.Add(CreationDateLabel);
        RoomTab.Controls.Add(RoomCreatorLabel);
        RoomTab.Controls.Add(MetaDataLabel);
        RoomTab.Controls.Add(MaxMembersLabel);
        RoomTab.Controls.Add(RoomLockedLabel);
        RoomTab.Controls.Add(AccessLevelLabel);
        RoomTab.Controls.Add(RoomNameLabel);
        RoomTab.Controls.Add(groupBox3);
        RoomTab.Location = new Point(4, 24);
        RoomTab.Name = "RoomTab";
        RoomTab.Size = new Size(586, 454);
        RoomTab.TabIndex = 2;
        RoomTab.Text = "Rooms";
        RoomTab.UseVisualStyleBackColor = true;
        // 
        // JoinRoomButton
        // 
        JoinRoomButton.Dock = DockStyle.Top;
        JoinRoomButton.Location = new Point(261, 136);
        JoinRoomButton.Name = "JoinRoomButton";
        JoinRoomButton.RightToLeft = RightToLeft.Yes;
        JoinRoomButton.Size = new Size(325, 23);
        JoinRoomButton.TabIndex = 3;
        JoinRoomButton.Text = "Join Room";
        JoinRoomButton.UseVisualStyleBackColor = true;
        JoinRoomButton.Click += JoinRoomButton_Click;
        // 
        // groupBox4
        // 
        groupBox4.AutoSize = true;
        groupBox4.Controls.Add(label4);
        groupBox4.Controls.Add(RoomNameInput);
        groupBox4.Controls.Add(PublicRoomImput);
        groupBox4.Controls.Add(label3);
        groupBox4.Controls.Add(MaxMembersInput);
        groupBox4.Controls.Add(CreateRoomButton);
        groupBox4.Dock = DockStyle.Bottom;
        groupBox4.Location = new Point(261, 298);
        groupBox4.Name = "groupBox4";
        groupBox4.Size = new Size(325, 156);
        groupBox4.TabIndex = 12;
        groupBox4.TabStop = false;
        groupBox4.Text = "New Room";
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(11, 27);
        label4.Name = "label4";
        label4.Size = new Size(42, 15);
        label4.TabIndex = 11;
        label4.Text = "Name:";
        // 
        // RoomNameInput
        // 
        RoomNameInput.Location = new Point(59, 22);
        RoomNameInput.Name = "RoomNameInput";
        RoomNameInput.Size = new Size(224, 23);
        RoomNameInput.TabIndex = 10;
        // 
        // PublicRoomImput
        // 
        PublicRoomImput.AutoSize = true;
        PublicRoomImput.Checked = true;
        PublicRoomImput.CheckState = CheckState.Checked;
        PublicRoomImput.Location = new Point(189, 86);
        PublicRoomImput.Name = "PublicRoomImput";
        PublicRoomImput.Size = new Size(94, 19);
        PublicRoomImput.TabIndex = 9;
        PublicRoomImput.Text = "Public Room";
        PublicRoomImput.UseVisualStyleBackColor = true;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(11, 51);
        label3.Name = "label3";
        label3.Size = new Size(118, 15);
        label3.TabIndex = 8;
        label3.Text = "Maximum Members:";
        // 
        // MaxMembersInput
        // 
        MaxMembersInput.Location = new Point(135, 51);
        MaxMembersInput.Name = "MaxMembersInput";
        MaxMembersInput.Size = new Size(148, 23);
        MaxMembersInput.TabIndex = 3;
        // 
        // CreateRoomButton
        // 
        CreateRoomButton.Location = new Point(11, 111);
        CreateRoomButton.Name = "CreateRoomButton";
        CreateRoomButton.Size = new Size(272, 23);
        CreateRoomButton.TabIndex = 2;
        CreateRoomButton.Text = "Create Room";
        CreateRoomButton.UseVisualStyleBackColor = true;
        CreateRoomButton.Click += CreateRoomButton_Click;
        // 
        // GUIDLabel
        // 
        GUIDLabel.AutoSize = true;
        GUIDLabel.Dock = DockStyle.Top;
        GUIDLabel.Font = new Font("OCR A Extended", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
        GUIDLabel.Location = new Point(261, 119);
        GUIDLabel.Name = "GUIDLabel";
        GUIDLabel.Size = new Size(62, 17);
        GUIDLabel.TabIndex = 11;
        GUIDLabel.Text = "GUID: ";
        // 
        // CreationDateLabel
        // 
        CreationDateLabel.AutoSize = true;
        CreationDateLabel.Dock = DockStyle.Top;
        CreationDateLabel.Font = new Font("OCR A Extended", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
        CreationDateLabel.Location = new Point(261, 102);
        CreationDateLabel.Name = "CreationDateLabel";
        CreationDateLabel.Size = new Size(134, 17);
        CreationDateLabel.TabIndex = 10;
        CreationDateLabel.Text = "Creation Date:";
        // 
        // RoomCreatorLabel
        // 
        RoomCreatorLabel.AutoSize = true;
        RoomCreatorLabel.Dock = DockStyle.Top;
        RoomCreatorLabel.Font = new Font("OCR A Extended", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
        RoomCreatorLabel.Location = new Point(261, 85);
        RoomCreatorLabel.Name = "RoomCreatorLabel";
        RoomCreatorLabel.Size = new Size(116, 17);
        RoomCreatorLabel.TabIndex = 9;
        RoomCreatorLabel.Text = "RoomCreator:";
        // 
        // MetaDataLabel
        // 
        MetaDataLabel.AutoSize = true;
        MetaDataLabel.Dock = DockStyle.Top;
        MetaDataLabel.Font = new Font("OCR A Extended", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
        MetaDataLabel.Location = new Point(261, 68);
        MetaDataLabel.Name = "MetaDataLabel";
        MetaDataLabel.Size = new Size(89, 17);
        MetaDataLabel.TabIndex = 8;
        MetaDataLabel.Text = "MetaData:";
        // 
        // MaxMembersLabel
        // 
        MaxMembersLabel.AutoSize = true;
        MaxMembersLabel.Dock = DockStyle.Top;
        MaxMembersLabel.Font = new Font("OCR A Extended", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
        MaxMembersLabel.Location = new Point(261, 51);
        MaxMembersLabel.Name = "MaxMembersLabel";
        MaxMembersLabel.Size = new Size(152, 17);
        MaxMembersLabel.TabIndex = 7;
        MaxMembersLabel.Text = "Maximum Members:";
        // 
        // RoomLockedLabel
        // 
        RoomLockedLabel.AutoSize = true;
        RoomLockedLabel.Dock = DockStyle.Top;
        RoomLockedLabel.Font = new Font("OCR A Extended", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
        RoomLockedLabel.Location = new Point(261, 34);
        RoomLockedLabel.Name = "RoomLockedLabel";
        RoomLockedLabel.Size = new Size(116, 17);
        RoomLockedLabel.TabIndex = 6;
        RoomLockedLabel.Text = "Room Locked:";
        RoomLockedLabel.Click += RoomLockedLabel_Click;
        // 
        // AccessLevelLabel
        // 
        AccessLevelLabel.AutoSize = true;
        AccessLevelLabel.Dock = DockStyle.Top;
        AccessLevelLabel.Font = new Font("OCR A Extended", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
        AccessLevelLabel.Location = new Point(261, 17);
        AccessLevelLabel.Name = "AccessLevelLabel";
        AccessLevelLabel.Size = new Size(125, 17);
        AccessLevelLabel.TabIndex = 5;
        AccessLevelLabel.Text = "Access Level:";
        AccessLevelLabel.Click += AccessLevelLabel_Click;
        // 
        // RoomNameLabel
        // 
        RoomNameLabel.AutoSize = true;
        RoomNameLabel.Dock = DockStyle.Top;
        RoomNameLabel.Font = new Font("OCR A Extended", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
        RoomNameLabel.Location = new Point(261, 0);
        RoomNameLabel.Name = "RoomNameLabel";
        RoomNameLabel.Size = new Size(98, 17);
        RoomNameLabel.TabIndex = 4;
        RoomNameLabel.Text = "Room Name:";
        // 
        // groupBox3
        // 
        groupBox3.Controls.Add(RefreshRoomsButton);
        groupBox3.Controls.Add(RoomList);
        groupBox3.Dock = DockStyle.Left;
        groupBox3.Location = new Point(0, 0);
        groupBox3.Name = "groupBox3";
        groupBox3.Size = new Size(261, 454);
        groupBox3.TabIndex = 3;
        groupBox3.TabStop = false;
        groupBox3.Text = "Room List";
        // 
        // RefreshRoomsButton
        // 
        RefreshRoomsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        RefreshRoomsButton.Location = new Point(6, 415);
        RefreshRoomsButton.Name = "RefreshRoomsButton";
        RefreshRoomsButton.Size = new Size(249, 23);
        RefreshRoomsButton.TabIndex = 1;
        RefreshRoomsButton.Text = "Refresh Rooms";
        RefreshRoomsButton.UseVisualStyleBackColor = true;
        RefreshRoomsButton.Click += RefreshRoomsButton_Click;
        // 
        // RoomList
        // 
        RoomList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        RoomList.FormattingEnabled = true;
        RoomList.ItemHeight = 15;
        RoomList.Location = new Point(6, 22);
        RoomList.Name = "RoomList";
        RoomList.Size = new Size(249, 379);
        RoomList.TabIndex = 0;
        RoomList.SelectedIndexChanged += RoomList_SelectedIndexChanged;
        // 
        // UserNameLabel
        // 
        UserNameLabel.AutoSize = true;
        UserNameLabel.Location = new Point(247, 27);
        UserNameLabel.Name = "UserNameLabel";
        UserNameLabel.Size = new Size(65, 15);
        UserNameLabel.TabIndex = 3;
        UserNameLabel.Text = "UserName:";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(618, 498);
        Controls.Add(tabControl1);
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
        UsersTab.PerformLayout();
        groupBox2.ResumeLayout(false);
        groupBox2.PerformLayout();
        RoomTab.ResumeLayout(false);
        RoomTab.PerformLayout();
        groupBox4.ResumeLayout(false);
        groupBox4.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)MaxMembersInput).EndInit();
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
    private Label MetaDataLabel;
    private Label MaxMembersLabel;
    private Label RoomLockedLabel;
    private Label AccessLevelLabel;
    private Label RoomNameLabel;
    private Label RoomCreatorLabel;
    private Label CreationDateLabel;
    private Label GUIDLabel;
    private GroupBox groupBox4;
    private Label label4;
    private TextBox RoomNameInput;
    private CheckBox PublicRoomImput;
    private Label label3;
    private NumericUpDown MaxMembersInput;
    private Label UserNameLabel;
}