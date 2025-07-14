using Messenger.Models;

public class MessageService
{
    private readonly AppDbContext _context;

    public MessageService(AppDbContext context)
    {
        _context = context;
    }

    public void SendMessage(MessageModel message)
    {
        _context.Messages.Add(message);
        _context.SaveChanges();
        MessageModel.messages.Add(message);
    }

    public MessageModel[] GetMessages(int userId)
    {
        return _context.Messages
            .Where(m => m.fromUserId == userId || m.toUserId == userId)
            .ToArray();
    }

    public List<MessageModel> LoadAllMessages()
    {
        return _context.Messages
            .Select(m => new MessageModel(m.fromUserId, m.toUserId, m.messageText, m.messageSentTime))
            .ToList();
    }
}