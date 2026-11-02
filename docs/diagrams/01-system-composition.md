# Діаграма 1. Композиція системи AgriculturalWebsite

```mermaid
flowchart TB
    Client["Клієнт / браузер"]
    GW["ApiGateway — Ocelot\nHTTP :5000\n[реалізовано в ЛР0]"]
    Mono["AgriculturalWebsite — моноліт\nHTTP :5193\n[наявний застосунок]"]
    DB[("SQLite: app.db\nIdentity, Products, Tractors, Trailers, ActivityLogs, chats, users_chat, chat_messages")]
    Auth["AuthService\nHTTP :5001\n[майбутнє, ЛР1]"]
    Catalog["CatalogService\nHTTP :5002\n[майбутнє, ЛР1]"]
    AuthDB[("miniproject_auth\n[майбутня окрема БД Auth]")]
    CatalogDB[("miniproject_catalog\n[майбутня окрема БД Catalog]")]

    Client -->|"HTTP/REST :5000"| GW
    GW -->|"HTTP/REST :5193 — усі маршрути в ЛР0"| Mono
    Mono -->|"EF Core / SQLite"| DB
    GW -.->|"майбутнє: /api/v1/auth/** та /api/v1/users/**"| Auth
    GW -.->|"майбутнє: /api/v1/catalog/**"| Catalog
    Auth -->|"EF Core — власна БД"| AuthDB
    Catalog -->|"EF Core — власна БД"| CatalogDB

    classDef current fill:#e8f3ff,stroke:#3572a5,color:#123;
    classDef future fill:#fff4df,stroke:#a66b00,color:#432;
    class Client,GW,Mono,DB current;
    class Auth,Catalog,AuthDB,CatalogDB future;
```

**Легенда:** суцільні стрілки — реальні зв’язки в ЛР0; пунктирні — цільова архітектура наступних робіт. AuthService і CatalogService та їхні бази даних у ЛР0 ще не реалізовані.

**Порти:** Gateway — `5000`, наявний моноліт — `5193` (з його `launchSettings.json`), майбутній AuthService — `5001`, майбутній CatalogService — `5002`. Порти майбутніх сервісів узгоджені з методичкою ЛР1; порт моноліту `5193` збережено відповідно до фактичного проєкту, а не замінено навчальним прикладом `5100`.

У майбутньому кожен сервіс володітиме власною БД (`miniproject_auth` і `miniproject_catalog`). Прямий доступ одного сервісу до БД іншого не передбачається.
