using Messenger.Models;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public void RegisterUser(UserModel user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }
}