# Integration: Pyrus REST API

## Overview

ApiPyrus выступает **HTTP-клиентом** для Pyrus REST API v4.
- Роль: consumer (отправляет запросы к Pyrus, получает ответы)
- Протокол: HTTPS
- Формат: JSON (Newtonsoft.Json)
- Аутентификация: API Key → Bearer token

## Data Flow

```
┌──────────────────┐     HTTP/JSON      ┌──────────────────┐
│  Consumer App    │                     │  Pyrus Cloud     │
│                  │                     │  api.pyrus.com   │
│  ┌────────────┐  │   GET /v4/auth     │                  │
│  │ ApiClient  │──┼──────────────────►│  Auth endpoint   │
│  │            │◄─┼──────────────────◄│  → access_token  │
│  │            │  │                     │                  │
│  │            │──┼── GET/POST/PUT ───►│  /v4/tasks       │
│  │            │◄─┼── JSON response ──◄│  /v4/catalogs    │
│  │            │  │                     │  /v4/members     │
│  │            │──┼── POST multipart ─►│  /v4/files/upload│
│  │            │◄─┼── JSON {guid} ────◄│                  │
│  │            │  │                     │                  │
│  │            │──┼── GET attachment ──►│  /services/...   │
│  │            │◄─┼── binary stream ──◄│                  │
│  └────────────┘  │                     │                  │
│                  │                     │                  │
│  ┌────────────┐  │   Webhook (incoming)│                  │
│  │ CheckSig() │◄─┼──────────────────◄│  Bot webhook     │
│  │ HMAC-SHA1  │  │   POST + X-Pyrus-Sig                  │
│  └────────────┘  │                     │                  │
└──────────────────┘                     └──────────────────┘
```

## Pyrus API → Internal Model Mapping

### Responses (API → DTO)

| Pyrus API Entity | C# DTO Class | File |
|---|---|---|
| Task object | `PyrusTask` | PyrusEntities.cs |
| Task list wrapper | `PyrusTasks` | PyrusEntities.cs |
| Task info wrapper | `PyrusTaskInfo` | PyrusEntities.cs |
| Field | `Field` / `FieldWithValue` | PyrusEntities.cs |
| Comment | `Comment` | PyrusEntities.cs |
| Attachment | `Attachment` | PyrusEntities.cs |
| Person/Member | `ValuePersone` | PyrusEntities.cs |
| Catalog | `Catalog` | PyrusEntities.cs |
| Catalog item | `Item` | PyrusEntities.cs |
| Role | `Role` | PyrusEntities.cs |
| Form template | `Form` | PyrusEntities.cs |
| Announcement | `AnnouncementInfo` | PyrusEntities.cs |
| Auth token | `Token` | PyrusEntities.cs |
| Uploaded file | `UploadedFile` | PyrusEntities.cs |
| Webhook event | `PyrusUpdates` | PyrusEntities.cs |

### Requests (Builder → Request DTO → JSON)

| Operation | Builder Class | Request DTO |
|---|---|---|
| Create task (form) | `CreateTaskByForm` | `PyrusRequestTask` |
| Update task (form) | `UpdateTaskByForm` | `PyrusRequestTask` |
| Close task | `CloseTaskByForm` | `PyrusRequestTask` (action=finished) |
| Reopen task | `ReopenTaskByForm` | `PyrusRequestTask` (action=reopened) |
| Create simple task | `CreateSimpleTask` | `PyrusRequestTask` |
| Update simple task | `UpdateSimpleTask` | `PyrusRequestTask` |
| Create catalog | `CreateCatalog` | `PyrusRequestCatalog` |
| Update catalog | `UpdateCatalog` | `PyrusRequestCatalog` |
| Update catalog items | `UpdateCatalogItems` | `PyrusRequestCatalogItems` |
| Create member | `CreateMember` | `PyrusRequestMember` |
| Update member | `UpdateMember` | `PyrusRequestMember` |
| Block member | `BlockMember` | (no body) |
| Create role | `CreateRole` | `PyrusRequestRole` |
| Update role | `UpdateRole` | `PyrusRequestRole` |
| Create announcement | `CreateAnnouncement` | `PyrusRequestTask` |
| Comment announcement | `CommentAnnouncement` | `PyrusRequestTask` |

## Project Structure for Integration

| File | Responsibility |
|---|---|
| `ApiClient.cs` | HTTP-транспорт, аутентификация, retry при 401 |
| `Models/DTOs/PyrusEntities.cs` | Десериализация response |
| `Models/DTOs/PyrusRequest*.cs` | Сериализация request body |
| `Models/Methods/**/*.cs` | Fluent API для формирования запросов |
| `Extentions/PyrusExtentions.cs` | JSON-сериализация (GetJson) |
| `Extentions/HashExtensions.cs` | Hex-кодирование для CheckSig |

## Key Interfaces

### Создание клиента
```csharp
using ApiPyrus;

var apiClient = new ApiClient("user@company.com", "api-key-from-pyrus");
apiClient.GotInfoLog += msg => logger.LogInformation(msg);
apiClient.GotErrorLog += msg => logger.LogError(msg);
```

### Read-операции
```csharp
List<PyrusTask> tasks = await apiClient.GetTasks(formId, "?fld4=value");
Catalog catalog = await apiClient.GetCatalog(catalogId);
List<ValuePersone> members = await apiClient.GetMembers();
```

### Write-операции (через Builder)
```csharp
PyrusTask task = await new CreateTaskByForm(formId)
    .AddField(new ValueFieldData(1, "value"))
    .Send(apiClient);
```

### Webhook verification
```csharp
bool valid = apiClient.CheckSig(body, signature, botSecret);
PyrusUpdates update = JsonConvert.DeserializeObject<PyrusUpdates>(body);
```

## Configuration

| Parameter | Purpose | Default |
|---|---|---|
| `url` | Pyrus API base URL | `https://api.pyrus.com` |
| `apiVersion` | API version path segment | `v4` |
| `requestsTimeOut` | HTTP request timeout | HttpClient default |
| `ignoreSslSecurityErros` | Skip SSL validation | `false` |
