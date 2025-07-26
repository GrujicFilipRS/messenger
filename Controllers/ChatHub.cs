using Messenger.Models;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    #if DEBUG
    bool debugMode = true;
    #else
    bool debugMode = false;
    #endif

    private readonly MessageService _messageService;

    // In-memory map: userId → connectionId
    private static readonly Dictionary<int, string> _userConnections = new();

    public ChatHub(MessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task RegisterUser(int userId)
    {
        lock (_userConnections)
        {
            _userConnections[userId] = Context.ConnectionId;
        }
        
        if (debugMode)
            Console.WriteLine($"User {userId} registered with connection {Context.ConnectionId}");

        _userConnections.TryGetValue(userId, out string connId);
        foreach (MessageModel messageModel in MessageModel.GetMessagesForUser(userId))
        {
            await Clients.Client(connId!).SendAsync("ReceivePreviousMessage", messageModel.messageText, messageModel.fromUserId, messageModel.toUserId, messageModel.messageSentTime);
        }
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        lock (_userConnections)
        {
            int? key = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (key != null)
            {
                _userConnections.Remove(key.Value);
                if (debugMode)
                    Console.WriteLine($"User {key} disconnected");
            }
        }

        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendPrivateMessage(int fromUserId, int toUserId, string message)
    {
        Console.WriteLine($"SendPrivateMessage called: from={fromUserId}, to={toUserId}, message={message}");

        try
        {
            bool toExists = _userConnections.TryGetValue(toUserId, out string targetConnId);
            bool fromExists = _userConnections.TryGetValue(fromUserId, out string senderConnId);

            if (debugMode)
                Console.WriteLine($"From exists: {fromExists}, To exists: {toExists}");

            if (toExists && fromExists)
            {
                DateTime currTime = DateTime.Now;

                await Clients.Client(targetConnId!).SendAsync("ReceiveMessage", message, fromUserId, currTime);
                await Clients.Client(senderConnId!).SendAsync("MessageSent", message, toUserId, currTime);

                if (debugMode)
                    Console.WriteLine("Message sent successfully.");
                
                MessageModel messageModel = new MessageModel(fromUserId, toUserId, message, currTime);
                _messageService.SendMessage(messageModel); // This needs to be accessed
            }
            else
            {
                if (debugMode)
                    Console.WriteLine("User not connected. Cannot send message.");
            }
        }
        catch (Exception ex)
        {
            if (debugMode) {
                Console.WriteLine($"[ERROR] in SendPrivateMessage: {ex.Message}");
                throw;
            }
        }
    }
}