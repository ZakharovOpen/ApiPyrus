# Test Plan

> Тестовый проект `ApiPyrusTester` расположен вне этого репозитория: `../ApiPyrusTester_net_standart/ApiPyrusTester.csproj`.
> Он подключён в `ApiPyrus.sln`.

## Unit Tests

### ApiClient

- `Constructor_DefaultParameters_SetsCorrectDefaults`
- `Constructor_WithTimeout_SetsHttpClientTimeout`
- `Constructor_IgnoreSsl_ConfiguresServicePointManager`
- `CheckSig_ValidSignature_ReturnsTrue`
- `CheckSig_InvalidSignature_ReturnsFalse`
- `CheckSig_WithExtRequestId_LogsRequestId`
- `ChangeToken_SetsTokenDirectly`
- `ApiRequest_NoToken_CallsAuthFirst`
- `ApiRequest_401Response_ReauthsAndRetries`
- `ApiRequest_NonOkResponse_ThrowsExceptionWithStatusAndBody`
- `GetFormsTemplates_ReturnsListOfForms`
- `GetCatalogs_ReturnsListOfCatalogs`
- `GetCatalog_ById_ReturnsCatalog`
- `GetMembers_ReturnsListOfPersone`
- `GetMember_ById_ReturnsPersone`
- `GetProfile_ReturnsPersone`
- `GetProfile_IncludeInactive_AddsQueryParam`
- `GetTasks_WithQueryString_BuildsCorrectUrl`
- `GetTasks_WithDictionary_BuildsQueryFromParams`
- `GetTasks_WithFieldsDictionary_PrefixesFieldIds`
- `GetTasks_IncludeArchived_AddsQueryParam`
- `GetTaskInfoById_ReturnsTask`
- `GetRoles_ReturnsListOfRoles`
- `GetLists_ReturnsListOfPyrusLists`
- `GetListTasks_WithDateRange_FormatsIso8601`
- `GetInbox_ReturnsTasksList`
- `GetAnnouncements_ReturnsList`
- `GetAnnouncement_ById_ReturnsAnnouncementInfo`
- `UploadFile_ReturnsGuid`
- `DownloadFile_ToStream_ReturnsStream`
- `DownloadFile_ToFile_WritesToDisk`
- `DownloadFile_401Response_ReauthsAndRetries`
- `AddScheduledDate_DelegatesToUpdateTaskByForm`
- `AddScheduledDatetimeUtc_DelegatesToUpdateTaskByForm`
- `CancelSchedule_DelegatesToUpdateTaskByForm`

### FieldWithValue / Field

- `GetValue_TextType_ReturnsValueString`
- `GetValue_DateType_ReturnsValueDate`
- `GetValue_MoneyType_ReturnsValueNumber`
- `GetValue_ProjectType_ReturnsValueProject`
- `GetValue_TableType_ReturnsValueTable`
- `GetValue_PersonType_ReturnsValuePersone`
- `GetValue_FileType_ReturnsValueFiles`
- `GetValue_CatalogType_ReturnsValueCatalog`
- `GetValue_CheckmarkType_ReturnsValueCheckmark`
- `GetValue_StatusType_ReturnsValueStatus`
- `GetValue_StepType_ReturnsValueIntegerNumber`
- `GetValue_NullValue_ReturnsValueError`
- `GetValue_UnsupportedType_ReturnsValueError`
- `GetValue_WrongCast_ThrowsInvalidCastException`
- `GetValueObject_ParseError_ReturnsValueError`

### Comment

- `GetOnlyReplyCommentText_WithReply_ReturnsTextAfterNewline`
- `GetOnlyReplyCommentText_NoReply_ReturnsFullText`
- `GetCommentTextForReply_WithReply_ReturnsFirstLine`
- `GetFormattedTextWithoutReplyComment_WithQuote_ReturnsAfterQuote`
- `GetCommentFormattedTextForReply_WithQuote_ReturnsBeforeQuote`

### Builders — Tasks

- `CreateTaskByForm_AddField_AddsToFieldsList`
- `CreateTaskByForm_AddFields_AddsRangeToFieldsList`
- `CreateTaskByForm_AddSubscribers_ById_SetsSubscribers`
- `CreateTaskByForm_AddSubscribers_ByEmail_SetsSubscribers`
- `CreateTaskByForm_AddApprovals_ById_SetsApprovals`
- `CreateTaskByForm_AddParentTaskId_SetsParentId`
- `CreateTaskByForm_AddListsIds_SetsListIds`
- `CreateTaskByForm_ScheduledDate_SetsDate`
- `CreateTaskByForm_FillDefaults_SetsFlag`
- `CreateTaskByForm_FluentChaining_ReturnsThis`
- `UpdateTaskByForm_UpdateField_AddsToFieldUpdates`
- `UpdateTaskByForm_UpdateFields_AddsRangeToFieldUpdates`
- `UpdateTaskByForm_AddText_SetsText`
- `UpdateTaskByForm_AddText_Formatted_SetsFormattedText`
- `UpdateTaskByForm_AddChannel_SetsChannelTypeAndPhone`
- `UpdateTaskByForm_AddAttachments_Guids_SetsAttachments`
- `UpdateTaskByForm_AddAttachments_Urls_SetsAttachments`
- `UpdateTaskByForm_AddAttachments_ValueAttachmentData_SetsAttachments`
- `UpdateTaskByForm_AddApprovalChoice_SetsDescription`
- `UpdateTaskByForm_SkipSatisfaction_SetsFlag`
- `UpdateTaskByForm_SkipNotification_SetsFlag`
- `UpdateTaskByForm_EditComment_SetsCommentId`
- `UpdateTaskByForm_ReplyComment_FormatsQuoteHtml`
- `UpdateTaskByForm_CancelSchedule_SetsFlag`
- `CloseTaskByForm_SetsActionFinished`
- `ReopenTaskByForm_SetsActionReopened`

### Builders — Simple Tasks

- `CreateSimpleTask_AddText_SetsText`
- `CreateSimpleTask_AddSubject_SetsSubject`
- `CreateSimpleTask_AddDueDate_SetsDueDate`
- `CreateSimpleTask_AddDue_SetsDue`
- `CreateSimpleTask_AddDuration_SetsDuration`
- `CreateSimpleTask_AddResponsible_ById_SetsResponsible`
- `CreateSimpleTask_AddParticipants_SetsParticipants`
- `UpdateSimpleTask_UpdateSubject_SetsSubject`
- `UpdateSimpleTask_ReassignTo_ById_SetsReassignTo`
- `UpdateSimpleTask_AddSpentMinutesInfo_SetsMinutes`

### Builders — Catalogs

- `CreateCatalog_AddHeaders_SetsHeaders`
- `CreateCatalog_AddItem_AddsToItems`
- `CreateCatalog_AddItems_AddsRange`
- `UpdateCatalog_SetCatalog_FetchesAndSetsHeadersAndItems`
- `UpdateCatalog_SetHeaders_FetchesHeaders`
- `UpdateCatalog_AddOrUpdateItem_ExistingItem_UpdatesValues`
- `UpdateCatalog_AddOrUpdateItem_NewItem_AddsItem`
- `UpdateCatalogItems_AddItemToUpsert_AddsItem`
- `UpdateCatalogItems_AddItemsToUpsert_AddsRange`
- `UpdateCatalogItems_AddItemToDelete_AddsKey`
- `UpdateCatalogItems_AddItemsToDelete_AddsKeys`

### Builders — Members, Roles, Announcements

- `CreateMember_ConstructorSetsAllFields`
- `UpdateMember_NullFields_NotIncludedInJson`
- `BlockMember_SendsDeleteRequest`
- `CreateRole_SetsNameAndMembers`
- `UpdateRole_SetsPartialFields`
- `CreateAnnouncement_AddText_SetsText`
- `CreateAnnouncement_AddAttachments_SetsAttachments`
- `CommentAnnouncement_AddText_SetsText`

### Extensions — PyrusExtentions

- `GetJson_DefaultSettings_IgnoresNullAndDefault`
- `GetJson_CustomSettings_UsesProvidedSettings`
- `GetFieldById_TopLevel_ReturnsField`
- `GetFieldById_InTitle_ReturnsNestedField`
- `GetFieldById_InMultipleChoice_ReturnsNestedField`
- `GetFieldById_NotFound_ReturnsNull`
- `TryGetFieldById_Found_ReturnsTrueAndField`
- `TryGetFieldById_NotFound_ReturnsFalse`
- `GetFieldsByType_TopLevel_ReturnsFields`
- `GetFieldsByType_InTitleAndMultipleChoice_ReturnsAll`
- `TryGetFieldsByType_NoMatch_ReturnsFalse`

### Extensions — HashExtensions

- `ToHexString_ValidHash_ReturnsHexUppercase`
- `ToHexString_LowerCase_ReturnsHexLowercase`
- `ToHexString_EmptyArray_ReturnsEmptyString`
- `ToHexString_NullArray_ThrowsArgumentNullException`

### Enums — EnumExtension

- `GetEnumMemberValue_ReturnsEnumMemberAttribute`
- `GetDescription_ReturnsDescriptionAttribute`
- `GetDescription_NoAttribute_ReturnsToString`

### ValueFieldData

- `Constructor_String_SetsIdAndValue`
- `Constructor_Int_SetsIdAndValue`
- `Constructor_Decimal_SetsIdAndValue`
- `Constructor_IFieldValueData_SetsIdAndValue`
- `Constructor_DateTime_Full_FormatsCorrectly`
- `Constructor_DateTime_DateOnly_FormatsCorrectly`
- `Constructor_DateTime_TimeOnly_FormatsCorrectly`
- `Constructor_Checkmark_Checked_SetsEnumMember`
- `Constructor_Checkmark_Unchecked_SetsUnchecked`
- `Constructor_Status_SetsEnumMember`
- `Constructor_ListOfRowData_SetsRows`

## Integration Tests

### Pyrus API (требуют реальные credentials)

- `Auth_ValidCredentials_ReturnsToken`
- `Auth_InvalidCredentials_ThrowsException`
- `GetFormsTemplates_ReturnsNonEmptyList`
- `GetCatalogs_ReturnsNonEmptyList`
- `CreateAndGetCatalog_RoundTrip_Success`
- `CreateTask_ByForm_ReturnsTaskWithId`
- `UpdateTask_AddComment_ReturnsUpdatedTask`
- `CloseAndReopen_Task_ChangesStatus`
- `UploadAndDownloadFile_RoundTrip_Success`
- `GetMembers_ReturnsNonEmptyList`
- `GetRoles_ReturnsNonEmptyList`

## Test Data

Для unit-тестов: mock JSON responses, имитирующие Pyrus API.
Для integration-тестов: тестовый аккаунт Pyrus с pre-configured формами и справочниками.
