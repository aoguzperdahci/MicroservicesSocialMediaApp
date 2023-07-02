using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace FollowerService.Consumers
{ 

public class UserCreatedEventConsumer
{
    private readonly string _rabbitMQHost;
    private readonly string _exchangeName;
    private readonly string _queueName;

    public UserCreatedEventConsumer(string rabbitMQHost, string exchangeName, string queueName)
    {
        _rabbitMQHost = rabbitMQHost;
        _exchangeName = exchangeName;
        _queueName = queueName;
    }

    public void StartListening()
    {
        var factory = new ConnectionFactory() { HostName = _rabbitMQHost };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic);
        channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: "UserCreated");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            HandleUserCreatedEvent(message);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
    }

    private string HandleUserCreatedEvent(string message)
    {
        // Assuming the message contains the username
        string username = message;

            return username;
    }

        public static implicit operator string(UserCreatedEventConsumer v)
        {
            throw new NotImplementedException();
        }
    }

}
