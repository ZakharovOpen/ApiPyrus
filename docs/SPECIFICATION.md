# Technical Specification: ApiPyrus

## General Description
ApiPyrus — NuGet-библиотека на C# (.NET Standard 2.0), предоставляющая типизированный HTTP-клиент для REST API сервиса Pyrus (pyrus.com).

**Назначение**: позволяет .NET-приложениям программно управлять задачами, справочниками, сотрудниками, ролями, объявлениями и файлами в Pyrus.

**Целевая аудитория**: разработчики, интегрирующие Pyrus в свои приложения (боты, CRM-коннекторы, бэкенд-сервисы).

**Платформа**: любая .NET-платформа, поддерживающая .NET Standard 2.0 (.NET Framework 4.6.1+, .NET Core 2.0+, .NET 5+).

## Documentation Map

| Документ | Содержание |
|---|---|
| [CLAUDE.md](../CLAUDE.md) | Главный файл инструкций — конвенции, скилы, команды |
| [SPECIFICATION.md](SPECIFICATION.md) | Этот файл — обзор и спецификация |
| [ARCHITECTURE.md](ARCHITECTURE.md) | Архитектура библиотеки, диаграммы потоков |
| [DATA_MODEL.md](DATA_MODEL.md) | DTO-модели, enums, интерфейс IValue |
| [API.md](API.md) | Обёрнутые HTTP-эндпоинты Pyrus API v4 |
| [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) | Файловая структура, DI, пакеты |
| [CONFIGURATION.md](CONFIGURATION.md) | Конфигурация ApiClient |
| [INTEGRATION.md](INTEGRATION.md) | Интеграция с Pyrus REST API |
| [TEST_PLAN.md](TEST_PLAN.md) | План тестирования |

## Modules

### 1. ApiClient (ядро)
Центральный класс. Управляет HTTP-соединением, аутентификацией, Bearer-токеном.
- Автоматическая аутентификация при первом запросе
- Re-auth при 401
- Event-based логирование
- IDisposable (управляет HttpClient)

### 2. Models/DTOs — модели данных
DTO-классы для десериализации ответов Pyrus API:
- Задачи (PyrusTask, Field, Comment, Attachment)
- Справочники (Catalog, Item, CatalogHeader)
- Сотрудники (ValuePersone, Organization)
- Роли (Role)
- Объявления (AnnouncementInfo)
- Таблицы (ValueTable, ValueRow, ValueCell)
- Значения полей (IValue и реализации)

### 3. Models/Methods — fluent builders
Builder-классы для формирования запросов:
- **Tasks**: CreateTaskByForm, UpdateTaskByForm, CloseTaskByForm, ReopenTaskByForm, CreateSimpleTask, UpdateSimpleTask
- **Catalogs**: CreateCatalog, UpdateCatalog, UpdateCatalogItems
- **Members**: CreateMember, UpdateMember, BlockMember
- **Roles**: CreateRole, UpdateRole
- **Announcements**: CreateAnnouncement, CommentAnnouncement

### 4. Extentions — расширения
- `PyrusExtentions` — GetJson, GetFieldById, GetFieldsByType, TryGet-варианты
- `HashExtensions` — ToHexString для проверки подписей

### Зависимости между модулями
```
Methods/* → DTOs (формируют request body из DTO)
Methods/* → Extentions (GetJson для сериализации)
Methods/* → ApiClient (вызывают internal методы через Send)
ApiClient → DTOs (десериализация ответов)
ApiClient → Extentions (ToHexString в CheckSig)
Extentions → DTOs (расширения для PyrusTask, Field)
```
