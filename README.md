`.NET Standard 2.0`
[![NuGet](https://zakharovopen.ru/imgs/ApiPyrus_net_standart.svg)](https://www.nuget.org/packages/ApiPyrus/)

# ApiPyrus

C# client for [Pyrus REST API](https://pyrus.com/en/help/api). Covers tasks, catalogs, members, roles, announcements, files, lists, and webhooks.

## Install

```
PM> NuGet\Install-Package ApiPyrus
```

## Quick start

```csharp
using ApiPyrus;
using ApiPyrus.Models.DTOs;

using var apiClient = new ApiClient("login@company.com", "your-api-key");

// Read tasks
List<PyrusTask> tasks = await apiClient.GetTasks(formId, "?fld4=value");

// Create a task
using ApiPyrus.Models.Methods.Tasks;

PyrusTask task = await new CreateTaskByForm(formId)
    .AddField(new ValueFieldData(1, "Hello"))
    .AddField(new ValueFieldData(2, new ValueChoiceData(5)))
    .Send(apiClient);
```

Authentication is automatic &mdash; a Bearer token is obtained on the first request and refreshed on 401.

## Configuration

```csharp
var apiClient = new ApiClient(
    login: "login@company.com",
    key: "your-api-key",
    url: "https://api.pyrus.com",          // custom for on-premise
    apiVersion: "v4",
    requestsTimeOut: TimeSpan.FromSeconds(30),
    ignoreSslSecurityErros: false           // true for self-signed certs
);

// Event-based logging (no ILogger dependency)
apiClient.GotInfoLog  += msg => Console.WriteLine(msg);
apiClient.GotErrorLog += msg => Console.Error.WriteLine(msg);
```

## Read methods

```csharp
// Tasks
PyrusTask task             = await apiClient.GetTaskInfoById(taskId);
List<PyrusTask> tasks      = await apiClient.GetTasks(formId, "?fld4=value&include_archived=y");
List<PyrusTask> tasks      = await apiClient.GetTasks(formId,
    new Dictionary<int, string> { { 4, "value" } }, includeArchived: true);
List<PyrusTask> inbox      = await apiClient.GetInbox(itemCount: 50);

// Forms, catalogs, members, roles, lists, announcements
List<Form> forms           = await apiClient.GetFormsTemplates();
List<Catalog> catalogs     = await apiClient.GetCatalogs();
Catalog catalog            = await apiClient.GetCatalog(catalogId);
List<ValuePersone> members = await apiClient.GetMembers();
ValuePersone profile       = await apiClient.GetProfile();
List<Role> roles           = await apiClient.GetRoles();
List<PyrusList> lists      = await apiClient.GetLists();
List<PyrusTask> listTasks  = await apiClient.GetListTasks(listId, from, to);
List<AnnouncementInfo> ann = await apiClient.GetAnnouncements();

// Files
Guid fileId  = await apiClient.UploadFile(@"C:\file.png");
bool ok      = await apiClient.DownloadFile(attachmentUrl, @"C:\out.png");
Stream stream = await apiClient.DownloadFile(attachmentUrl);
```

## Write methods (Fluent Builders)

All write operations use a fluent builder pattern: `new Builder(...).Method(...).Send(apiClient)`.

### Tasks

```csharp
using ApiPyrus.Models.Methods.Tasks;

// Create task by form
PyrusTask task = await new CreateTaskByForm(formId)
    .AddField(new ValueFieldData(1, "text value"))
    .AddField(new ValueFieldData(2, new ValueChoiceData(5)))
    .AddField(new ValueFieldData(3, new ValueItemData(catalogItemId)))
    .AddField(new ValueFieldData(4, DateTime.Now, DateTimeFormatTypes.DateOnly))
    .AddField(new ValueFieldData(5, 1500.50m))
    .AddField(new ValueFieldData(6, CheckmarkTypes.Checked))
    .AddSubscribers(new List<ValueIdData> { new ValueIdData(userId) })
    .AddApprovals(approvalSteps)
    .FillDefaults()
    .Send(apiClient);

// Update / comment
PyrusTask updated = await new UpdateTaskByForm(taskId)
    .AddText("Comment text")
    .UpdateField(new ValueFieldData(1, "new value"))
    .AddAttachments(new List<Guid> { fileGuid })
    .AddChannel(ChannelTypes.Email)
    .AddApprovalChoice(ApprovalTypes.Approved)
    .SkipNotification()
    .Send(apiClient);

// Close / Reopen (inherit all UpdateTaskByForm methods)
await new CloseTaskByForm(taskId).AddText("Done").Send(apiClient);
await new ReopenTaskByForm(taskId).AddText("Reopening").Send(apiClient);

// Simple tasks
PyrusTask simple = await new CreateSimpleTask()
    .AddSubject("Subject")
    .AddText("Description")
    .AddDueDate("2025-06-01")
    .AddResponsible(new ValueIdData(userId))
    .Send(apiClient);
```

### Catalogs

```csharp
using ApiPyrus.Models.Methods.Catalogs;

// Create
Catalog cat = await new CreateCatalog("My catalog")
    .AddHeaders(new List<string> { "Name", "Email" })
    .AddItem(new ValuesList(new List<string> { "Pavel", "p@mail.com" }))
    .Send(apiClient);

// Update (full replace)
var info = await new UpdateCatalog(catalogId, apiClient)
    .SetCatalog()   // fetches current state from API
    .AddItem(new ValuesList(new List<string> { "New", "Row" }))
    .Send();

// Update (diff: upsert + delete)
var diff = await new UpdateCatalogItems(catalogId, apiClient)
    .AddItemToUpsert(new ValuesList(new List<string> { "key", "val" }))
    .AddItemToDelete("old-key")
    .Send();
```

### Members & Roles

```csharp
using ApiPyrus.Models.Methods.Members;
using ApiPyrus.Models.Methods.Roles;

await new CreateMember("John", "Doe", "john@mail.com").Send(apiClient);
await new UpdateMember(memberId, firstName: "Jane").Send(apiClient);
await new BlockMember(memberId).Send(apiClient);

await new CreateRole("Managers", new List<long> { id1, id2 }).Send(apiClient);
await new UpdateRole(roleId, addMembersIds: new List<long> { id3 }).Send(apiClient);
```

### Announcements

```csharp
using ApiPyrus.Models.Methods.Announcements;

await new CreateAnnouncement().AddText("Hello everyone!").Send(apiClient);
await new CommentAnnouncement(annId).AddText("Comment").Send(apiClient);
```

## Formatted text

Pass `formatedText: true` to send HTML via the `formatted_text` field.

```csharp
await new UpdateTaskByForm(taskId)
    .AddText("<b>Important!</b> Status: <mark data-color=\"green\">Done</mark>", formatedText: true)
    .Send(apiClient);
```

Supported tags: `<b>`, `<i>`, `<s>`, `<code>`, `<br/>`, `<div data-type="heading">`, `<q>`, `<mark data-color="red|yellow|green|blue">`, `<ul><li>`, `<ol><li>`, `<a href="...">`, `<button>`.

Reply to a comment (quote is built automatically):

```csharp
await new UpdateTaskByForm(taskId)
    .ReplyComment(replyNoteId, "quoted text", "my reply")
    .Send(apiClient);
```

## Working with fields

```csharp
using ApiPyrus.Extentions;

// Get field by ID (searches nested Title / MultipleChoice fields too)
if (task.TryGetFieldById(fieldId, out Field field))
    Console.WriteLine(field.GetValue<ValueString>().String);

// Get fields by type
if (task.TryGetFieldsByType(FieldTypes.Text, out List<Field> fields))
    foreach (var f in fields)
        Console.WriteLine(f.GetValue<ValueString>().String);
```

### IValue types

| FieldTypes | IValue class | Property |
|---|---|---|
| Text, Phone, Time, Note, Email | `ValueString` | `.String` |
| Date, DueDate, DueDateTime, CreationDate | `ValueDate` | `.Date` |
| Money, Number | `ValueNumber` | `.Number` |
| Step | `ValueIntegerNumber` | `.IntegerNumber` |
| Project | `ValueProject` | `.Projects` |
| FormLink | `ValueFormLink` | `.TaskIds` |
| Title | `ValueTitle` | `.Checkmark`, `.Fields` |
| MultipleChoice | `ValueMultipleChoice` | `.ChoiceIds`, `.ChoiceNames` |
| Table | `ValueTable` | `.Rows[].Cells[]` |
| Author, Person | `ValuePersone` | `.Id`, `.FirstName`, `.Email` |
| File | `ValueFiles` | `.Files[]` |
| Catalog | `ValueCatalog` | `.ItemId`, `.Values` |
| Checkmark, Flag | `ValueCheckmark` | `.Checkmark` |
| Status | `ValueStatus` | `.Status` |

## Scheduling

```csharp
await apiClient.AddScheduledDate(taskId, "2025-06-01");           // date only
await apiClient.AddScheduledDatetimeUtc(taskId, "2025-06-01T10:00:00Z"); // UTC
await apiClient.CancelSchedule(taskId);
```

## Webhook signature verification

```csharp
bool valid = apiClient.CheckSig(requestBody, xPyrusSigHeader, botSecret);
var update = JsonConvert.DeserializeObject<PyrusUpdates>(requestBody);
```

## JSON serialization

```csharp
using ApiPyrus.Extentions;

string json = task.GetJson();                              // pretty, no nulls
string json = task.Fields.GetJson(Formatting.None);        // compact
string json = entity.GetJson(customJsonSerializerSettings);
```

## License

MIT
