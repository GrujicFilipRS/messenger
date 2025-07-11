using Microsoft.EntityFrameworkCore;
using Messenger.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<UserModel> Users { get; set; }
}
