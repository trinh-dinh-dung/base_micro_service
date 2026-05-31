using Application.Abstractions.Messaging;
using Application.Common.Appsetting;
using Application.Common.Status;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Infrastructure.Messaging
{
    public class RabbitMQClient : IRabbitMQClient
    {
        private readonly IOptions<Appsettings> _appsettings;

        public RabbitMQClient(IOptions<Appsettings> appsettings)
        {
            _appsettings = appsettings;
        }

        public void SendRabbitMQClientQueues(QueueServiceBusiness queuesRequest)
        {
            try
            {
                queuesRequest.BusinessServiceType = (int)BusinessServiceTypeSendRabbitMq.SopService;
                var factory = new ConnectionFactory
                {
                    UserName = _appsettings.Value.Rabbitmq_UserName,
                    Password = _appsettings.Value.Rabbitmq_Password,
                    VirtualHost = _appsettings.Value.Rabbitmq_VirtualHost,
                    HostName = _appsettings.Value.Rabbitmq_HostName,
                    Port = _appsettings.Value.Rabbitmq_Port,
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(
                    queue: queuesRequest.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var message = JsonConvert.SerializeObject(queuesRequest);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: queuesRequest.QueueName,
                    basicProperties: null,
                    body: body);
            }
            catch (Exception)
            {
                // Logged by caller if needed
            }
        }
    }
}
