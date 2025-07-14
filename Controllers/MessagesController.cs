using Microsoft.AspNetCore.Mvc;
using Messenger.Models;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MessagesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IEnumerable<MessageModel> Get() => _context.Messages.ToList();
}