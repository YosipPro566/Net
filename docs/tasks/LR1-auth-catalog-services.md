# Лабораторна робота №1. Auth-сервіс і Catalog-сервіс

**Тема:** Винесення Auth-сервісу та Catalog-сервісу з моноліту
**Мета:** Винести з моноліту перші два незалежні сервіси: автентифікації (`AuthService`) та каталогу агропродукції й техніки (`CatalogService`); налаштувати локальну перевірку токена в кожному сервісі; перемкнути відповідні маршрути на шлюзі без зміни клієнта.

---

## 1. Головні правила та архітектурні домовленості

1. **Database per service:**
   - Кожен сервіс має **власну БД**: `miniproject_auth` для Auth, `miniproject_catalog` для Catalog.
   - Окремий `DbContext` та власна історія міграцій EF Core.
   - **Заборонено:** спільні FK між базами, JOIN між базами, пряме підключення одного сервісу до БД іншого.
   - Зв'язок сутностей між сервісами — **тільки через ідентифікатор (UserId, ItemId)**.
2. **Дані каталогу переносяться одноразово:**
   - Копіювання з моноліту в нову базу (SQL скрипт або утиліта).
   - Після перемикання шлюзу стара таблиця моноліту вважається застарілою і не змінюється.
3. **Розподілена перевірка токена:**
   - Токени видає тільки `AuthService`.
   - `CatalogService` перевіряє валідність підпису локально (без виклику AuthService по мережі).
   - Параметри `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` обов'язково однакові для всіх сервісів. Спільний метод-розширення `AddJwtAuth` доцільно винести в бібліотеку `Shared`.

---

## 2. Специфікація AuthService (порт `5001`)

### 2.1. Модель даних (`miniproject_auth`)
- **`User`**: `Id` (GUID / int), `Email` (унікальний індекс), `PasswordHash`, `DisplayName`, `CreatedAt`, `IsActive`.
- **`Role`**: `Id`, `Name` (як мінімум `User` та `Admin`, сідуються під час запуску).
- **`UserRole`**: зв'язок багато-до-багатьох.
- **`RefreshToken`**: `Id`, `UserId`, `Token` (криптостійкий рядок з `RandomNumberGenerator`), `ExpiresAt`, `CreatedAt`, `RevokedAt`, `ReplacedByToken`.

### 2.2. Контракт API (`AuthService`)
| Метод і маршрут | Доступ | Призначення | HTTP Коди |
|---|---|---|---|
| `POST /api/v1/auth/register` | Анонімний | Реєстрація нового користувача (роль `User`) | `201 Created`, `400 Bad Request`, `409 Conflict` (дублікат email) |
| `POST /api/v1/auth/login` | Анонімний | Вхід, повертає пару: Access token (15 хв) + Refresh token | `200 OK`, `400 Bad Request`, `401 Unauthorized` |
| `POST /api/v1/auth/refresh` | Анонімний | Оновлення пари токенів (ротація refresh токена) | `200 OK`, `400 Bad Request`, `401 Unauthorized` |
| `POST /api/v1/auth/logout` | Авторизований | Відкликання refresh токена (`RevokedAt`) | `204 No Content`, `401 Unauthorized` |
| `GET /api/v1/users/me` | Авторизований | Профіль користувача. `UserId` береться з токена (`ClaimTypes.NameIdentifier`)! | `200 OK`, `401 Unauthorized` |
| `GET /api/v1/users` | Тільки `Admin` | Список користувачів із пагінацією | `200 OK`, `401`, `403 Forbidden` |
| `PUT /api/v1/users/{id}/roles`| Тільки `Admin` | Зміна ролей користувача | `204 No Content`, `400`, `401`, `403`, `404` |
| `GET /health` | Анонімний | Стан працездатності та доступності БД | `200 OK`, `503 Service Unavailable` |

---

## 3. Специфікація CatalogService (порт `5002`)

### 3.1. Модель даних (`miniproject_catalog`)
- **`Item`** (для нашої предметної області — `Product`, `Tractor`, `Trailer` або уніфікований `CatalogItem`):
  `Id`, `Title`, `Description`, `Price` (decimal 18,2), `CategoryId`, `IsActive` (soft delete), `CreatedAt`, додаткові характеристики (потужність к.с., рік, вантажопідйомність, фото URL).
- **`Category`**: `Id`, `Name` (наприклад: Зернові культури, Трактори важкі, Трактори середні, Причепи самоскидні).

### 3.2. Контракт API (`CatalogService`)
| Метод і маршрут | Доступ | Призначення | HTTP Коди |
|---|---|---|---|
| `GET /api/v1/catalog/items` | Публічний | Список товарів/техніки з пагінацією, фільтрацією та сортуванням | `200 OK`, `400 Bad Request` |
| `GET /api/v1/catalog/items/{id}` | Публічний | Детальна інформація однієї позиції | `200 OK`, `404 Not Found` |
| `POST /api/v1/catalog/items` | Тільки `Admin` | Створення нової позиції техніки/товару | `201 Created`, `400`, `401`, `403` |
| `PUT /api/v1/catalog/items/{id}` | Тільки `Admin` | Повне оновлення позиції | `204 No Content`, `400`, `401`, `403`, `404` |
| `DELETE /api/v1/catalog/items/{id}` | Тільки `Admin` | М'яке видалення (`IsActive = false`) | `204 No Content`, `401`, `403`, `404` |
| `GET /api/v1/catalog/categories` | Публічний | Довідник категорій техніки | `200 OK` |
| `GET /health` | Публічний | Стан сервісу та доступність його БД | `200 OK`, `503` |

**Параметри спискового ендпоінта:** `page` (>=1), `pageSize` (<=100), `search`, `categoryId`, `sortBy`, `desc`.
Формат відповіді:
```json
{
  "items": [ { "id": 1, "title": "Трактор МТЗ-82", "price": 450000.00 } ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 42
}
```

---

## 4. Обов'язкові технічні вимоги
1. **Ізоляція EF Core:** Жодна сутність EF Core не виходить назовні через контролери; використовуються виключно DTO (`ItemDto`, `ItemDetailsDto`, `CreateItemRequest`, `UpdateItemRequest`).
2. **Пагінація в БД:** `Skip()` / `Take()`, фільтрація та сортування обов'язково генеруються в SQL, а не в пам'яті сервера (`IQueryable`).
3. **Валідація:** `[ApiController]` або FluentValidation; некоректний вхідний запит повертає `400 Bad Request` з `ValidationProblemDetails`.
4. **OpenAPI / Swagger:** Повне документування: XML-коментарі `/// <summary>`, коди відповідей через `[ProducesResponseType]`, кнопка **Authorize** для тестування з Bearer токеном.
5. **Логування:** `ILogger` без чутливих даних (паролі й токени ніколи не логуються).

---

## 5. Оновлення шлюзу (`ApiGateway`)
В `ocelot.json` змінюються цільові адреси (downstream) для винесених маршрутів:
- `/api/v1/auth/{everything}` -> `localhost:5001`
- `/api/v1/users/{everything}` -> `localhost:5001`
- `/api/v1/catalog/{everything}` -> `localhost:5002`
- `/api/{everything}` (catch-all) -> залишається на Моноліт (`localhost:5193`)
- Порядок маршрутів: конкретні сервісні маршрути розташовані **вище** за перехоплювач!
