using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Catalogs
{
    public class UpdateCatalogItems
    {
        public UpdateCatalogItems(long catalogId, ApiClient apiClient)
        {
            _catalogId = catalogId;
            _apiClient = apiClient;
        }

        private PyrusRequestCatalogItems _entityForJson = new PyrusRequestCatalogItems();
        private long _catalogId;
        private ApiClient _apiClient;

        /// <summary>
        /// Отправить запрос на обновление справочника.
        /// </summary>
        public async Task<CatalogUpdateInfo> Send(string extRequestId = "")
            => await _apiClient.UpdateCatalogDiff(_catalogId, _entityForJson.GetJson(), extRequestId);

        /// <summary>
        /// Добавляем позиции справочника для добавления/обновления.
        /// </summary>
        public UpdateCatalogItems AddItemsToUpsert(List<ValuesList> items)
        {
            if (_entityForJson.UpsertItems == null)
                _entityForJson.UpsertItems = new List<ValuesList>();
            _entityForJson.UpsertItems.AddRange(items);
            return this;
        }

        /// <summary>
        /// Добавляем позицию справочника для добавления/обновления.
        /// </summary>
        public UpdateCatalogItems AddItemToUpsert(ValuesList item)
        {
            if (_entityForJson.UpsertItems == null)
                _entityForJson.UpsertItems = new List<ValuesList>();
            _entityForJson.UpsertItems.Add(item);
            return this;
        }

        /// <summary>
        /// Добавляем позиции справочника для удаления.
        /// </summary>
        public UpdateCatalogItems AddItemsToDelete(List<string> itemsKeys)
        {
            if (_entityForJson.DeleteItemsKeys == null)
                _entityForJson.DeleteItemsKeys = new List<string>();
            _entityForJson.DeleteItemsKeys.AddRange(itemsKeys);
            return this;
        }


        /// <summary>
        /// Добавляем позицию справочника для удаления.
        /// </summary>
        public UpdateCatalogItems AddItemToDelete(string itemKey)
        {
            if (_entityForJson.DeleteItemsKeys == null)
                _entityForJson.DeleteItemsKeys = new List<string>();
            _entityForJson.DeleteItemsKeys.Add(itemKey);
            return this;
        }
    }
}
