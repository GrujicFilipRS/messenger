using Messenger.Models;

public static class DataSeeder
{
    public static void Initialize(AppDbContext context)
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // If Users table is empty, seed admin user
        if (!context.Users.Any())
        {
            var admin = new UserModel("admin", "admin123");
            context.Users.Add(admin);
            context.SaveChanges();
        }
    }
}