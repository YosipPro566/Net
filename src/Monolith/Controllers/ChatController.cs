using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriculturalWebsite.Data;
using AgriculturalWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AgriculturalWebsite.Controllers
{
    [Authorize] // Стабільна Cookie-авторизація сайту
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Захист від 404: автоматичний редірект з /Chat на /Chat/Room
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Room));
        }

        // Головна сторінка чатів (Робочий кабінет повідомлень фермера)
        public async Task<IActionResult> Room()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                               ?? User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Challenge();
            }

            var chats = await _context.UserChats
                .Where(uc => uc.UserId == currentUserId)
                .Select(uc => uc.Chat)
                .Where(c => c != null)
                .OrderByDescending(c => c!.ChatId)
                .ToListAsync();

            ViewBag.CurrentUserId = currentUserId;
            return View(chats);
        }

        // Асинхронний ендпоінт для завантаження історії повідомлень
        [HttpGet]
        [Route("api/chat/{chatId}/messages")]
        public async Task<IActionResult> GetChatMessages(int chatId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                               ?? User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var userInChat = await _context.UserChats
                .AnyAsync(uc => uc.ChatId == chatId && uc.UserId == currentUserId);

            if (!userInChat)
            {
                return Forbid();
            }

            var messages = await _context.ChatMessages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    MessageId = m.MessageId,
                    ChatId = m.ChatId,
                    UserId = m.UserId,
                    Text = m.Text,
                    SentAt = m.SentAt.ToString("o"),
                    IsRead = m.IsRead
                })
                .ToListAsync();

            return Json(messages);
        }

        // Універсальний метод створення чату за Email продавця
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartItemChat(string sellerId, string itemName)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                               ?? User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Json(new { success = false, message = "Потрібно авторизуватися!" });
            }

            if (string.IsNullOrEmpty(sellerId) || sellerId == "null" || sellerId == "undefined")
            {
                return Json(new { success = false, message = "Помилка: Не вказано пошту продавця!" });
            }

            // Шукаємо фермера в базі AspNetUsers за Email
            var sellerUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == sellerId.Trim().ToLower());

            if (sellerUser == null)
            {
                return Json(new { success = false, message = $"Фермера з поштою {sellerId} не знайдено в базі сайту!" });
            }

            string realSellerGuid = sellerUser.Id; // Отримуємо справжній Guid ID користувача

            // Перевірка: чи не пишемо ми самі собі
            if (currentUserId.Trim().ToLower() == realSellerGuid.Trim().ToLower())
            {
                return Json(new { success = false, message = "Ви не можете почати чат самі з собою (це ваше оголошення)!" });
            }

            // Перевіряємо чи є вже чат щодо цього товару
            var existingChat = await _context.UserChats
                .Where(uc => uc.UserId == currentUserId)
                .Select(uc => uc.Chat)
                .FirstOrDefaultAsync(c => c != null && c.Name == itemName);

            if (existingChat != null)
            {
                return Json(new { success = true, chatId = existingChat.ChatId });
            }

            // Створюємо нову кімнату
            var newChat = new Chat
            {
                Name = itemName ?? "Обговорення товару",
                CreatedAt = DateTime.UtcNow
            };

            _context.Chats.Add(newChat);
            await _context.SaveChangesAsync();

            // Зв'язуємо покупця та продавця
            var ucBuyer = new UserChat { ChatId = newChat.ChatId, UserId = currentUserId, JoinedAt = DateTime.UtcNow };
            var ucSeller = new UserChat { ChatId = newChat.ChatId, UserId = realSellerGuid, JoinedAt = DateTime.UtcNow };

            _context.UserChats.AddRange(ucBuyer, ucSeller);
            await _context.SaveChangesAsync();

            return Json(new { success = true, chatId = newChat.ChatId });
        }
    }
}