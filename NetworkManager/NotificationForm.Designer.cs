namespace NetworkManager
{
    partial class NotificationForm
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
            RoomNameLabel = new Label();
            MessageTextLaebl = new Label();
            SuspendLayout();
            // 
            // RoomNameLabel
            // 
            RoomNameLabel.AutoSize = true;
            RoomNameLabel.Font = new Font("Anka/Coder", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            RoomNameLabel.Location = new Point(14, 16);
            RoomNameLabel.Name = "RoomNameLabel";
            RoomNameLabel.Size = new Size(125, 18);
            RoomNameLabel.TabIndex = 0;
            RoomNameLabel.Text = "RoomNameLabel";
            // 
            // MessageTextLaebl
            // 
            MessageTextLaebl.AutoSize = true;
            MessageTextLaebl.Font = new Font("Anka/Coder", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            MessageTextLaebl.Location = new Point(14, 65);
            MessageTextLaebl.Name = "MessageTextLaebl";
            MessageTextLaebl.Size = new Size(152, 18);
            MessageTextLaebl.TabIndex = 1;
            MessageTextLaebl.Text = "MessageTextlabel";
            // 
            // NotificationForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(326, 143);
            Controls.Add(MessageTextLaebl);
            Controls.Add(RoomNameLabel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "NotificationForm";
            StartPosition = FormStartPosition.Manual;
            Text = "NotificationForm";
            Load += NotificationForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label RoomNameLabel;
        private Label MessageTextLaebl;
    }
}