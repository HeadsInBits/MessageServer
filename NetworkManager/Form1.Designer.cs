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
        components = new System.ComponentModel.Container();
        tabControl1 = new TabControl();
        NetworkTab = new TabPage();
        groupBox5 = new GroupBox();
        label6 = new Label();
        label5 = new Label();
        PortInput = new TextBox();
        AddressInput = new TextBox();
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
        UserNameLabel = new Label();
        RefreshUsersButton = new Button();
        UserList = new ListBox();
        RoomTab = new TabPage();
        metaDataGridView = new DataGridView();
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
        RefreshRoomsListTimer = new System.Windows.Forms.Timer(components);
        tabControl1.SuspendLayout();
        NetworkTab.SuspendLayout();
        groupBox5.SuspendLayout();
        groupBox1.SuspendLayout();
        statusStrip1.SuspendLayout();
        UsersTab.SuspendLayout();
        groupBox2.SuspendLayout();
        RoomTab.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)metaDataGridView).BeginInit();
        groupBox4.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)MaxMembersInput).BeginInit();
        groupBox3.SuspendLayout();
        SuspendLayout();
        // 
        // tabControl1
        // 
        tabControl1.Controls.Add(NetworkTab);
        tabControl1.Controls.Add(UsersTab);
        tabControl1.Controls.Add(RoomTab);
        tabControl1.Dock = DockStyle.Fill;
        tabControl1.Font = new Font("Anka/Coder Narrow", 12F, FontStyle.Bold, GraphicsUnit.Point);
        tabControl1.Location = new Point(0, 0);
        tabControl1.Name = "tabControl1";
        tabControl1.SelectedIndex = 0;
        tabControl1.Size = new Size(618, 498);
        tabControl1.TabIndex = 3;
        tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
        // 
        // NetworkTab
        // 
        NetworkTab.Controls.Add(groupBox5);
        NetworkTab.Controls.Add(groupBox1);
        NetworkTab.Location = new Point(4, 28);
        NetworkTab.Name = "NetworkTab";
        NetworkTab.Padding = new Padding(3);
        NetworkTab.Size = new Size(610, 466);
        NetworkTab.TabIndex = 0;
        NetworkTab.Text = "Network";
        NetworkTab.UseVisualStyleBackColor = true;
        // 
        // groupBox5
        // 
        groupBox5.Controls.Add(label6);
        groupBox5.Controls.Add(label5);
        groupBox5.Controls.Add(PortInput);
        groupBox5.Controls.Add(AddressInput);
        groupBox5.Dock = DockStyle.Top;
        groupBox5.Location = new Point(3, 3);
        groupBox5.Name = "groupBox5";
        groupBox5.Size = new Size(604, 70);
        groupBox5.TabIndex = 2;
        groupBox5.TabStop = false;
        groupBox5.Text = "Server Details";
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.Location = new Point(339, 28);
        label6.Name = "label6";
        label6.Size = new Size(37, 19);
        label6.TabIndex = 3;
        label6.Text = "Port";
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.Location = new Point(22, 28);
        label5.Name = "label5";
        label5.Size = new Size(58, 19);
        label5.TabIndex = 2;
        label5.Text = "Address";
        // 
        // PortInput
        // 
        PortInput.Location = new Point(382, 25);
        PortInput.Name = "PortInput";
        PortInput.Size = new Size(88, 26);
        PortInput.TabIndex = 1;
        PortInput.Text = "8080";
        // 
        // AddressInput
        // 
        AddressInput.Location = new Point(93, 25);
        AddressInput.Name = "AddressInput";
        AddressInput.Size = new Size(218, 26);
        AddressInput.TabIndex = 0;
        AddressInput.Text = "localhost";
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(statusStrip1);
        groupBox1.Controls.Add(PasswordInput);
        groupBox1.Controls.Add(UserInput);
        groupBox1.Controls.Add(label2);
        groupBox1.Controls.Add(label1);
        groupBox1.Controls.Add(LoginButton);
        groupBox1.Dock = DockStyle.Bottom;
        groupBox1.Location = new Point(3, 357);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new Size(604, 106);
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
        statusStrip1.Size = new Size(598, 22);
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
        PasswordInput.Size = new Size(218, 26);
        PasswordInput.TabIndex = 4;
        // 
        // UserInput
        // 
        UserInput.Location = new Point(93, 22);
        UserInput.Name = "UserInput";
        UserInput.Size = new Size(218, 26);
        UserInput.TabIndex = 3;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(15, 51);
        label2.Name = "label2";
        label2.Size = new Size(65, 19);
        label2.TabIndex = 2;
        label2.Text = "Password";
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(15, 23);
        label1.Name = "label1";
        label1.Size = new Size(65, 19);
        label1.TabIndex = 1;
        label1.Text = "UserName";
        // 
        // LoginButton
        // 
        LoginButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        LoginButton.Location = new Point(339, 22);
        LoginButton.Name = "LoginButton";
        LoginButton.Size = new Size(227, 52);
        LoginButton.TabIndex = 0;
        LoginButton.Text = "Login";
        LoginButton.UseVisualStyleBackColor = true;
        LoginButton.Click += LoginButton_Click;
        // 
        // UsersTab
        // 
        UsersTab.Controls.Add(groupBox2);
        UsersTab.Location = new Point(4, 28);
        UsersTab.Name = "UsersTab";
        UsersTab.Padding = new Padding(3);
        UsersTab.Size = new Size(610, 466);
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
        groupBox2.Size = new Size(604, 460);
        groupBox2.TabIndex = 2;
        groupBox2.TabStop = false;
        groupBox2.Text = "User List";
        // 
        // UserNameLabel
        // 
        UserNameLabel.AutoSize = true;
        UserNameLabel.Dock = DockStyle.Left;
        UserNameLabel.Location = new Point(207, 22);
        UserNameLabel.Name = "UserNameLabel";
        UserNameLabel.Size = new Size(72, 19);
        UserNameLabel.TabIndex = 3;
        UserNameLabel.Text = "UserName:";
        // 
        // RefreshUsersButton
        // 
        RefreshUsersButton.AutoSize = true;
        RefreshUsersButton.Dock = DockStyle.Bottom;
        RefreshUsersButton.Location = new Point(207, 428);
        RefreshUsersButton.Name = "RefreshUsersButton";
        RefreshUsersButton.Size = new Size(394, 29);
        RefreshUsersButton.TabIndex = 2;
        RefreshUsersButton.Text = "Refresh Users";
        RefreshUsersButton.UseVisualStyleBackColor = true;
        RefreshUsersButton.Click += RefreshUsersButton_Click;
        // 
        // UserList
        // 
        UserList.Dock = DockStyle.Left;
        UserList.FormattingEnabled = true;
        UserList.ItemHeight = 19;
        UserList.Location = new Point(3, 22);
        UserList.Name = "UserList";
        UserList.Size = new Size(204, 435);
        UserList.TabIndex = 0;
        // 
        // RoomTab
        // 
        RoomTab.Controls.Add(metaDataGridView);
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
        RoomTab.Location = new Point(4, 28);
        RoomTab.Name = "RoomTab";
        RoomTab.Size = new Size(610, 466);
        RoomTab.TabIndex = 2;
        RoomTab.Text = "Rooms";
        RoomTab.UseVisualStyleBackColor = true;
        // 
        // metaDataGridView
        // 
        metaDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        metaDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        metaDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        metaDataGridView.BackgroundColor = SystemColors.Info;
        metaDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        metaDataGridView.Location = new Point(267, 139);
        metaDataGridView.MultiSelect = false;
        metaDataGridView.Name = "metaDataGridView";
        metaDataGridView.ReadOnly = true;
        metaDataGridView.RowHeadersVisible = false;
        metaDataGridView.RowTemplate.Height = 25;
        metaDataGridView.Size = new Size(335, 134);
        metaDataGridView.TabIndex = 13;
        // 
        // JoinRoomButton
        // 
        JoinRoomButton.Dock = DockStyle.Bottom;
        JoinRoomButton.Location = new Point(261, 279);
        JoinRoomButton.Name = "JoinRoomButton";
        JoinRoomButton.RightToLeft = RightToLeft.Yes;
        JoinRoomButton.Size = new Size(349, 23);
        JoinRoomButton.TabIndex = 3;
        JoinRoomButton.Text = "Join Room";
        JoinRoomButton.UseVisualStyleBackColor = true;
        JoinRoomButton.Click += JoinRoomButton_Click;
        // 
        // groupBox4
        // 
        groupBox4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        groupBox4.Controls.Add(label4);
        groupBox4.Controls.Add(RoomNameInput);
        groupBox4.Controls.Add(PublicRoomImput);
        groupBox4.Controls.Add(label3);
        groupBox4.Controls.Add(MaxMembersInput);
        groupBox4.Controls.Add(CreateRoomButton);
        groupBox4.Dock = DockStyle.Bottom;
        groupBox4.FlatStyle = FlatStyle.Flat;
        groupBox4.Location = new Point(261, 302);
        groupBox4.Name = "groupBox4";
        groupBox4.Size = new Size(349, 164);
        groupBox4.TabIndex = 12;
        groupBox4.TabStop = false;
        groupBox4.Text = "New Room";
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(11, 27);
        label4.Name = "label4";
        label4.Size = new Size(44, 19);
        label4.TabIndex = 11;
        label4.Text = "Name:";
        // 
        // RoomNameInput
        // 
        RoomNameInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        RoomNameInput.Location = new Point(59, 22);
        RoomNameInput.Name = "RoomNameInput";
        RoomNameInput.Size = new Size(248, 26);
        RoomNameInput.TabIndex = 10;
        // 
        // PublicRoomImput
        // 
        PublicRoomImput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        PublicRoomImput.AutoSize = true;
        PublicRoomImput.Checked = true;
        PublicRoomImput.CheckState = CheckState.Checked;
        PublicRoomImput.Location = new Point(6, 109);
        PublicRoomImput.Name = "PublicRoomImput";
        PublicRoomImput.Size = new Size(105, 23);
        PublicRoomImput.TabIndex = 9;
        PublicRoomImput.Text = "Public Room";
        PublicRoomImput.UseVisualStyleBackColor = true;
        // 
        // label3
        // 
        label3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        label3.AutoSize = true;
        label3.Location = new Point(11, 51);
        label3.Name = "label3";
        label3.Size = new Size(121, 19);
        label3.TabIndex = 8;
        label3.Text = "Maximum Members:";
        // 
        // MaxMembersInput
        // 
        MaxMembersInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        MaxMembersInput.Location = new Point(135, 51);
        MaxMembersInput.Name = "MaxMembersInput";
        MaxMembersInput.Size = new Size(172, 26);
        MaxMembersInput.TabIndex = 3;
        MaxMembersInput.Value = new decimal(new int[] { 15, 0, 0, 0 });
        // 
        // CreateRoomButton
        // 
        CreateRoomButton.Dock = DockStyle.Bottom;
        CreateRoomButton.Location = new Point(3, 138);
        CreateRoomButton.Name = "CreateRoomButton";
        CreateRoomButton.Size = new Size(343, 23);
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
        groupBox3.Size = new Size(261, 466);
        groupBox3.TabIndex = 3;
        groupBox3.TabStop = false;
        groupBox3.Text = "Room List";
        // 
        // RefreshRoomsButton
        // 
        RefreshRoomsButton.Dock = DockStyle.Top;
        RefreshRoomsButton.Location = new Point(3, 22);
        RefreshRoomsButton.Name = "RefreshRoomsButton";
        RefreshRoomsButton.Size = new Size(255, 29);
        RefreshRoomsButton.TabIndex = 1;
        RefreshRoomsButton.Text = "Refresh Rooms";
        RefreshRoomsButton.UseVisualStyleBackColor = true;
        RefreshRoomsButton.Click += RefreshRoomsButton_Click;
        // 
        // RoomList
        // 
        RoomList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        RoomList.FormattingEnabled = true;
        RoomList.ItemHeight = 19;
        RoomList.Location = new Point(3, 60);
        RoomList.Name = "RoomList";
        RoomList.Size = new Size(255, 403);
        RoomList.TabIndex = 0;
        RoomList.SelectedIndexChanged += RoomList_SelectedIndexChanged;
        // 
        // RefreshRoomsListTimer
        // 
        RefreshRoomsListTimer.Interval = 1500;
        RefreshRoomsListTimer.Tick += RefreshRoomsListTimer_Tick;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(224, 224, 224);
        ClientSize = new Size(618, 498);
        Controls.Add(tabControl1);
        MinimumSize = new Size(550, 350);
        Name = "Form1";
        Text = "Network Manager";
        FormClosing += Form1_FormClosing;
        Load += Form1_Load;
        tabControl1.ResumeLayout(false);
        NetworkTab.ResumeLayout(false);
        groupBox5.ResumeLayout(false);
        groupBox5.PerformLayout();
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
        ((System.ComponentModel.ISupportInitialize)metaDataGridView).EndInit();
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
    private GroupBox groupBox5;
    private TextBox AddressInput;
    private TextBox PortInput;
    private Label label6;
    private Label label5;
    private System.Windows.Forms.Timer RefreshRoomsListTimer;
    private DataGridView metaDataGridView;
}