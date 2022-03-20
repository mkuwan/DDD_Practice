using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Autofac.Core.Lifetime;
using Moq;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Xunit;
using Xunit.Sdk;

namespace Samples.RabbitMQ
{
    /// <summary>
    /// Fanout: ExchangeとBindされている全てのキューに送ります
    /// Direct: Exchangeは、Messageに付与されているRouting KeyとBinding Keyを見て、Routing Key = Binding KeyとなるようなQueueにMessageを送ります
    /// Topic:  ExchangeはRouting Keyを元に、部分一致で配送先を選択させることができます
    /// </summary>
    public class RabbitMqTest
    {

        [Fact]
        public void PublishReceiveTest()
        {
            var message = "Hello RabbitMQ";
            int retryCount = 0;
            while (string.IsNullOrEmpty(SendAndReceive.ResultOfSending) && retryCount < 5)
            {
                SendAndReceive.SendMessage(message);
                SendAndReceive.ReceiveMessage();
                retryCount++;
            }
            var result = SendAndReceive.ResultOfSending;

            Assert.Equal(message, result);
        }


        internal static class SendAndReceive
        {
            public static string ResultOfSending { get; private set; } = String.Empty;

            public static int SendCount = 0;
            public static int ReceiveCount = 0;

            private static readonly ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            public static void SendMessage(string message)
            {
                SendCount++;

                var policy = Policy.Handle<BrokerUnreachableException>()
                    .Or<SocketException>()
                    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, span) =>
                        {
                            //ResultOfSending = $"イベントを発行できませんでした{exception.Message}";
                        });

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "hello",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);


                    var body = Encoding.UTF8.GetBytes(message);

                    policy.Execute(() =>
                    {
                        channel.BasicPublish(
                            exchange: "",
                            routingKey: "hello",
                            basicProperties: null,
                            body: body);
                    });
                }
            }

            public static void ReceiveMessage()
            {
                ReceiveCount++;

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(
                    queue: "hello",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += OnReceived;

                channel.BasicConsume(
                    queue: "hello",
                    autoAck: true,
                    consumer: consumer);
            }
            private static void OnReceived(object? sender, BasicDeliverEventArgs eventArgs)
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                ResultOfSending = message;
            }
        }


    }
}
