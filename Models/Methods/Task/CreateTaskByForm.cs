using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Tasks
{
    public class CreateTaskByForm
    {
        public CreateTaskByForm(long formId)
            => _entityForJson.FormId = formId;

        private PyrusRequestTask _entityForJson = new PyrusRequestTask();

        /// <summary>
        /// Создать задачу в Pyrus.
        /// </summary>
        public async Task<PyrusTask> Send(ApiClient apiClient, string extRequestId = "")
            => await apiClient.CreateTask(_entityForJson.GetJson(), extRequestId);

        /// <summary>
        /// Добавить массив наблюдателей задачи, — состоит из идентификаторов id.
        /// </summary>
        public CreateTaskByForm AddSubscribers(List<ValueIdData> subscribers)
        {
            _entityForJson.Subscribers = subscribers;
            return this;
        }

        /// <summary>
        /// Добавить массив наблюдателей задачи, — состоит из адресов электронной почты пользователей.
        /// </summary>
        public CreateTaskByForm AddSubscribers(List<ValueEmailData> subscribers)
        {
            _entityForJson.Subscribers = subscribers;
            return this;
        }

        /// <summary>
        /// Добавить идентификатор надзадачи.
        /// </summary>
        public CreateTaskByForm AddParentTaskId(long parentTaskId)
        {
            _entityForJson.ParentTaskId = parentTaskId;
            return this;
        }

        /// <summary>
        /// Добавить задачу в списки.
        /// </summary>
        public CreateTaskByForm AddListsIds(List<long> listIds)
        {
            _entityForJson.ListIds = listIds;
            return this;
        }

        /// <summary>
        ///Запланировать задачу на указанную дату. Задача вернется во входящие в 7 утра во временной зоне клиента.
        ///Формат: YYYY-MM-DD.
        /// !!! Используется, если задача создается через api даннные пользователя, в противном случае воспользоваться методом AddScheduledDate api клиента.
        /// </summary>
        public CreateTaskByForm ScheduledDate(string scheduledDate)
        {
            _entityForJson.ScheduledDate = scheduledDate;
            return this;
        }

        /// <summary>
        /// Запланировать задачу на указанное время в нулевой временной зоне.
        /// Формат: YYYY-MM-DDThh:mm:ssZ
        /// !!! Используется, если задача создается через api даннные пользователя, в противном случае воспользоваться методом AddScheduledDatetimeUtc api клиента.
        /// </summary>
        public CreateTaskByForm ScheduledDatetimeUtc(string scheduledDateTime)
        {
            _entityForJson.ScheduledDatetimeUtc = scheduledDateTime;
            return this;
        }

        /// <summary>
        /// Добавить массив шагов согласования, состоящий из массивов идентификаторов id.
        /// </summary>
        public CreateTaskByForm AddApprovals(List<List<ValueIdData>> approvals)
        {
            _entityForJson.Approvals = approvals;
            return this;
        }

        /// <summary>
        /// Добавить массив шагов согласования, состоящий из массивов эл. адресов email пользователей.
        /// </summary>
        public CreateTaskByForm AddApprovals(List<List<ValueEmailData>> approvals)
        {
            _entityForJson.Approvals = approvals;
            return this;
        }

        /// <summary>
        /// Добавить значение поля в задачу.
        /// </summary>
        public CreateTaskByForm AddField(ValueFieldData field)
        {
            if (_entityForJson.Fields == null)
                _entityForJson.Fields = new List<ValueFieldData>();
            _entityForJson.Fields.Add(field);
            return this;
        }

        /// <summary>
        /// Добавить список значений полей в задачу.
        /// </summary>
        public CreateTaskByForm AddFields(List<ValueFieldData> fields)
        {
            if (_entityForJson.Fields == null)
                _entityForJson.Fields = new List<ValueFieldData>();
            _entityForJson.Fields.AddRange(fields);
            return this;
        }

        /// <summary>
        /// Флаг, указывающий необходимость заполнения полей формы значениями по умолчанию из шаблона формы.
        /// </summary>
        public CreateTaskByForm FillDefaults()
        {
            _entityForJson.FillDefaults = true;
            return this;
        }

    }
}
