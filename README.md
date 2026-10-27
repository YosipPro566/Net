# AgriculturalWebsite — ЛР0: Ocelot перед монолітом

## Що зроблено

- Збережено старий `AgriculturalWebsite` у `src/Monolith` без змін його вихідних файлів.
- Додано окремий `src/ApiGateway` на `http://localhost:5000`.
- Додано явні маршрути `/api/auth/{everything}`, `/api/products/{everything}`, `/api/tractors/{everything}` і `/api/trailers/{everything}` перед catch-all `/api/{everything}`.
- Додано catch-all `/{everything}` для решти шляхів моноліту (вебсторінки, Swagger тощо).
- Додано `/health` (200 OK), логування запитів, `X-Correlation-Id`, CORS і конфігурацію Ocelot.
- Додано інвентаризацію, два ADR та три діаграми.

## Порти

| Компонент | Адреса |
|---|---|
| API Gateway | `http://localhost:5000` |
| Моноліт (наявний HTTP-профіль) | `http://localhost:5193` |
| Swagger моноліту в Development | `http://localhost:5193/swagger` |

Порт `5193` взято з оригінального `src/Monolith/Properties/launchSettings.json`. Моноліт не переналаштовувався.

## Запуск

Потрібен .NET SDK 9 і доступ до NuGet для відновлення пакетів. SQLite-база створюється автоматично з міграцій під час запуску; локальний файл бази не додається до репозиторію.

Перед першим запуском налаштуй локальний ключ JWT через User Secrets (заміни placeholder на власний випадковий секрет щонайменше з 32 символів):

```powershell
dotnet user-secrets set "Jwt:Key" "<your-random-secret-at-least-32-characters>" --project src/Monolith
```

Для створення тестових користувачів додатково налаштуй секрети відповідних облікових записів; без цих параметрів користувачі автоматично не створюються:

```powershell
dotnet user-secrets set "SeedUsers:Admin:Email" "<admin-email>" --project src/Monolith
dotnet user-secrets set "SeedUsers:Admin:Password" "<admin-password>" --project src/Monolith
```

Для агрономів використовуй такі самі ключі з секціями `SeedUsers:Agronomist1`, `SeedUsers:Agronomist2` та `SeedUsers:Agronomist3`.

У першому терміналі запусти моноліт:

```bash
cd src/Monolith
dotnet restore
dotnet run --launch-profile http
```

У другому терміналі з кореня репозиторію запусти шлюз:

```bash
dotnet run --project src/ApiGateway
```

Перевір здоров’я шлюзу:

```bash
curl -i http://localhost:5000/health
```

Очікується `HTTP/1.1 200 OK` та JSON із `status: Healthy`.

Приклад запиту через шлюз:

```bash
curl -i http://localhost:5000/api/products
```

Для порівняння напряму з монолітом:

```bash
curl -i http://localhost:5193/api/products
```

Порівняй статус і тіло відповіді. Для POST-запитів використовуй валідний JSON і відповідні вимоги авторизації конкретного API.

## Конфігурація

- Ocelot-маршрути: `src/ApiGateway/ocelot.json`.
- Адреса downstream за замовчуванням задається в JSON-конфігурації; її можна перевизначати змінними середовища .NET Configuration, наприклад `Routes__0__DownstreamHostAndPorts__0__Host` / `Routes__0__DownstreamHostAndPorts__0__Port` (Auth), `Routes__1__...` (Products), `Routes__2__...` (Tractors), `Routes__3__...` (Trailers), `Routes__4__...` (API catch-all), `Routes__5__...` (кореневий шлях) і `Routes__6__...` (загальний catch-all). Значення для всіх маршрутів потрібно узгоджено змінити, якщо змінюється downstream. Аналогічно можна перевизначити `Monolith__Host` / `Monolith__Port` для журналу шлюзу.
- Порт шлюзу задається в `src/ApiGateway/Properties/launchSettings.json`.
- CORS origin за замовчуванням: `Cors:AllowedOrigin` у `src/ApiGateway/appsettings.json`; його можна змінити через `Cors__AllowedOrigin`.
- Для маршрутів `/api/auth/**` і `/api/products/**` використовується той самий downstream, що й для catch-all, доки сервісів ще немає.

## Документація

- Інвентаризація моноліту: `docs/architecture/monolith-inventory.md`.
- Архітектурні рішення: `docs/adr/0001-service-decomposition.md`, `docs/adr/0002-ocelot-gateway.md`.
- Діаграми: `docs/diagrams/01-system-composition.md`, `02-sequence-current.md`, `03-sequence-future.md`.

## Сценарій перевірки перед захистом

1. Запустити моноліт і шлюз.
2. Переконатися, що `GET http://localhost:5000/health` повертає 200.
3. Порівняти `GET /api/products` через шлюз і напряму.
4. Перевірити GET, POST і ще один маршрут, які відповідають реальним endpoint-ам та вимогам авторизації.
5. Тимчасово змінити downstream-порт явного маршруту `/api/auth/{everything}` на неіснуючий: запит до Auth має впасти, а інші маршрути — продовжити працювати. Потім повернути правильний порт.
6. Зупинити моноліт і перевірити поведінку шлюзу.
7. Переконатися, що в консолі шлюзу є метод, шлях, downstream-адреса, статус, тривалість і correlation ID.

## Відомі обмеження перевірки

У середовищі підготовки немає встановленого .NET SDK, тому компіляцію та запуск тут не було виконано. Після відкриття в Visual Studio або локального `dotnet restore` перевір синтаксис, відновлення пакета Ocelot і сценарії з цього README. Успішний запуск не слід вважати підтвердженим до цих перевірок.
