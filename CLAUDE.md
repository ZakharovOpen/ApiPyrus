# ApiPyrus — C# клиент для Pyrus REST API

## Project Overview
ApiPyrus — NuGet-библиотека (.NET Standard 2.0) для работы с REST API сервиса Pyrus (pyrus.com).
Предоставляет типизированный клиент для всех основных методов API: задачи, справочники, сотрудники, роли, объявления, файлы.
Стек: C# / .NET Standard 2.0 / Newtonsoft.Json 13.0.3.

## Documentation
- [docs/USAGE.md](docs/USAGE.md) — **Quick Reference для внешних проектов** (подключить в CLAUDE.md потребителя)
- [docs/SPECIFICATION.md](docs/SPECIFICATION.md) — техническая спецификация и обзор модулей
- [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) — архитектура библиотеки
- [docs/DATA_MODEL.md](docs/DATA_MODEL.md) — модель данных (DTO, enums, IValue)
- [docs/API.md](docs/API.md) — обёрнутые API-эндпоинты Pyrus
- [docs/PROJECT_STRUCTURE.md](docs/PROJECT_STRUCTURE.md) — структура проекта и компоненты
- [docs/CONFIGURATION.md](docs/CONFIGURATION.md) — конфигурация ApiClient
- [docs/INTEGRATION.md](docs/INTEGRATION.md) — интеграция с Pyrus REST API
- [docs/TEST_PLAN.md](docs/TEST_PLAN.md) — план тестирования

## Skills

| Role      | Skill file                     |
|-----------|--------------------------------|
| Developer | `docs/skills/dev-csharp.md`    |
| Tester    | `docs/skills/qa-csharp.md`     |

## Key Conventions

### Naming
- **Namespace root**: `ApiPyrus`
- **DTOs**: `ApiPyrus.Models.DTOs` — классы с префиксом `Pyrus` / `Value` для ответов API
- **Builders**: `ApiPyrus.Models.Methods.{Domain}` — fluent-builder классы (CreateTaskByForm, UpdateCatalog и т.д.)
- **Extensions**: `ApiPyrus.Extentions` (с опечаткой — историческое написание, не менять)
- **Файлы**: PascalCase, совпадает с именем основного класса

### Patterns
- **Fluent Builder** — все операции записи (Create/Update/Close/Reopen) используют цепочку методов с `.Send(apiClient)` в конце
- **Наследование builders** — `CloseTaskByForm` и `ReopenTaskByForm` наследуют `UpdateTaskByForm`
- **Internal methods** в `ApiClient` — write-операции (`CreateTask`, `UpdateCatalog` и т.д.) имеют модификатор `internal`, вызываются только через builders
- **Public methods** в `ApiClient` — read-операции (`GetTasks`, `GetCatalogs` и т.д.)
- **Event-based logging** — `GotInfoLog` / `GotErrorLog` events вместо ILogger

### Serialization
- **Только Newtonsoft.Json** (JsonConvert, JsonProperty, StringEnumConverter)
- **Запрещено**: System.Text.Json
- Настройки по умолчанию для GetJson: `NullValueHandling.Ignore`, `DefaultValueHandling.Ignore`, `Formatting.Indented`
- Enums сериализуются через `[EnumMember(Value = "...")]` + `StringEnumConverter` или через `[Description("...")]`

### Code Style
- XML-документация на русском языке
- CS1591 / CS1587 подавлены в `.editorconfig`
- `LangVersion 12`
- Нет `.editorconfig` правил форматирования — следовать существующему стилю

### Configuration
- Библиотека не использует appsettings/env — вся конфигурация через конструктор `ApiClient`
- Параметры: `login`, `key`, `url` (default `https://api.pyrus.com`), `apiVersion` (default `v4`), `requestsTimeOut`, `ignoreSslSecurityErros`

### Authorization
- API Key auth: `GET /v4/auth?login={login}&security_key={key}` → Bearer token
- Автоматический re-auth при 401
- Метод `CheckSig` для проверки HMAC-SHA1 подписи webhook-запросов

### Logging
- Event-based: `ApiClient.GotInfoLog` / `ApiClient.GotErrorLog` (delegate `StringEvent`)
- Нет ILogger/Serilog

### Testing
- Тестовый проект `ApiPyrusTester` расположен вне репозитория: `../ApiPyrusTester_net_standart/ApiPyrusTester.csproj`
- Подключён в `ApiPyrus.sln`

### Database
- Нет БД — чистая HTTP-клиентская библиотека

### Pre-commit checklist
Перед каждым коммитом:
1. **Версия** — поднять `Version`, `AssemblyVersion`, `FileVersion` в `ApiPyrus.csproj` и обновить `PackageReleaseNotes`
2. **CHANGELOG** — добавить запись с датой и списком изменений
3. **Документация** — обновить затронутые файлы в `docs/` (API.md, DATA_MODEL.md, USAGE.md и т.д.) если изменились публичные API, DTO, enums или builders

## Commands
```bash
# Сборка (MSBuild из Visual Studio, НЕ dotnet build)
"Z:\Programs\Microsoft Visual Studio\Product\MSBuild\Current\Bin/MSBuild.exe" ApiPyrus.sln /p:Configuration=Debug /restore /v:minimal

# NuGet pack
dotnet pack ApiPyrus.csproj -c Release
```
