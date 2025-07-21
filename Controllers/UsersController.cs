using Microsoft.AspNetCore.Mvc;
using Messenger.Models;
using Microsoft.AspNetCore.Authorization;

[RequireHttps]
[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<UserModel> Get() => _context.Users.ToList();

    [HttpPost]
    public IActionResult Post(UserModel user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = user.id }, user);
    }
}