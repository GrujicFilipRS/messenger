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
        UserModel.users.Add(user);
    }

    public List<UserModel> LoadAllUsers()
    {
        return _context.Users
            .Select(u => new UserModel(u.id, u.username, u.hashedPassword))
            .ToList();
    }
}