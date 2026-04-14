# ApiPyrus — Quick Reference

> Подключи этот файл в CLAUDE.md внешнего проекта, чтобы ИИ знал как использовать библиотеку.

```
NuGet: Install-Package ApiPyrus
Target: .NET Standard 2.0
Depends: Newtonsoft.Json 13.0.3
```

## 1. Создание клиента

```csharp
using ApiPyrus;

// Минимально
var apiClient = new ApiClient("login@mail.com", "pyrus-api-key");

// Полный конструктор
var apiClient = new ApiClient(
    login: "login@mail.com",
    key: "pyrus-api-key",
    url: "https://api.pyrus.com",     // on-premise — свой URL
    apiVersion: "v4",
    requestsTimeOut: TimeSpan.FromSeconds(30),
    ignoreSslSecurityErros: false      // true для самоподписанных сертификатов
);

// Логирование (events, не ILogger)
apiClient.GotInfoLog += msg => Console.WriteLine(msg);
apiClient.GotErrorLog += msg => Console.Error.WriteLine(msg);

// IDisposable — оборачивать в using
using var client = new ApiClient("login", "key");
```

Аутентификация автоматическая — Bearer-токен получается при первом запросе и обновляется при 401.

## 2. Read-методы (public)

```csharp
using ApiPyrus.Models.DTOs;

// Задачи
PyrusTask task            = await apiClient.GetTaskInfoById(taskId);
List<PyrusTask> tasks     = await apiClient.GetTasks(formId, "?fld4=value&include_archived=y");
List<PyrusTask> tasks     = await apiClient.GetTasks(formId, new Dictionary<int, string> { {4, "value"} }, includeArchived: true);
List<PyrusTask> inbox     = await apiClient.GetInbox(itemCount: 50);

// Формы
List<Form> forms          = await apiClient.GetFormsTemplates();

// Справочники
List<Catalog> catalogs    = await apiClient.GetCatalogs();
Catalog catalog           = await apiClient.GetCatalog(catalogId);

// Сотрудники
List<ValuePersone> members = await apiClient.GetMembers();
ValuePersone member       = await apiClient.GetMember(memberId);
ValuePersone profile      = await apiClient.GetProfile(includeInactive: true);

// Роли
List<Role> roles          = await apiClient.GetRoles();

// Списки
List<PyrusList> lists     = await apiClient.GetLists();
List<PyrusTask> listTasks = await apiClient.GetListTasks(listId, from, to, itemCount: 100);

// Объявления
List<AnnouncementInfo> anns = await apiClient.GetAnnouncements();
AnnouncementInfo ann      = await apiClient.GetAnnouncement(announcementId);

// Файлы
Guid fileId               = await apiClient.UploadFile(@"C:\file.png");
bool ok                   = await apiClient.DownloadFile(attachmentUrl, @"C:\out.png");
Stream stream             = await apiClient.DownloadFile(attachmentUrl);

// Планирование
await apiClient.AddScheduledDate(taskId, "2025-06-01");
await apiClient.AddScheduledDatetimeUtc(taskId, "2025-06-01T10:00:00Z");
await apiClient.CancelSchedule(taskId);
```

## 3. Форматированный текст (formatted_text)

При `AddText(text, formatedText: true)` текст передаётся в поле `formatted_text` — Pyrus рендерит HTML.

### Поддерживаемые теги

| Тег | Назначение | Пример |
|---|---|---|
| `<b>` | Жирный | `<b>текст</b>` |
| `<i>` | Курсив | `<i>текст</i>` |
| `<s>` | Зачёркнутый | `<s>текст</s>` |
| `<code>` | Код (inline) | `<code>var x = 1;</code>` |
| `<br/>` | Перенос строки | `строка1<br/>строка2` |
| `<div data-type="heading">` | Заголовок | `<div data-type="heading">Заголовок</div>` |
| `<q>` | Цитата (блок) | `<q>цитата</q>` |
| `<mark data-color="...">` | Выделение цветом | `<mark data-color="red">важно</mark>` |
| `<ul><li>` | Маркированный список | `<ul><li>пункт 1</li><li>пункт 2</li></ul>` |
| `<ol><li>` | Нумерованный список | `<ol><li>первый</li><li>второй</li></ol>` |
| `<a href="...">` | Ссылка | `<a href="https://example.com">текст</a>` |
| `<button>` | Кнопка | `<button>Нажми</button>` |

Цвета для `<mark>`: `red`, `yellow`, `green`, `blue`.

### Примеры

```csharp
// Простой форматированный комментарий
await new UpdateTaskByForm(taskId)
    .AddText("<b>Важно!</b> Задача обновлена.<br/><i>Подробности ниже.</i>", formatedText: true)
    .Send(apiClient);

// Список
await new UpdateTaskByForm(taskId)
    .AddText("<div data-type=\"heading\">Итоги</div><ul><li>Пункт 1</li><li>Пункт 2</li></ul>", formatedText: true)
    .Send(apiClient);

// Выделение цветом
await new UpdateTaskByForm(taskId)
    .AddText("Статус: <mark data-color=\"green\">Готово</mark>", formatedText: true)
    .Send(apiClient);

// Ответ на комментарий (цитата — формируется автоматически)
await new UpdateTaskByForm(taskId)
    .ReplyComment(replyNoteId, "текст цитаты", "мой ответ")
    .Send(apiClient);
```

## 4. Write-методы (через Fluent Builders)

Паттерн: `new Builder(...).Method(...).Method(...).Send(apiClient)`

```csharp
using ApiPyrus.Models.DTOs;
using ApiPyrus.Models.Methods.Tasks;
using ApiPyrus.Models.Methods.Catalogs;
using ApiPyrus.Models.Methods.Members;
using ApiPyrus.Models.Methods.Roles;
using ApiPyrus.Models.Methods.Announcements;
```

### Задачи по форме

```csharp
// Создать
PyrusTask task = await new CreateTaskByForm(formId)
    .AddField(new ValueFieldData(1, "текст"))
    .AddField(new ValueFieldData(2, new ValueChoiceData(5)))
    .AddField(new ValueFieldData(3, new ValueItemData(catalogItemId)))
    .AddField(new ValueFieldData(4, DateTime.Now, DateTimeFormatTypes.DateOnly))
    .AddField(new ValueFieldData(5, 1500.50m))
    .AddField(new ValueFieldData(6, CheckmarkTypes.Checked))
    .AddFields(listOfFields)
    .AddSubscribers(new List<ValueIdData> { new ValueIdData(userId) })
    .AddApprovals(new List<List<ValueIdData>> { new List<ValueIdData> { new ValueIdData(approverId) } })
    .AddParentTaskId(parentId)
    .AddListsIds(new List<long> { listId })
    .FillDefaults()
    .ScheduledDate("2025-06-01")
    .Send(apiClient);

// Обновить / комментировать
PyrusTask updated = await new UpdateTaskByForm(taskId)
    .AddText("Комментарий")
    .AddText("<b>HTML</b>", formatedText: true)
    .UpdateField(new ValueFieldData(1, "новое значение"))
    .UpdateFields(listOfFields)
    .AddAttachments(new List<Guid> { fileGuid })
    .AddAttachments(new List<ValueAttachmentData> { new ValueAttachmentData(guid, rootId) })
    .AddSubscribers(new List<ValueIdData> { new ValueIdData(userId) })
    .RemoveSubscribers(new List<ValueIdData> { new ValueIdData(userId) })
    .AddApprovals(approvalSteps)
    .RemoveApprovals(approvalSteps)
    .AddApprovalChoice(ApprovalTypes.Approved)
    .AddChannel(ChannelTypes.Email)
    .AddChannel(ChannelTypes.SMS, phone: "+79991112233")
    .AddListsIds(new List<long> { listId })
    .RemovedTasksIds(new List<long> { listId })
    .UpdateSpentMinutesInfo(30)
    .EditComment(commentId)
    .ReplyComment(replyNoteId, "цитата", "ответ")
    .SkipSatisfaction()
    .SkipNotification()
    .CancelSchedule()
    .Send(apiClient);

// Закрыть (наследует UpdateTaskByForm — все те же методы доступны)
PyrusTask closed = await new CloseTaskByForm(taskId)
    .AddText("Закрываю")
    .Send(apiClient);

// Переоткрыть
PyrusTask reopened = await new ReopenTaskByForm(taskId)
    .AddText("Переоткрываю")
    .Send(apiClient);
```

### Простые задачи

```csharp
PyrusTask simple = await new CreateSimpleTask()
    .AddSubject("Заголовок")
    .AddText("Описание")
    .AddDueDate("2025-06-01")
    .AddDue("2025-06-01T10:00:00Z")
    .AddDuration(60)
    .AddResponsible(new ValueIdData(userId))
    .AddParticipants(new List<ValueIdData> { new ValueIdData(userId) })
    .AddSubscribers(new List<ValueIdData> { new ValueIdData(userId) })
    .Send(apiClient);

PyrusTask updatedSimple = await new UpdateSimpleTask(taskId)
    .AddText("Комментарий")
    .UpdateSubject("Новый заголовок")
    .UpdatDueDate("2025-07-01")
    .ReassignTo(new ValueIdData(newUserId))
    .AddSpentMinutesInfo(15)
    .Send(apiClient);
```

### Справочники

```csharp
// Создать
Catalog cat = await new CreateCatalog("Название")
    .AddHeaders(new List<string> { "Имя", "Фамилия" })
    .AddItem(new ValuesList(new List<string> { "Павел", "Захаров" }))
    .AddItems(listOfValuesList)
    .Send(apiClient);

// Обновить (полная замена)
CatalogUpdateInfo info = await new UpdateCatalog(catalogId, apiClient)
    .SetCatalog()                  // загрузит текущие headers + items из API
    .AddItem(new ValuesList(...))
    .AddOrUpdateItem(0, "key", new List<string> { "key", "val1", "val2" })
    .Send();

// Обновить (diff — upsert/delete)
CatalogUpdateInfo diff = await new UpdateCatalogItems(catalogId, apiClient)
    .AddItemToUpsert(new ValuesList(new List<string> { "key", "val" }))
    .AddItemsToUpsert(listOfValuesList)
    .AddItemToDelete("key-to-delete")
    .AddItemsToDelete(new List<string> { "key1", "key2" })
    .Send();
```

### Сотрудники

```csharp
ValuePersone member = await new CreateMember("Имя", "Фамилия", "email@mail.com",
    position: "Разработчик", departmentId: 123).Send(apiClient);

ValuePersone updated = await new UpdateMember(memberId,
    firstName: "НовоеИмя", banned: false).Send(apiClient);

ValuePersone blocked = await new BlockMember(memberId).Send(apiClient);
```

### Роли

```csharp
Role role = await new CreateRole("Менеджеры", new List<long> { memberId1, memberId2 })
    .Send(apiClient);

Role updated = await new UpdateRole(roleId, roleName: "Новое имя",
    addMembersIds: new List<long> { id3 },
    removeMembersIds: new List<long> { id1 }).Send(apiClient);
```

### Объявления

```csharp
AnnouncementInfo ann = await new CreateAnnouncement()
    .AddText("Текст объявления")
    .AddAttachments(new List<Guid> { fileGuid })
    .Send(apiClient);

AnnouncementInfo comment = await new CommentAnnouncement(announcementId)
    .AddText("Комментарий")
    .Send(apiClient);
```

## 5. Работа с полями задачи

```csharp
using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;

// Получить поле по ID (ищет и во вложенных Title/MultipleChoice)
Field field = task.GetFieldById(fieldId);
Field field = task.Fields.GetFieldById(fieldId);

// Безопасный вариант
if (task.TryGetFieldById(fieldId, out Field field))
    Console.WriteLine(field.GetValue<ValueString>().String);

// Получить поля по типу
List<Field> textFields = task.GetFieldsByType(FieldTypes.Text);
if (task.TryGetFieldsByType(FieldTypes.Catalog, out List<Field> catFields))
    foreach (var f in catFields)
        Console.WriteLine(f.GetValue<ValueCatalog>().ItemId);
```

### Таблица типов полей → IValue

| FieldTypes | IValue class | Доступ к значению |
|---|---|---|
| Text, Phone, Time, Note, Email | `ValueString` | `.String` |
| Date, DueDate, DueDateTime, CreationDate | `ValueDate` | `.Date` |
| Money, Number | `ValueNumber` | `.Number` (decimal) |
| Step | `ValueIntegerNumber` | `.IntegerNumber` (int) |
| Project | `ValueProject` | `.Projects` |
| FormLink | `ValueFormLink` | `.TaskIds` |
| Title | `ValueTitle` | `.Checkmark`, `.Fields` |
| MultipleChoice | `ValueMultipleChoice` | `.ChoiceIds`, `.ChoiceNames`, `.Fields` |
| Table | `ValueTable` | `.Rows[].Cells[]` |
| Author, Person | `ValuePersone` | `.Id`, `.FirstName`, `.Email` |
| File | `ValueFiles` | `.Files[]` (.Id, .Name, .Url) |
| Catalog | `ValueCatalog` | `.ItemId`, `.ItemIds`, `.Values` |
| Checkmark, Flag | `ValueCheckmark` | `.Checkmark` (enum) |
| Status | `ValueStatus` | `.Status` (enum) |

### ValueFieldData — значения для записи

```csharp
new ValueFieldData(fieldId, "строка")
new ValueFieldData(fieldId, 123)
new ValueFieldData(fieldId, 99.50m)
new ValueFieldData(fieldId, new ValueChoiceData(choiceId))
new ValueFieldData(fieldId, new ValueItemData(catalogItemId))
new ValueFieldData(fieldId, new ValueIdData(personId))
new ValueFieldData(fieldId, new ValueEmailData("user@mail.com"))
new ValueFieldData(fieldId, new ValueFormLinkData(new List<long> { taskId }))
new ValueFieldData(fieldId, new ValueTitleData(new List<ValueFieldData> { ... }))
new ValueFieldData(fieldId, new ValueMultipleChoiceData(choiceIds, fields))
new ValueFieldData(fieldId, new List<ValueRowData> { new ValueRowData(0, cells) })
new ValueFieldData(fieldId, DateTime.Now, DateTimeFormatTypes.Full)
new ValueFieldData(fieldId, CheckmarkTypes.Checked)
new ValueFieldData(fieldId, StatusTypes.Open)
```

## 6. Webhook

```csharp
// Проверка подписи (HMAC-SHA1)
bool valid = apiClient.CheckSig(requestBody, xPyrusSigHeader, botSecret);

// Десериализация webhook-события
var update = JsonConvert.DeserializeObject<PyrusUpdates>(requestBody);
PyrusTask task = update.Task;
long taskId = update.TaskId;
```

## 7. JSON-сериализация

```csharp
using ApiPyrus.Extentions;

// Любой объект → JSON (NullValueHandling.Ignore, DefaultValueHandling.Ignore)
string json = task.GetJson();
string json = task.Fields.GetJson(Formatting.None);
string json = entity.GetJson(new JsonSerializerSettings { ... });
```

## 8. Enums

| Enum | Values |
|---|---|
| `ChannelTypes` | Email, Telegram, Facebook, Vkontakte, Viber, Instagram, PrivateChannel, WhatsApp, WebWidget, MobileApp, SMS |
| `ActionTypes` | Finished, Reopened |
| `ApprovalTypes` | Approved, Acknowledged, Rejected, Revoked |
| `CheckmarkTypes` | None, Checked, Unchecked |
| `StatusTypes` | Open, Closed |
| `FieldTypes` | Text, Phone, Money, Number, Date, DueDate, Catalog, Checkmark, Flag, Table, Person, Author, File, ... |
| `DateTimeFormatTypes` | Full, DateOnly, TimeOnly |
