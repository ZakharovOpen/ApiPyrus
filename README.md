 ```.NET STANDART 2.0```
[![NuGet](https://zakharovopen.ru/imgs/ApiPyrus_net_standart.svg)](https://www.nuget.org/packages/ApiPyrus/4.6.1)
# ApiPyrus
This is C# Pyrus API client. This library allows to use all available API methods.
## Install
#### .NET Standart 2.0
``` bash
PM> NuGet\Install-Package ApiPyrus -Version 4.6.1
```
## Create instance
```C#
using ApiPyrus;
...
ApiClient apiClient = new ApiClient("login", "apiKey");
```
## Get catalogs request
```C#
using ApiPyrus.Models.DTOs;
...
PyrusCatalogs catalogs = await apiClient.GetСatalogs();
```
## Create catalog request
```C#
using ApiPyrus.Models.DTOs;
using ApiPyrus.Models.Methods.Catalogs;
...
Catalog newCatalog = await  new CreateCatalog("Created new catalog").AddHeaders(new List<string>() { "Name", "LastName" }).AddItems(new List<ValuesList>() { new ValuesList() { Values = new List<string>() { "Pavel", "Zakharov" } } }).Send(apiClient);
```
![image](https://user-images.githubusercontent.com/88644943/217810505-cef36e03-332f-46ee-a0c5-c93ecb6aa81c.png)


## Attachments
```C#
using ApiPyrus.Models.DTOs;
...
bool success = await apiClient.DownloadFiles("https://pyrus.com/services/attachment?id=12345678", "C:\\Files\\File1.png");
Guid attachmentId = await apiClient.UploadDataAsync("C:\\Files\\File1.png");
```
## Get tasks request
```C#
using ApiPyrus.Models.DTOs;
...
PyrusTasks pyrusTasks = await apiClient.GetTasks(123456, "?fld4=343&fld10=79991112233");
```
## Create task
```C#
using ApiPyrus.Models.DTOs;
using ApiPyrus.Models.Methods.Tasks;
...
 PyrusTask createdTask = await new CreateTaskByForm(12345).AddField(new ValueField(1, new ValueChoice(5))).AddTasksIds(new List<int> { 1, 2, 3}).Send(apiClient);
```
## Update task
```C#
using ApiPyrus.Models.DTOs;
using ApiPyrus.Models.Methods.Tasks;
...
 PyrusTask updatedTask = await new UpdateTaskByForm(12345).UpdateField(new ValueField(5, new ValueChoice(2))).AddText("Text").Send(apiClient);
```
## Field
Field value can be differents objects, details https://pyrus.com/en/help/api/fields.
The field has the following methods to get it:
```C#
public T GetValue<T>() where T : IValue
public IValue GetValueObject()
```
## IValue
ValueClasses for fields types [enum FieldTypes]:
 - ValueString [Text, Phone, Time, Note, Email]
 - ValueDate [DueDate, CreationDate, Date, DueDateTime]
 - ValueNumber [Money, Number]
 - ValueProject [Project]
 - ValueFormLink [FormLink]
 - ValueTitle [Title]
 - ValueMultipleChoice [MultipleChoice]
 - ValueTable [Table]
 - ValuePersone [Author, Person]
 - ValueFiles [File]
 - ValueCatalog [Catalog]
 - ValueCheckmark [Checkmark, Flag]
 - ValueIntegerNumber [Step]
 - ValueStatus [Status]
 If a conversion error occurs, then 'ValueError'

## Extentions
Extension methods have been added to the library.
```C#
 public static string GetJson<T>(this T entityForJson, JsonSerializerSettings settings)
 public static string GetJson<T>(this T entityForJson, Formatting jsonFormatting = Formatting.Indented, NullValueHandling nullValueHandling = NullValueHandling.Ignore, DefaultValueHandling defaultValueHandling = DefaultValueHandling.Ignore, ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Ignore)
 
 public static Field GetFieldById(this PyrusTask task, long fieldId)
 public static Field GetFieldById(this List<Field> fields, long fieldId)
 public static bool TryGetFieldById(this PyrusTask task, long fieldId, out Field field)
 public static bool TryGetFieldById(this List<Field> fields, long fieldId, out Field field)

 public static List<Field> GetFieldsByType(this PyrusTask task, FieldTypes type)
 public static List<Field> GetFieldsByType(this List<Field> fields, FieldTypes type)
 public static bool TryGetFieldsByType(this PyrusTask task, FieldTypes type, out List<Field> fields)
 public static bool TryGetFieldsByType(this List<Field> fields, FieldTypes type, out List<Field> fieldsByType)
        

```
Example:
```C#
using ApiPyrus.Extentions;
...
  PyrusTask task = new PyrusTask();
  var taskJson = task.GetJson();
  var fieldsJson = task.Fields.GetJson(Formatting.None);
...
  if(task.Fields.TryGetFieldById(3, out Field field))
  {
     Console.WriteLine(field.GetValue<ValueString>());
  }
...
  if (task.Fields.TryGetFieldsByType(FieldTypes.Text, out List<Field> fields))
  {
    foreach (var field in fields)
    {
        Console.WriteLine(field.GetValue<ValueString>());
    }
  }
```

## Example of field parsing
```C#
async Task Generate()
{
    var tasks = await apiClient.GetTasks(1111, string.Empty);
    var tasksInfo = new List<Dictionary<string, string>>();
    foreach (var task in tasks)
    {
        var fieldsInfo = new Dictionary<string, string>()
        {
            { "TaskId", task.Id.ToString() },
            { "Created", task.CreateDate.ToString() },
            { "Closed", task.CloseDate.ToString() }
        };
        foreach (var field in task.Fields)
            GetFieldInfo(field, task.Id, fieldsInfo);

        tasksInfo.Add(fieldsInfo);
    }
}

```
```C#
void GetFieldInfo(Field field, long taskId, Dictionary<string, string> fieldsInfo)
{
    try
    {
        if (field.Value == null)
        {
            fieldsInfo.Add(field.Name, string.Empty);
            Console.WriteLine($"Task '{taskId}' field '{field.Name}' value '{field.Type}' is null");
            return;
        }

        switch (field.Type)
        {
            case FieldTypes.Note:
            case FieldTypes.Text:
            case FieldTypes.Phone:
            case FieldTypes.Time:
            case FieldTypes.Email:
                fieldsInfo.Add(field.Name, field.GetValue<ValueString>().String);
                break;
            case FieldTypes.DueDate:
            case FieldTypes.CreationDate:
            case FieldTypes.Date:
            case FieldTypes.DueDateTime:
                fieldsInfo.Add(field.Name, field.GetValue<ValueDate>().Date.ToString());
                break;
            case FieldTypes.Money:
            case FieldTypes.Number:
                fieldsInfo.Add(field.Name, field.GetValue<ValueNumber>().Number.ToString());
                break;
            case FieldTypes.Project:
                var value = new List<string>();

                foreach (var project in field.GetValue<ValueProject>()?.Projects)
                    value.Add(project.Name);

                fieldsInfo.Add(field.Name, string.Join(", ", value));
                break;
            case FieldTypes.FormLink:
                fieldsInfo.Add(field.Name, string.Join(", ", field.GetValue<ValueFormLink>().TaskIds));
                break;
            case FieldTypes.Title:
                var title = field.GetValue<ValueTitle>();
                if (title.Fields != null && title.Fields.Any())
                {
                    foreach (var titleField in title.Fields)
                        GetFieldInfo(titleField, taskId, fieldsInfo);
                }
                break;
            case FieldTypes.MultipleChoice:
                var multipleChoice = field.GetValue<ValueMultipleChoice>();
                fieldsInfo.Add(field.Name, string.Join(", ", multipleChoice.ChoiceNames));
                if (multipleChoice.Fields != null && multipleChoice.Fields.Any())
                    foreach (var multipleChoiceField in multipleChoice.Fields)
                        GetFieldInfo(multipleChoiceField, taskId, fieldsInfo);
                break;
            case FieldTypes.Table:
                var table = field.GetValue<ValueTable>();
                foreach (var row in table.Rows)
                {
                    foreach (var cell in row.Cells) 
                        GetFieldInfo(cell, taskId, fieldsInfo);
                }
                break;
            case FieldTypes.Author:
            case FieldTypes.Person:
                var person = field.GetValue<ValuePersone>();
                fieldsInfo.Add(field.Name, $"{person.LastName} {person.FirstName}");
                break;
            case FieldTypes.File:
                var files = field.GetValue<ValueFiles>();
                fieldsInfo.Add(field.Name, string.Join(", ", field.GetValue<ValueFiles>().Files.Select(x => x.Name)));
                break;
            case FieldTypes.Catalog:
                fieldsInfo.Add(field.Name, string.Join(", ", field.GetValue<ValueCatalog>().Values));
                break;
            case FieldTypes.Checkmark:
            case FieldTypes.Flag:
                fieldsInfo.Add(field.Name, field.GetValue<ValueCheckmark>().Checkmark.ToString());
                break;
            case FieldTypes.Step:
                fieldsInfo.Add(field.Name, field.GetValue<ValueIntegerNumber>().IntegerNumber.ToString());
                break;
            case FieldTypes.Status:
                fieldsInfo.Add(field.Name, field.GetValue<ValueStatus>().Status.ToString());
                break;
            case FieldTypes.None:
            default:
                {
                    fieldsInfo.Add(field.Name, string.Empty);
                    var error = field.GetValue<ValueError>().ErrorMessage.ToString();
                    Console.WriteLine($"Task '{taskId}' field '{field.Name}' parse value '{field.Type}' exeption:\n{error}");
                    break;
                }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Task '{taskId}' field '{field.Name}'[{field.Id}] parse value '{field.Type}' exeption:\n{ex}");
        if(!fieldsInfo.ContainsKey(field.Name))
            fieldsInfo.Add(field.Name, string.Empty);
    }
}
```
P.S. There are also methods for working with simple tasks, members, catalogs, announcements, roles. Located in the "ApiPyrus.Models.Methods" namespace and in the "apiClient" instance.
