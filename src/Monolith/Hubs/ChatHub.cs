using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AgriculturalWebsite.Services.Interfaces;
using System.Security.Claims;

namespace AgriculturalWebsite.Hubs;

// ВАЖЛИВО: Поєднуємо Cookie-авторизацію сайту та JWT-токени з вашого Program.cs
[Authorize(AuthenticationSchemes = "Identity.Application," + JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        await Clients.Group(chatId).SendAsync("UserJoined", Context.User?.Identity?.Name ?? "Гість");
    }

    public async Task LeaveChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
        await Clients.Group(chatId).SendAsync("UserLeft", Context.User?.Identity?.Name ?? "Гість");
    }

    public async Task SendMessage(string chatId, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        // Отримуємо ID поточного користувача
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            // Якщо користувач не розпізнаний, ми явно про це повідомляємо фронтенду
            throw new HubException("Ви повинні бути авторизовані для відправки повідомлень.");
        }

        // Безпечно конвертуємо рядок chatId в число int
        if (!int.TryParse(chatId, out int parsedChatId))
        {
            throw new HubException("Некоректний ідентифікатор чату.");
        }

        // Зберігаємо повідомлення в базу даних
        var savedMessage = await _chatService.SaveMessageAsync(
            parsedChatId,
            userId,
            message
        );

        // Розсилаємо повідомлення в реальному часі всім учасникам кімнати
        await Clients.Group(chatId).SendAsync("ReceiveMessage", new
        {
            MessageId = savedMessage.MessageId,
            ChatId = savedMessage.ChatId,
            UserId = savedMessage.UserId,
            UserName = Context.User?.Identity?.Name ?? "Користувач",
            Text = savedMessage.Text,
            SentAt = savedMessage.SentAt
        });
    }

    public async Task MarkAsRead(string chatId, int messageId)
    {
        // Логіка для відмітки повідомлення як прочитаного
        await Clients.OthersInGroup(chatId).SendAsync("MessageRead", messageId);
    }

    public async Task NotifyTyping(string chatId)
    {
        var userName = Context.User?.Identity?.Name ?? "Користувач";
        await Clients.OthersInGroup(chatId).SendAsync("UserTyping", userName);
    }
}