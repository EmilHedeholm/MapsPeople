using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataModels;
using DatabaseAccess;


namespace MappingGUI {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void AddBtn_Click(object sender, EventArgs e) {
            MappingEntry entry = new MappingEntry();
            entry.Id = IdTxt.Text;
            try {
                entry.ConsumerId = int.Parse(ConsumerIdTxt.Text);
            }catch(FormatException fe) {
                ErrorLbl.Text = "ConsumerId is not an int";
            }
            entry.ExternalId = ExternalIdTxt.Text;
            MappingDataAccess dataAccess = new MappingDataAccess();
            try {
                dataAccess.Insert(entry);
            }catch(Exception) {
                ErrorLbl.Text = "cannot insert new entry with same Id as one that already exists";
            }
            DisplayData();
        }

        private void DisplayData() {
            MappingDataAccess dataAccess = new MappingDataAccess();
            dataGridView1.DataSource = dataAccess.GetAll();
        }

        private void Form1_Load(object sender, EventArgs e) {
            DisplayData();
        }

        private void EditBtn_Click(object sender, EventArgs e) {
            MappingEntry entry = new MappingEntry();
            entry.Id = IdTxt.Text;
            entry.ConsumerId = int.Parse(ConsumerIdTxt.Text);
            entry.ExternalId = ExternalIdTxt.Text;
            MappingDataAccess dataAccess = new MappingDataAccess();
            if (dataAccess.FindById(entry.Id) != null) {
                dataAccess.Update(entry);
                ErrorLbl.Text = "";
            } else {
                ErrorLbl.Text = "Cannot update an entry that does not exist";
            }
            DisplayData();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) {
            IdTxt.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            ConsumerIdTxt.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            ExternalIdTxt.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
        }
    }
}
