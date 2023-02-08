```.NET Framework 4.7.2```
[![NuGet](https://zakharovopen.ru/webapp/ApiPyrus.svg)](https://www.nuget.org/packages/ApiPyrus/2.0.2)<br />
 ```.NET 6.0```
[![NuGet](https://zakharovopen.ru/webapp/ApiPyrus_net6.svg)](https://www.nuget.org/packages/ApiPyrus/3.0.1)
# ApiPyrus
This is C# Pyrus API client. This library allows to use all available API methods.
## Install
#### .NET Framework 4.7.2
``` bash
PM> NuGet\Install-Package ApiPyrus -Version 2.0.2
```
#### .NET 6.0
``` bash
PM> NuGet\Install-Package ApiPyrus -Version 3.0.1
```
## Create instance
```C#
using ApiPyrus;
...
ApiClient apiClient = new ApiClient("login", "apiKey");
```
## Create get tasks request
```C#
using ApiPyrus.Models.DTOs;
...
PyrusTasks pyrusTasks = apiClient.GetTasks(123456, "?fld4=343&fld10=79991112233");
```
## Create get catalogs request
```C#
using ApiPyrus.Models.DTOs;
...
PyrusCatalogs catalogs = apiClient.GetСatalogs();
```
## Attachments
```C#
using ApiPyrus.Models.DTOs;
...
bool success = apiClient.DownloadFiles("https://pyrus.com/services/attachment?id=12345678", "C:\\Files\\File1.png");
Guid attachmentId = await apiClient.UploadDataAsync("C:\\Files\\File1.png");
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

P.S. There are also methods for working with simple tasks, members, catalogs, announcements, roles. Located in the "ApiPyrus.Models.Methods" namespace and in the "apiClient" instance
