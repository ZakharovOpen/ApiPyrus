# Plan: Fix bugs found during code review

## Context
Ревью кода ApiPyrus выявило 16 багов разной критичности. Этот план покрывает исправление всех критических и средних багов. Низкие — по желанию.

## Files to modify
- `ApiClient.cs`
- `Models/DTOs/PyrusEntities.cs`
- `Models/DTOs/PyrusEnums.cs`
- `Models/Methods/Catalogs/UpdateCatalog.cs`
- `Extentions/PyrusExtentions.cs`

---

## Critical fixes

### 1. `ApiClient.cs:101` — `Auth()` без `await`
```
- Auth();
+ await Auth();
```

### 2. `ApiClient.cs:193` — `GetMember` использует DELETE вместо GET
```
- await ApiRequest($"/{_apiVersion}/members/{memberId}", "DELETE", ...)
+ await ApiRequest($"/{_apiVersion}/members/{memberId}", externalRequestId: ...)
```
Убрать `"DELETE"` — по умолчанию `method = "GET"`.

### 3. `PyrusEntities.cs:861,867` — `hh` вместо `HH` (12-часовой формат)
```
- ValueData = _valueData.ToString("yyyy-MM-ddThh:mm:ssZ");
+ ValueData = _valueData.ToString("yyyy-MM-ddTHH:mm:ssZ");
```
```
- ValueData = _valueData.ToString("hh:mm");
+ ValueData = _valueData.ToString("HH:mm");
```

### 4. `ApiClient.cs:438` — `AddScheduledDate` вызывает не тот метод
```
- return await new UpdateTaskByForm(taskId).ScheduledDatetimeUtc(scheduledDate).Send(this);
+ return await new UpdateTaskByForm(taskId).ScheduledDate(scheduledDate).Send(this);
```

### 5. `PyrusExtentions.cs:10-31` — гонка на `JsonConvert.DefaultSettings`
Передавать `settings` напрямую в `SerializeObject` вместо мутации глобального состояния:
```csharp
// Было:
JsonConvert.DefaultSettings = () => settings;
var json = JsonConvert.SerializeObject(entityForJson);
JsonConvert.DefaultSettings = () => new JsonSerializerSettings();

// Стало:
var json = JsonConvert.SerializeObject(entityForJson, settings);
```
Применить к обеим перегрузкам `GetJson`.

---

## Medium fixes

### 6. `ApiClient.cs:99-102` — бесконечная рекурсия при 401
Добавить флаг `isRetry` в рекурсивный вызов, чтобы не повторять re-auth бесконечно:
```
- returnText = await ApiRequest(additionalUrl, method, body, false, externalRequestId);
+ returnText = await ApiRequest(additionalUrl, method, body, true, externalRequestId);
```
Это использует `isAuthRequest = true`, что пропустит блок re-auth при повторном 401. Но лучше ввести отдельный параметр `isRetry`, чтобы не путать семантику. Однако минимальное изменение — передать `isAuthRequest: true`, тогда при повторном 401 будет выброшен exception вместо рекурсии.

### 7. `ApiClient.cs:316-319` — `includeArchived: false` игнорируется
```
- if(includeArchived.HasValue)
+ if(includeArchived.HasValue && includeArchived.Value)
```

### 8. `PyrusEntities.cs:968` — `[JsonProperty("Name")]` → `"name"`
```
- [JsonProperty("Name")]
+ [JsonProperty("name")]
```

### 9. `UpdateCatalog.cs:85` — IndexOutOfRange в `AddOrUpdateItem`
Цикл обновления должен учитывать минимальную длину:
```csharp
// Было:
for (int i = 1; i < entity.Values.Count; i++)
    entity.Values[i] = itemValues[i];

// Стало:
var count = Math.Min(entity.Values.Count, itemValues.Count);
for (int i = 0; i < count; i++)
    entity.Values[i] = itemValues[i];
```

### 10. `PyrusEntities.cs:380,387` — NRE при `FormattedText == null`
Добавить null-проверку:
```
- !FormattedText.Contains("<quote data-noteid")
+ (FormattedText == null || !FormattedText.Contains("<quote data-noteid"))
```
Аналогично для `GetCommentFormattedTextForReply`. Также для `Text` в `GetOnlyReplyCommentText`/`GetCommentTextForReply`.

### 11. `PyrusEnums.cs:298` — NRE в `GetEnumMemberValue`
```csharp
// Было:
return enumMemberAttribute.Value;

// Стало:
return enumMemberAttribute?.Value ?? enumValue.ToString();
```

### 12. `ApiClient.cs:28-34` — глобальное отключение SSL
Это архитектурная проблема `ServicePointManager` в .NET Standard 2.0 — нет хорошего локального решения. **Не исправляем**, но добавим XML-предупреждение в документации конструктора.

### 13. `PyrusEntities.cs:861` — Z без UTC-конвертации
Уже исправляется в рамках бага #3 (формат `HH`). Дополнительно заменить литерал `Z` на формат с timezone:
```
- "yyyy-MM-ddTHH:mm:ssZ"
+ "yyyy-MM-ddTHH:mm:ss'Z'"
```
Оставляем `'Z'` как литерал — это соответствует поведению API Pyrus, который ожидает UTC-время с суффиксом Z. Ответственность за передачу UTC лежит на потребителе.

---

## Skip (low priority, не ломают логику)
- #14 — утечка HttpResponseMessage в DownloadFile (требует рефакторинг возвращаемого Stream)
- #15 — UploadFile без проверки токена (лишний round-trip, не крашит)
- #16 — StatusTypes default = 0 (edge case при неизвестном статусе)

---

## Verification
Проект собирается через MSBuild:
```bash
"Z:\Programs\Microsoft Visual Studio\Product\MSBuild\Current\Bin/MSBuild.exe" ApiPyrus.sln /p:Configuration=Debug /restore /v:minimal
```
После всех исправлений — запустить сборку и убедиться, что нет ошибок компиляции.
