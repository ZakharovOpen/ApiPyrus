# Project Structure

## File Tree

```
ApiPyrus/
├── ApiPyrus.sln                           # Solution (включает ApiPyrusTester)
├── ApiPyrus.csproj                        # Проект библиотеки (.NET Standard 2.0)
├── ApiClient.cs                           # Главный HTTP-клиент
├── pyrus_icon.png                         # Иконка NuGet-пакета
├── .editorconfig                          # Подавление CS1591/CS1587
├── .gitignore
├── CHANGELOG
├── README.md
├── CLAUDE.md                              # Инструкции для Claude Code
│
├── Models/
│   ├── DTOs/
│   │   ├── PyrusEntities.cs               # Все response-DTO (PyrusTask, Field, Catalog, ...)
│   │   ├── PyrusEnums.cs                  # Enums + EnumExtension
│   │   ├── PyrusRequestTask.cs            # Request DTO для задач/объявлений
│   │   ├── PyrusRequestCatalog.cs         # Request DTO для справочников
│   │   ├── PyrusRequestUpdateCatalogItems.cs # Request DTO для diff-обновления
│   │   ├── PyrusRequestMember.cs          # Request DTO для сотрудников
│   │   └── PyrusRequestRole.cs            # Request DTO для ролей
│   │
│   └── Methods/
│       ├── Task/
│       │   ├── CreateTaskByForm.cs        # Создание задачи по форме (Builder)
│       │   ├── UpdateTaskByForm.cs        # Обновление/комментирование задачи (Builder)
│       │   ├── CloseTaskByForm.cs         # Закрытие задачи (extends UpdateTaskByForm)
│       │   ├── ReopenTaskByForm.cs        # Переоткрытие задачи (extends UpdateTaskByForm)
│       │   ├── CreateSimpleTask.cs        # Создание простой задачи (Builder)
│       │   └── UpdateSimpleTask.cs        # Обновление простой задачи (Builder)
│       │
│       ├── Catalogs/
│       │   ├── CreateCatalog.cs           # Создание справочника (Builder)
│       │   ├── UpdateCatalog.cs           # Полное обновление справочника (Builder)
│       │   └── UpdateCatalogItems.cs      # Diff-обновление справочника (Builder)
│       │
│       ├── Members/
│       │   ├── CreateMember.cs            # Создание сотрудника
│       │   ├── UpdateMember.cs            # Обновление сотрудника
│       │   └── BlockMember.cs             # Блокировка (удаление) сотрудника
│       │
│       ├── Roles/
│       │   ├── CreateRole.cs              # Создание роли
│       │   └── UpdateRole.cs              # Обновление роли
│       │
│       └── Announcements/
│           ├── CreateAnnouncement.cs      # Создание объявления (Builder)
│           └── CommentAnnouncement.cs     # Комментирование объявления (Builder)
│
├── Extentions/
│   ├── PyrusExtentions.cs                 # GetJson, GetFieldById, GetFieldsByType
│   └── HashExtensions.cs                  # ToHexString (internal)
│
└── docs/                                  # Документация для Claude Code
    ├── SPECIFICATION.md
    ├── ARCHITECTURE.md
    ├── DATA_MODEL.md
    ├── API.md
    ├── PROJECT_STRUCTURE.md
    ├── CONFIGURATION.md
    ├── INTEGRATION.md
    ├── TEST_PLAN.md
    └── skills/
        ├── dev-extension.md
        └── qa-extension.md
```

## Key Components

### ApiClient (ApiClient.cs)
Центральный класс. Реализует `IDisposable` для управления `HttpClient`.
- **Public read methods**: `GetTasks`, `GetCatalogs`, `GetMembers`, `GetRoles`, `GetLists`, `GetInbox`, `GetAnnouncements`, `GetProfile`, `GetFormsTemplates`, `DownloadFile`, `UploadFile`, `CheckSig`, `ChangeToken`
- **Internal write methods**: `CreateTask`, `CloseOrCommentTask`, `CreateCatalog`, `UpdateCatalog`, `UpdateCatalogDiff`, `AddMembers`, `UpdateMembers`, `BlockMember`, `CreateRole`, `UpdateRole`, `CreateAnnouncement`, `CommentAnnouncement`
- **Private**: `Auth`, `ApiRequest`
- **Events**: `GotInfoLog`, `GotErrorLog`
- **Convenience methods**: `AddScheduledDate`, `AddScheduledDatetimeUtc`, `CancelSchedule`

### Fluent Builders (Models/Methods/)
Паттерн: каждый builder хранит приватный `_entityForJson` (request DTO), предоставляет fluent-методы для заполнения, и `.Send(apiClient)` для отправки.

Пример:
```csharp
PyrusTask task = await new CreateTaskByForm(formId)
    .AddField(new ValueFieldData(1, new ValueChoiceData(5)))
    .AddSubscribers(new List<ValueIdData> { new ValueIdData(123) })
    .FillDefaults()
    .Send(apiClient);
```

### Extensions (Extentions/)
- `GetJson<T>()` — универсальная сериализация с настройками (NullValueHandling.Ignore, DefaultValueHandling.Ignore)
- `GetFieldById` / `TryGetFieldById` — поиск поля по ID, включая вложенные (Title, MultipleChoice)
- `GetFieldsByType` / `TryGetFieldsByType` — поиск полей по типу, включая вложенные
- `ToHexString` — конвертация byte[] → hex string (internal, для CheckSig)

## DI / Dependency Registration
Библиотека не использует DI-контейнер. `ApiClient` создаётся через конструктор:
```csharp
var apiClient = new ApiClient("login", "apiKey");
```

## Packages / Dependencies

| Package | Version | Purpose |
|---|---|---|
| Newtonsoft.Json | 13.0.3 | JSON serialization/deserialization |

> Единственная внешняя зависимость. Все остальное — BCL (.NET Standard 2.0).
