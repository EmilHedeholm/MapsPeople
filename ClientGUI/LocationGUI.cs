using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatabaseAccess;
using DataModels;

namespace ClientGUI {
    public partial class LocationGUI : Form {
        public List<Message> Messages { get; set; }
        public DataTable stateTable { get; set; }
        public LocationGUI(List<Message> messages) {
            InitializeComponent();
            stateTable = new DataTable();
            UpdateLocationListBox(messages);
            Messages = messages;
          

        }

        private void UpdateLocationListBox(List<Message> messages) {
            locationListBox.ClearSelected();
            foreach( var msg in messages) {
                locationListBox.Items.Add(msg.LocationId);
            }
        }

        private void locationListBox_SelectedIndexChanged(object sender, EventArgs e) {
            string locationId = locationListBox.Text;
            Message msg = GetMessageByLocationId(locationId);
            Source source = msg.Source;
            if (source != null) {
                UpdateSoureListBox(source);
            }
          
        }

        private Message GetMessageByLocationId(string locationId) {
            Message message = null;
            foreach(var msg in Messages) {
                if (msg.LocationId.Equals(locationId)) {
                    message = msg;
                }
            }
            return message;
        }

        private void UpdateSoureListBox(Source source) {
            sourceListBox.Items.Clear();
            stateTable.Clear();         
            sourceListBox.Items.Add(source.Type + "" + "" + source.TimeStamp.ToString());
            List<State> states = source.State;
                if (states.Count != 0) {
                    foreach(var state in states) {
                        stateTable.Rows.Add(locationListBox.Text, source.Type, state.Property, state.Value);
                    }

               }
            
            stateDataGridView.DataSource = stateTable;
        }

        private void LocationGUI_Load(object sender, EventArgs e) {
           
            stateTable.Columns.Add("LocationId", typeof(string));
            stateTable.Columns.Add("SourceType", typeof(string));
            stateTable.Columns.Add("Property", typeof(string));
            stateTable.Columns.Add("Value", typeof(string));
            stateDataGridView.DataSource = stateTable;
        }

        private void backButton_Click(object sender, EventArgs e) {
            Form1 openForm = new Form1();
            openForm.Show();
            this.Hide();
        }
    }
}
