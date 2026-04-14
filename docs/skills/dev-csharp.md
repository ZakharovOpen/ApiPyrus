---
name: dev-csharp
description: >
  Developer skill for ApiPyrus library. Activate when writing, reviewing, or architecting C# code:
  new API methods, builders, DTOs, extensions for the ApiPyrus NuGet package.
user_invocable: true
---

# Skill: Developer — ApiPyrus

## Activation
Активируй при любой задаче, связанной с написанием, рефакторингом или ревью C#-кода в этом проекте.

## Context files — прочитай перед работой

| File | Purpose |
|---|---|
| `CLAUDE.md` | Конвенции проекта, команды сборки |
| `docs/ARCHITECTURE.md` | Архитектура и data flow |
| `docs/DATA_MODEL.md` | Все DTO, enums, IValue hierarchy |
| `docs/API.md` | Обёрнутые HTTP-эндпоинты |
| `docs/PROJECT_STRUCTURE.md` | Структура файлов |
| `docs/CONFIGURATION.md` | Параметры ApiClient |
| `docs/INTEGRATION.md` | Маппинг Pyrus API ↔ C# |

---

## Base C# conventions

### General rules
- Код пишется на C# 12 (.NET Standard 2.0)
- XML-документация на **русском языке**
- Следовать стилю существующего кода — нет отдельных правил форматирования в `.editorconfig`
- Не добавлять docstrings/комментарии к коду, который не менялся
- Минимизировать количество зависимостей
- Не использовать `async void` — только `async Task` / `async Task<T>`
- Все публичные классы/методы — с XML-документацией
- CS1591 / CS1587 подавлены — не тратить время на предупреждения по документации

### Naming
- **PascalCase** — классы, методы, свойства, события
- **camelCase** с `_` prefix — приватные поля (`_entityForJson`, `_taskId`)
- **PascalCase** — файлы (совпадают с именем основного класса)
- Namespace = путь в проекте: `ApiPyrus.Models.DTOs`, `ApiPyrus.Models.Methods.Tasks`

### Serialization
- **Только Newtonsoft.Json** — `JsonConvert`, `JsonProperty`, `StringEnumConverter`
- **Запрещено**: `System.Text.Json`
- Настройки GetJson: `NullValueHandling.Ignore`, `DefaultValueHandling.Ignore`, `Formatting.Indented`
- Enums: `[EnumMember(Value = "...")]` + `StringEnumConverter` или `[Description("...")]` + `GetDescription()`

### Error handling
- Не добавлять избыточную обработку ошибок для внутреннего кода
- Exceptions — через `throw new Exception(message)` (без кастомных exception-классов)
- При ошибке HTTP: бросать Exception с кодом, ReasonPhrase и телом ответа

### Async
- Все HTTP-вызовы — `async/await`
- Не использовать `.Result` или `.Wait()` — только `await`

---

## Project-specific conventions

### Solution structure

```
ApiPyrus.sln
├── ApiPyrus (netstandard2.0) — основная библиотека
└── ApiPyrusTester — тесты (вне этого репозитория)
```

Single-project library. Нет слоёв (Application / Domain / Infrastructure) — это flat library.

### Stack

| Component | Package / Version |
|---|---|
| Target framework | .NET Standard 2.0 |
| Language version | C# 12 (LangVersion 12) |
| JSON | Newtonsoft.Json 13.0.3 |
| HTTP | System.Net.Http (BCL) |
| Crypto | System.Security.Cryptography (BCL) |
| Package format | NuGet (GeneratePackageOnBuild) |

### Packages NOT used (do not add)

- `System.Text.Json` — проект использует исключительно Newtonsoft.Json
- `Microsoft.Extensions.DependencyInjection` — нет DI, клиент создаётся через конструктор
- `Microsoft.Extensions.Logging` / `Serilog` — логирование через events (GotInfoLog/GotErrorLog)
- `Microsoft.Extensions.Http` / `IHttpClientFactory` — HttpClient управляется вручную
- `Polly` — retry-логика встроена (re-auth при 401)
- `RestSharp` / `Refit` / `Flurl` — HTTP через System.Net.Http напрямую

### Architecture: what NOT to expect

- **Нет DI, нет слоёв** — это flat NuGet-библиотека, а не ASP.NET приложение
- **Нет контроллеров/сервисов** — есть ApiClient (HTTP) + Builders (fluent API)
- **Нет БД, нет миграций**
- **Internal vs Public** — write-методы ApiClient помечены `internal`, read-методы — `public`
- **Event-based logging** вместо ILogger

### Domain model

Ключевые сущности Pyrus API, обёрнутые в DTO:

- **PyrusTask** — задача (формальная или простая)
- **Field** / **FieldWithValue** — поле задачи с типизированным значением через IValue
- **Catalog** / **Item** — справочник и его записи
- **ValuePersone** — пользователь / сотрудник
- **Role** — роль (группа пользователей)
- **AnnouncementInfo** — объявление
- **Form** — шаблон формы

---

## Patterns — как добавлять функциональность

### Добавление нового API-метода (Read)
1. Добавить public async method в `ApiClient.cs`
2. Вызвать `ApiRequest(url)` с нужным HTTP-методом
3. Десериализовать через `JsonConvert.DeserializeObject<T>`
4. Добавить XML-документацию на русском

### Добавление нового API-метода (Write)
1. Создать Request DTO в `Models/DTOs/` (если нового формата)
2. Добавить internal async method в `ApiClient.cs`
3. Создать Builder в `Models/Methods/{Domain}/`
4. Builder хранит `_entityForJson` и предоставляет fluent-методы
5. `.Send(apiClient)` вызывает internal метод ApiClient

### Добавление нового IValue-типа
1. Создать класс, реализующий `IValue`, в `PyrusEntities.cs`
2. Добавить case в `FieldWithValue.GetValueObject()` switch
3. Обновить XML-документацию интерфейса `IValue`
4. Обновить `docs/DATA_MODEL.md`

### Fluent Builder pattern (эталон)

```csharp
// Builder хранит request DTO
private PyrusRequestTask _entityForJson = new PyrusRequestTask();

// Fluent-метод заполняет поле и возвращает this
public CreateTaskByForm AddField(ValueFieldData field)
{
    if (_entityForJson.Fields == null)
        _entityForJson.Fields = new List<ValueFieldData>();
    _entityForJson.Fields.Add(field);
    return this;
}

// Send сериализует и вызывает internal метод ApiClient
public async Task<PyrusTask> Send(ApiClient apiClient, string extRequestId = "")
    => await apiClient.CreateTask(_entityForJson.GetJson(), extRequestId);
```

### DI — нет. Прямое создание:
```csharp
using var apiClient = new ApiClient("login", "key");
```

---

## Forbidden in this project

- **System.Text.Json** — только Newtonsoft.Json
- **IHttpClientFactory** — HttpClient создаётся в конструкторе ApiClient
- **ILogger / DI** — логирование через events
- **Менять namespace `Extentions`** — историческая опечатка, не исправлять (breaking change для потребителей)
- **Менять internal на public** у write-методов ApiClient — они используются только через builders
- **async void** — только `async Task` / `async Task<T>`
- **Добавлять зависимости** без крайней необходимости — библиотека имеет единственную зависимость (Newtonsoft.Json)
