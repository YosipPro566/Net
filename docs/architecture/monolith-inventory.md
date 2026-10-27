# Інвентаризація моноліту AgriculturalWebsite

> Інвентаризацію складено за наявними контролерами, моделями та `Data/ApplicationDbContext.cs`.
> Для сутностей, таблиць і зв’язків, які не можна достовірно встановити лише з оголошень у коді, наведено назви DbSet/типові назви EF Core; перед захистом їх варто звірити з реальною схемою SQLite.

| Контролер / компонент | Основні сутності | Таблиці / DbSet | Хто ще читає або пише |
|---|---|---|---|
| `AccountController` | `IdentityUser`, `IdentityRole` та моделі Identity | `AspNetUsers`, `AspNetRoles` та пов’язані таблиці Identity | Інші захищені контролери читають користувача/роль для авторизації; `AuthApiController` видає JWT для API |
| `AuthApiController` (`/api/auth`) | обліковий запис Identity | `AspNetUsers`, таблиці Identity | API-клієнти отримують токен; майбутній Auth-сервіс володітиме автентифікацією |
| `ProductsController`, `ProductsApiController` | `Product` | `Products` (`DbSet<Product>`) | `ProductService`, `ProductRepository`; `ApplicationDbContext`; інші частини сайту можуть посилатися на каталог |
| `TractorsController`, `TractorsApiController` | `Tractor` | `Tractors` (`DbSet<Tractor>`) | `TractorService`, `TractorRepository`; `ApplicationDbContext` |
| `TrailersController`, `TrailersApiController` | `Trailer` | `Trailers` (`DbSet<Trailer>`) | `TrailerService`, `TrailerRepository`; `ApplicationDbContext` |
| `Favorite` | улюблені позиції та користувач | `Favorites` (`DbSet<Favorite>`) | пов’язує користувача з вибраними позиціями; залежить від Identity та каталогу |
| `LogsApiController` | `ActivityLog` | `ActivityLogs` (`DbSet<ActivityLog>`) | `ActivityLogService`; логування дій може викликатися з інших сервісів/контролерів |
| `ChatController`, `ChatHub` | `Chat`, `UserChat`, `ChatMessage` | `Chats`, `UserChats`, `ChatMessages` | `ChatService`; SignalR-клієнти та Identity-користувачі |
| `HomeController` | `ErrorViewModel` та дані для сторінок | прямі таблиці не визначаються за назвою контролера | рендерить сторінки сайту й може використовувати інші сервіси |
| `ApplicationDbContext` | усі сутності вище | Identity-таблиці, `Products`, `Tractors`, `Trailers`, `ActivityLogs`, `Chats`, `UserChats`, `ChatMessages`, `Favorites` | спільний контекст і SQLite-файл `app.db`; наразі дані фізично не ізольовані між доменами |

## Висновок для декомпозиції

Першими кандидатами на майбутнє винесення є **Auth** (`/api/auth`) і **Catalog** (товари, трактори та причепи). Чати, журнали активності, обране та сторінки сайту на першому етапі залишаються в моноліті. Це цільовий напрямок, а не реалізація окремих сервісів у ЛР0.
