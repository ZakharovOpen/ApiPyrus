using Newtonsoft.Json;
using System.Collections.Generic;

namespace ApiPyrus.Models.DTOs
{
    public class PyrusRequestTask
    {
        [JsonProperty("form_id")]
        public long FormId { get; set; }

        [JsonProperty("approvals")]
        public object Approvals { get; set; }//Users UsersType

        [JsonProperty("approval_choice")]
        /*
         * Use ApprovalTypes
        approved — утвердить
        acknowledged — отметить, что задача просмотрена
        rejected — отклонить
        revoked — аннулировать свое согласование
          */
        public object ApprovalChoice { get; set; }

        [JsonProperty("approvals_added")]
        public object ApprovalsAdded { get; set; } //  Добавляемая маршрутизация, является массивом шагов согласования, состоящим из массивов идентификаторов id или эл. адресов email пользователей.Применимо только для задачи по форме.

        [JsonProperty("approvals_removed")]
        public object ApprovalsRemoved { get; set; } //  Удаляемая маршрутизация, является массивом шагов согласования, состоящим из массивов идентификаторов id или эл. адресов email пользователей.Применимо только для задачи по форме.

        [JsonProperty("approvals_rerequested")]
        public object ApprovalsRerequested { get; set; } //  Повторно запрошенная маршрутизация, является массивом шагов согласования, состоящим из массивов идентификаторов id или эл. адресов email пользователей.Применимо только для задачи по форме.

        [JsonProperty("subscribers")]
        public object Subscribers { get; set; } // Массив наблюдателей: массив идентификаторов id или адресов электронной почты пользователей. При создании задачи  

        [JsonProperty("subscribers_added")]
        public object SubscribersAdded { get; set; } //Добавляемые наблюдатели: массив идентификаторов id или адресов электронной почты пользователей.

        [JsonProperty("subscribers_removed")]
        public object SubscribersRemoved { get; set; } // Удаляемые наблюдатели: массив идентификаторов id или адресов электронной почты пользователей.

        [JsonProperty("subscribers_rerequested")]
        public object SubscribersRerequested { get; set; } // Наблюдатели, от которых требуется дополнительное согласование: массив идентификаторов id или адресов электронной почты пользователей.

        [JsonProperty("participants")]
        public object Participants { get; set; } // Массив участников задачи, состоящий из идентификаторов id или эл. адресов email пользователей.Применимо только для простой задачи.

        [JsonProperty("participants_added")]
        public object ParticipantsAdded { get; set; } // Добавляемые участники, массив идентификаторов id или эл. адресов email пользователей.Применимо только для простой задачи.

        [JsonProperty("participants_removed")]
        public object ParticipantsRemoved { get; set; } //Удаляемые участники, массив идентификаторов id или эл. адресов email пользователей.Применимо только для простой задачи.

        [JsonProperty("parent_task_id")]
        public long? ParentTaskId { get; set; } //Идентификатор надзадачи.

        [JsonProperty("edit_comment_id")]
        public long? EditCommentId { get; set; } //Идентификатор комментария, который требуется изменить.

        [JsonProperty("text")]
        public string Text { get; set; } //Текст комментария.

        [JsonProperty("formatted_text")]
        public string FormattedTtext { get; set; } //Текст комментария - форматированный.

        [JsonProperty("responsible")]
        public object Responsible { get; set; } //Users UsersType, 

        [JsonProperty("reassign_to")]
        public object ReassignTo { get; set; } //Идентификатор id или эл. адрес email пользователя на которого будет переназначена задача.

        [JsonProperty("subject")]
        public string Subject { get; set; }  //Заголовок задачи. Применимо только для простой задачи.

        [JsonProperty("due_date")]
        public object DueDate { get; set; } //Срок задачи (может быть указано только одно из свойств due_date или due).

        [JsonProperty("due")]
        public object Due { get; set; } //Срок задачи со временем (может быть указано только одно из свойств due_date или due).

        [JsonProperty("duration")]
        public long? Duration { get; set; } //Продолжительность события в минутах (используется только совместно с due).

        [JsonProperty("fill_defaults")]
        public bool? FillDefaults { get; set; } //Флаг, указывающий необходимость заполнения полей формы значениями по умолчанию из шаблона формы.По умолчанию: false. 

        [JsonProperty("fields")]
        public List<ValueFieldData> Fields { get; set; } //Массив добавляемых значений формы. Каждое значение включает в себя поля id и value. 

        [JsonProperty("field_updates")]
        public List<ValueFieldData> FieldUpdates { get; set; } //Массив обновляемых значений формы. Каждое значение включает в себя поля id и value. 

        [JsonProperty("channel")]
        public ValueChannelData Channel { get; set; }  //Использовать класс ChanelType ,Комментарий будет отправлен по внешнему каналу (email, Telegram, Facebook, VK, Viber, Instagram, Private ValueChannel [Заметки], WhatsApp, Web Widget, Mobile App, SMS**). Применимо только для задачи по форме.

        [JsonProperty("comment_as_roles")]
        public List<CommentAsRole> CommentAsRoles { get; set; }

        [JsonProperty("attachments")]
        public object AttachmentsList { get; set; } // Use AttachmentsType

        [JsonProperty("list_ids")]
        public List<long> ListIds { get; set; } //	Массив идентификаторов списков

        [JsonProperty("added_list_ids")]
        public List<long> AddedListIds { get; set; } //	Массив идентификаторов списков в которые необходимо добавить задачу.

        [JsonProperty("removed_list_ids")]
        public List<long> RemovedListIds { get; set; } //	Массив идентификаторов списков из которых необходимо исключить задачу.

        [JsonProperty("scheduled_date")]
        public object ScheduledDate { get; set; }  //Запланировать задачу на указанную дату. Задача вернется во входящие в 7 утра во временной зоне клиента.Формат: YYYY-MM-DD.

        [JsonProperty("scheduled_datetime_utc")]
        public object ScheduledDatetimeUtc { get; set; }  //Запланировать задачу на указанное время в нулевой временной зоне.Формат: YYYY-MM-DDThh:mm:ssZ

        [JsonProperty("cancel_schedule")]
        public bool? CancelSchedule { get; set; }  //Отменить планирование. Задача вернется во входящие.

        [JsonProperty("spent_minutes")]
        public long? SpentMinutes { get; set; }  //Затраченное время в минутах..

        [JsonProperty("skip_satisfaction")]
        public bool? SkipSatisfaction { get; set; }  //Не запрашивать у клиента обратную связь для оценки качества обслуживания при закрытии заявки.

        [JsonProperty("skip_notification")]
        public bool? SkipNotification { get; set; }  //Позволяет комментировать задачу без отправки уведомлений участникам, такая задача не отображается как непрочитанная.

        [JsonProperty("action")]
        public string Action { get; set; } // Use ActionType.  finished — закрыть задачу ; reopened — переоткрыть задачу
    }

}

