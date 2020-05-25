using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MappingGUI {
    public partial class Form1 : Form {
        List<MappingEntry> entries = new List<MappingEntry>();
        public Form1() {
            InitializeComponent();
        }

        private void AddBtn_Click(object sender, EventArgs e) {
            MappingEntry entry = new MappingEntry();
            entry.Id = IdTxt.Text;
            entry.ConsumerId = int.Parse(ConsumerIdTxt.Text);
            entry.ExternalId = ExternalIdTxt.Text;
            entries.Add(entry);
        }

        private void Form1_Load(object sender, EventArgs e) {
            BindingSource binding = new BindingSource();
            binding.DataSource = entries;
            dataGridView1.DataSource = binding;
        }
    }
}
