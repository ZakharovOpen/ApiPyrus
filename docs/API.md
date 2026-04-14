# API Endpoints

Base URL: `https://api.pyrus.com` (configurable)
API Version: `v4` (configurable)
Authorization: Bearer token (auto-obtained via auth endpoint)

## Auth

### GET `/v4/auth` — Аутентификация
Вызывается автоматически при первом запросе или при получении 401.

Query parameters:
| Parameter | Type | Description |
|---|---|---|
| login | string | Email пользователя |
| security_key | string | API-ключ |

Response 200:
```json
{ "access_token": "string" }
```

C# method: `ApiClient.Auth()` (private)

---

## Tasks

### POST `/v4/tasks` — Создать задачу
Authorization: Bearer token
Body: JSON (PyrusRequestTask)

Response 200:
```json
{ "task": { ...PyrusTask... } }
```

C# methods:
- `ApiClient.CreateTask(json)` (internal)
- Builder: `new CreateTaskByForm(formId).AddField(...).Send(apiClient)`
- Builder: `new CreateSimpleTask().AddText(...).Send(apiClient)`

### GET `/v4/tasks/{taskId}` — Получить задачу по ID

Response 200:
```json
{ "task": { ...PyrusTask... } }
```

C# method: `ApiClient.GetTaskInfoById(taskId)`

### POST `/v4/tasks/{taskId}/comments` — Комментировать / обновить / закрыть задачу
Body: JSON (PyrusRequestTask)

Response 200:
```json
{ "task": { ...PyrusTask... } }
```

C# methods:
- `ApiClient.CloseOrCommentTask(taskId, json)` (internal)
- Builder: `new UpdateTaskByForm(taskId).AddText(...).Send(apiClient)`
- Builder: `new CloseTaskByForm(taskId).Send(apiClient)`
- Builder: `new ReopenTaskByForm(taskId).Send(apiClient)`
- Builder: `new UpdateSimpleTask(taskId).AddText(...).Send(apiClient)`

---

## Forms

### GET `/v4/forms` — Получить шаблоны всех форм

Response 200:
```json
{ "forms": [ { ...Form... } ] }
```

C# method: `ApiClient.GetFormsTemplates()`

### GET `/v4/forms/{formId}/register{query}` — Получить задачи по форме

Query parameters (в строке):
| Parameter | Type | Description |
|---|---|---|
| fld{N} | string | Фильтр по полю N |
| include_archived | string | `y` — включить архивные |

Response 200:
```json
{ "tasks": [ { ...PyrusTask... } ] }
```

C# methods:
- `ApiClient.GetTasks(formId, query)` — с raw query string
- `ApiClient.GetTasks(formId, Dictionary<object, object>)` — с параметрами
- `ApiClient.GetTasks(formId, Dictionary<int, string>, includeArchived)` — с id полей

---

## Catalogs

### GET `/v4/catalogs` — Получить все справочники

Response 200:
```json
{ "catalogs": [ { ...Catalog... } ] }
```

C# method: `ApiClient.GetCatalogs()`

### GET `/v4/catalogs/{catalogId}` — Получить справочник по ID

Response 200: `Catalog` JSON

C# method: `ApiClient.GetCatalog(catalogId)`

### PUT `/v4/catalogs` — Создать справочник
Body: JSON (PyrusRequestCatalog)

Response 200: `Catalog` JSON

C# methods:
- `ApiClient.CreateCatalog(json)` (internal)
- Builder: `new CreateCatalog(name).AddHeaders(...).AddItems(...).Send(apiClient)`

### POST `/v4/catalogs/{catalogId}` — Обновить справочник (полная замена)
Body: JSON (PyrusRequestCatalog)

Response 200: `CatalogUpdateInfo` JSON

C# methods:
- `ApiClient.UpdateCatalog(catalogId, json)` (internal)
- Builder: `new UpdateCatalog(catalogId, apiClient).SetCatalog().AddItem(...).Send()`

### POST `/v4/catalogs/{catalogId}/diff` — Обновить справочник (diff)
Body: JSON (PyrusRequestCatalogItems — upsert/delete)

Response 200: `CatalogUpdateInfo` JSON

C# methods:
- `ApiClient.UpdateCatalogDiff(catalogId, json)` (internal)
- Builder: `new UpdateCatalogItems(catalogId, apiClient).AddItemToUpsert(...).Send()`

---

## Members

### GET `/v4/members` — Получить всех сотрудников

Response 200:
```json
{ "members": [ { ...ValuePersone... } ] }
```

C# method: `ApiClient.GetMembers()`

### DELETE `/v4/members/{memberId}` — Получить сотрудника по ID
> Примечание: в коде используется HTTP DELETE для GET-запроса — особенность Pyrus API

C# method: `ApiClient.GetMember(memberId)`

### POST `/v4/members` — Добавить сотрудника
Body: JSON (PyrusRequestMember)

C# methods:
- `ApiClient.AddMembers(json)` (internal)
- Builder: `new CreateMember(firstName, lastName, email).Send(apiClient)`

### PUT `/v4/members/{memberId}` — Обновить сотрудника

C# methods:
- `ApiClient.UpdateMembers(memberId, json)` (internal)
- Builder: `new UpdateMember(memberId, firstName: "New").Send(apiClient)`

### DELETE `/v4/members/{memberId}` — Заблокировать сотрудника

C# methods:
- `ApiClient.BlockMember(memberId)` (internal)
- Builder: `new BlockMember(memberId).Send(apiClient)`

---

## Profile

### GET `/v4/profile` — Получить профиль текущего пользователя

Query parameters:
| Parameter | Type | Description |
|---|---|---|
| include_inactive | bool | Включить неактивных |

C# method: `ApiClient.GetProfile(includeInactive)`

---

## Roles

### GET `/v4/roles` — Получить роли

Response 200:
```json
{ "roles": [ { ...Role... } ] }
```

C# method: `ApiClient.GetRoles()`

### POST `/v4/roles` — Создать роль

C# methods:
- `ApiClient.CreateRole(json)` (internal)
- Builder: `new CreateRole(name, memberIds).Send(apiClient)`

### PUT `/v4/roles/{roleId}` — Обновить роль

C# methods:
- `ApiClient.UpdateRole(roleId, json)` (internal)
- Builder: `new UpdateRole(roleId, name, addIds, removeIds).Send(apiClient)`

---

## Lists

### GET `/v4/lists` — Получить все списки

Response 200:
```json
{ "lists": [ { ...PyrusList... } ] }
```

C# method: `ApiClient.GetLists()`

### GET `/v4/lists/{listId}/tasks` — Получить задачи списка

Query parameters:
| Parameter | Type | Description |
|---|---|---|
| item_count | int | Количество (default 100) |
| include_archived | bool | Включить архивные |
| modified_before | string | До даты (ISO 8601) |
| modified_after | string | После даты (ISO 8601) |

C# method: `ApiClient.GetListTasks(listId, from, to, itemCount, includeInactive)`

---

## Inbox

### GET `/v4/inbox` — Получить задачи из входящих

Query parameters:
| Parameter | Type | Description |
|---|---|---|
| item_count | int | Количество (default 100) |

C# method: `ApiClient.GetInbox(itemCount)`

---

## Announcements

### GET `/v4/announcements` — Получить все объявления

C# method: `ApiClient.GetAnnouncements()`

### GET `/v4/announcements/{announcementId}` — Получить объявление по ID

C# method: `ApiClient.GetAnnouncement(announcementId)`

### POST `/v4/announcements` — Создать объявление

C# methods:
- `ApiClient.CreateAnnouncement(json)` (internal)
- Builder: `new CreateAnnouncement().AddText(...).Send(apiClient)`

### POST `/v4/announcements/{announcementId}/comments` — Комментировать объявление

C# methods:
- `ApiClient.CommentAnnouncement(json, announcementId)` (internal)
- Builder: `new CommentAnnouncement(id).AddText(...).Send(apiClient)`

---

## Files

### POST `/v4/files/upload` — Загрузить файл
Content-Type: multipart/form-data

Response 200:
```json
{ "guid": "uuid", "md5_hash": "string" }
```

C# method: `ApiClient.UploadFile(fullFileName)`

### GET `{attachmentUrl}` — Скачать файл
Authorization: Bearer token

C# methods:
- `ApiClient.DownloadFile(attachmentUrl, fileFullName)` — сохранить в файл
- `ApiClient.DownloadFile(attachmentUrl)` — вернуть Stream

---

## Response Codes

| Code | When |
|---|---|
| 200 | Успешный запрос |
| 401 | Невалидный/истёкший токен → auto re-auth |
| Other | Exception с кодом и ReasonPhrase |
