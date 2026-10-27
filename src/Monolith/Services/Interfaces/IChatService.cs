using AgriculturalWebsite.Models;

namespace AgriculturalWebsite.Services.Interfaces;

public interface IChatService
{
    Task<List<ChatMessage>> GetChatHistoryAsync(int chatId);
    Task<ChatMessage> SaveMessageAsync(int chatId, string userId, string text);
    Task<Chat?> GetOrCreateChatAsync(string clientId, string sellerId, string? productName = null);
    Task<List<Chat>> GetUserChatsAsync(string userId);
    Task<List<UserChat>> GetChatParticipantsAsync(int chatId);
}
