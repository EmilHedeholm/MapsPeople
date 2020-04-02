﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Client.DataAccess {
    public class RabbitMQAccess {
        public void GetData(string queueName) {
            ConnectionFactory connectionFactory = new ConnectionFactory {
                HostName = "localhost"
            };
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            channel.BasicQos(0, 1, false);

            //var consumer = new EventingBasicConsumer(channel);
            //consumer.Received += (sender, ea) => {
            //    var body = ea.Body;
            //    var message = Encoding.UTF8.GetString(body);
            //    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            //};

            channel.BasicConsume(queueName, false, consumer);
        }
    }
}