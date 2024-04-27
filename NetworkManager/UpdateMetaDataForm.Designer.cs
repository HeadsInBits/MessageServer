namespace NetworkManager
{
    partial class UpdateMetaDataForm
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
            metaDataGrid = new DataGridView();
            SaveButton = new Button();
            AddButton = new Button();
            KeyText = new TextBox();
            ValueText = new TextBox();
            label1 = new Label();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)metaDataGrid).BeginInit();
            SuspendLayout();
            // 
            // metaDataGrid
            // 
            metaDataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            metaDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            metaDataGrid.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            metaDataGrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            metaDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            metaDataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            metaDataGrid.Location = new Point(16, 17);
            metaDataGrid.MultiSelect = false;
            metaDataGrid.Name = "metaDataGrid";
            metaDataGrid.ReadOnly = true;
            metaDataGrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Sunken;
            metaDataGrid.RowTemplate.Height = 25;
            metaDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            metaDataGrid.Size = new Size(405, 421);
            metaDataGrid.TabIndex = 0;
            metaDataGrid.CellClick += metaDataGrid_CellClick;
            // 
            // SaveButton
            // 
            SaveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            SaveButton.Location = new Point(276, 530);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(145, 23);
            SaveButton.TabIndex = 1;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += button1_Click;
            // 
            // AddButton
            // 
            AddButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            AddButton.Location = new Point(21, 530);
            AddButton.Name = "AddButton";
            AddButton.Size = new Size(185, 23);
            AddButton.TabIndex = 2;
            AddButton.Text = "Add";
            AddButton.UseVisualStyleBackColor = true;
            AddButton.Click += AddButton_Click;
            // 
            // KeyText
            // 
            KeyText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            KeyText.Location = new Point(16, 454);
            KeyText.Name = "KeyText";
            KeyText.Size = new Size(405, 23);
            KeyText.TabIndex = 3;
            // 
            // ValueText
            // 
            ValueText.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ValueText.Location = new Point(16, 498);
            ValueText.Name = "ValueText";
            ValueText.Size = new Size(405, 23);
            ValueText.TabIndex = 4;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(21, 438);
            label1.Name = "label1";
            label1.Size = new Size(26, 15);
            label1.TabIndex = 5;
            label1.Text = "Key";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(16, 482);
            label2.Name = "label2";
            label2.Size = new Size(35, 15);
            label2.TabIndex = 6;
            label2.Text = "Value";
            // 
            // UpdateMetaDataForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(433, 565);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(ValueText);
            Controls.Add(KeyText);
            Controls.Add(AddButton);
            Controls.Add(SaveButton);
            Controls.Add(metaDataGrid);
            Name = "UpdateMetaDataForm";
            Text = "UpdateMetaDataForm";
            ((System.ComponentModel.ISupportInitialize)metaDataGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView metaDataGrid;
        private Button SaveButton;
        private Button AddButton;
        private TextBox KeyText;
        private TextBox ValueText;
        private Label label1;
        private Label label2;
    }
}