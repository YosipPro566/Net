# AgriculturalWebsite — ЛР0: Ocelot перед монолітом

## Що зроблено

- Збережено старий `AgriculturalWebsite` у `src/Monolith` без змін його вихідних файлів.
- Додано окремий `src/ApiGateway` на `http://localhost:5000`.
- Додано явні маршрути `/api/auth/{everything}`, `/api/products/{everything}`, `/api/tractors/{everything}` і `/api/trailers/{everything}` перед catch-all `/api/{everything}`.
- Додано catch-all `/{everything}` для решти шляхів моноліту (вебсторінки, Swagger тощо).
- Додано `/health` (200 OK), логування запитів, `X-Correlation-Id`, CORS і конфігурацію Ocelot.
- Додано інвентаризацію за контролерами, DbSet і міграціями, два ADR та три діаграми.
- У цільовій схемі наступної ЛР заплановано AuthService на `5001` з БД `miniproject_auth` і CatalogService на `5002` з БД `miniproject_catalog`; у ЛР0 ці сервіси ще не створені.

## Порти

| Компонент | Адреса |
|---|---|
| API Gateway | `http://localhost:5000` |
| Моноліт (наявний HTTP-профіль) | `http://localhost:5193` |
| Swagger моноліту в Development | `http://localhost:5193/swagger` |

Порт `5193` взято з оригінального `src/Monolith/Properties/launchSettings.json`. Моноліт не переналаштовувався.

## Запуск

Потрібен .NET SDK 9, доступ до NuGet для відновлення пакетів та SQLite-файл `src/Monolith/app.db`.

Перед запуском моноліту налаштуй власний JWT signing key через User Secrets (не додавай ключ до файлів конфігурації):

```bash
dotnet user-secrets set "Jwt:Key" "<твій випадковий ключ щонайменше на 32 символи>" --project src/Monolith
```

Тестові облікові записи не створюються з паролями за замовчуванням. Якщо вони потрібні, налаштуй їх через User Secrets, наприклад:

```bash
dotnet user-secrets set "SeedUsers:Admin:Email" "admin@example.com" --project src/Monolith
dotnet user-secrets set "SeedUsers:Admin:Password" "<твій пароль>" --project src/Monolith
```

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
- Адреси downstream Ocelot бере з `ocelot.json`. Для перевизначення адреси через змінні середовища .NET Configuration використовуй `Routes__N__DownstreamHostAndPorts__0__Host` та `Routes__N__DownstreamHostAndPorts__0__Port`, де `N` — індекс маршруту в масиві `Routes` (нумерація з нуля). Індекси: `0` Auth, `1` Products, `2` Tractors, `3` Trailers, `4` API catch-all, `5` кореневий шлях, `6` загальний catch-all. Оскільки в ЛР0 всі маршрути ведуть на моноліт, змінюй downstream для всіх маршрутів, які мають обслуговуватися монолітом. `Monolith__Host` / `Monolith__Port` впливають лише на адресу, яку показує журнал шлюзу; самі по собі вони не змінюють маршрути Ocelot.
- Порт шлюзу задається в `src/ApiGateway/Properties/launchSettings.json`.
- CORS origin за замовчуванням: `Cors:AllowedOrigin` у `src/ApiGateway/appsettings.json`; його можна змінити через `Cors__AllowedOrigin`.
- Для маршрутів `/api/auth/**` і `/api/products/**` використовується той самий downstream, що й для catch-all, доки сервісів ще немає.

## План наступного етапу (ЛР1)

За методичкою ЛР1 заплановані окремі AuthService (`http://localhost:5001`) і CatalogService (`http://localhost:5002`) з власними базами `miniproject_auth` та `miniproject_catalog`. Цільові маршрути — `/api/v1/auth/**`, `/api/v1/users/**` і `/api/v1/catalog/**`. У ЛР0 ці сервіси та їхні БД не створюються; шлюз поки пересилає запити до наявного моноліту на `5193`. Порт `5193` збережено, оскільки він реально заданий у `src/Monolith/Properties/launchSettings.json`; `5100` у методичці ЛР1 — приклад/базове значення, яке не відповідає цьому репозиторію.

## Документація та дорожня карта робіт (ЛР0 — ЛР4)

- **[Загальна дорожня карта робіт (Roadmap)](./docs/tasks/LABS-ROADMAP.md)** — зведена схема трансформації, порти, бази даних та зв'язок сутностей аграрної тематики.
- **[Повний посібник до захисту та слайди презентації](./docs/presentation/defense-guide.md)** — готова презентація на 7 слайдів, сценарій Live Demo та пояснення змін у бекенді.
- **[Аудит помилок, конфліктів та підводних каменів за методичками](./docs/analysis/cross-lab-conflicts-and-traps.md)** — розбір розбіжностей, пасток, вразливостей та таблиця штрафів викладачів.
- **Специфікації завдань за лабораторними роботами:**
  - [ЛР0: Проєктування декомпозиції та шлюз перед монолітом](./docs/tasks/LR0-decomposition-gateway.md)
  - [ЛР1: Auth-сервіс і Catalog-сервіс](./docs/tasks/LR1-auth-catalog-services.md)
  - [ЛР2: Винесення операційного сервісу та тести](./docs/tasks/LR2-operations-service-and-tests.md)
  - [ЛР3: Розширення API Gateway (складений запит, кеш, rate limit)](./docs/tasks/LR3-gateway-composite-cache-ratelimit.md)
  - [ЛР4: Клієнтський застосунок та презентація](./docs/tasks/LR4-client-application-and-presentation.md)

### Документація етапу ЛР0:
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
