# ApiPyrus Architecture

## General Diagram

```
┌─────────────────────────────────────────────────────────┐
│                   Consumer Application                   │
│         (бот, веб-сервис, консольное приложение)         │
└───────────┬─────────────────────────────┬───────────────┘
            │ public read methods          │ fluent builders
            │ (GetTasks, GetCatalogs...)   │ (CreateTaskByForm, ...)
            ▼                              ▼
┌───────────────────────┐    ┌──────────────────────────┐
│      ApiClient        │◄───│   Models/Methods/*       │
│  (HTTP + Auth + Log)  │    │   (Fluent Builders)      │
│                       │    │   .Send(apiClient)       │
│  - Auth()             │    └──────────┬───────────────┘
│  - ApiRequest()       │               │ uses GetJson()
│  - DownloadFile()     │               ▼
│  - UploadFile()       │    ┌──────────────────────────┐
│  - CheckSig()         │    │   Extentions/            │
└───────────┬───────────┘    │   - PyrusExtentions      │
            │                │   - HashExtensions       │
            │ HTTP           └──────────────────────────┘
            ▼
┌───────────────────────┐
│   Pyrus REST API      │
│   api.pyrus.com/v4    │
└───────────────────────┘
```

## Module Dependencies

```
┌──────────────┐     ┌──────────────────┐     ┌──────────────┐
│  Methods/*   │────►│  Models/DTOs     │◄────│  Extentions  │
│  (Builders)  │     │  (Entities,      │     │              │
│              │────►│   Enums, IValue) │     │              │
└──────┬───────┘     └──────────────────┘     └──────▲───────┘
       │                                              │
       │              ┌──────────────────┐            │
       └─────────────►│   ApiClient.cs   │────────────┘
                      └──────────────────┘
```

- **Methods/** → **ApiClient** — builders вызывают internal методы (CreateTask, UpdateCatalog, etc.)
- **Methods/** → **DTOs** — builders формируют request body из DTO-классов
- **Methods/** → **Extentions** — используют GetJson() для сериализации
- **ApiClient** → **DTOs** — десериализует JSON-ответы в DTO
- **ApiClient** → **Extentions** — ToHexString() для CheckSig
- **Extentions** → **DTOs** — расширения для PyrusTask/Field/IValue

## Data Flow

### Read (GET-запросы)
```
Consumer → ApiClient.GetXxx()
  → ApiRequest(url, "GET")
    → [if no token] Auth() → Bearer token
    → HttpClient.SendAsync(GET)
    → [if 401] Auth() → retry
    → JsonConvert.DeserializeObject<T>(response)
  → возвращает DTO
```

### Write (POST/PUT/DELETE через Builder)
```
Consumer → new CreateTaskByForm(formId)
  .AddField(...)
  .AddSubscribers(...)
  .Send(apiClient)
    → builder.GetJson() → JSON body
    → ApiClient.CreateTask(json)
      → ApiRequest(url, "POST", body)
        → [auth if needed]
        → HttpClient.SendAsync(POST)
        → JsonConvert.DeserializeObject<T>(response)
      → возвращает DTO
```

### File Upload
```
Consumer → ApiClient.UploadFile(fullFileName)
  → MultipartFormDataContent + ByteArrayContent
  → POST /v4/files/upload
  → returns Guid (attachment id)
```

### Webhook Signature Verification
```
Consumer → ApiClient.CheckSig(msg, sig, secret)
  → HMACSHA1(secret).ComputeHash(msg)
  → ToHexString()
  → compare with sig
```

## Technology Stack

| Component | Technology |
|---|---|
| Language | C# (LangVersion 12) |
| Target | .NET Standard 2.0 |
| HTTP | System.Net.Http.HttpClient |
| JSON | Newtonsoft.Json 13.0.3 |
| Crypto | System.Security.Cryptography (HMACSHA1) |
| Package | NuGet (GeneratePackageOnBuild) |
| Build | MSBuild (Visual Studio) |
