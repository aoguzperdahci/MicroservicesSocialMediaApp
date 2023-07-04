﻿namespace FollowerService.MessageBus
{
    public interface IEventProcessor
    {
        Task<bool> ProcessEvent(MessageEvent messageEvent);
    }
}
