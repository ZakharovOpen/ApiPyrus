```.NET Framework 4.7.2```
[![NuGet](https://zakharovopen.ru/webapp/ApiPyrus.svg)](https://www.nuget.org/packages/ApiPyrus/2.0.3)<br />
 ```.NET 6.0```
[![NuGet](https://zakharovopen.ru/webapp/ApiPyrus_net6.svg)](https://www.nuget.org/packages/ApiPyrus/3.0.3)
# ApiPyrus
This is C# Pyrus API client. This library allows to use all available API methods.
## Install
#### .NET Framework 4.7.2
``` bash
PM> NuGet\Install-Package ApiPyrus -Version 2.0.3
```
#### .NET 6.0
``` bash
PM> NuGet\Install-Package ApiPyrus -Version 3.0.3
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
PyrusCatalogs catalogs = apiClient.GetСatalogs();
```
## Create catalog request
```C#
using ApiPyrus.Models.DTOs;
using ApiPyrus.Models.Methods.Catalogs;
...
Catalog newCatalog = new CreateCatalog("Created new catalog").AddHeaders(new List<string>() { "Name", "LastName" }).AddItems(new List<ValuesList>() { new ValuesList() { Values = new List<string>() { "Pavel", "Zakharov" } } }).Send(apiClient);
```
![image](https://user-images.githubusercontent.com/88644943/217810505-cef36e03-332f-46ee-a0c5-c93ecb6aa81c.png)


## Attachments
```C#
using ApiPyrus.Models.DTOs;
...
bool success = apiClient.DownloadFiles("https://pyrus.com/services/attachment?id=12345678", "C:\\Files\\File1.png");
Guid attachmentId = await apiClient.UploadDataAsync("C:\\Files\\File1.png");
```
## Get tasks request
```C#
using ApiPyrus.Models.DTOs;
...
PyrusTasks pyrusTasks = apiClient.GetTasks(123456, "?fld4=343&fld10=79991112233");
```
## Create task
```C#
using ApiPyrus.Models.DTOs;
using ApiPyrus.Models.Methods.Tasks;
...
 PyrusTask createdTask = new CreateTaskByForm(12345).AddField(new ValueField(1, new ValueChoice(5))).AddTasksIds(new List<int> { 1, 2, 3}).Send(apiClient);
```
## Update task
```C#
using ApiPyrus.Models.DTOs;
using ApiPyrus.Models.Methods.Tasks;
...
 PyrusTask updatedTask = new UpdateTaskByForm(12345).UpdateField(new ValueField(5, new ValueChoice(2))).AddText("Text").Send(apiClient);
```

P.S. There are also methods for working with simple tasks, members, catalogs, announcements, roles. Located in the "ApiPyrus.Models.Methods" namespace and in the "apiClient" instance. "ValueField" can be different object, details https://pyrus.com/en/help/api/fields .
