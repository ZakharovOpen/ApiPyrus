using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ApiPyrus.Models.DTOs
{
    /// <summary>
    /// 1.Email - Отправить комментарий по email<br/>
    /// 2.Telegram - Отправить комментарий в telegram<br/>
    /// 3.Facebook - Отправить комментарий в facebook<br/>
    /// 4.Vkontakte - Отправить комментарий в Вконтакте<br/>
    /// 5.Viber - Отправить комментарий в viber<br/>
    /// 6.Instagram - Отправить комментарий в instagram<br/>
    /// 7.PrivateChannel - Отправить комментарий в приватный канал<br/>
    /// 8.WhatsApp - Отправить комментарий в whatsApp<br/>
    /// 9.WebWidget - Отправить комментарий в виджет<br/>
    /// 10.MobileApp - Отправить комментарий в мобильное приложение<br/>
    /// 11.SMS - Отправить комментарий по СМС.<br/>
    /// Отправка SMS-сообщений возможна после заключения дополнительного соглашения.<br/>
    /// Для подключения интерации с SMS обратитесь в службу поддержки Pyrus
    /// </summary>
    public enum ChannelTypes
    {
        [Description("email")]
        /// <summary>  
        /// Отправить комментарий по email.  
        /// </summary>  
        Email = 1,

        [Description("telegram")]
        /// <summary>  
        /// Отправить комментарий в telegram.  
        /// </summary>  
        Telegram = 2,

        [Description("facebook")]
        /// <summary>  
        /// Отправить комментарий в facebook.  
        /// </summary>  
        Facebook = 3,

        [Description("vk")]
        /// <summary>  
        /// Отправить комментарий в Вконтакте.  
        /// </summary>  
        Vkontakte = 4,

        [Description("viber")]
        /// <summary>  
        /// Отправить комментарий в viber.  
        /// </summary>  
        Viber = 5,

        [Description("instagram")]
        /// <summary>  
        /// Отправить комментарий в instagram.  
        /// </summary>  
        Instagram = 6,

        [Description("private_channel")]
        /// <summary>  
        /// Отправить комментарий в приватный канал.  
        /// </summary>  
        PrivateChannel = 7,

        [Description("whats_app")]
        /// <summary>  
        /// Отправить комментарий в whatsApp.  
        /// </summary>  
        WhatsApp = 8,

        [Description("web_widget")]
        /// <summary>  
        /// Отправить комментарий в виджет.  
        /// </summary>  
        WebWidget = 9,

        [Description("mobile_app")]
        /// <summary>  
        /// Отправить комментарий в мобильное приложение.  
        /// </summary>  
        MobileApp = 10,

        [Description("sms")]
        /// <summary>  
        /// Отправить комментарий по СМС.
        /// Отправка SMS-сообщений возможна после заключения дополнительного соглашения. 
        /// Для подключения интерации с SMS обратитесь в службу поддержки Pyrus
        /// </summary>  
        SMS = 11
    }

    /// <summary>
    /// 1.Finished - закрыть задачу<br/>
    /// 2.Reopened - переоткрыть задачу
    /// </summary>
    public enum ActionTypes
    {
        [Description("finished")]
        /// <summary>  
        ///  Закрыть задачу.  
        /// </summary>  
        Finished = 1,


        [Description("reopened")]
        /// <summary>  
        ///  Преоткрыть задачу.  
        /// </summary>  
        /// 
        Reopened = 2
    }

    /// <summary>
    /// 1.Approved - утвердить<br/>
    /// 2.Acknowledged - отметить, что задача просмотрена<br/>
    /// 3.Rejected - отклонить<br/>
    /// 4.Revoked - аннулировать свое согласование
    /// </summary>
    public enum ApprovalTypes
    {
        [Description("approved")]
        /// <summary>  
        ///  Утвердить.  
        /// </summary>  
        Approved = 1,

        [Description("acknowledged")]
        /// <summary>  
        ///  Отметить, что задача просмотрена.  
        /// </summary>  
        Acknowledged = 2,

        [Description("rejected")]
        /// <summary>  
        ///  Отклонить.  
        /// </summary>  
        Rejected = 3,

        [Description("revoked")]
        /// <summary>  
        ///  Аннулировать свое согласование.  
        /// </summary>  
        Revoked = 4
    }

    /// <summary>
    /// 1.None - отсутствует<br/>
    /// 2.Checked - установлен<br/>
    /// 3.Unchecked - не установлен
    /// </summary>
    /// 
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CheckmarkTypes
    {
        [EnumMember(Value = "none")]
        /// <summary>  
        ///  Отсутствует.  
        /// </summary>  
        None = 1,

        [EnumMember(Value = "checked")]
        /// <summary>  
        ///  Установлен.  
        /// </summary>  
        Checked = 2,

        [EnumMember(Value = "unchecked")]
        /// <summary>  
        ///  Не установлен.  
        /// </summary>  
        Unchecked = 3
    }

    /// <summary>
    /// 1.Open - открыта<br/>
    /// 2.Closed - закрыта
    /// </summary>
    public enum StatusTypes
    {
        [EnumMember(Value = "open")]
        /// <summary>  
        ///  Открыта.  
        /// </summary>  
        Open = 1,

        [EnumMember(Value = "closed")]
        /// <summary>  
        ///  Закрыта.  
        /// </summary>  
        Closed = 2
    }

    public enum FieldTypes
    {
        [EnumMember(Value = "none")]
        None,

        [EnumMember(Value = "project")]
        Project,

        [EnumMember(Value = "form_link")]
        FormLink,

        [EnumMember(Value = "title")]
        Title,

        [EnumMember(Value = "multiple_choice")]
        MultipleChoice,

        [EnumMember(Value = "table")]
        Table,

        [EnumMember(Value = "author")]
        Author,

        [EnumMember(Value = "person")]
        Person,

        [EnumMember(Value = "file")]
        File,

        [EnumMember(Value = "text")]
        Text,

        [EnumMember(Value = "money")]
        Money,

        [EnumMember(Value = "number")]
        Number,

        [EnumMember(Value = "date")]
        Date,

        [EnumMember(Value = "catalog")]
        Catalog,

        [EnumMember(Value = "checkmark")]
        Checkmark,

        [EnumMember(Value = "due_date")]
        DueDate,

        [EnumMember(Value = "due_date_time")]
        DueDateTime,

        [EnumMember(Value = "time")]
        Time,

        [EnumMember(Value = "email")]
        Email,

        [EnumMember(Value = "phone")]
        Phone,

        [EnumMember(Value = "flag")]
        Flag,

        [EnumMember(Value = "step")]
        Step,

        [EnumMember(Value = "status")]
        Status,

        [EnumMember(Value = "creation_date")]
        CreationDate,

        [EnumMember(Value = "note")]
        Note,
    }
    public enum DateTimeFormatTypes
    {
        /// <summary>  
        ///  Полный формат даты YYYY-MM-DDThh:mm:ssZ.  
        /// </summary>  
        Full = 0,

        /// <summary>  
        ///  Только дата YYYY-MM-DD.  
        /// </summary>  
        DateOnly = 1,

        /// <summary>  
        ///  Толлько время HH:mm.  
        /// </summary>  
        TimeOnly = 2
    }

    internal static class EnumExtension
    {
        internal static string GetEnumMemberValue(this Enum enumValue)
        {
            var enumMemberAttribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(
                enumValue.GetType().GetField(enumValue.ToString()), typeof(EnumMemberAttribute));
            return enumMemberAttribute.Value;
        }
        internal static string GetDescription(this Enum value)
        {
            var descriptionAttribute = (DescriptionAttribute)value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(false)
                .Where(a => a is DescriptionAttribute)
                .FirstOrDefault();

            return descriptionAttribute != null ? descriptionAttribute.Description : value.ToString();
        }
    }
}
