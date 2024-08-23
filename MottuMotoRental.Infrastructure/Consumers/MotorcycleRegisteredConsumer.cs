using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MongoDB.Driver;
using MottuMotoRental.Infrastructure.Data;

namespace MottuMotoRental.Infrastructure.Consumers
{
    public class MotorcycleRegisteredConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly ILogger<MotorcycleRegisteredConsumer> _logger;
        private readonly IMongoDbContext _mongoContext;

        public MotorcycleRegisteredConsumer(IServiceScopeFactory serviceScopeFactory,
            IConnectionFactory connectionFactory, ILogger<MotorcycleRegisteredConsumer> logger,
            IMongoDbContext mongoContext)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _mongoContext = mongoContext;

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "motorcycle_events_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonSerializer.Deserialize<Payload>(body);

                    if (message?.Data?.Year == 2024)
                    {
                        await _mongoContext.Motorcycles.InsertOneAsync(message.Data);
                        _logger.LogInformation(
                            $"Motorcycle with ID {message.Data.Id} from year 2024 successfully processed and stored.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the motorcycle registered event.");
                }
            };

            _channel.BasicConsume(queue: "motorcycle_events_queue", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public class Payload
        {
            public string Event { get; set; }
            public Motorcycle Data { get; set; }
        }


        public override void Dispose()
        {
            _channel.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}