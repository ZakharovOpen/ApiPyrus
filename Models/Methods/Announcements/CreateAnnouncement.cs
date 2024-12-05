using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Announcements
{
    public class CreateAnnouncement
    {
        private PyrusRequestTask _entityForJson = new PyrusRequestTask();

        /// <summary>
        /// Создать объявление в Pyrus.
        /// </summary>
        public async Task<AnnouncementInfo> Send(ApiClient apiClient, string extRequestId = "")
            => await apiClient.CreateAnnouncement(_entityForJson.GetJson(), extRequestId);

        /// <summary>
        /// Добавить текст.
        /// text - текст сообщение
        /// formatedText - true, если текст комментария содержит форматирование html.
        /// </summary>
        public CreateAnnouncement AddText(string text, bool formatedText = false)
        {
            if (formatedText)
                _entityForJson.FormattedTtext = text;
            else
                _entityForJson.Text = text;
            return this;
        }

        /// <summary>
        /// Добавить файлы.
        /// Массив строк, каждая из которых может быть:
        /// Guid загруженного файла. Загрузить файл и получить его идентификатор можно с помощью метода files/upload.
        /// Или id уже приложенного к какой-либо задаче файла.
        /// </summary>
        public CreateAnnouncement AddAttachments(List<Guid> attachments)
        {
            _entityForJson.AttachmentsList = attachments;
            return this;
        }

        /// <summary>
        /// Добавить файлы.
        /// Массив строк, каждая из которых может быть
        /// url уже приложенного к какой-либо задаче файла (можно указать и URL произвольного файла из сети, но в этом случае нельзя будет указать к нему имя ссылки).
        /// </summary>
        public CreateAnnouncement AddAttachments(List<string> attachments)
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
        public CreateAnnouncement AddAttachments(List<ValueAttachmentData> attachments)
        {
            _entityForJson.AttachmentsList = attachments;
            return this;
        }

        /// <summary>
        /// Добавить файлы.
        /// fullFileName - полоное имя файла, включая расположение
        /// apiClient - pyrus api клиент
        /// </summary>
        public async Task<CreateAnnouncement> AddAttachments(string fullFileName, ApiClient apiClient)
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

    }
}
