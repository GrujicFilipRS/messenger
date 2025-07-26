using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    // In-memory map: userId → connectionId
    private static readonly Dictionary<int, string> _userConnections = new();

    public async Task RegisterUser(int userId)
    {
        lock (_userConnections)
        {
            _userConnections[userId] = Context.ConnectionId;
        }
        #if DEBUG
        Console.WriteLine($"User {userId} registered with connection {Context.ConnectionId}");
        #endif
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        lock (_userConnections)
        {
            int? key = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (key != null)
            {
                _userConnections.Remove(key.Value);
                #if DEBUG
                Console.WriteLine($"User {key} disconnected");
                #endif
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

            #if DEBUG
            Console.WriteLine($"From exists: {fromExists}, To exists: {toExists}");
            #endif

            if (toExists && fromExists)
            {
                await Clients.Client(targetConnId!).SendAsync("ReceiveMessage", message, fromUserId, DateTime.Now);
                await Clients.Client(senderConnId!).SendAsync("MessageSent", message, toUserId, DateTime.Now);

                #if DEBUG
                Console.WriteLine("Message sent successfully.");
                #endif
            }
            else
            {
                #if DEBUG
                Console.WriteLine("User not connected. Cannot send message.");
                #endif
            }
        }
        catch (Exception ex)
        {
            #if DEBUG
            Console.WriteLine($"[ERROR] in SendPrivateMessage: {ex.Message}");
            throw;
            #endif
        }
    }
}