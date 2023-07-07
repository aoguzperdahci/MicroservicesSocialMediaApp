using PostService.Services;

namespace PostService.MessageBus
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
            bool result = false;
            switch (messageEvent.EventType)
            {
                case "UserDeleted":
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var postService = scope.ServiceProvider.GetRequiredService<IPostService>();
                        await postService.DeleteUserPostsAsync(messageEvent.Message);
                        result = true;
                    }
                    break;

                default:
                    result= true;
                    break;
            }

            return result;
        }
    }
}
