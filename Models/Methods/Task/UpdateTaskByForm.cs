using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Tasks
{
    public class UpdateTaskByForm
    {
        public UpdateTaskByForm(long taskId)
            => _taskId = taskId;

        private long _taskId;
        protected PyrusRequestTask _entityForJson = new PyrusRequestTask();

        /// <summary>
        /// Отправить комментарий/ обновить задачу в Pyrus.
        /// </summary>
        public async Task<PyrusTask> Send(ApiClient apiClient, string extRequestId = "")
            => await apiClient.CloseOrCommentTask(this._taskId, _entityForJson.GetJson(), extRequestId);


        /// <summary>
        /// Отменить планирование. Задача вернется во входящие.
        /// </summary>
        public UpdateTaskByForm CancelSchedule()
        {
            _entityForJson.CancelSchedule = true;
            return this;
        }

        /// <summary>
        /// Запланировать задачу на указанную дату. Задача вернется во входящие в 7 утра во временной зоне клиента.
        /// Формат: YYYY-MM-DD.
        /// !!! Используется, если задача создается через api даннные пользователя, в противном случае воспользоваться методом AddScheduledDate api клиента.
        /// </summary>
        public UpdateTaskByForm ScheduledDate(string scheduledDate)
        {
            _entityForJson.ScheduledDate = scheduledDate;
            return this;
        }

        /// <summary>
        /// Запланировать задачу на указанное время в нулевой временной зоне.
        /// Формат: YYYY-MM-DDThh:mm:ssZ
        /// !!! Используется, если задача создается через api даннные пользователя, в противном случае воспользоваться методом AddScheduledDatetimeUtc api клиента.
        /// </summary>
        public UpdateTaskByForm ScheduledDatetimeUtc(string scheduledDateTime)
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
        public UpdateTaskByForm AddAttachments(List<Guid> attachments)
        {
            _entityForJson.AttachmentsList = attachments;
            return this;
        }

        /// <summary>
        /// Добавить файлы.
        /// Массив строк, каждая из которых может быть
        /// url уже приложенного к какой-либо задаче файла (можно указать и URL произвольного файла из сети, но в этом случае нельзя будет указать к нему имя ссылки).
        /// </summary>
        public UpdateTaskByForm AddAttachments(List<string> attachments)
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
        public UpdateTaskByForm AddAttachments(List<ValueAttachmentData> attachments)
        {
            _entityForJson.AttachmentsList = attachments;
            return this;
        }

        /// <summary>
        /// Добавить файлы.
        /// fullFileName - полоное имя файла, включая расположение
        /// apiClient - pyrus api клиент
        /// </summary>
        public async Task<UpdateTaskByForm> AddAttachments(string fullFileName, ApiClient apiClient)
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
        public UpdateTaskByForm AddText(string text, bool formatedText = false)
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
        public UpdateTaskByForm AddSubscribers(List<ValueIdData> subscribers)
        {
            _entityForJson.SubscribersAdded = subscribers;
            return this;
        }

        /// <summary>
        /// Добавить наблюдателей задачи, — состоит из адресов электронной почты пользователей.
        /// </summary>
        public UpdateTaskByForm AddSubscribers(List<ValueEmailData> subscribers)
        {
            _entityForJson.SubscribersAdded = subscribers;
            return this;
        }

        /// <summary>
        /// Удалить наблюдателей задачи, — состоит из идентификаторов id.
        /// </summary>
        public UpdateTaskByForm RemoveSubscribers(List<ValueIdData> subscribers)
        {
            _entityForJson.SubscribersRemoved = subscribers;
            return this;
        }

        /// <summary>
        /// Удалить наблюдателей задачи, — состоит из адресов электронной почты пользователей.
        /// </summary>
        public UpdateTaskByForm RemoveSubscribers(List<ValueEmailData> subscribers)
        {
            _entityForJson.SubscribersRemoved = subscribers;
            return this;
        }

        /// <summary>
        /// Наблюдатели, от которых требуется дополнительное согласование, — состоит из идентификаторов id.
        /// </summary>
        public UpdateTaskByForm ReRequestSubscribers(List<ValueIdData> subscribers)
        {
            _entityForJson.SubscribersRerequested = subscribers;
            return this;
        }
        /// <summary>
        /// Наблюдатели, от которых требуется дополнительное согласование, — состоит из адресов электронной почты пользователей.
        /// </summary>
        public UpdateTaskByForm ReRequestSubscribers(List<ValueEmailData> subscribers)
        {
            _entityForJson.SubscribersRerequested = subscribers;
            return this;
        }

        /// <summary>
        /// Добавить задачу в списки.
        /// </summary>
        public UpdateTaskByForm AddListsIds(List<long> listIds)
        {
            _entityForJson.AddedListIds = listIds;
            return this;
        }

        /// <summary>
        /// Удалить задачу из списков.
        /// </summary>
        public UpdateTaskByForm RemovedTasksIds(List<long> listIds)
        {
            _entityForJson.RemovedListIds = listIds;
            return this;
        }

        /// <summary>
        /// Добавить массив шагов согласования, состоящий из массивов идентификаторов id.
        /// </summary>
        public UpdateTaskByForm AddApprovals(List<List<ValueIdData>> approvals)
        {
            _entityForJson.ApprovalsAdded = approvals;
            return this;
        }

        /// <summary>
        /// Добавить массив шагов согласования, состоящий из массивов эл. адресов email пользователей.
        /// </summary>
        public UpdateTaskByForm AddApprovals(List<List<ValueEmailData>> approvals)
        {
            _entityForJson.Approvals = approvals;
            return this;
        }

        /// <summary>
        /// Удалить массив шагов согласования, состоящий из массивов идентификаторов id.
        /// </summary>
        public UpdateTaskByForm RemoveApprovals(List<List<ValueIdData>> approvals)
        {
            _entityForJson.ApprovalsRemoved = approvals;
            return this;
        }

        /// <summary>
        /// Удалить массив шагов согласования, состоящий из массивов эл. адресов email пользователей.
        /// </summary>
        public UpdateTaskByForm RemoveApprovals(List<List<ValueEmailData>> approvals)
        {
            _entityForJson.ApprovalsRemoved = approvals;
            return this;
        }

        /// <summary>
        /// Повторно запрошенная маршрутизация, является массивом шагов согласования, состоящим из массивов идентификаторов id.
        /// </summary>
        public UpdateTaskByForm ReRequestedApprovals(List<List<ValueIdData>> approvals)
        {
            _entityForJson.ApprovalsRerequested = approvals;
            return this;
        }

        /// <summary>
        /// Повторно запрошенная маршрутизация, является массивом шагов согласования, состоящим из массивов эл. адресов email пользователей.
        /// </summary>
        public UpdateTaskByForm ReRequestedApprovals(List<List<ValueEmailData>> approvals)
        {
            _entityForJson.ApprovalsRerequested = approvals;
            return this;
        }

        /// <summary>
        /// Добавить вариант утверждения.
        /// </summary>
        public UpdateTaskByForm AddApprovalChoice(ApprovalTypes approvalChoice)
        {
            _entityForJson.ApprovalChoice = approvalChoice.GetDescription();
            return this;
        }

        /// <summary>
        /// Обновить значение поля в задаче.
        /// </summary>
        public UpdateTaskByForm UpdateField(ValueFieldData field)
        {
            if (_entityForJson.FieldUpdates == null)
                _entityForJson.FieldUpdates = new List<ValueFieldData>();

            _entityForJson.FieldUpdates.Add(field);
            return this;
        }

        /// <summary>
        /// Обновить значений полей в задаче списком.
        /// </summary>
        public UpdateTaskByForm UpdateFields(List<ValueFieldData> fields)
        {
            if (_entityForJson.FieldUpdates == null)
                _entityForJson.FieldUpdates = new List<ValueFieldData>();

            _entityForJson.FieldUpdates.AddRange(fields);
            return this;
        }

        /// <summary>
        /// Комментарий будет отправлен по внешнему каналу (email, Telegram, Facebook, VK, Viber, Instagram, Private Channel [Заметки], WhatsApp, Web Widget, Mobile App, SMS**)
        /// **Если отправляем sms то обязательно пердедаем параметр phone
        /// </summary>
        public UpdateTaskByForm AddChannel(ChannelTypes channelType, string phone = null)
        {
            _entityForJson.Channel = new ValueChannelData(channelType.GetDescription(), phone);
            return this;
        }

        /// <summary>
        /// Обновить затраченное время в минутах.
        /// </summary>
        public UpdateTaskByForm UpdateSpentMinutesInfo(int minutes)
        {
            _entityForJson.SpentMinutes = minutes;
            return this;
        }

        /// <summary>
        /// Не запрашивать у клиента обратную связь для оценки качества обслуживания при закрытии заявки.
        /// </summary>
        public UpdateTaskByForm SkipSatisfaction()
        {
            _entityForJson.SkipSatisfaction = true;
            return this;
        }

        /// <summary>
        /// Позволяет комментировать задачу без отправки уведомлений участникам, такая задача не отображается как непрочитанная.
        /// </summary>
        public UpdateTaskByForm SkipNotification()
        {
            _entityForJson.SkipNotification = true;
            return this;
        }

        /// <summary>
        /// Обновить текст существующего комментария. Опциональный параметр, целочисленный идентификатор существующего комментария, который требуется изменить.
        /// </summary>
        public UpdateTaskByForm EditComment(long editedCommentId)
        {
            _entityForJson.EditCommentId = editedCommentId;
            return this;
        }

        /// <summary>
        /// Сделать ответ на сообщение
        /// </summary>
        public UpdateTaskByForm ReplyComment(long replyCommentId, string replyCommentText, string text)
        {
            _entityForJson.FormattedTtext = $"<quote data-noteid=\"{replyCommentId}\" >{replyCommentText}</quote>{text}";
            return this;
        }
    }
}
