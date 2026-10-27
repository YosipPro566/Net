using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AgriculturalWebsite.Models;

namespace AgriculturalWebsite.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Tractor> Tractors { get; set; }
    public DbSet<Trailer> Trailers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<UserChat> UserChats { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChatMessage>()
            .HasIndex(m => m.ChatId, "IX_chat_messages_chat_id");

        modelBuilder.Entity<UserChat>()
            .HasIndex(uc => new { uc.UserId, uc.ChatId }, "IX_users_chat_user_chat");
    }
}
