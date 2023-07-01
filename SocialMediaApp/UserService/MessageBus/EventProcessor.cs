using UserService.Services;

namespace UserService.MessageBus
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
            bool result;
            switch (messageEvent.EventType)
            {
                case "UserCreatedFailed":
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        result = await userService.DeleteAsync(messageEvent.Message);
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
