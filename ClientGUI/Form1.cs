﻿using System;
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
using DataModels;
using Newtonsoft.Json;

namespace ClientGUI {
    public partial class Form1 : Form {
        Receiver receiver = new Receiver();
        List<ExternalModel> messages = new List<ExternalModel>();
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
            warningLabel.Text = "";
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
                        break;
                    case "rabbitmq":
                        choiceLabel.Text = "Enter a queue ID";
                        QueTopicLabel.Text = "Queque ID";
                        break;
                    default:
                        warningLabel.Visible = true;
                        break;
                }        
        }

        private List<ExternalModel> GetAllLocations(string queueId, string database) {
            string jsonstr;
            var request = WebRequest.Create($"https://localhost:44346/api/Send?id={queueId}&database={database}") as HttpWebRequest;
            var response = request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                jsonstr = sr.ReadToEnd();
            }
            messages = JsonConvert.DeserializeObject<List<ExternalModel>>(jsonstr);
            return messages;
        }

        private void okButton_Click(object sender, EventArgs e) {
            string userName = userNameTextBox.Text;
            string messageBroker = messageTextBox.Text;
            string locacationId = queTopTextBox.Text;
            ExternalModel message = null;
            warningLabel.Visible = false;
            warningLabel.ForeColor = Color.Red;
            warningLabel.Text = "";
           
                switch (messageBroker) {
                    case "kafka":
                        //string topic = queTopTextBox.Text;                   
                        if (locacationId !=null) {
                           GetAllLocations(locacationId, Database);
                           //message = receiver.ReceiveDataFromKafka(userName, topic);                    
                        } else {
                             warningLabel.Visible = true;
                             warningLabel.Text = "Input Topic, try again";
                        }
                        break;
                    case "rabbitmq":
                       // string queueId = queTopTextBox.Text;
                        if (locacationId != null) {
                           GetAllLocations(locacationId, Database);
                          // message = receiver.Consume(userName, queueId);
                        } else {
                           warningLabel.Visible = true;
                           warningLabel.Text = "Input Queque ID, try again";
                        }                   
                        break;
                    default:
                        warningLabel.Visible = true;
                        warningLabel.Text = "not a recognized messagebroker, try again";
                        break;              
                }
            if(messages.Count != 0) {
                List<Message> msgs = ConvertMessages(messages);
                if (message != null) {
                    Message msg = ConvertMessage(message);
                    List<Message> updateMsgs = updateMessages(msgs, msg);
                    LocationGUI openForm = new LocationGUI(updateMsgs, messageBroker, userName, locacationId);
                    openForm.Show();
                    //this.Hide();
                } else {
                    LocationGUI openForm = new LocationGUI(msgs, messageBroker, userName, locacationId);
                    openForm.Show();
                    //this.Hide();

                }
            }
            if (userName == null) {
                warningLabel.Visible = true;       
                warningLabel.Text = "input user name, try again";

            }

            if (Database == null) {
                warningLabel.Visible = true;
                warningLabel.Text = "Select a database, try again";

            }

            if (messageBroker == null) {
                warningLabel.Visible = true;
                warningLabel.Text = "Select a messageBroker, try again";

            }
       
        }

        private List<Message> updateMessages(List<Message> msgs, Message msg) {
            List<Message> upMsgs = new List<Message>();
            foreach ( var message in msgs) {
                if (message.LocationId.Equals(msg.LocationId)) {
                    message.Source = msg.Source;
                }
                upMsgs.Add(message);

            }
            return upMsgs;
        }

        private List<Message> ConvertMessages(List<ExternalModel> messages) {
            List<Message> msgs = new List<Message>();
            foreach (var message in messages) {
                msgs.Add(ConvertMessage(message));
            }
            return msgs;
        }

        private Message ConvertMessage(ExternalModel message) {
            Message msg = new Message();
            List<string> parentIds = new List<string>();
            foreach (var id in message.ParentIds) {
                parentIds.Add(id);
            }
            msg.LocationId= parentIds[0];
            msg.Source = message.Source;
            return msg;
        }      
    }
 }
