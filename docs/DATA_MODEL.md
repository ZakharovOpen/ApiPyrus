# Data Model

## Entity Relationships

```
PyrusTask ──┬── List<Field>        (поля задачи)
            ├── List<Comment>      (комментарии)
            ├── List<Attachment>   (вложения)
            ├── ValuePersone       (автор)
            ├── List<Subscriber>   (наблюдатели)
            └── List<PyrusStep>    (шаги согласования)

Field ──────┬── FieldTypes         (тип поля — enum)
            ├── IValue             (типизированное значение)
            ├── Info               (метаданные поля)
            └── VisibilityCondition

Catalog ────┬── List<CatalogHeader>
            └── List<Item>

ValuePersone ─── Organization ─── List<Role>

AnnouncementInfo ─── List<Comment>

Form ───────┬── List<Field>
            └── List<PrintForm>
```

## Main Entities

### PyrusTask
| Field | Type | JSON | Description |
|---|---|---|---|
| Id | long | `id` | ID задачи |
| Text | string | `text` | Текст задачи |
| FormattedTtext | string | `formatted_text` | HTML-форматированный текст |
| CreateDate | DateTime | `create_date` | Дата создания |
| LastModifiedDate | DateTime | `last_modified_date` | Дата последнего изменения |
| CloseDate | DateTime? | `close_date` | Дата закрытия |
| Author | ValuePersone | `author` | Автор |
| Due | string | `due` | Срок |
| ListIds | List\<long\> | `list_ids` | ID списков |
| FormId | long | `form_id` | ID формы |
| CurrentStep | int | `current_step` | Текущий шаг согласования |
| ParentTaskId | long? | `parent_task_id` | ID надзадачи |
| IsClosed | bool | `is_closed` | Закрыта ли задача |
| Fields | List\<Field\> | `fields` | Поля |
| Comments | List\<Comment\> | `comments` | Комментарии |
| Attachments | List\<Attachment\> | `attachments` | Вложения |
| Subscribers | List\<Subscriber\> | `subscribers` | Наблюдатели |
| Steps | List\<PyrusStep\> | `steps` | Шаги |
| ScheduledDate | string | `scheduled_date` | Запланированная дата (YYYY-MM-DD) |
| ScheduledDatetimeUtc | string | `scheduled_datetime_utc` | Запланированное время UTC |

### Field (extends FieldWithValue)
| Field | Type | JSON | Description |
|---|---|---|---|
| Id | long | `id` | ID поля |
| Name | string | `name` | Имя |
| Type | FieldTypes | `type` | Тип поля (enum) |
| Value | object | `value` | Сырое значение |
| Tooltip | string | `tooltip` | Подсказка |
| Info | Info | `info` | Метаданные |
| Default_value | string | `default_value` | Значение по умолчанию |
| ParentId | long | `parent_id` | ID родительского поля |
| RelatedFieldId | long? | `related_field_id` | ID связанного поля |
| VisibilityCondition | VisibilityCondition | `visibility_condition` | Условие видимости |

### Comment
| Field | Type | JSON | Description |
|---|---|---|---|
| Id | long | `id` | ID комментария |
| Text | string | `text` | Текст |
| FormattedText | string | `formatted_text` | HTML-текст |
| CreateDate | DateTime | `create_date` | Дата создания |
| Author | ValuePersone | `author` | Автор |
| ReassignedTo | ValuePersone | `reassigned_to` | Переназначен на |
| FieldUpdates | List\<Field\> | `field_updates` | Обновления полей |
| Attachments | List\<Attachment\> | `attachments` | Вложения |
| SmsInfo | SmsInfo | `sms_info` | SMS-информация |
| ReplyNoteId | long? | `reply_note_id` | ID цитируемого комментария |
| EditCommentId | long? | `edit_comment_id` | ID редактируемого комментария |

### ValuePersone (implements IValue)
| Field | Type | JSON | Description |
|---|---|---|---|
| Id | long | `id` | ID пользователя |
| FirstName | string | `first_name` | Имя |
| LastName | string | `last_name` | Фамилия |
| Email | string | `email` | Email |
| Type | string | `type` | Тип |
| DepartmentId | long? | `department_id` | ID отдела |
| DepartmentName | string | `department_name` | Название отдела |
| Banned | bool? | `banned` | Заблокирован |
| Position | string | `position` | Должность |
| Phone | string | `phone` | Телефон |
| OrganizationId | long? | `organization_id` | ID организации |
| Organization | Organization | `organization` | Организация |

### Catalog
| Field | Type | JSON | Description |
|---|---|---|---|
| CatalogId | long | `catalog_id` | ID справочника |
| Name | string | `name` | Название |
| CatalogHeaders | List\<CatalogHeader\> | `catalog_headers` | Заголовки |
| Items | List\<Item\> | `items` | Позиции |
| Version | string | `version` | Версия |
| Deleted | bool | `deleted` | Удалён |
| ExternalVersion | string | `external_version` | Внешняя версия |

### Role
| Field | Type | JSON | Description |
|---|---|---|---|
| Id | long | `id` | ID роли |
| Name | string | `name` | Название |
| Banned | bool | `banned` | Заблокирована |
| MemberIds | List\<long\> | `member_ids` | ID участников |

### AnnouncementInfo
| Field | Type | JSON | Description |
|---|---|---|---|
| Id | long | `id` | ID объявления |
| Text | string | `text` | Текст |
| FormattedText | string | `formatted_text` | HTML-текст |
| CreateDate | DateTime | `create_date` | Дата создания |
| Author | ValuePersone | `author` | Автор |
| Comments | List\<Comment\> | `comments` | Комментарии |

### Form
| Field | Type | JSON | Description |
|---|---|---|---|
| Id | long | `id` | ID формы |
| Name | string | `name` | Название |
| DefaultPersonId | long | `default_person_id` | Ответственный по умолчанию |
| DeletedOrClosed | bool | `deleted_or_closed` | Удалена/закрыта |
| Folder | List\<string\> | `folder` | Путь в папках |
| Steps | Dictionary\<string, string\> | `steps` | Шаги |
| Fields | List\<Field\> | `fields` | Поля формы |
| PrintForms | List\<PrintForm\> | `print_forms` | Печатные формы |

## Enumerations

### FieldTypes
Тип поля задачи. Сериализуется через `StringEnumConverter` + `EnumMember`.

| Value | EnumMember | IValue class |
|---|---|---|
| None | `none` | ValueError |
| Text | `text` | ValueString |
| Phone | `phone` | ValueString |
| Time | `time` | ValueString |
| Note | `note` | ValueString |
| Email | `email` | ValueString |
| Date | `date` | ValueDate |
| DueDate | `due_date` | ValueDate |
| DueDateTime | `due_date_time` | ValueDate |
| CreationDate | `creation_date` | ValueDate |
| Money | `money` | ValueNumber |
| Number | `number` | ValueNumber |
| Project | `project` | ValueProject |
| FormLink | `form_link` | ValueFormLink |
| Title | `title` | ValueTitle |
| MultipleChoice | `multiple_choice` | ValueMultipleChoice |
| Table | `table` | ValueTable |
| Author | `author` | ValuePersone |
| Person | `person` | ValuePersone |
| File | `file` | ValueFiles |
| Catalog | `catalog` | ValueCatalog |
| Checkmark | `checkmark` | ValueCheckmark |
| Flag | `flag` | ValueCheckmark |
| Step | `step` | ValueIntegerNumber |
| Status | `status` | ValueStatus |

### ChannelTypes
Канал отправки комментария. Сериализуется через `Description`.

| Value | Description | Int |
|---|---|---|
| Email | `email` | 1 |
| Telegram | `telegram` | 2 |
| Facebook | `facebook` | 3 |
| Vkontakte | `vk` | 4 |
| Viber | `viber` | 5 |
| Instagram | `instagram` | 6 |
| PrivateChannel | `private_channel` | 7 |
| WhatsApp | `whats_app` | 8 |
| WebWidget | `web_widget` | 9 |
| MobileApp | `mobile_app` | 10 |
| SMS | `sms` | 11 |

### ActionTypes
| Value | Description | Int |
|---|---|---|
| Finished | `finished` | 1 |
| Reopened | `reopened` | 2 |

### ApprovalTypes
| Value | Description | Int |
|---|---|---|
| Approved | `approved` | 1 |
| Acknowledged | `acknowledged` | 2 |
| Rejected | `rejected` | 3 |
| Revoked | `revoked` | 4 |

### CheckmarkTypes
| Value | EnumMember | Int |
|---|---|---|
| None | `none` | 1 |
| Checked | `checked` | 2 |
| Unchecked | `unchecked` | 3 |

### StatusTypes
| Value | EnumMember | Int |
|---|---|---|
| Open | `open` | 1 |
| Closed | `closed` | 2 |

### DateTimeFormatTypes
| Value | Format | Int |
|---|---|---|
| Full | `YYYY-MM-DDThh:mm:ssZ` | 0 |
| DateOnly | `YYYY-MM-DD` | 1 |
| TimeOnly | `HH:mm` | 2 |

## IValue Interface Hierarchy

```
IValue (marker interface)
├── ValueString         — Text, Phone, Time, Note, Email
├── ValueDate           — DueDate, CreationDate, Date, DueDateTime
├── ValueNumber         — Money, Number (decimal)
├── ValueIntegerNumber  — Step (int)
├── ValueBigIntegerNumber — (long)
├── ValueProject        — Project
├── ValueFormLink       — FormLink
├── ValueTitle          — Title
├── ValueMultipleChoice — MultipleChoice
├── ValueTable          — Table
├── ValuePersone        — Author, Person
├── ValueFiles          — File
├── ValueCatalog        — Catalog
├── ValueCheckmark      — Checkmark, Flag
├── ValueStatus         — Status
├── ValueChoiceInfo     — (choice_id/choice_ids)
└── ValueError          — ошибка парсинга
```

## Request DTOs (Builder Models)

### PyrusRequestTask
Используется всеми Task/Announcement builders. Содержит все возможные поля для создания/обновления задач.
Файл: `Models/DTOs/PyrusRequestTask.cs`

### PyrusRequestCatalog
Используется CreateCatalog, UpdateCatalog.
Файл: `Models/DTOs/PyrusRequestCatalog.cs`

### PyrusRequestCatalogItems
Используется UpdateCatalogItems (diff-обновление).
Файл: `Models/DTOs/PyrusRequestUpdateCatalogItems.cs`

### PyrusRequestMember
Используется CreateMember, UpdateMember.
Файл: `Models/DTOs/PyrusRequestMember.cs`

### PyrusRequestRole
Используется CreateRole, UpdateRole.
Файл: `Models/DTOs/PyrusRequestRole.cs`

## IFieldValueData — типы значений для запросов

```
IFieldValueData (marker interface)
├── ValueChoiceData       — {choice_id: int}
├── ValueItemData         — {item_id: long}
├── ValueIdData           — {id: long}
├── ValueEmailData        — {email: string}
├── ValueAttachmentData   — {guid, root_id} | {attachment_id} | {url, name}
├── ValueChannelData      — {type, phone}
├── ValueMultipleChoiceData — {choice_ids, fields}
├── ValueTitleData        — {fields}
└── ValueFormLinkData     — {task_ids}
```
