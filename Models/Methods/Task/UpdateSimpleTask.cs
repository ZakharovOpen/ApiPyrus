using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Tasks
{
    public class UpdateSimpleTask
    {
        public UpdateSimpleTask(long taskId)
            => _taskId = taskId;

        private long _taskId;
        private PyrusRequestTask _entityForJson = new PyrusRequestTask();

        /// <summary>
        /// Отправить комментарий/ обновить задачу в Pyrus.
        /// </summary>
        public async Task<PyrusTask> Send(ApiClient apiClient, string extRequestId = "")
            => await apiClient.CloseOrCommentTask(this._taskId, _entityForJson.GetJson(), extRequestId);


        /// <summary>
        /// Обновить заголовок задачи. 
        /// </summary>
        public UpdateSimpleTask UpdateSubject(string subject)
        {
            _entityForJson.Subject = subject;
            return this;
        }

        /// <summary>
        /// Обновить срок задачи. Дата в формате YYYY-MM-DD.
        /// </summary>
        public UpdateSimpleTask UpdatDueDate(string dueDate)
        {
            _entityForJson.DueDate = dueDate;
            return this;
        }

        /// <summary>
        /// Обновить срок задачи со временем. Дата в формате YYYY-MM-DDThh:mm:ssZ.
        /// </summary>
        public UpdateSimpleTask AddDue(string dueDate)
        {
            _entityForJson.Due = dueDate;
            return this;
        }

        /// <summary>
        /// Обновить продолжительность события в минутах.
        /// </summary>
        public UpdateSimpleTask AddDuration(long duration)
        {
            _entityForJson.Duration = duration;
            return this;
        }

        /// <summary>
        /// Отменить планирование. Задача вернется во входящие.
        /// </summary>
        public UpdateSimpleTask CancelSchedule()
        {
            _entityForJson.CancelSchedule = true;
            return this;
        }

        /// <summary>
        /// Запланировать задачу на указанную дату. Задача вернется во входящие в 7 утра во временной зоне клиента.
        /// Формат: YYYY-MM-DD.
        /// !!! Используется, если задача создается через api даннные пользователя, в противном случае воспользоваться методом AddScheduledDate api клиента.
        /// </summary>
        public UpdateSimpleTask ScheduledDate(string scheduledDate)
        {
            _entityForJson.ScheduledDate = scheduledDate;
            return this;
        }

        /// <summary>
        /// Запланировать задачу на указанное время в нулевой временной зоне.
        /// Формат: YYYY-MM-DDThh:mm:ssZ
        /// !!! Используется, если задача создается через api даннные пользователя, в противном случае воспользоваться методом AddScheduledDatetimeUtc api клиента.
        /// </summary>
        public UpdateSimpleTask ScheduledDatetimeUtc(string scheduledDateTime)
        {
            _entityForJson.ScheduledDatetimeUtc = scheduledDateTime;
            return this;
        }

        /// <summary>
        /// Добавить файлы.
        /// Массив строк, каждая из которых может быть:
        /// Guid загруженного файла. Загрузить файл и получить его идентификатор можно с помощью метода files/upload.
        /// Или id уже приложенного к какой-либо задаче файла.
        /// </summary>
        public UpdateSimpleTask AddAttachments(List<Guid> attachments)
        {
            _entityForJson.AttachmentsList = attachments;
            return this;
        }

        /// <summary>
        /// Добавить файлы.
        /// Массив строк, каждая из которых может быть
        /// url уже приложенного к какой-либо задаче файла (можно указать и URL произвольного файла из сети, но в этом случае нельзя будет указать к нему имя ссылки).
        /// </summary>
        public UpdateSimpleTask AddAttachments(List<string> attachments)
        {
            _entityForJson.AttachmentsList = attachments;
            return this;
        }

        /// <summary>
        /// Добавить файлы.
        ///Массив объектов, которые могут содержать следующие поля:
        ///guid — уникальный идентификатор загруженного файла.
        ///root_id — ID уже приложенного файла для создания новой версии (опционально).
        ///или    attachment_id — ID уже приложенного к какой-либо задаче файла.
        ///или    url — URL уже приложенного к какой-либо задаче файла, либо URL произвольного файла из сети (в этом случае следует указать name).
        ///name — имя, которое будет отображаться для ссылки(опционально; для URL файла из Pyrus игнорируется).
        /// </summary>
        public UpdateSimpleTask AddAttachments(List<ValueAttachmentData> attachments)
        {
            _entityForJson.AttachmentsList = attachments;
            return this;
        }

        /// <summary>
        /// Добавить файлы.
        /// fullFileName - полоное имя файла, включая расположение
        /// apiClient - pyrus api клиент
        /// </summary>
        public async Task<UpdateSimpleTask> AddAttachments(string fullFileName, ApiClient apiClient)
        {
            var newAttachmentGuid = await apiClient.UploadFile(fullFileName);
            List<Guid> currentAttachments = new List<Guid>();
            try
            {
                currentAttachments = JsonConvert.DeserializeObject<List<Guid>>(_entityForJson.AttachmentsList.ToString());
                if (currentAttachments == null)
                    currentAttachments = new List<Guid>();
            }
            catch (Exception)
            {
                currentAttachments = new List<Guid>();
            }

            currentAttachments.Add(newAttachmentGuid);
            _entityForJson.AttachmentsList = currentAttachments;
            return this;
        }

        /// <summary>
        /// Добавить текст.
        /// text - текст сообщение
        /// formatedText - true, если текст комментария содержит форматирование html.
        /// </summary>
        public UpdateSimpleTask AddText(string text, bool formatedText = false)
        {
            if (formatedText)
                _entityForJson.FormattedTtext = text;
            else
                _entityForJson.Text = text;
            return this;
        }

        /// <summary>
        /// Добавить наблюдателей задачи, — состоит из идентификаторов id.
        /// </summary>
        public UpdateSimpleTask AddSubscribers(List<ValueIdData> subscribers)
        {
            _entityForJson.SubscribersAdded = subscribers;
            return this;
        }

        /// <summary>
        /// Добавить наблюдателей задачи, — состоит из адресов электронной почты пользователей.
        /// </summary>
        public UpdateSimpleTask AddSubscribers(List<ValueEmailData> subscribers)
        {
            _entityForJson.SubscribersAdded = subscribers;
            return this;
        }

        /// <summary>
        /// Удалить наблюдателей задачи, — состоит из идентификаторов id.
        /// </summary>
        public UpdateSimpleTask RemoveSubscribers(List<ValueIdData> subscribers)
        {
            _entityForJson.SubscribersRemoved = subscribers;
            return this;
        }

        /// <summary>
        /// Удалить наблюдателей задачи, — состоит из адресов электронной почты пользователей.
        /// </summary>
        public UpdateSimpleTask RemoveSubscribers(List<ValueEmailData> subscribers)
        {
            _entityForJson.SubscribersRemoved = subscribers;
            return this;
        }

        /// <summary>
        /// Наблюдатели, от которых требуется дополнительное согласование, — состоит из идентификаторов id.
        /// </summary>
        public UpdateSimpleTask ReRequestSubscribers(List<ValueIdData> subscribers)
        {
            _entityForJson.SubscribersRerequested = subscribers;
            return this;
        }
        /// <summary>
        /// Наблюдатели, от которых требуется дополнительное согласование, — состоит из адресов электронной почты пользователей.
        /// </summary>
        public UpdateSimpleTask ReRequestSubscribers(List<ValueEmailData> subscribers)
        {
            _entityForJson.SubscribersRerequested = subscribers;
            return this;
        }

        /// <summary>
        /// Добавить задачу в списки.
        /// </summary>
        public UpdateSimpleTask AddListsIds(List<long> listIds)
        {
            _entityForJson.AddedListIds = listIds;
            return this;
        }

        /// <summary>
        /// Удалить задачу из списков.
        /// </summary>
        public UpdateSimpleTask RemovedTasksIds(List<long> listIds)
        {
            _entityForJson.RemovedListIds = listIds;
            return this;
        }

        /// <summary>
        /// Id пользователя на которого будет переназначена задача..
        /// </summary>
        public UpdateSimpleTask ReassignTo(ValueIdData reassignTo)
        {
            _entityForJson.ReassignTo = reassignTo;
            return this;
        }

        /// <summary>
        /// Эл. адрес email пользователя на которого будет переназначена задача.
        /// </summary>
        public UpdateSimpleTask ReassignTo(ValueEmailData reassignTo)
        {
            _entityForJson.ReassignTo = reassignTo;
            return this;
        }

        /// <summary>
        /// Затраченное время в минутах.
        /// </summary>
        public UpdateSimpleTask AddSpentMinutesInfo(int minutes)
        {
            _entityForJson.SpentMinutes = minutes;
            return this;
        }
    }
}
