# Діаграма 1. Композиція системи AgriculturalWebsite

```mermaid
flowchart TB
    Client["Клієнт / браузер"]
    GW["ApiGateway — Ocelot\nHTTP :5000\n[реалізовано в ЛР0]"]
    Mono["AgriculturalWebsite — моноліт\nHTTP :5193\n[наявний застосунок]"]
    DB[("SQLite: app.db\nIdentity, Products, Tractors, Trailers, Favorites, ActivityLogs, Chats")]
    Auth["Auth service\n[майбутнє, порт визначити окремо]"]
    Catalog["Catalog service\n[майбутнє, порт визначити окремо]"]
    AuthDB[("Auth DB\n[майбутнє]")]
    CatalogDB[("Catalog DB\n[майбутнє]")]

    Client -->|"HTTP/REST :5000"| GW
    GW -->|"HTTP/REST :5193 — усі поточні маршрути"| Mono
    Mono -->|"EF Core / SQLite"| DB
    GW -.->|"майбутнє: /api/auth/**"| Auth
    GW -.->|"майбутнє: /api/products/** та інші маршрути каталогу"| Catalog
    Auth -->|"DB access"| AuthDB
    Catalog -->|"DB access"| CatalogDB

    classDef current fill:#e8f3ff,stroke:#3572a5,color:#123;
    classDef future fill:#fff4df,stroke:#a66b00,color:#432;
    class Client,GW,Mono,DB current;
    class Auth,Catalog,AuthDB,CatalogDB future;
```

Легенда: суцільні стрілки — поточні виклики ЛР0; пунктирні стрілки — цільовий стан наступних робіт. Усі маршрути шлюзу наразі ведуть до моноліту.
