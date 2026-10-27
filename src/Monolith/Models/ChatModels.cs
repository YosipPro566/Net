using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriculturalWebsite.Models;

[Table("chats")]
public class Chat
{
    [Key]
    [Column("chat_id")]
    public int ChatId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(255)]
    [Column("description")]
    public string? Description { get; set; }

    // Зворотні зв'язки для коректної роботи LINQ та Entity Framework
    public ICollection<UserChat> UserChats { get; set; } = new List<UserChat>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}

[Table("users_chat")]
public class UserChat
{
    [Key]
    [Column("user_chat_id")]
    public int UserChatId { get; set; }

    [Required]
    [MaxLength(450)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("chat_id")]
    public int ChatId { get; set; }

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [Column("is_admin")]
    public bool IsAdmin { get; set; } = false;

    [ForeignKey(nameof(ChatId))]
    public Chat? Chat { get; set; }
}

[Table("chat_messages")]
public class ChatMessage
{
    [Key]
    [Column("message_id")]
    public int MessageId { get; set; }

    [Column("chat_id")]
    public int ChatId { get; set; }

    [Required]
    [MaxLength(450)]
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Column("text")]
    public string Text { get; set; } = string.Empty;

    [Column("sent_at")]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [ForeignKey(nameof(ChatId))]
    public Chat? Chat { get; set; }
}