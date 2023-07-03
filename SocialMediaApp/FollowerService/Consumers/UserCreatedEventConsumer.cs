using Neo4jClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Channels;

namespace FollowerService.Consumers
{ 

public class UserCreatedEventConsumer
{

    private readonly IGraphClient _graphClient;

        public UserCreatedEventConsumer( IGraphClient graphClient)
    {

            _graphClient = graphClient;
    }

    public void StartListening()
    {


            var factory = new ConnectionFactory() { 
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
            factory.ClientProvidedName = "UserService";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            //channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Direct);
            channel.QueueDeclare("message-queue", false, false);
            channel.QueueBind(queue: "message-queue",
                exchange: "trigger",
                routingKey: "UserCreated");

            var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(message);
            HandleUserCreatedEvent(message);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue: "message-queue", autoAck: false, consumer: consumer);
            Console.ReadLine();
        }

    private void HandleUserCreatedEvent(string message)
    {
        // Assuming the message contains the username
        string username = message;
            // Create a new user node using the username
            var query = _graphClient.Cypher
                .Create("(u:User {Username: $username})")
                .WithParam("username", username);

            query.ExecuteWithoutResultsAsync();
            
    }

        public static implicit operator string(UserCreatedEventConsumer v)
        {
            throw new NotImplementedException();
        }
    }

}
