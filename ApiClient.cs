using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using ApiPyrus.Models.Methods.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApiPyrus
{
    public class ApiClient : IDisposable
    {
        public ApiClient(string login, string key, string url = "https://api.pyrus.com", string apiVersion = "v4", TimeSpan? requestsTimeOut = null, bool ignoreSslSecurityErros = false)
        {
            _pyrusApiLogin = login;
            _pyrusApiKey = key;
            _url = url;
            _apiVersion = apiVersion;
            _requestsTimeout = requestsTimeOut;
            if (ignoreSslSecurityErros)
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.DefaultConnectionLimit = 9999;
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                _httpClient = new HttpClient(handler);
            }
            else
                _httpClient = new HttpClient();
            if (requestsTimeOut.HasValue)
                _httpClient.Timeout = requestsTimeOut.Value;
        }

        public delegate void StringEvent(string message);
        public event StringEvent GotInfoLog;
        public event StringEvent GotErrorLog;
        private string _url { get; }
        private string _token { get; set; }
        private string _pyrusApiLogin { get; }
        private string _pyrusApiKey { get; }
        private string _apiVersion { get; }
        private TimeSpan? _requestsTimeout { get; }
        private HttpClient _httpClient;

        /// <summary>
        /// Проверить подпись запроса
        /// </summary>
        public bool CheckSig(string msg, string sig, string secret, string extRequestId = "")
        {
            GotInfoLog?.Invoke($"{(string.IsNullOrEmpty(extRequestId) ? "" : $"[{extRequestId}] ")}Try check sign...");
            byte[] hashValue;
            using (HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secret)))
            using (var outStream = new MemoryStream())
            {
                hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(msg));
                outStream.Write(hashValue, 0, hashValue.Length);
            }
            var hashString = hashValue.ToHexString();
            if (hashString == sig)
            {
                GotInfoLog?.Invoke($"{(string.IsNullOrEmpty(extRequestId) ? "" : $"[{extRequestId}] ")}Signature verified successfully.");
                return true;
            }
            GotErrorLog?.Invoke($"{(string.IsNullOrEmpty(extRequestId) ? "" : $"[{extRequestId}] ")}Signature verification error.");
            return false;
        }

        private async Task<string> ApiRequest(string additionalUrl, string method = "GET", string body = null, bool isAuthRequest = false, string externalRequestId = "")
        {
            if (!isAuthRequest && string.IsNullOrEmpty(_token))
                await Auth();
            var returnText = string.Empty;

            using (var request = new HttpRequestMessage(new HttpMethod(method), _url + additionalUrl))
            {
                if (!isAuthRequest)
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + _token);
                if (!string.IsNullOrEmpty(body))
                {
                    var content = new StringContent(body, Encoding.UTF8, "application/json");
                    request.Content = content;
                }
                GotInfoLog?.Invoke($"{(string.IsNullOrEmpty(externalRequestId) ? "" : $"[{externalRequestId}]")}[{method}]{additionalUrl} {(string.IsNullOrEmpty(_token) ? "" : $"\n Token: {_token}")} {(string.IsNullOrEmpty(body) ? "" : $"\n Body: {body}")}");
                using (var response = await _httpClient.SendAsync(request, 0))
                {
                    returnText = await response.Content.ReadAsStringAsync();
                    if (!isAuthRequest && response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Auth();
                        returnText = await ApiRequest(additionalUrl, method, body, false, externalRequestId);
                    }
                    else if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var mess = ((int)response.StatusCode).ToString() + (response.ReasonPhrase != null ? ". " + response.ReasonPhrase : "");
                        GotErrorLog?.Invoke(mess + ". " + returnText);
                        throw new Exception(mess + ". " + returnText);
                    }
                    GotInfoLog?.Invoke($"{(string.IsNullOrEmpty(externalRequestId) ? "" : $"[{externalRequestId}] ")}{returnText}");
                }
            }
            return returnText;

        }
        private async Task Auth(string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/auth?login={_pyrusApiLogin}&security_key={_pyrusApiKey}", isAuthRequest: true, externalRequestId: extRequestId);
            _token = JsonConvert.DeserializeObject<Token>(responseString).AccessToken;
        }
        public void ChangeToken(string token)
            => _token = token;


        /// <summary>
        /// Получить шаблоны всех форм
        /// </summary>
        public async Task<List<Form>> GetFormsTemplates(string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/forms", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<FormsTemplates>(responseString)?.Forms;
        }

        /// <summary>
        /// Получить все справочники
        /// </summary>
        public async Task<List<Catalog>> GetCatalogs(string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/catalogs", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<PyrusCatalogs>(responseString)?.Catalogs;
        }

        /// <summary>
        /// Получить справочник по id
        /// </summary>
        public async Task<Catalog> GetCatalog(long catalogId, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/catalogs/{catalogId}", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<Catalog>(responseString);
        }

        /// <summary>
        /// Создать справочник
        /// </summary>
        internal async Task<Catalog> CreateCatalog(string json, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/catalogs", "PUT", json, externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<Catalog>(responseString);
        }

        /// <summary>
        /// Обновить справочник по id
        /// </summary>
        internal async Task<CatalogUpdateInfo> UpdateCatalog(long catalogId, string json, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/catalogs/{catalogId}", "POST", json, externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<CatalogUpdateInfo>(responseString);
        }

        /// <summary>
        /// Обновить справочник по id
        /// </summary>
        internal async Task<CatalogUpdateInfo> UpdateCatalogDiff(long catalogId, string json, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/catalogs/{catalogId}/diff", "POST", json, externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<CatalogUpdateInfo>(responseString);
        }

        /// <summary>
        /// Получить всех сотрудников
        /// </summary>
        public async Task<List<ValuePersone>> GetMembers(string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/members", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<MembersList>(responseString)?.Members;
        }

        /// <summary>
        /// Получить сотрудника по id
        /// </summary>
        public async Task<ValuePersone> GetMember(long memberId, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/members/{memberId}", "DELETE", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<ValuePersone>(responseString);
        }

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        internal async Task<ValuePersone> AddMembers(string json, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/members", "POST", json, externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<ValuePersone>(responseString);
        }


        /// <summary>
        /// Обновить сотрудника по id
        /// </summary>
        internal async Task<ValuePersone> UpdateMembers(long memberId, string json, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/members/{memberId}", "PUT", json, externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<ValuePersone>(responseString);
        }

        /// <summary>
        /// Удалить сотрудника по id
        /// </summary>
        internal async Task<ValuePersone> BlockMember(long memberId, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/members/{memberId}", "DELETE", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<ValuePersone>(responseString);
        }

        /// <summary>
        /// Получить профиль текущего пользователя
        /// </summary>
        public async Task<ValuePersone> GetProfile(bool includeInactive = false, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/profile?include_inactive={includeInactive}", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<ValuePersone>(responseString);
        }

        /// <summary>
        /// Создать задачу
        /// </summary>
        internal async Task<PyrusTask> CreateTask(string json, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/tasks", "POST", json, externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<PyrusTaskInfo>(responseString)?.Task;
        }

        /// <summary>
        /// Получить задачи по форме GET запрос
        /// </summary>
        /// <param name="formId"> Id формы </param>>
        /// <param name="query"> Параметры запроса. Пример: ?fld2=gt10000,lt15000ampfld1=IT%20conference%20in%20Amsterdamampfld3=277ampinclude_archived=y</param>>
        /// <param name="extRequestId"> Параметры запроса, внешний id запроса </param>>
        public async Task<List<PyrusTask>> GetTasks(long formId, string query, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/forms/{formId}/register{query}", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<PyrusTasks>(responseString)?.Tasks;
        }

        ///// <summary>
        ///// Получить задачи по форме POST запрос
        ///// </summary>
        ///// <param name="formId"> Id формы </param>>
        ///// <param name="bodyQueryParams"> Параметры запроса. Пример: Dictionary&lt;string, object&gt;() { {"fld22", "6565"}, {"include_archived", "y"}} </param>>
        ///// <param name="extRequestId"> Параметры запроса, внешний id запроса </param>>
        //public async Task<List<PyrusTask>> GetTasks(long formId, Dictionary<string, object> bodyQueryParams, string extRequestId = "")
        //{
        //    foreach (var bodyQueryParam in bodyQueryParams)
        //    {
        //        if(bodyQueryParam.)
        //            throw new ArgumentException($"Значение для ключа '{param.Key}' должно быть типа int или decimal.");
        //    }
        //    var responseString = await ApiRequest($"/{_apiVersion}/forms/{formId}/register", "POST", JsonConvert.SerializeObject(bodyQueryParams), externalRequestId: extRequestId);
        //    return JsonConvert.DeserializeObject<PyrusTasks>(responseString)?.Tasks;
        //}

        /// <summary>
        /// Получить задачи по форме
        /// </summary>
        /// <param name="formId"> Id формы </param>>
        /// <param name="queryParams"> Параметры запроса. Пример: Dictionary&lt;string, object&gt;() { {"fld22", "6565"}, {"include_archived", "y"}} </param>>
        /// <param name="extRequestId"> Параметры запроса, внешний id запроса </param>>
        public async Task<List<PyrusTask>> GetTasks(long formId, Dictionary<object, object> queryParams = null, string extRequestId = "")
        {
            var responseString = string.Empty;
            var query = string.Empty;
            if (queryParams != null && queryParams.Count > 0)
            {
                var intParams = queryParams?.Where(x => int.TryParse(x.Key.ToString(), out var y))?.ToList();
                if (intParams != null && intParams.Count > 0)
                    query = "?" + string.Join("&", intParams.Select(x => $"{x.Key}={x.Value}"));

                var strParams = queryParams?.Where(x => !intParams.Any(y => y.Key == x.Key))?.ToList();
                if (strParams != null && strParams.Count > 0)
                    query += (string.IsNullOrEmpty(query) ? "?" : "&") + string.Join("&", strParams.Select(x => $"{x.Key}={x.Value}"));
            }
            return await GetTasks(formId, query, extRequestId);
        }

        /// <summary>
        /// Получить задачи по форме
        /// </summary>
        /// <param name="formId"> Id формы </param>>
        /// <param name="fieldsQueryParams"> Параметры запроса, id полей формы можно передвать. Пример: Dictionary&lt;int, string&gt;() { {22, "6565"}} </param>>
        /// <param name="includeArchived"> Параметры запроса, вклюать архивированные задачи или нет </param>>
        /// <param name="extRequestId"> Параметры запроса, внешний id запроса </param>>
        public async Task<List<PyrusTask>> GetTasks(long formId, Dictionary<int, string> fieldsQueryParams = null, bool? includeArchived = null, string extRequestId = "")
        {
            var responseString = string.Empty;
            var query = string.Empty;
            if (fieldsQueryParams != null && fieldsQueryParams.Count > 0)
            {
                var intParams = fieldsQueryParams?.Where(x => int.TryParse(x.Key.ToString(), out var y))?.ToList();
                if (intParams != null && intParams.Count > 0)
                    query = "?" + string.Join("&", intParams.Select(x => $"fld{x.Key}={x.Value}"));

                var strParams = fieldsQueryParams?.Where(x => !intParams.Any(y => y.Key == x.Key))?.ToList();
                if (strParams != null && strParams.Count > 0)
                    query += (string.IsNullOrEmpty(query) ? "?" : "&") + string.Join("&", strParams.Select(x => $"{x.Key}={x.Value}"));
            }
            if(includeArchived.HasValue)
            {
                query += (string.IsNullOrEmpty(query) ? "?" : "&") + $"include_archived=y";
            }
            return await GetTasks(formId, query, extRequestId);
        }

        /// <summary>
        /// Получить задачe по id
        /// </summary>
        public async Task<PyrusTask> GetTaskInfoById(long taskId, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/tasks/{taskId}", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<PyrusTaskInfo>(responseString)?.Task;
        }

        /// <summary>
        /// Закрыть или прокомментировать задачу по id
        /// </summary>
        internal async Task<PyrusTask> CloseOrCommentTask(long taskId, string json, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/tasks/{taskId}/comments", "POST", json, externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<PyrusTaskInfo>(responseString)?.Task;
        }

        /// <summary>
        /// Получить роли сотрудников
        /// </summary>
        public async Task<List<Role>> GetRoles(string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/roles", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<Roles>(responseString)?.RolesList;
        }

        /// <summary>
        /// Создать роль сотрудников
        /// </summary>
        internal async Task<Role> CreateRole(string json, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/roles", "POST", json, externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<Role>(responseString);
        }

        /// <summary>
        /// Обновть роль сотрудников по id
        /// </summary>
        internal async Task<Role> UpdateRole(long roleId, string json, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/roles/{roleId}", "PUT", json, externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<Role>(responseString);
        }

        /// <summary>
        /// Получить списки
        /// </summary>
        public async Task<List<PyrusList>> GetLists(string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/lists", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<PyrusLists>(responseString)?.Lists;
        }

        /// <summary>
        /// Получить задачи по id списки
        /// </summary>
        public async Task<List<PyrusTask>> GetListTasks(long listId, DateTime from, DateTime to, int itemCount = 100, bool includeInactive = true, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/lists/{listId}/tasks?item_count={itemCount}&include_archived={includeInactive}&modified_before={to.ToString("yyyy-MM-ddTHH:mm:ssZ")}&modified_after={from.ToString("yyyy-MM-ddTHH:mm:ssZ")}", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<PyrusTasks>(responseString)?.Tasks;
        }

        /// <summary>
        /// Получить задачи из входящих
        /// </summary>
        public async Task<List<PyrusTask>> GetInbox(int itemCount = 100, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/inbox?item_count={itemCount}", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<PyrusTasks>(responseString)?.Tasks;
        }

        /// <summary>
        /// Создать объявление 
        /// </summary>
        internal async Task<AnnouncementInfo> CreateAnnouncement(string json, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/announcements", "POST", json, externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<Announcement>(responseString)?.AnnouncementInfo;
        }

        /// <summary>
        /// Прокомментировать объявление 
        /// </summary>
        internal async Task<AnnouncementInfo> CommentAnnouncement(string json, long announcementId, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/announcements/{announcementId}/comments", "POST", json, externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<Announcement>(responseString)?.AnnouncementInfo;
        }

        /// <summary>
        /// Получить объявление по id 
        /// </summary>
        public async Task<AnnouncementInfo> GetAnnouncement(long announcementId, string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/announcements/{announcementId}", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<Announcement>(responseString)?.AnnouncementInfo;
        }

        /// <summary>
        /// Получить все объявления
        /// </summary>
        public async Task<List<AnnouncementInfo>> GetAnnouncements(string extRequestId = "")
        {
            var responseString = await ApiRequest($"/{_apiVersion}/announcements", externalRequestId: extRequestId);
            return JsonConvert.DeserializeObject<Announcements>(responseString)?.AnnouncementsList;
        }

        /// <summary>
        ///Запланировать задачу на указанную дату. Задача вернется во входящие в 7 утра во временной зоне клиента.
        ///Формат: YYYY-MM-DD.
        ///!!! Необходимо создаеть apiClient с помощью даннные пользователя !!!
        /// </summary>
        public async Task<PyrusTask> AddScheduledDate(long taskId, string scheduledDate)
        {
            return await new UpdateTaskByForm(taskId).ScheduledDatetimeUtc(scheduledDate).Send(this);
        }

        /// <summary>
        /// Запланировать задачу на указанное время в нулевой временной зоне.
        /// Формат: YYYY-MM-DDThh:mm:ssZ
        /// !!! Используется, если задача создается через api даннные пользователя, в противном случае воспользоваться методом SheduleDate api клиента.
        /// </summary>
        public async Task<PyrusTask> AddScheduledDatetimeUtc(long taskId, string scheduledDatetimeUtc)
        {
            return await new UpdateTaskByForm(taskId).ScheduledDatetimeUtc(scheduledDatetimeUtc).Send(this);
        }

        /// <summary>
        /// Отменить планирование. Задача вернется во входящие.
        /// </summary>
        public async Task<PyrusTask> CancelSchedule(long taskId)
        {
            return await new UpdateTaskByForm(taskId).CancelSchedule().Send(this);
        }

        /// <summary>
        /// Скачать файл из Pyrus
        /// </summary>
        public async Task<bool> DownloadFile(string attachmentUrl, string fileFullName)
        {
            using (Stream inputStream = await DownloadFile(attachmentUrl))
            using (FileStream fileStream = File.OpenWrite(fileFullName))
            {
                await inputStream.CopyToAsync(fileStream);
                return true;
            }
        }

        /// <summary>
        /// Скачать файл из Pyrus
        /// </summary>
        public async Task<Stream> DownloadFile(string attachmentUrl)
        {
            if (string.IsNullOrEmpty(_token))
                await Auth();
            using (var request = new HttpRequestMessage(HttpMethod.Get, attachmentUrl))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Auth();
                    return await DownloadFile(attachmentUrl);
                }
                else if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync();
                }
                else
                {
                    var mess = ((int)response.StatusCode).ToString() + (response.ReasonPhrase != null ? ". " + response.ReasonPhrase : "");
                    throw new Exception(mess + ". " + await response.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Загрузить фото в Pyrus
        /// </summary>
        public async Task<Guid> UploadFile(string fullFileName)
        {
            Guid guid = new Guid();
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{_url}/{_apiVersion}/files/upload"))
            {
                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + _token);
                var multipartContent = new MultipartFormDataContent
                {
                    { new ByteArrayContent(File.ReadAllBytes(fullFileName)), Path.GetFileName(fullFileName), Path.GetFileName(fullFileName) }
                };
                request.Content = multipartContent;
                var response = await _httpClient.SendAsync(request, 0);
                string body = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Auth();
                    return await UploadFile(fullFileName);
                }
                else if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(body);
                var uploadedFile = JsonConvert.DeserializeObject<UploadedFile>(body);
                guid = uploadedFile.Id;
            }

            return guid;
        }

        public void Dispose()
            => _httpClient?.Dispose();
    }
}