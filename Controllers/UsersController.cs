using Microsoft.AspNetCore.Mvc;
using Messenger.Models;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    public IEnumerable<UserModel> Get() => _context.Users.ToList();

    [Authorize]
    [HttpPost]
    public IActionResult Post(UserModel user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = user.id }, user);
    }
}