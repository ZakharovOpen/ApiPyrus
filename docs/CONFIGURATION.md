# Configuration

## ApiClient Constructor

Вся конфигурация передаётся через конструктор `ApiClient`. Конфигурационных файлов (appsettings, .env) нет — это библиотека.

```csharp
public ApiClient(
    string login,                        // Email пользователя Pyrus
    string key,                          // API-ключ Pyrus
    string url = "https://api.pyrus.com",// Base URL (можно указать on-premise)
    string apiVersion = "v4",            // Версия API
    TimeSpan? requestsTimeOut = null,    // Таймаут HTTP-запросов
    bool ignoreSslSecurityErros = false  // Игнорировать SSL-ошибки
)
```

### Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `login` | string | **required** | Email пользователя или сервисного аккаунта Pyrus |
| `key` | string | **required** | API Security Key из настроек Pyrus |
| `url` | string | `https://api.pyrus.com` | Base URL. Для on-premise: `https://your-pyrus.company.com` |
| `apiVersion` | string | `v4` | Версия API. Используется в путях: `/{apiVersion}/tasks` |
| `requestsTimeOut` | TimeSpan? | null (HttpClient default) | Таймаут для всех HTTP-запросов |
| `ignoreSslSecurityErros` | bool | `false` | При `true`: отключает валидацию SSL-сертификатов, включает TLS 1.0/1.1/1.2, увеличивает connection limit до 9999 |

## Authentication

Аутентификация выполняется автоматически:
1. При первом запросе — если `_token` пуст
2. При получении 401 — re-auth и повтор запроса

Endpoint: `GET /{apiVersion}/auth?login={login}&security_key={key}`
Response: `{ "access_token": "..." }`
Token хранится в приватном поле `_token` и передаётся через `Authorization: Bearer {token}`.

Можно установить токен вручную:
```csharp
apiClient.ChangeToken("existing-token");
```

## SSL Configuration

При `ignoreSslSecurityErros = true`:
```csharp
ServicePointManager.Expect100Continue = true;
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
    | SecurityProtocolType.Tls11
    | SecurityProtocolType.Tls12;
ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
ServicePointManager.DefaultConnectionLimit = 9999;

HttpClientHandler handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
```

> **Warning**: используйте `ignoreSslSecurityErros = true` только для on-premise с самоподписанными сертификатами.

## Logging

Event-based логирование — нет зависимости от конкретного фреймворка:

```csharp
var apiClient = new ApiClient("login", "key");
apiClient.GotInfoLog += (message) => Console.WriteLine($"[INFO] {message}");
apiClient.GotErrorLog += (message) => Console.WriteLine($"[ERROR] {message}");
```

Логируются:
- Все исходящие запросы (метод, URL, токен, body)
- Все ответы (response body)
- Проверка подписи (CheckSig)
- Ошибки (HTTP status code + ReasonPhrase + body)

## Serialization Settings

Дефолтные настройки для `GetJson()`:
```csharp
Formatting = Formatting.Indented
NullValueHandling = NullValueHandling.Ignore
DefaultValueHandling = DefaultValueHandling.Ignore
ReferenceLoopHandling = ReferenceLoopHandling.Ignore
```

Можно передать кастомные `JsonSerializerSettings`:
```csharp
var json = entity.GetJson(new JsonSerializerSettings { ... });
```

## Webhook Signature Verification

Для проверки подписи входящих webhook-запросов от Pyrus:
```csharp
bool isValid = apiClient.CheckSig(
    msg: requestBody,    // Тело запроса
    sig: signatureHeader, // Значение из заголовка X-Pyrus-Sig
    secret: "your-secret" // Секрет бота
);
```
Алгоритм: HMAC-SHA1(secret, msg) → hex string → сравнение с sig.
