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
            //queTopTextBox.Visible = false;
            QueTopicLabel.Text = "";
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
            databaseTextBox.Text = databaseListBox.Text;
            Database = databaseTextBox.Text;

        }

        private void messageListBox_SelectedIndexChanged(object sender, EventArgs e) {
            
           
                messageTextBox.Text = messageListBox.Text;
               string messageBroker = messageTextBox.Text;
                choiceLabel.Text = "";
               
                switch (messageBroker) {
                    case "kafka":
                        choiceLabel.Text = "Enter a topic";
                        QueTopicLabel.Text = "Topic";
                        //queTopTextBox.Visible = true;
                        //string topic = queTopTextBox.Text;
                        //GetAllLocations(topic, Database);
                        //receiver.ReceiveDataFromKafka(userName, topic);
                      
                        break;
                    case "rabbitmq":
                        choiceLabel.Text = "Enter a queue ID";
                        QueTopicLabel.Text = "Queque ID";
                        //queTopTextBox.Visible = true;
                        //string queueId = queTopTextBox.Text;
                        //GetAllLocations(queueId, Database);
                        //receiver.Consume(userName, queueId);
                        
                        break;
                    default:
                        warningLabel.Visible = true;
                        break;
                }        
        }

        private void GetAllLocations(string queueId, string database) {
            string jsonstr;
            var request = WebRequest.Create($"https://localhost:44346/api/Send?id={queueId}&database={database}") as HttpWebRequest;
            var response = request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                jsonstr = sr.ReadToEnd();
            }
            Console.WriteLine(jsonstr);
        }

        private void okButton_Click(object sender, EventArgs e) {
            string userName = userNameTextBox.Text;
            string messageBroker = messageTextBox.Text;
            warningLabel.Visible = false;
            warningLabel.Text = "not a recognized messagebroker, try again";
           
                switch (messageBroker) {
                    case "kafka":
                        string topic = queTopTextBox.Text;
                        GetAllLocations(topic, Database);
                        receiver.ReceiveDataFromKafka(userName, topic);
                        //choice = false;
                        break;
                    case "rabbitmq":
                        string queueId = queTopTextBox.Text;
                        GetAllLocations(queueId, Database);
                        receiver.Consume(userName, queueId);
                        //choice = false;
                        break;
                    default:
                        warningLabel.Visible = true;
                        //warningLabel.Text = "not a recognized messagebroker, try again";
                        break;
                }
            
        }

        private void nextButton_Click(object sender, EventArgs e) {
            LocationGUI openForm = new LocationGUI(Database);
            openForm.Show();
            this.Hide();
        }
    }
 }

