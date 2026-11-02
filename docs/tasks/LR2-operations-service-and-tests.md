# Лабораторна робота №2. Винесення операційного сервісу та інтеграційні тести

**Тема:** Винесення операційного сервісу з моноліту та інтеграційні тести
**Мета:** Винести сервіс головної дії користувача над каталогом (для `AgriculturalWebsite` це **`AgriOperationsService`** — сервіс оренди техніки або замовлень сільгосппродукції: `BookingService` / `RentalService` / `OrderService`), надати йому власну БД, налаштувати міжсервісну взаємодію з `CatalogService` через gRPC з дедлайнами та ідемпотентністю, і довести інтеграційними тестами незмінність поведінки системи.

---

## 1. Головне правило та регресії
> **Правило:** Після винесення система повинна працювати точно так само, як до винесення! Ті самі маршрути, статус-коди, поля JSON, валідації та бізнес-правила. Клієнт не повинен помітити підміни.

Для фіксації поточної поведінки **ДО будь-яких змін коду** знімається базовий відбиток (`baseline`):
- Запити виконуються до моноліту (`:5193`).
- Відповіді зберігаються у теці `tests/baseline/*.json` та `*.status`.
- Порівняння результатів документується у файлі `docs/baseline-diff.md`.

---

## 2. Предметна область для AgriculturalWebsite
- **Каталог (з ЛР1):** Трактори, причепи, навісне обладнання, зерно/добрива.
- **Операційний сервіс (ЛР2):** `AgriRentalService` (або `OrderingService`), порт `5003`, gRPC `5103`.
- **Головна сутність:** `Rental` (Оренда техніки) або `Order` (Замовлення продукції).

### 2.1. Модель даних `Rental` (`miniproject_operations`)
```csharp
public class Rental
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty; // З JWT токена, НЕ з тіла запиту!
    public int ItemId { get; set; }                    // ID трактора з CatalogService (БЕЗ FK!)
    public string ItemTitleSnapshot { get; set; } = string.Empty; // Копія назви на момент оренди
    public decimal ItemPriceSnapshot { get; set; }               // Копія ціни на момент оренди
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public RentalStatus Status { get; set; }            // Pending, Confirmed, Completed, Cancelled
    public string IdempotencyKey { get; set; } = string.Empty;   // Унікальний індекс
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
```
> [!IMPORTANT]
> **Пастка `ItemSnapshot`:** Якщо зберегти тільки `ItemId`, то при зміні ціни трактора в каталозі старі замовлення «перерахуються» за новою ціною. Необхідно зберігати фіксований знімок назви та тарифу на момент здійснення операції!

---

## 3. Міжсервісна взаємодія через gRPC

Під час оформлення оренди `OperationsService` перевіряє існування та статус техніки в `CatalogService` через gRPC-клієнт (`shared/protos/catalog.proto`):

```csharp
var deadline = DateTime.UtcNow.AddSeconds(2); // Обов'язковий дедлайн!
try
{
    var item = await _catalogClient.GetItemAsync(
        new GetItemRequest { Id = dto.ItemId },
        deadline: deadline,
        cancellationToken: ct);

    if (!item.IsActive)
    {
        return BadRequest(new ProblemDetails { Detail = "Техніка тимчасово недоступна для оренди." });
    }
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
{
    // Повертаємо 404 (або 400), як це робив моноліт
    return NotFound(new ProblemDetails { Detail = "Позицію каталогу не знайдено." });
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded || ex.StatusCode == StatusCode.Unavailable)
{
    // Каталог не відповів вчасно або впав -> 503 + Retry-After
    Response.Headers.RetryAfter = "10";
    return StatusCode(StatusCodes.Status503ServiceUnavailable,
        new ProblemDetails { Title = "Сервіс каталогу тимчасово недоступний", Status = 503 });
}
```

---

## 4. Ідемпотентність через `Idempotency-Key`
1. Клієнт передає HTTP-заголовок `Idempotency-Key: <unique-uuid>`.
2. Сервіс перевіряє наявність ключа в БД/кеші:
   - Якщо ключ уже оброблявся: повертає збережений раніше результат (`201` або `200`) без повторного створення запису!
   - Якщо ключ новий: операція створюється в транзакції, а ключ зберігається в базі даних з `UNIQUE INDEX`.
3. Захищає від подвійного списання/бронювання при мережевих повторах клієнта.

---

## 5. Контракт API (`AgriOperationsService`)

| Метод і маршрут | Доступ | Призначення |
|---|---|---|
| `GET /api/v1/rentals` | Авторизований | Список своїх операцій (пагінація, фільтр за статусом) |
| `GET /api/v1/rentals/{id}` | Власник або Admin | Деталі операції (`404`, якщо чужа операція — захист від витоку ID) |
| `POST /api/v1/rentals` | Авторизований | Створення оренди (перевірка каталогу через gRPC + `Idempotency-Key`) |
| `PUT /api/v1/rentals/{id}` | Власник | Зміна параметрів (дати, коментар) |
| `POST /api/v1/rentals/{id}/cancel` | Власник або Admin | Скасування оренди (зміна статусу на `Cancelled`, а не фізичне видалення) |
| `GET /api/v1/rentals/all` | Admin | Всі операції системи для адміністрування |
| `GET /health` | Публічний | Стан сервісу та доступність його БД |

---

## 6. Інтеграційні тести (мінімум 8 обов'язкових сценаріїв)

Тестовий проєкт: `tests/Operations.IntegrationTests` на базі `WebApplicationFactory<Program>`.
У тестах:
- Використовується окрема тестова БД `miniproject_operations_test` (перестворюється перед тестами).
- Клієнт каталогу підміняється тестовим стабом/моком (`Mock<CatalogServiceClient>`), щоб тести не залежали від роботи реального сервісу каталогу.
- Токен авторизації генерується тестовою фабрикою.

### Перелік сценаріїв:
1. **Створення з коректними даними:** `201 Created`, тіло з новим `id`, заголовок `Location`, запис збережено в БД.
2. **Створення без токена:** `401 Unauthorized` (ідентично до поведінки моноліту).
3. **Створення з неіснуючим `ItemId`:** `404 Not Found` або `400 Bad Request` (відповідає поведінці моноліту).
4. **Порушення бізнес-правила:** дата кінця оренди раніше за початок, дата в минулому (`400 Bad Request`).
5. **Повтор запиту з тим самим `Idempotency-Key`:** Другий запис не створюється, повертається попередній результат.
6. **Отримання своєї операції:** `200 OK`, повний набір полів згідно з baseline.
7. **Спроба отримати чужу операцію:** `404 Not Found` (не розкриваємо факт існування операції іншого користувача).
8. **Скасування операції:** Статус змінюється на `Cancelled`, повторне скасування повертає коректний статус/помилку.
9. *(Бонус)* Каталог недоступний (стаб кидає `RpcException: Unavailable`) -> `503 Service Unavailable` з `ProblemDetails`.
10. *(Бонус)* Список з пагінацією та фільтрацією по статусу -> коректна сторінка й `totalCount`.

---

## 7. Документація для здачі ЛР2
- `docs/adr/0003-operations-extraction.md` — вибір сервісу оренди, володіння таблицями, зв'язок з каталогом.
- `docs/baseline-diff.md` — порівняльна таблиця результатів до/після зняття моноліту.
- Оновлена діаграма контейнерів у `docs/diagrams/`.
- Перемкнений маршрут у `src/ApiGateway/ocelot.json`: `/api/v1/rentals/{everything}` -> `:5003`.
