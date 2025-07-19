using Microsoft.AspNetCore.Mvc;
using Messenger.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MessagesController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet]
    public IEnumerable<MessageModel> Get() => _context.Messages.ToList();
}