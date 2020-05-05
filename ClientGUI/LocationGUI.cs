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
using Confluent.Kafka;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace ClientGUI {
    public partial class LocationGUI : Form {
        public List<Message> Messages { get; set; }
        public DataTable stateTable { get; set; }
        delegate void UpdateLocationListBoxCallBack(List<Message> messages); 
        public LocationGUI(List<Message> messages, string messageBroker, string username, string locationId) {
            InitializeComponent();
            stateTable = new DataTable();
            UpdateLocationListBox(messages);
            Messages = messages;
            if (messageBroker.Equals("kafka")) {
                ReceiveDataFromKafka(username, locationId);
            } else if (messageBroker.Equals("rabbitmq")) {
                Consume(username, locationId);
            }

        }

        private void UpdateLocationListBox(List<Message> messages) {
            if (this.locationListBox.InvokeRequired) {
                UpdateLocationListBoxCallBack d = new UpdateLocationListBoxCallBack(UpdateLocationListBox);
                this.Invoke(d, new object[] { messages });
            } else {
                locationListBox.ClearSelected();
                foreach (var msg in messages) {
                    locationListBox.Items.Add(msg.LocationId);
                }
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
            this.DialogResult = DialogResult.OK;
        }
        private List<Message> updateMessages(List<Message> msgs, Message msg) {
            List<Message> upMsgs = new List<Message>();
            foreach (var message in msgs) {
                if (message.LocationId.Equals(msg.LocationId)) {
                    message.Source = msg.Source;
                }
                upMsgs.Add(message);

            }
            return upMsgs;
        }
        private Message ConvertMessage(ExternalModel message) {
            Message msg = new Message();
            List<string> parentIds = new List<string>();
            foreach (var id in message.ParentIds) {
                parentIds.Add(id);
            }
            msg.LocationId = parentIds[0];
            msg.Source = message.Source;
            return msg;
        }
        public ExternalModel Consume(string userQueue, string queueID) {
            ExternalModel message1 = null;
            try {
                ConnectionFactory factory = new ConnectionFactory();
                factory.HostName = "localhost";
                IConnection conn = factory.CreateConnection();
                IModel channel = conn.CreateModel();
                channel.ExchangeDeclare(exchange: "Customer1",
                                            type: "topic");
                var args = new Dictionary<string, object>();
                args.Add("x-message-ttl", 30000);

                channel.QueueDeclare(queue: userQueue, durable: true,
                                   exclusive: false,
                                   autoDelete: true,
                                   arguments: args);
                channel.QueueBind(queue: userQueue, exchange: "Customer1", routingKey: $"#.{queueID}.#");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    if (message != null) {
                        message1 = JsonConvert.DeserializeObject<ExternalModel>(message);
                        Message message2 = ConvertMessage(message1);
                        UpdateLocationListBox(updateMessages(Messages, message2));
                        Console.WriteLine(message);
                    }
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: userQueue,
                                     autoAck: false,
                                     consumer: consumer);
                Console.ReadLine();


            } catch (Exception e) {
                if (e is AlreadyClosedException) {
                    Console.WriteLine("The connectionis already closed");
                } else if (e is BrokerUnreachableException) {
                    Console.WriteLine("The broker cannot be reached");
                } else if (e is OperationInterruptedException) {
                    Console.WriteLine("The operation was interupted");
                } else if (e is ConnectFailureException) {
                    Console.WriteLine("Could not connect to the broker broker");
                } else {
                    Console.WriteLine("Something went wrong");
                }
            }
            return message1;
        }

        public ExternalModel ReceiveDataFromKafka(string userName, string topic) {
            ExternalModel message = null;
            using (var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig { BootstrapServers = "localhost", GroupId = userName }).Build()) {
                consumer.Subscribe(topic);
                while (true) {
                    var result = consumer.Consume();
                    message = JsonConvert.DeserializeObject<ExternalModel>(result.Message.Value);
                    Message message2 = ConvertMessage(message);
                    UpdateLocationListBox(updateMessages(Messages, message2));
                    Console.WriteLine(result.Message.Value);
                    return message;
                }
            }
        }

        private void searchButton_Click(object sender, EventArgs e) {
            warnLabel.Visible = false;
            string locationId = locationIdTextBox.Text;
            Message message = null;
            if (locationId != null) {
                message = GetMessageByLocationId(locationId);
                if (message != null) {
                    Source source = message.Source;
                    if (source != null) {
                        UpdateSoureListBox(source);
                        warnLabel.Visible = true;
                        warnLabel.ForeColor = Color.Green;
                        warnLabel.Text = "Location:" +""+ locationId+""+"is found, source is updated.";
                    } else {
                        warnLabel.Visible = true;
                        warnLabel.ForeColor = Color.Green;
                        warnLabel.Text = "Location:" + "" + locationId + "" + "is found, without source.";
                    }

                } else {
                    warnLabel.Visible = true;
                    warnLabel.ForeColor = Color.Red;
                    warnLabel.Text = "Location is not found, try again.";
                }
                

            } else {
                warnLabel.Visible = true;
                warnLabel.ForeColor = Color.Red;
                warnLabel.Text = "No input, nput locationId, try again.";
            }


        }
    }
}
