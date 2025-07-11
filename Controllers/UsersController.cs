using Microsoft.AspNetCore.Mvc;
using Messenger.Models;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IEnumerable<UserModel> Get() => _context.Users.ToList();

    [HttpPost]
    public IActionResult Post(UserModel user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = user.id }, user);
    }

    [HttpPost]
    public IActionResult RegisterUserPost(string username, string password)
    {
        UserModel user = new UserModel(username, password);

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(new { message = "User created", id = user.id });
    }
}