using FollowerService.Services;

namespace FollowerService.MessageBus
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<bool> ProcessEvent(MessageEvent messageEvent)
        {
            Console.WriteLine($"---> {messageEvent.EventType} event for {messageEvent.Message}");
            bool result;
            switch (messageEvent.EventType)
            {
                case "UserCreated":
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var followerService = scope.ServiceProvider.GetRequiredService<IFollowerService>();
                        await followerService.CreateUser(messageEvent.Message);
                        result = true;
                    }
                    break;
                case "UserDeleted":
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var followerService = scope.ServiceProvider.GetRequiredService<IFollowerService>();
                        await followerService.DeleteUser(messageEvent.Message);
                        result = true;
                    }
                    break;

                default:
                    result= false;
                    break;
            }

            return result;
        }
    }
}
