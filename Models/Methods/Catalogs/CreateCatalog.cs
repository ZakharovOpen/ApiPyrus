using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Catalogs
{
    public class CreateCatalog
    {
        public CreateCatalog(string catalogName)
        {
            _entityForJson.Name = catalogName;
            _entityForJson.CatalogHeaders = new List<string>();
            _entityForJson.Items = new List<ValuesList>();
        }

        private PyrusRequestCatalog _entityForJson = new PyrusRequestCatalog();


        /// <summary>
        /// Отправить запрос на создание справочника.
        /// </summary>
        public async Task<Catalog> Send(ApiClient apiClient, string extRequestId = "")
            => await apiClient.CreateCatalog(_entityForJson.GetJson(), extRequestId);


        /// <summary>
        /// Добавляем заголовки справочника.
        /// </summary>
        public CreateCatalog AddHeaders(List<string> headers)
        {
            _entityForJson.CatalogHeaders = headers;
            return this;
        }

        /// <summary>
        /// Добавляем позицию справочника.
        /// </summary>
        public CreateCatalog AddItem(ValuesList item)
        {
            if (_entityForJson.Items == null)
                _entityForJson.Items = new List<ValuesList>();
            _entityForJson.Items.Add(item);
            return this;
        }

        /// <summary>
        /// Добавляем позиуии справочника.
        /// </summary>
        public CreateCatalog AddItems(List<ValuesList> items)
        {
            if (_entityForJson.Items == null)
                _entityForJson.Items = new List<ValuesList>();
            _entityForJson.Items.AddRange(items);
            return this;
        }
    }
}
