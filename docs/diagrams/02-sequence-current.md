# Діаграма 2. Поточний сценарій: перегляд товарів через шлюз

```mermaid
sequenceDiagram
    autonumber
    participant C as Клієнт / браузер
    participant G as ApiGateway :5000
    participant M as AgriculturalWebsite :5193
    participant D as SQLite app.db

    C->>G: HTTP/REST GET /api/products
    G->>M: HTTP/REST GET /api/products
    M->>D: EF Core / SQLite SELECT Products
    D-->>M: Результат запиту
    M-->>G: HTTP 200 + JSON
    G-->>C: HTTP 200 + відповідь без зміни змісту
```

Це поточний шлях: шлюз лише перенаправляє запит. Фізичний файл БД — `src/Monolith/app.db`; порт моноліту взято з наявного `Properties/launchSettings.json` (`5193`).
