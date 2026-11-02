# Інвентаризація моноліту AgriculturalWebsite

Інвентаризацію складено за контролерами, моделями, `Data/ApplicationDbContext.cs`,
міграціями та `ApplicationDbContextModelSnapshot.cs`. Назви таблиць нижче наведені
за фактичною конфігурацією EF Core там, де вона присутня. Якщо DbSet є в контексті,
але таблиці немає в міграціях/знімку моделі, це окремо позначено.

| Контролер / компонент | Основні сутності | Таблиці / DbSet | Хто ще читає або пише |
|---|---|---|---|
| `AccountController` | `IdentityUser`, `IdentityRole` та сутності ASP.NET Core Identity | `AspNetUsers`, `AspNetRoles` та інші таблиці Identity (`AspNetUserRoles`, `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins`, `AspNetUserTokens`) | Identity використовується під час входу, реєстрації та перевірки доступу; `AuthApiController` працює з користувачами для видачі JWT |
| `AuthApiController` (`/api/auth`) | обліковий запис Identity | `AspNetUsers` та пов’язані таблиці Identity | API-клієнти отримують JWT; у майбутній архітектурі володіння автентифікацією переходить до Auth-сервісу |
| `ProductsController`, `ProductsApiController` | `Product` | `Products` (`DbSet<Product>`) | `ProductService`, `ProductRepository`, `ApplicationDbContext`; належить до майбутнього Catalog-сервісу |
| `TractorsController`, `TractorsApiController` | `Tractor` | `Tractors` (`DbSet<Tractor>`) | `TractorService`, `TractorRepository`, `ApplicationDbContext`; належить до майбутнього Catalog-сервісу |
| `TrailersController`, `TrailersApiController` | `Trailer` | `Trailers` (`DbSet<Trailer>`) | `TrailerService`, `TrailerRepository`, `ApplicationDbContext`; належить до майбутнього Catalog-сервісу |
| `Favorite` | `Favorite` (UserId, ItemId, ItemType) | `Favorites` (`DbSet<Favorite>` у контексті); у наявних міграціях та `ApplicationDbContextModelSnapshot` таблицю `Favorites` не знайдено | Пов’язує користувача з вибраною позицією; фактичне збереження потребує звірки/міграції |
| `LogsApiController` | `ActivityLog` | `ActivityLogs` (`DbSet<ActivityLog>`) | `ActivityLogService`; зберігає журнал активності |
| `ChatController`, `ChatHub` | `Chat`, `UserChat`, `ChatMessage` | `chats`, `users_chat`, `chat_messages` | `ChatService`; SignalR-клієнти та Identity-користувачі |
| `HomeController` | `ErrorViewModel` та дані для MVC-сторінок | Окрема таблиця за назвою контролера не визначається | Відображає сторінки сайту; використовує інші компоненти застосунку |
| `ApplicationDbContext` | усі зареєстровані сутності вище та Identity | Identity-таблиці, `Products`, `Tractors`, `Trailers`, `ActivityLogs`, `chats`, `users_chat`, `chat_messages`; `Favorites` присутній як DbSet, але відсутній у поточному snapshot міграцій | Один спільний контекст і SQLite-файл `app.db`; у ЛР0 дані ще не ізольовані між доменами |

## Висновок для декомпозиції

Першими кандидатами на винесення є **Auth** (реєстрація, вхід, користувачі та видача JWT) і **Catalog** (товари, трактори та причепи). У ЛР0 ці сервіси ще не створюються: відповідні наявні маршрути шлюзу лише перенаправляють запити до моноліту. Чати, журнали активності, обране та MVC-сторінки на цьому етапі залишаються в моноліті.

**Примітка щодо `Favorites`:** клас і DbSet є в коді, але таблиця не відображена в наявних міграціях і snapshot. Перед твердженням, що таблиця фізично існує в `app.db`, перевір її через SQLite або створи потрібну міграцію в окремій роботі. Це не змінює моноліт у межах ЛР0.
