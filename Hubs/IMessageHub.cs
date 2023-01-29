using System;
namespace SignalRChatApp.Hubs
{
    public interface IMessageHub
    {
        Task JoinRoom(string groupName);
        Task LeaveRoom(string groupName);
    }
}

