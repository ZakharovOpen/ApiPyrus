using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Tasks
{
    public class CreateSimpleTask
    {
        private PyrusRequestTask _entityForJson = new PyrusRequestTask();

        /// <summary>
        /// Создать задачу в Pyrus.
        /// </summary>
        public async Task<PyrusTask> Send(ApiClient apiClient, string extRequestId = "")
            => await apiClient.CreateTask(_entityForJson.GetJson(), extRequestId);


        /// <summary>
        /// Добавить текст.
        /// text - текст сообщение
        /// formatedText - true, если текст комментария содержит форматирование html.
        /// </summary>
        public CreateSimpleTask AddText(string text, bool formatedText = false)
        {
            if (formatedText)
                _entityForJson.FormattedTtext = text;
            else
                _entityForJson.Text = text;
            return this;
        }

        /// <summary>
        /// Добавить заголовок задачи. 
        /// </summary>
        public CreateSimpleTask AddSubject(string subject)
        {
            _entityForJson.Subject = subject;
            return this;
        }

        /// <summary>
        /// Добавить срок задачи. Дата в формате YYYY-MM-DD.
        /// </summary>
        public CreateSimpleTask AddDueDate(string dueDate)
        {
            _entityForJson.DueDate = dueDate;
            return this;
        }

        /// <summary>
        /// Добавить срок задачи со временем. Дата в формате YYYY-MM-DDThh:mm:ssZ.
        /// </summary>
        public CreateSimpleTask AddDue(string dueDate)
        {
            _entityForJson.Due = dueDate;
            return this;
        }

        /// <summary>
        /// Продолжительность события в минутах
        /// </summary>
        public CreateSimpleTask AddDuration(long duration)
        {
            _entityForJson.Duration = duration;
            return this;
        }

        /// <summary>
        /// Добавить массив наблюдателей задачи, — состоит из идентификаторов id.
        /// </summary>
        public CreateSimpleTask AddSubscribers(List<ValueIdData> subscribers)
        {
            _entityForJson.Subscribers = subscribers;
            return this;
        }

        /// <summary>
        /// Добавить массив наблюдателей задачи, — состоит из адресов электронной почты пользователей.
        /// </summary>
        public CreateSimpleTask AddSubscribers(List<ValueEmailData> subscribers)
        {
            _entityForJson.Subscribers = subscribers;
            return this;
        }

        /// <summary>
        /// Добавить идентификатор надзадачи.
        /// </summary>
        public CreateSimpleTask AddParentTaskId(long parentTaskId)
        {
            _entityForJson.ParentTaskId = parentTaskId;
            return this;
        }

        /// <summary>
        /// Добавить задачу в списки.
        /// </summary>
        public CreateSimpleTask AddListsIds(List<long> listIds)
        {
            _entityForJson.ListIds = listIds;
            return this;
        }

        /// <summary>
        ///Запланировать задачу на указанную дату. Задача вернется во входящие в 7 утра во временной зоне клиента.
        ///Формат: YYYY-MM-DD.
        /// !!! Используется, если задача создается через api даннные пользователя, в противном случае воспользоваться методом AddScheduledDate api клиента.
        /// </summary>
        public CreateSimpleTask ScheduledDate(string scheduledDate)
        {
            _entityForJson.ScheduledDate = scheduledDate;
            return this;
        }

        /// <summary>
        /// Запланировать задачу на указанное время в нулевой временной зоне.
        /// Формат: YYYY-MM-DDThh:mm:ssZ
        /// !!! Используется, если задача создается через api даннные пользователя, в противном случае воспользоваться методом AddScheduledDatetimeUtc api клиента.
        /// </summary>
        public CreateSimpleTask ScheduledDatetimeUtc(string scheduledDateTime)
        {
            _entityForJson.ScheduledDatetimeUtc = scheduledDateTime;
            return this;
        }

        /// <summary>
        /// Id пользователя ответственного за задачу.
        /// </summary>
        public CreateSimpleTask AddResponsible(ValueIdData responsible)
        {
            _entityForJson.Responsible = responsible;
            return this;
        }

        /// <summary>
        /// Эл. адрес email пользователя ответственного за задачу.
        /// </summary>
        public CreateSimpleTask AddResponsible(ValueEmailData responsible)
        {
            _entityForJson.Responsible = responsible;
            return this;
        }

        /// <summary>
        /// Id пользователя ответственного за задачу.
        /// </summary>
        public CreateSimpleTask AddParticipants(List<ValueIdData> participants)
        {
            _entityForJson.Participants = participants;
            return this;
        }

        /// <summary>
        /// Эл. адрес email пользователя ответственного за задачу.
        /// </summary>
        public CreateSimpleTask AddParticipants(List<ValueEmailData> participants)
        {
            _entityForJson.Participants = participants;
            return this;
        }

    }
}
