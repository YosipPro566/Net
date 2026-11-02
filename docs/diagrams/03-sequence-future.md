# Діаграма 3. Майбутній сценарій: перегляд каталогу після декомпозиції

```mermaid
sequenceDiagram
    autonumber
    participant C as Клієнт / браузер
    participant G as ApiGateway :5000
    participant Cat as CatalogService :5002
    participant DB as miniproject_catalog
    participant A as AuthService :5001
    participant M as Моноліт :5193 (решта функцій)

    Note over C,M: Цільовий стан після винесення сервісів; у ЛР0 ця схема ще не працює

    C->>G: HTTP/REST GET /api/v1/catalog/items
    G->>Cat: HTTP/REST GET /api/v1/catalog/items
    Note over Cat: Локальна перевірка підпису JWT для захищених операцій
    Cat->>DB: EF Core / SQL — запит до власної БД
    DB-->>Cat: Дані каталогу
    Cat-->>G: HTTP 200 + DTO/JSON
    G-->>C: HTTP 200 + відповідь

    Note over A,Cat: AuthService :5001 видає JWT; CatalogService :5002 перевіряє його локально за узгодженими Jwt:Key, Jwt:Issuer та Jwt:Audience
    C->>G: HTTP/REST запит до іншої функції сайту
    G->>M: HTTP/REST до моноліту :5193
    M-->>G: HTTP-відповідь
    G-->>C: HTTP-відповідь
```

У цільовому стані AuthService видає токен, але CatalogService не звертається до AuthService для кожної перевірки: він локально перевіряє підпис і параметри токена. Запити до каталогу та його БД обробляє CatalogService; інші функції, які ще не винесені, продовжує обслуговувати моноліт.

Шляхи `/api/v1/auth/**`, `/api/v1/users/**` і `/api/v1/catalog/**`, а також порти `5001` і `5002` узгоджені з методичкою ЛР1. Це план майбутнього перемикання, а не реалізовані в ЛР0 маршрути.
