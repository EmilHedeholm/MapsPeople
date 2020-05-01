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
        public IDataAccess dataAccess { get; set; }
        public DataTable stateTable { get; set; }
        public LocationGUI(string database, List<string> locationIds) {
            InitializeComponent();
            stateTable = new DataTable();
            //List<string> locationIds = new List<string>();
            //locationIds.Add("287d4074d6c647a49f215fb1");
            //locationIds.Add("ee268551ad3d42218ef83b5c");
            //locationIds.Add("db8929a8474a4752a5a984a8");
            //locationIds.Add("1b4b349a49fa4e22845f6790");
            UpdateLocationListBox(locationIds);
            switch (database) {
                case "neo4j":
                    dataAccess = new Neo4jDataAccess();
                    break;
                case "mongodb":
                    dataAccess = new MongoDBDataAccess();
                    break;
                case "mssql":
                    dataAccess = new SQLDataAccess();
                    break;
                default:
                    warningLabel.Text = "not a recognized database, try again";
                    break;
            }

        }

        private void UpdateLocationListBox(List<string> locationIds) {
            foreach( string locationId in locationIds) {
                locationListBox.Items.Add(locationId);
            }
        }

        private void locationListBox_SelectedIndexChanged(object sender, EventArgs e) {
            string locationId = locationListBox.Text;
            Location location = dataAccess.GetLocationById(locationId);
            List<Source> sources = location.Sources;
            UpdateSoureListBox(sources);
        }

        private void UpdateSoureListBox(List<Source> sources) {
            foreach(var source in sources) {
                sourceListBox.Items.Add(source.Type + "" + "" + source.TimeStamp.ToString());
                List<State> states = source.State;
                if (states.Count != 0) {
                    foreach(var state in states) {
                        stateTable.Rows.Add(locationListBox.Text, source.Type, state.Property, state.Value);
                    }

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
    }
}
