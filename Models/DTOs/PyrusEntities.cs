using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiPyrus.Models.DTOs
{
    #region Task

    public class PyrusTaskInfo
    {
        [JsonProperty("task")]
        public PyrusTask Task { get; set; }
    }

    public class PyrusUpdates : PyrusTaskInfo
    {
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("task_id")]
        public long TaskId { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("bot_settings")]
        public string BotSettings { get; set; }

    }
    /// <summary>  
    /// To get more methods, use the 'ApiPyrus.Extentions' namespace.
    /// </summary>  
    public class PyrusTask
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("formatted_text")]
        public string FormattedTtext { get; set; }

        [JsonProperty("create_date")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("last_modified_date")]
        public DateTime LastModifiedDate { get; set; }

        [JsonProperty("close_date")]
        public DateTime? CloseDate { get; set; }

        [JsonProperty("author")]
        public ValuePersone Author { get; set; }

        [JsonProperty("due")]
        public string Due { get; set; }

        [JsonProperty("list_ids")]
        public List<long> ListIds { get; set; }

        [JsonProperty("last_note_id")]
        public long LastNoteId { get; set; }

        [JsonProperty("subscribers")]
        public List<Subscriber> Subscribers { get; set; }

        [JsonProperty("form_id")]
        public long FormId { get; set; }

        [JsonProperty("approvals")]
        public List<List<object>> Approvals { get; set; }

        [JsonProperty("current_step")]
        public int CurrentStep { get; set; }

        [JsonProperty("parent_task_id")]
        public long? ParentTaskId { get; set; }

        [JsonProperty("steps")]
        public List<PyrusStep> Steps { get; set; }

        [JsonProperty("scheduled_date")]
        public string ScheduledDate { get; set; }

        [JsonProperty("scheduled_datetime_utc")]
        public string ScheduledDatetimeUtc { get; set; }

        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }

        [JsonProperty("comments")]
        public List<Comment> Comments { get; set; }

        [JsonProperty("is_closed")]
        public bool IsClosed { get; set; }
    }
    public class PyrusTasks
    {

        [JsonProperty("tasks")]
        public List<PyrusTask> Tasks { get; set; }
    }
    public class PyrusStep
    {

        [JsonProperty("step")]
        public int Step { get; set; }

        [JsonProperty("elapsed_time")]
        public long ElapsedTime { get; set; }
    }
    public class Subscriber
    {
        [JsonProperty("person")]
        public ValuePersone Person { get; set; }
    }
    public class Attachment
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("root_id", NullValueHandling = NullValueHandling.Ignore)]
        public long RootId { get; set; }
    }
    public class FieldWithValue
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FieldTypes Type { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        /// <summary>  
        /// Generic method only supports 'IValue' classes.
        /// View the annotation to the 'IValue' interface to set correct class.
        /// </summary>  
        public T GetValue<T>() where T : IValue
        {
            var value = this.GetValueObject();
            if (value != null && value is T)
                return (T)value;

            throw new InvalidCastException($"Невозможно привести значение поля '{this.Value}' к типу {typeof(T).Name}");
        }

        /// <summary>  
        /// View the annotation to the 'IValue' interface to convert it to the desired class.
        /// </summary>  
        public IValue GetValueObject()
        {
            if (Value == null)
                return new ValueError() { ErrorMessage = "'Value' is null" };
            try
            {
                switch (this.Type)
                {
                    case FieldTypes.Text:
                    case FieldTypes.Phone:
                    case FieldTypes.Time:
                    case FieldTypes.Note:
                    case FieldTypes.Email:
                        return new ValueString() { String = this.Value.ToString() };
                    case FieldTypes.DueDate:
                    case FieldTypes.CreationDate:
                    case FieldTypes.Date:
                    case FieldTypes.DueDateTime:
                        return new ValueDate() { Date = DateTime.Parse(this.Value.ToString()) };
                    case FieldTypes.Money:
                    case FieldTypes.Number:
                        return new ValueNumber() { Number = decimal.Parse(this.Value.ToString()) };
                    case FieldTypes.Project:
                        return JsonConvert.DeserializeObject<ValueProject>(this.Value.ToString());
                    case FieldTypes.FormLink:
                        return JsonConvert.DeserializeObject<ValueFormLink>(this.Value.ToString());
                    case FieldTypes.Title:
                        return JsonConvert.DeserializeObject<ValueTitle>(this.Value.ToString());
                    case FieldTypes.MultipleChoice:
                        return JsonConvert.DeserializeObject<ValueMultipleChoice>(this.Value.ToString());
                    case FieldTypes.Table:
                        return new ValueTable() { Rows = JsonConvert.DeserializeObject<List<ValueRow>>(this.Value.ToString()) };
                    case FieldTypes.Author:
                    case FieldTypes.Person:
                        return JsonConvert.DeserializeObject<ValuePersone>(this.Value.ToString());
                    case FieldTypes.File:
                        return new ValueFiles() { Files = JsonConvert.DeserializeObject<List<Attachment>>(this.Value.ToString()) };
                    case FieldTypes.Catalog:
                        return JsonConvert.DeserializeObject<ValueCatalog>(this.Value.ToString());
                    case FieldTypes.Checkmark:
                    case FieldTypes.Flag:
                        {
                            var value = new ValueCheckmark();
                            switch (this.Value.ToString())
                            {
                                case "checked":
                                    value.Checkmark = CheckmarkTypes.Checked;
                                    break;
                                case "unchecked":
                                    value.Checkmark = CheckmarkTypes.Unchecked;
                                    break;
                                default:
                                    value.Checkmark = CheckmarkTypes.None;
                                    break;
                            }
                            return value;
                        }
                    case FieldTypes.Step:
                        return new ValueIntegerNumber() { IntegerNumber = int.Parse(this.Value.ToString()) };
                    case FieldTypes.Status:
                        {
                            var value = new ValueStatus();
                            switch (this.Value.ToString())
                            {
                                case "open":
                                    value.Status = StatusTypes.Open;
                                    break;
                                case "closed":
                                    value.Status = StatusTypes.Closed;
                                    break;
                                default:
                                    break;
                            }
                            return value;
                        }
                    case FieldTypes.None:
                    default:
                        return new ValueError() { ErrorMessage = "Sorry. Unsupported field type, try to parse 'Value' yourself. https://pyrus.com/en/help/api/fields" };
                }
            }
            catch (Exception ex)
            {
                return new ValueError() { ErrorMessage = $"Sorry. Cannot parse 'Value', try to parse yourself. https://pyrus.com/en/help/api/fields \n{ex.Message}" };
            }
        }
    }
    public class Field : FieldWithValue
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tooltip")]
        public string Tooltip { get; set; }

        [JsonProperty("info")]
        public Info Info { get; set; }

        [JsonProperty("default_value")]
        public string Default_value { get; set; }

        [JsonProperty("visibility_condition")]
        public VisibilityCondition VisibilityCondition { get; set; }

        [JsonProperty("related_field_id")]
        public long? RelatedFieldId { get; set; }

        [JsonProperty("parent_id")]
        public long ParentId { get; set; }
    }

    public class CommentAsRole
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
    public class SmsInfo
    {

        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
    }
    public class Comment
    {

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("formatted_text")]
        public string FormattedText { get; set; }

        [JsonProperty("create_date")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("author")]
        public ValuePersone Author { get; set; }

        [JsonProperty("added_list_ids")]
        public List<long> AddedListIds { get; set; }

        [JsonProperty("reassigned_to")]
        public ValuePersone ReassignedTo { get; set; }

        [JsonProperty("field_updates")]
        public List<Field> FieldUpdates { get; set; }

        [JsonProperty("approvals_added")]
        public List<object> ApprovalsAdded { get; set; }

        [JsonProperty("due")]
        public string Due { get; set; }

        [JsonProperty("subscribers_added")]
        public List<ValuePersone> SubscribersAdded { get; set; }

        [JsonProperty("approvals_removed")]
        public List<object> ApprovalsRemoved { get; set; }

        [JsonProperty("comment_as_roles")]
        public List<CommentAsRole> CommentAsRoles { get; set; }

        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty("sms_info")]
        public SmsInfo SmsInfo { get; set; }

        [JsonProperty("reply_note_id")]
        public long? ReplyNoteId { get; set; }

        [JsonProperty("edit_comment_id")]
        public long? EditCommentId { get; set; }

        public string GetOnlyReplyCommentText()
        {
            if (!ReplyNoteId.HasValue || ReplyNoteId.Value <= 0 || string.IsNullOrEmpty(Text) || !Text.Contains('\n'))
                return Text;
            return Text.Substring(Text.IndexOf('\n') + 1);
        }

        public string GetСommentTextForReply()
        {
            if (!ReplyNoteId.HasValue || ReplyNoteId.Value <= 0 || string.IsNullOrEmpty(Text) || !Text.Contains('\n'))
                return string.Empty;
            return Text.Substring(0, Text.IndexOf('\n'));
        }

        public string GetFormattedTextWithoutReplyComment()
        {
            if (!ReplyNoteId.HasValue || ReplyNoteId.Value <= 0 || string.IsNullOrEmpty(FormattedText) || !FormattedText.Contains("<quote data-noteid"))
                return FormattedText;
            return FormattedText.Split(new string[] { "</quote>" }, StringSplitOptions.None).Last();
        }

        public string GetСommentFormattedTextForReply()
        {
            if (!ReplyNoteId.HasValue || ReplyNoteId.Value <= 0 || string.IsNullOrEmpty(FormattedText) || !FormattedText.Contains("<quote data-noteid"))
                return string.Empty;
            return FormattedText.Split(new string[] { "</quote>" }, StringSplitOptions.None).First();
        }
    }
    #endregion

    #region FormsTamplate
    public class FormsTemplates
    {
        [JsonProperty("forms")]
        public List<Form> Forms { get; set; }
    }
    public class Form
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("default_person_id")]
        public long DefaultPersonId { get; set; }

        [JsonProperty("deleted_or_closed")]
        public bool DeletedOrClosed { get; set; }

        [JsonProperty("folder")]
        public List<string> Folder { get; set; }

        [JsonProperty("steps")]
        public Dictionary<string, string> Steps { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }

        [JsonProperty("print_forms")]
        public List<PrintForm> PrintForms { get; set; }

    }
    public class PrintForm
    {
        [JsonProperty("print_form_id")]
        public long PrintFormId { get; set; }

        [JsonProperty("print_form_name")]
        public string PrintFormName { get; set; }
    }

    public class TaskTemplateInfo
    {
        [JsonProperty("decimal_places")]
        public long? DecimalPlaces { get; set; }

        [JsonProperty("required_step")]
        public long? RequiredStep { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("options")]
        public List<TaskTemplateOption> Options { get; set; }

        [JsonProperty("catalog_id")]
        public long? CatalogId { get; set; }

        [JsonProperty("fields")]
        public List<TaskTemplateFields> Fields { get; set; }
    }
    public class TaskTemplateFields
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parent_id")]
        public long? ParentId { get; set; }

        [JsonProperty("info")]
        public TaskTemplateInfo Info { get; set; }
    }
    public class TaskTemplateOption
    {
        [JsonProperty("choice_id")]
        public long ChoiceId { get; set; }

        [JsonProperty("choice_value")]
        public string ChoiceValue { get; set; }

        [JsonProperty("fields")]
        public List<TaskTemplateFields> Fields { get; set; }
    }
    #endregion

    #region Person
    public class MembersList
    {
        [JsonProperty("members")]
        public List<ValuePersone> Members { get; set; }
    }
    public class ValuePersone : IValue
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("external_id", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ExternalId { get; set; }

        [JsonProperty("department_id", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? DepartmentId { get; set; }

        [JsonProperty("department_name", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DepartmentName { get; set; }

        [JsonProperty("banned", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool? Banned { get; set; }

        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Position { get; set; }

        [JsonProperty("skype", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Skype { get; set; }

        [JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Phone { get; set; }

        [JsonProperty("login_phone", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LoginPhone { get; set; }

        [JsonProperty("rights", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? Rights { get; set; }

        [JsonProperty("web_session_settings", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object WebSessionSettings { get; set; }

        [JsonProperty("web_session_inactive_settings", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object WebSessionInactiveSettings { get; set; }

        [JsonProperty("mobile_session_inactive_settings", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object MobileSessionInactiveSettings { get; set; }

        [JsonProperty("mobile_session_settings", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object MobileSessionSettings { get; set; }
        [JsonProperty("web_session_restriction_settings", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object WebSessionRestrictionSettings { get; set; }
        [JsonProperty("mobile_session_restriction_settings", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object MobileDessionRestrictionSettings { get; set; }

        [JsonProperty("locale", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Locale { get; set; }

        [JsonProperty("organization_id", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long? OrganizationId { get; set; }

        [JsonProperty("organization", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Organization Organization { get; set; }
    }
    public class Organization
    {
        [JsonProperty("organization_id")]
        public long OrganizationId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("persons")]
        public List<ValuePersone> Persons { get; set; }

        [JsonProperty("roles")]
        public List<Role> Roles { get; set; }

        [JsonProperty("department_catalog_id")]
        public long DepartmentCatalogId { get; set; }
    }
    #endregion

    #region Table
    public class ValueTable : IValue
    {
        [JsonProperty("rows")]
        public List<ValueRow> Rows { get; set; }
    }
    public class ValueRow
    {
        [JsonProperty("row_id")]
        public int RowId { get; set; }

        [JsonProperty("cells")]
        public List<ValueCell> Cells { get; set; }
    }
    public class ValueCell : Field
    {
        [JsonProperty("row_id")]
        public int RowId { get; set; }
    }

    #endregion

    #region Catalog
    public class PyrusCatalogs
    {
        [JsonProperty("catalogs")]
        public List<Catalog> Catalogs { get; set; }
    }
    public class ValueCatalog : IValue
    {
        [JsonProperty("item_id")]
        public long ItemId { get; set; }

        [JsonProperty("item_ids")]
        public List<long> ItemIds { get; set; }

        [JsonProperty("headers")]
        public List<string> Headers { get; set; }

        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Values { get; set; }

        [JsonProperty("rows")]
        public List<List<string>> Rows { get; set; }
    }
    public class Catalog
    {
        [JsonProperty("catalog_id")]
        public long CatalogId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("catalog_headers")]
        public List<CatalogHeader> CatalogHeaders { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("external_version")]
        public string ExternalVersion { get; set; }
    }
    public class CatalogHeader
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
    public class CatalogInfo
    {
        [JsonProperty("catalog_id")]
        public long CatalogId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("external_version")]
        public long External_version { get; set; }

        [JsonProperty("column_settings")]
        public List<ColumnSetting> ColumnSettings { get; set; }

        [JsonProperty("catalog_headers")]
        public List<CatalogHeader> CatalogHeaders { get; set; }

        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }
    public class ColumnSetting
    {
        [JsonProperty("original_position")]
        public long OriginalPosition { get; set; }

        [JsonProperty("sort_order")]
        public long SortOrder { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("group_by")]
        public bool? GroupBy { get; set; }
    }
    public class Item
    {
        [JsonProperty("item_id")]
        public long ItemId { get; set; }

        [JsonProperty("values")]
        public List<string> Values { get; set; }
        public Item()
        {
            Values = new List<string>();
        }
    }
    public class CatalogUpdateInfo
    {
        [JsonProperty("apply")]
        public bool Apply { get; set; }

        [JsonProperty("catalog_headers")]
        public List<CatalogHeader> CatalogHeaders { get; set; }

        [JsonProperty("added")]
        public List<Item> Added { get; set; }

        [JsonProperty("deleted")]
        public List<Item> Deleted { get; set; }

        [JsonProperty("updated")]
        public List<Item> Updated { get; set; }
    }
    public class ValuesList
    {
        [JsonProperty("values")]
        public List<string> Values { get; set; }
        public ValuesList()
        {
            Values = new List<string>();
        }
        public ValuesList(List<string> _values)
        {
            Values = _values;
        }
    }
    #endregion

    #region Role
    public class Role
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("banned")]
        public bool Banned { get; set; }

        [JsonProperty("member_ids")]
        public List<long> MemberIds { get; set; }
    }
    public class Roles
    {
        [JsonProperty("roles")]
        public List<Role> RolesList { get; set; }
    }
    #endregion

    #region Answers
    public class Answer
    {
        [JsonProperty("response")]
        public string Response { get; set; }
    }
    public class Token
    {

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
    public class UploadedFile
    {
        [JsonProperty("guid")]
        public Guid Id { get; set; }

        [JsonProperty("md5_hash")]
        public string Md5_hash { get; set; }
    }
    public class ValueChoiceInfo : IValue
    {
        [JsonProperty("choice_id")]
        public int ChoiceId { get; set; }

        [JsonProperty("choice_ids")]
        public List<int> ChoiceIds { get; set; }

        [JsonProperty("choice_names")]
        public List<string> ChoiceNames { get; set; }
    }
    public class ItemOfHandbook
    {

        [JsonProperty("item_id")]
        public long ItemId { get; set; }

        [JsonProperty("item_ids")]
        public List<long> ItemIds { get; set; }

        [JsonProperty("headers")]
        public List<string> Headers { get; set; }

        [JsonProperty("values")]
        public List<string> Values { get; set; }

        [JsonProperty("rows")]
        public List<List<string>> Rows { get; set; }
    }

    #endregion

    #region RequestFieldValues

    public interface IFieldValueData { }
    public class ValueFieldData
    {
        /// <summary>
        /// </summary>
        /// <param name="_id">идентификатор поля</param>
        /// <param name="_valueData">любой из объектов реализующий интерфейс IFieldValueData (ValueChoiceData, ValueItemData, ValueIdData, ValueEmailData, ValueAttachmentData, ValueChannelData, ValueMultipleChoiceData, ValueTitleData, ValueFormLinkData), CheckmarkTypes, StatusTypes, List of ValueRowData , string, int, decimal, DateTime или ,при необходимости, любой другой объект </param>

        public ValueFieldData(int _id, string _valueData)
        {
            Id = _id;
            ValueData = _valueData;
        }
        public ValueFieldData(int _id, int _valueData)
        {
            Id = _id;
            ValueData = _valueData;
        }
        public ValueFieldData(int _id, long _valueData)
        {
            Id = _id;
            ValueData = _valueData;
        }
        public ValueFieldData(int _id, decimal _valueData)
        {
            Id = _id;
            ValueData = _valueData;
        }
        public ValueFieldData(int _id, IFieldValueData _valueData)
        {
            Id = _id;
            ValueData = _valueData;
        }
        public ValueFieldData(int _id, List<ValueRowData> _valueData)
        {
            Id = _id;
            ValueData = _valueData;
        }
        public ValueFieldData(int _id, DateTime _valueData, DateTimeFormatTypes dateTimeFormat)
        {
            Id = _id;
            switch (dateTimeFormat)
            {
                case DateTimeFormatTypes.Full:
                    ValueData = _valueData.ToString("yyyy-MM-ddTHH:mm:ss'Z'");
                    break;
                case DateTimeFormatTypes.DateOnly:
                    ValueData = _valueData.ToString("yyyy-MM-dd");
                    break;
                case DateTimeFormatTypes.TimeOnly:
                    ValueData = _valueData.ToString("HH:mm");
                    break;
            }
        }
        public ValueFieldData(int _id, CheckmarkTypes _valueData)
        {
            Id = _id;
            switch (_valueData)
            {
                case CheckmarkTypes.Checked:
                    ValueData = _valueData.GetEnumMemberValue();
                    break;
                case CheckmarkTypes.None:
                case CheckmarkTypes.Unchecked:
                default:
                    ValueData = CheckmarkTypes.Unchecked.GetEnumMemberValue();
                    break;
            }

        }
        public ValueFieldData(int _id, StatusTypes _valueData)
        {
            Id = _id;
            ValueData = _valueData.GetEnumMemberValue();
        }

        public ValueFieldData(int _id, object _valueData)
        {
            Id = _id;
            ValueData = _valueData;
        }


        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("value")]
        public object ValueData { get; set; }
    }

    public class ValueChoiceData : IFieldValueData
    {
        public ValueChoiceData(int id)
            => ChoiceId = id;

        [JsonProperty("choice_id")]
        public int ChoiceId { get; set; }
    }

    public class ValueItemData : IFieldValueData
    {
        public ValueItemData(long id)
            => ItemId = id;

        [JsonProperty("item_id")]
        public long ItemId { get; set; }
    }
    public class ValueIdData : IFieldValueData
    {
        public ValueIdData(long id)
            => Id = id;

        [JsonProperty("id")]
        public long Id { get; set; }
    }
    public class ValueEmailData : IFieldValueData
    {
        public ValueEmailData(string _email)
            => Email = _email;

        [JsonProperty("email")]
        public string Email { get; set; }
    }
    public class ValueAttachmentData : IFieldValueData
    {
        public ValueAttachmentData(Guid guid, Guid rootId)
        {
            Guid = guid;
            RootId = rootId;
        }
        public ValueAttachmentData(Guid attachmentId)
        {
            AttachmentId = attachmentId;
        }
        public ValueAttachmentData(string url, string name = "")
        {
            Url = url;
            Name = name;
        }
        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        [JsonProperty("root_id")]
        public Guid RootId { get; set; }

        [JsonProperty("attachment_id")]
        public Guid AttachmentId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
    public class ValueChannelData : IFieldValueData
    {
        public ValueChannelData(string type, string phone = null)
        {
            Type = type;
            Phone = phone;
        }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
    public class ValueMultipleChoiceData : IFieldValueData
    {
        public ValueMultipleChoiceData(List<int> choiceIds, List<ValueFieldData> fields)
        {
            ChoiceIds = choiceIds;
            Fields = fields;
        }
        [JsonProperty("choice_ids")]
        public List<int> ChoiceIds { get; set; }

        [JsonProperty("fields")]
        public List<ValueFieldData> Fields { get; set; }
    }
    public class ValueTitleData : IFieldValueData
    {
        public ValueTitleData(List<ValueFieldData> fields)
            => Fields = fields;

        [JsonProperty("fields")]
        public List<ValueFieldData> Fields { get; set; }
    }
    public class ValueFormLinkData : IFieldValueData
    {
        public ValueFormLinkData(List<long> taskIds)
            => TaskIds = taskIds;

        [JsonProperty("task_ids")]
        public List<long> TaskIds { get; set; }
    }
    public class ValueRowData
    {
        public ValueRowData(int rowId, List<ValueFieldData> cells, bool? delete = null)
        {
            RowId = rowId;
            Cells = cells;
            Delete = delete;
        }
        [JsonProperty("row_id")]
        public int RowId { get; set; }

        [JsonProperty("cells")]
        public List<ValueFieldData> Cells { get; set; }

        [JsonProperty("delete")]
        public bool? Delete { get; set; }
    }

    #endregion

    #region Announcements
    public class Announcement
    {
        [JsonProperty("announcement")]
        public AnnouncementInfo AnnouncementInfo { get; set; }
    }

    public class AnnouncementInfo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("formatted_text")]
        public string FormattedText { get; set; }

        [JsonProperty("create_date")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("author")]
        public ValuePersone Author { get; set; }

        [JsonProperty("comments")]
        public List<Comment> Comments { get; set; }
    }
    public class Announcements
    {
        [JsonProperty("announcements")]
        public List<AnnouncementInfo> AnnouncementsList { get; set; }
    }
    #endregion

    #region Others
    /// <summary>  
    /// ValueClasses for fields types [FieldTypes enum]:<br/>
    /// - ValueString [Text, Phone, Time, Note, Email]<br/>
    /// - ValueDate [DueDate, CreationDate, Date, DueDateTime]<br/>
    /// - ValueNumber [Money, Number]<br/>
    /// - ValueProject [Project]<br/>
    /// - ValueFormLink [FormLink]<br/>
    /// - ValueTitle [Title]<br/>
    /// - ValueMultipleChoice [MultipleChoice]<br/>
    /// - ValueTable [Table]<br/>
    /// - ValuePersone [Author, Person]<br/>
    /// - ValueFiles [File]<br/>
    /// - ValueCatalog [Catalog]<br/>
    /// - ValueCheckmark [Checkmark, Flag]<br/>
    /// - ValueIntegerNumber [Step]<br/>
    /// - ValueStatus [Status]<br/>
    /// <para>  
    /// If a conversion error occurs, then 'ValueError'
    /// </para>  
    /// </summary>  
    public interface IValue { }
    public class ValueString : IValue
    {
        [JsonProperty("string")]
        public string String { get; set; }
    }
    public class ValueFiles : IValue
    {
        [JsonProperty("files")]
        public List<Attachment> Files { get; set; }
    }
    public class ValueNumber : IValue
    {
        [JsonProperty("number")]
        public decimal Number { get; set; }
    }
    public class ValueIntegerNumber : IValue
    {
        [JsonProperty("integerNumber")]
        public int IntegerNumber { get; set; }
    }
    public class ValueBigIntegerNumber : IValue
    {
        [JsonProperty("bigIntegerNumber")]
        public long IntegerNumber { get; set; }
    }
    public class ValueStatus : IValue
    {
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusTypes Status { get; set; }
    }
    public class ValueError : IValue
    {
        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
    }
    public class ValueDate : IValue
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
    public class ValueMultipleChoice : IValue
    {
        [JsonProperty("choice_ids")]
        public List<int> ChoiceIds { get; set; }

        [JsonProperty("choice_names")]
        public List<string> ChoiceNames { get; set; }

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<Field> Fields { get; set; }
    }
    public class ValueProject : IValue
    {
        [JsonProperty("projects")]
        public List<Project> Projects { get; set; }
    }
    public class ValueCheckmark : IValue
    {
        [JsonProperty("checkmark")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CheckmarkTypes Checkmark { get; set; }
    }
    public class Parent
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Project
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parent")]
        public Parent Parent { get; set; }
    }
    public class ValueFormLink : IValue
    {
        [JsonProperty("task_ids")]
        public List<long> TaskIds { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }
    }
    public class ValueTitle : IValue
    {
        [JsonProperty("checkmark")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CheckmarkTypes Checkmark { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }
    }
    public class ChildFieldsInfo
    {
        [JsonProperty("checkmark")]
        public string Checkmark { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }
    }
    public class Column
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tooltip")]
        public string Tooltip { get; set; }

        [JsonProperty("parent_id")]
        public long ParentId { get; set; }

        [JsonProperty("visibility_condition")]
        public VisibilityCondition VisibilityCondition { get; set; }

        [JsonProperty("info")]
        public Info Info { get; set; }
    }
    public class Info
    {
        [JsonProperty("columns")]
        public List<Column> Columns { get; set; }

        [JsonProperty("is_table")]
        public bool IsTable { get; set; }

        [JsonProperty("default_prefix")]
        public string DefaultPrefix { get; set; }

        [JsonProperty("required_step")]
        public int? RequiredStep { get; set; }

        [JsonProperty("is_required")]
        public bool? IsRequired { get; set; }

        [JsonProperty("is_form_title")]
        public bool? IsFormTitle { get; set; }

        [JsonProperty("large_view")]
        public bool? LargeView { get; set; }

        [JsonProperty("options")]
        public List<Option> Options { get; set; }

        [JsonProperty("multiline")]
        public bool? Multiline { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("immutable_step")]
        public int? ImmutableStep { get; set; }

        [JsonProperty("decimal_places")]
        public long? DecimalPlaces { get; set; }

        [JsonProperty("catalog_id")]
        public long? CatalogId { get; set; }

        [JsonProperty("multiple_choice")]
        public ValueMultipleChoice MultipleChoice { get; set; }

        [JsonProperty("form_id")]
        public long? FormId { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }

        [JsonProperty("group_display_type")]
        public string GroupDisplayType { get; set; }

        [JsonProperty("max_length")]
        public long? MaxLength { get; set; }

        [JsonProperty("display_as")]
        public long? DisplayAs { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("default_catalog_enabled")]
        public bool? DefaultCatalogEnabled { get; set; }

        [JsonProperty("default_catalog_id")]
        public long? DefaultCatalogId { get; set; }

        [JsonProperty("mask")]
        public string Mask { get; set; }

    }
    public class Option
    {
        [JsonProperty("choice_id")]
        public int ChoiceId { get; set; }

        [JsonProperty("choice_value")]
        public string ChoiceValue { get; set; }

        [JsonProperty("deleted")]
        public bool? Deleted { get; set; }

        [JsonProperty("fields")]
        public List<Field> Fields { get; set; }
    }
    public class VisibilityCondition
    {

        [JsonProperty("field_id")]
        public int FieldId { get; set; }

        [JsonProperty("condition_type")]
        public int ConditionType { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("children")]
        public List<VisibilityCondition> Children { get; set; }
    }
    public class Child
    {
        [JsonProperty("field_id")]
        public int FieldId { get; set; }

        [JsonProperty("condition_type")]
        public int ConditionType { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("children")]
        public object Children { get; set; }
    }
    public class PyrusLists
    {
        [JsonProperty("lists")]
        public List<PyrusList> Lists { get; set; }
    }
    public class PyrusList
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("children")]
        public List<Child> Children { get; set; }
    }
    public class PyrusListChild
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    #endregion
}

