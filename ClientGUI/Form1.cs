using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientGUI {
    public partial class Form1 : Form {
        Receiver receiver = new Receiver();
        public string Database { get; set;}
        public Form1() {
            InitializeComponent();
            UpdateDatabaseListBox();
            UpdateMessageListBox();
        }

        private void UpdateMessageListBox() {
            List<string> messages = new List<string>();
            messages.Add("kafka");
            messages.Add("rabbitmq");
            foreach(string message in messages) {
                messageListBox.Items.Add(message);
            }
        }

        private void UpdateDatabaseListBox() {
            List<string> databases = new List<string>();
            databases.Add("neo4j");
            databases.Add("mongodb");
            databases.Add("mssql");
            foreach (string database in databases) {
                databaseListBox.Items.Add(database);
            }
        }

        private void databaseListBox_SelectedIndexChanged(object sender, EventArgs e) {
            Database = databaseListBox.Text;

        }

        private void messageListBox_SelectedIndexChanged(object sender, EventArgs e) {
            string messageBroker = messageListBox.Text;
            choiceLabel.Text = "";
            warningLabel.Text = "";
            string userName = userNameTextBox.Text;
            var choice = true;
            while (choice) {
                switch (messageBroker) {
                    case "kafka":
                        choiceLabel.Text = "Enter a topic";
                        string topic = topicTextBox.Text;
                        GetAllLocations(topic, Database);
                        receiver.ReceiveDataFromKafka(userName, topic);
                        choice = false;
                        break;
                    case "rabbitmq":
                        choiceLabel.Text = "Enter a queue ID";
                        string queueID = queTextBox.Text;
                        GetAllLocations(queueID, Database);
                        receiver.Consume(userName, queueID);
                        choice = false;
                        break;
                    default:
                        warningLabel.Text="not a recognized messagebroker, try again";
                        break;
                }
            }
        }

        private void GetAllLocations(string queueID, string database) {
            string jsonstr;
            var request = WebRequest.Create($"https://localhost:44346/api/Send?id={queueId}&database={database}") as HttpWebRequest;
            var response = request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                jsonstr = sr.ReadToEnd();
            }
            Console.WriteLine(jsonstr);
        }
    }
    }
}
