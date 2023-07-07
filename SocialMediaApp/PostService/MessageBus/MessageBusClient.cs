using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PostService.MessageBus
{
    public class MessageBusClient : BackgroundService, IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;

        public MessageBusClient(
            IConfiguration configuration,
            IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"], Port = int.Parse(_configuration["RabbitMQPort"]) };
            factory.ClientProvidedName = "PostService";

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _channel.QueueDeclare("post-message-queue", false, false);
            _channel.QueueBind(queue: "post-message-queue",
                exchange: "trigger",
                routingKey: "routing-key");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, args) =>
            {
                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var messageEvent = JsonSerializer.Deserialize<MessageEvent>(message);
                if (await _eventProcessor.ProcessEvent(messageEvent))
                {
                    _channel.BasicAck(args.DeliveryTag, false);
                }
                else
                {
                    _channel.BasicNack(args.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: "post-message-queue", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger",
                            routingKey: "routing-key",
                            basicProperties: null,
                            body: body);
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}
