# Діаграма 3. Майбутній сценарій: каталог після декомпозиції

```mermaid
sequenceDiagram
    autonumber
    participant C as Клієнт / браузер
    participant G as ApiGateway :5000
    participant A as Auth service (майбутнє)
    participant Cat as Catalog service (майбутнє)
    participant DB as Catalog DB (майбутнє)
    participant M as Моноліт :5193 (решта функцій)

    C->>G: HTTP/REST GET /api/products + Bearer JWT
    G->>Cat: HTTP/REST GET /api/products + X-Correlation-Id
    Note over Cat: Перевірка JWT (issuer, підпис, строк дії)
    Cat->>DB: DB-запит до власної БД
    DB-->>Cat: Дані каталогу
    Cat-->>G: HTTP 200 + JSON
    G-->>C: HTTP 200 + JSON

    Note over A,Cat: Auth service видає JWT; Catalog перевіряє токен локально за узгодженим ключем або через JWKS
    C-->>G: Інші запити, що ще не винесені
    G-->>M: HTTP/REST до моноліту :5193
```

Пунктирна логіка цієї діаграми — цільова, не реалізована в ЛР0. Перевірку JWT показано на межі сервісу, а не як бізнес-логіку шлюзу. Виклик від шлюзу до окремого сервісу та від сервісу до його БД стає мережевим/інфраструктурним переходом.
