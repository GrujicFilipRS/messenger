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

        Console.WriteLine($"User {userId} registered with connection {Context.ConnectionId}");
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        lock (_userConnections)
        {
            int? key = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (key != null)
            {
                _userConnections.Remove(key.Value);
                Console.WriteLine($"User {key} disconnected");
            }
        }

        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendPrivateMessage(int toUserId, string message)
    {
        if (_userConnections.TryGetValue(toUserId, out var targetConnId))
        {
            await Clients.Client(targetConnId).SendAsync("ReceiveMessage", message);
        }
    }
}