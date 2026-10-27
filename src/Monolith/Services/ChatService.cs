using Microsoft.EntityFrameworkCore;
using AgriculturalWebsite.Data;
using AgriculturalWebsite.Models;
using AgriculturalWebsite.Services.Interfaces;

namespace AgriculturalWebsite.Services;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;

    public ChatService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChatMessage>> GetChatHistoryAsync(int chatId)
    {
        return await _context.ChatMessages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<ChatMessage> SaveMessageAsync(int chatId, string userId, string text)
    {
        var message = new ChatMessage
        {
            ChatId = chatId,
            UserId = userId,
            Text = text,
            SentAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<Chat?> GetOrCreateChatAsync(string clientId, string sellerId, string? productName = null)
    {
        // Шукаємо існуючий чат між цими користувачами
        var existingChat = await _context.UserChats
            .Where(uc => uc.UserId == clientId || uc.UserId == sellerId)
            .GroupBy(uc => uc.ChatId)
            .Where(g => g.Count() == 2)
            .Select(g => g.Key)
            .FirstOrDefaultAsync();

        if (existingChat != 0)
        {
            return await _context.Chats.FindAsync(existingChat);
        }

        // Створюємо новий чат
        var chatName = string.IsNullOrEmpty(productName)
            ? "Чат з продавцем"
            : $"Чат про: {productName}";

        var chat = new Chat
        {
            Name = chatName,
            Description = string.IsNullOrEmpty(productName)
                ? "Консультація з продавцем"
                : $"Обговорення продукту: {productName}",
            CreatedAt = DateTime.UtcNow
        };

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        // Додаємо обох користувачів до чату
        _context.UserChats.Add(new UserChat
        {
            UserId = clientId,
            ChatId = chat.ChatId,
            JoinedAt = DateTime.UtcNow,
            IsAdmin = false
        });

        _context.UserChats.Add(new UserChat
        {
            UserId = sellerId,
            ChatId = chat.ChatId,
            JoinedAt = DateTime.UtcNow,
            IsAdmin = true
        });

        await _context.SaveChangesAsync();
        return chat;
    }

    public async Task<List<Chat>> GetUserChatsAsync(string userId)
    {
        return await _context.UserChats
            .Where(uc => uc.UserId == userId)
            .Include(uc => uc.Chat)
            .Select(uc => uc.Chat!)
            .ToListAsync();
    }

    public async Task<List<UserChat>> GetChatParticipantsAsync(int chatId)
    {
        return await _context.UserChats
            .Where(uc => uc.ChatId == chatId)
            .ToListAsync();
    }
}
