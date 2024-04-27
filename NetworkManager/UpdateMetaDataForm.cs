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
    public partial class UpdateMetaDataForm : Form
    {
        public Dictionary<string, string> _meta;
        //   public Dictionary<string, string> _returnData;
        //  public List<(string, string)> _tmpData = new List<(string, string)>();

        public UpdateMetaDataForm(Dictionary<string, string> meta)
        {
            _meta = meta;
            InitializeComponent();
            //metaDataGrid.DataSource = (from d in _meta orderby d.Value select new { d.Key, d.Value }).ToList();
            metaDataGrid.Columns.Add("Key", "Key");
            metaDataGrid.Columns.Add("Value", "Value");

            foreach (var item in _meta)
            {
                metaDataGrid.Rows.Add(item.Key, item.Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _meta.Clear();
            foreach (DataGridViewRow row in metaDataGrid.Rows)
            {
                if (row.Cells["Key"].Value != null)
                    _meta.Add((string)row.Cells["Key"].Value, (string)row.Cells["Value"].Value);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (AddButton.Text == "Add New")
            {
                foreach (DataGridViewRow row in metaDataGrid.Rows)
                {
                    if ((string)row.Cells["Key"].Value == KeyText.Text)
                    {
                        MessageBox.Show("Key Allready Exists, must be unique");
                        return;
                    }
                }
                metaDataGrid.Rows.Add(KeyText.Text, ValueText.Text);
            }
            else
            {
                metaDataGrid.Rows[metaDataGrid.CurrentCell.RowIndex].Cells["Key"].Value = KeyText.Text;
                metaDataGrid.Rows[metaDataGrid.CurrentCell.RowIndex].Cells["Value"].Value = ValueText.Text;

            }
        }

        private void metaDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
            KeyText.Text = (string) metaDataGrid.Rows[metaDataGrid.CurrentCell.RowIndex].Cells["Key"].Value;
            ValueText.Text = (string)metaDataGrid.Rows[metaDataGrid.CurrentCell.RowIndex].Cells["Value"].Value;
            if(KeyText.Text == "")
            {
                AddButton.Text = "Add New";
            }
            else AddButton.Text = "Update Row";
        }
    }
}
