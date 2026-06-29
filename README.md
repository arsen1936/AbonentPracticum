# Internal Utils — Внутренние утилиты компании

Веб-приложение с набором вспомогательных инструментов для сотрудников.
Проект-заготовка для студенческой практики.

## Стек

| Слой | Технология |
|------|-----------|
| Backend | C# / ASP.NET Core 8 Web API |
| Frontend | Vanilla JavaScript (SPA на хэш-роутере) |
| База данных | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| Контейнеризация | Docker + Docker Compose |
| Веб-сервер (frontend) | Nginx (Alpine) |

## Быстрый старт

### Требования

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows/Mac) или Docker Engine + Docker Compose (Linux)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (для локальной разработки)
- Git

### Запуск (Docker)

```bash
# 1. Клонировать репозиторий
git clone <repo-url> intern-utils
cd intern-utils

# 2. Запустить все сервисы
docker-compose up -d --build

# 3. Открыть в браузере
# Frontend:  http://localhost:3000
# Swagger:   http://localhost:8080/swagger
```

### Остановка

```bash
docker-compose down          # Остановить контейнеры
docker-compose down -v       # Остановить + удалить данные БД
```

---

## Структура проекта

```
├── docker-compose.yml           # Оркестрация сервисов
├── .gitignore
├── .dockerignore
├── README.md                    # Этот файл
├── docs/
│   ├── assignment.md            # Задание на практику
│   ├── internship-plan.md       # План двухнедельной практики
│   └── utilities-list.md        # Каталог утилит (27 шт.)
└── src/
    ├── backend/
    │   └── WebApp.Api/
    │       ├── Controllers/     # REST API контроллеры
    │       ├── Models/          # Модели данных + DTO
    │       ├── Services/        # Бизнес-логика утилит
    │       ├── Data/            # EF Core контекст + БД
    │       ├── Program.cs       # Точка входа, DI, миграции
    │       ├── Dockerfile       # Сборка backend-образа
    │       └── WebApp.Api.csproj
    ├── frontend/
    │   ├── index.html           # SPA — точка входа
    │   ├── css/style.css        # Стили
    │   ├── js/                  # JavaScript-модули
    │   │   ├── config.js        # Конфигурация API
    │   │   ├── api.js           # HTTP-клиент
    │   │   ├── router.js        # Хэш-роутер SPA
    │   │   ├── app.js           # Инициализация
    │   │   └── pages/           # Страницы (дашборд, утилита)
    │   ├── nginx.conf           # Конфигурация Nginx
    │   └── Dockerfile
    └── nginx/
        └── nginx.conf           # Альтернативный конфиг Nginx
```

---

## Сервисы в docker-compose

| Сервис | Порт | Описание |
|--------|------|----------|
| `postgres` | `5432` | PostgreSQL 16 |
| `api` | `8080` | ASP.NET Core Web API (Swagger на `/swagger`) |
| `frontend` | `3000` | Nginx + SPA (проксирует `/api/*` → `api:8080`) |

---

## API (REST)

| Метод | URL | Описание |
|-------|-----|----------|
| `GET` | `/api/utilities` | Список всех утилит |
| `GET` | `/api/utilities/{endpoint}` | Информация об утилите |
| `POST` | `/api/utilities/{endpoint}/execute` | Выполнить утилиту |
| `GET` | `/api/utilities/{endpoint}/history` | История выполнений |

Swagger UI доступен по адресу: http://localhost:8080/swagger

### Формат запроса выполнения

```json
POST /api/utilities/sum-numbers/execute
Content-Type: application/json

{
  "input": "10\n20\n30"
}
```

### Формат ответа

```json
{
  "output": "Сумма: 60",
  "success": true,
  "error": null
}
```

---

## Как добавить новую утилиту (для студентов)

### 1. Создать сервис в `Services/`

```csharp
// Services/MyNewService.cs
namespace WebApp.Api.Services;

public class MyNewService : IUtilityService
{
    public string Endpoint => "my-new-utility";

    public string Execute(string input)
    {
        // Бизнес-логика
        return $"Результат: {input.ToUpper()}";
    }
}
```

### 2. Зарегистрировать в `Program.cs`

```csharp
builder.Services.AddSingleton<IUtilityService, MyNewService>();
```

### 3. Обновить seed-данные в `AppDbContext.cs`

```csharp
// В методе OnModelCreating, в HasData():
new Utility {
    Id = 21,
    Name = "Моя утилита",
    Description = "Описание...",
    Endpoint = "my-new-utility",
    Category = "Прочее",
    Difficulty = 1,
    IsImplemented = true  // ← отметить как реализованную
}
```

### 4. (Опционально) Добавить UI во frontend

Если утилите нужен специфичный интерфейс (дополнительные поля, чекбоксы, select'ы) — добавить соответствующую страницу в `frontend/js/pages/` и маршрут в `app.js`.

---

## Разработка без Docker

```bash
# Backend
cd src/backend/WebApp.Api
dotnet restore
dotnet run

# БД: нужен локальный PostgreSQL на localhost:5432
# Конфигурация: appsettings.Development.json
```

---

## Полезные навыки, приобретаемые в ходе практики

- Написание production-кода на C# с соблюдением принципов SOLID
- Проектирование REST API (ASP.NET Core)
- Работа с ORM (Entity Framework Core + PostgreSQL)
- SPA на чистом JavaScript (роутинг, динамический рендеринг)
- Контейнеризация приложений (Docker, docker-compose)
- Система контроля версий (GitHub: ветки, pull requests, code review)
- Unit-тестирование (xUnit)
- Документирование кода и API (Swagger / OpenAPI)

