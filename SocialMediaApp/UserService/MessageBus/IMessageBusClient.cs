namespace UserService.MessageBus
{
    public interface IMessageBusClient
    {
        void PublishCreateUserEvent(string message);
    }
}
