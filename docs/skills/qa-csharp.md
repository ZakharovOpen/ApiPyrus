---
name: qa-csharp
description: >
  QA skill for ApiPyrus library. Activate when writing, reviewing, or running tests
  for the ApiPyrus NuGet package.
user_invocable: true
---

# Skill: QA Tester — ApiPyrus

## Activation
Активируй при любой задаче, связанной с написанием, ревью или запуском тестов в этом проекте.

## Context files — прочитай перед работой

| File | Purpose |
|---|---|
| `CLAUDE.md` | Конвенции проекта |
| `docs/TEST_PLAN.md` | Полный план тестирования |
| `docs/DATA_MODEL.md` | DTO, enums, IValue — что тестировать |
| `docs/API.md` | API-эндпоинты — контракты |
| `docs/ARCHITECTURE.md` | Data flow — как данные проходят |

---

## Base C# testing conventions

### General testing rules
- Один тестовый класс на один тестируемый класс
- Naming: `MethodName_Scenario_ExpectedResult`
- Arrange / Act / Assert — чёткое разделение в каждом тесте
- Один assert per test (по возможности)
- Не тестировать приватные методы напрямую — только через public API
- Не мокать то, чем владеешь — мокать только внешние зависимости (HTTP)
- Тесты должны быть независимыми друг от друга — не зависеть от порядка запуска
- Не использовать Thread.Sleep — использовать async/await

### Test structure
```csharp
[Fact]
public async Task MethodName_WhenCondition_ShouldExpectedResult()
{
    // Arrange
    var sut = CreateSystemUnderTest();

    // Act
    var result = await sut.DoSomething();

    // Assert
    Assert.NotNull(result);
}
```

### Serialization testing
- Тестировать реальную сериализацию/десериализацию через Newtonsoft.Json
- Проверять `[JsonProperty]` маппинг — JSON field names должны совпадать с API
- Проверять `NullValueHandling.Ignore` — null-поля не должны попадать в JSON
- Проверять enum сериализацию — `StringEnumConverter`, `EnumMember`, `Description`

### Build & Run
```bash
# Сборка (MSBuild из Visual Studio, НЕ dotnet build)
"Z:\Programs\Microsoft Visual Studio\Product\MSBuild\Current\Bin/MSBuild.exe" ApiPyrus.sln /p:Configuration=Debug /restore /v:minimal

# Запуск тестов (после сборки MSBuild)
dotnet test ../ApiPyrusTester_net_standart/ApiPyrusTester.csproj --no-build
```

---

## Project-specific test setup

### Test project layout

```
ApiPyrus.sln
├── ApiPyrus/                  — основная библиотека (этот репозиторий)
└── ApiPyrusTester/            — тестовый проект (../ApiPyrusTester_net_standart/)
    └── ApiPyrusTester.csproj
```

> Тестовый проект находится **вне этого репозитория**. При создании тестов нужно работать с `../ApiPyrusTester_net_standart/`.

### Test packages

Рекомендуемые (если тестовый проект создаётся с нуля):

| Package | Purpose |
|---|---|
| xUnit | Test framework |
| xunit.runner.visualstudio | VS test runner |
| Moq | HTTP mocking |
| RichardSzalay.MockHttp | Mock HttpMessageHandler |
| FluentAssertions | Readable assertions |
| Microsoft.NET.Test.Sdk | Test SDK |

### Existing test infrastructure

Тестовый проект `ApiPyrusTester` уже подключён в solution. Его содержимое находится вне репозитория — необходимо проверить его наличие перед работой.

---

## Test fixtures / factories

### Mock Pyrus API Responses
Для unit-тестов необходимо создать JSON-файлы с типичными ответами Pyrus API:

```
TestData/
├── auth_response.json          — {"access_token": "test-token"}
├── task_response.json          — {"task": {...}}
├── tasks_response.json         — {"tasks": [{...}]}
├── catalog_response.json       — {"catalogs": [{...}]}
├── members_response.json       — {"members": [{...}]}
├── roles_response.json         — {"roles": [{...}]}
├── forms_response.json         — {"forms": [{...}]}
├── announcements_response.json — {"announcements": [{...}]}
└── upload_response.json        — {"guid": "...", "md5_hash": "..."}
```

### Mock HttpClient Pattern
```csharp
var mockHandler = new MockHttpMessageHandler();
mockHandler.When("*/v4/auth*")
    .Respond("application/json", File.ReadAllText("TestData/auth_response.json"));

var httpClient = new HttpClient(mockHandler);
// Inject via reflection or constructor (requires refactoring for testability)
```

### Integration test class pattern

```csharp
public class ApiClientIntegrationTests : IDisposable
{
    private readonly ApiClient _apiClient;

    public ApiClientIntegrationTests()
    {
        // Credentials from environment variables
        var login = Environment.GetEnvironmentVariable("PYRUS_LOGIN");
        var key = Environment.GetEnvironmentVariable("PYRUS_API_KEY");
        _apiClient = new ApiClient(login, key);
    }

    [Fact]
    public async Task GetCatalogs_ReturnsNonEmptyList()
    {
        var catalogs = await _apiClient.GetCatalogs();
        Assert.NotNull(catalogs);
        Assert.NotEmpty(catalogs);
    }

    public void Dispose() => _apiClient?.Dispose();
}
```

---

## Mandatory test checklist

### ApiClient — HTTP & Auth
| Scenario | Expected |
|---|---|
| Valid credentials → Auth | Returns token, subsequent requests use Bearer |
| Invalid credentials → Auth | Throws Exception |
| 401 during request | Re-auth + retry |
| Non-OK response | Exception with status code and body |
| Request timeout | HttpClient timeout applied |

### Builders — Fluent API
| Scenario | Expected |
|---|---|
| Each fluent method | Returns `this` for chaining |
| Builder.Send() | Calls correct ApiClient internal method |
| Builder serialization | GetJson() produces valid JSON with correct field names |
| Empty optional fields | Omitted from JSON (NullValueHandling.Ignore) |

### FieldWithValue — Type parsing
| Scenario | Expected |
|---|---|
| Each FieldType → GetValue\<T\> | Returns correct IValue subclass |
| Null value | Returns ValueError |
| Invalid cast | Throws InvalidCastException |
| Parse error | Returns ValueError with message |

### Extensions
| Scenario | Expected |
|---|---|
| GetFieldById — top level | Returns field |
| GetFieldById — in Title | Searches nested fields |
| GetFieldById — in MultipleChoice | Searches nested fields |
| GetFieldById — not found | Returns null |
| GetFieldsByType — mixed nesting | Returns all matching |
| GetJson — default settings | NullValueHandling.Ignore |

### CheckSig — HMAC verification
| Scenario | Expected |
|---|---|
| Valid signature | Returns true |
| Invalid signature | Returns false |
| Logs with extRequestId | Message includes [requestId] |

---

## Coverage targets

| Layer | Target |
|---|---|
| ApiClient (public methods) | 90% |
| Builders (fluent methods) | 95% |
| FieldWithValue.GetValueObject | 100% (all FieldTypes) |
| Extensions (PyrusExtentions) | 95% |
| Extensions (HashExtensions) | 100% |
| Enums (EnumExtension) | 100% |
| DTOs (constructors, ValueFieldData) | 90% |

---

## Forbidden in tests

- **Не мокать Newtonsoft.Json** — тестировать реальную сериализацию/десериализацию
- **Не создавать тестовый проект внутри этого репозитория** — использовать `../ApiPyrusTester_net_standart/`
- **Не добавлять System.Text.Json** даже в тесты — consistency
- **Не хардкодить credentials** — только из env vars для integration tests
- **Не тестировать приватные методы напрямую** — тестировать через public API
