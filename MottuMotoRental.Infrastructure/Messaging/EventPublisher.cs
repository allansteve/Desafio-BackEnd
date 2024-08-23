using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using MottuMotoRental.Core.Interfaces;

namespace MottuMotoRental.Infrastructure.Messaging
{
    public class EventPublisher : IEventPublisher
    {
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _virtualHost;

        public EventPublisher(string hostname, string queueName, string userName = "rabbitmq", string password = "rabbitmq", string virtualHost = "/")
        {
            _hostname = hostname ?? "localhost";
            _queueName = queueName;
            _userName = userName;
            _password = password;
            _virtualHost = virtualHost;
        }

        public void Publish<T>(T @event)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostname,
                Port = 5672,
                UserName = _userName,
                Password = _password,
                VirtualHost = _virtualHost
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
        }
    }
}