using Microsoft.EntityFrameworkCore;
using Messenger.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<MessageModel> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>()
            .Property<int>("id")
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserModel>()
            .Property<string>("username")
            .HasColumnName("Username");

        modelBuilder.Entity<UserModel>()
            .Property<string>("hashedPassword")
            .HasColumnName("HashedPassword");

        modelBuilder.Entity<UserModel>()
            .HasKey("id");
    }
}
