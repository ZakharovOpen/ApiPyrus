using ApiPyrus.Extentions;
using ApiPyrus.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPyrus.Models.Methods.Catalogs
{
    public class UpdateCatalog
    {
        public UpdateCatalog(long catalogId, ApiClient apiClient, bool isApply = true)
        {
            _catalogId = catalogId;
            _entityForJson.Apply = isApply;
            _entityForJson.CatalogHeaders = new List<string>();
            _entityForJson.Items = new List<ValuesList>();
            _apiClient = apiClient;
        }

        private PyrusRequestCatalog _entityForJson = new PyrusRequestCatalog();
        private long _catalogId;
        private ApiClient _apiClient;

        /// <summary>
        /// Отправить запрос на обновление справочника.
        /// </summary>
        public async Task<CatalogUpdateInfo> Send(string extRequestId = "")
            => await _apiClient.UpdateCatalog(_catalogId, _entityForJson.GetJson(), extRequestId);

        /// <summary>
        /// Устанавливаем заголовки и позиции справочника из текущего справочника.
        /// <param name="catalog" - каталог из которого необходимо установить заголовки, если null то метод будет брать данные запросом в api/>
        /// </summary>
        public async Task<UpdateCatalog> SetCatalog(Catalog catalog = null)
        {
            if (catalog == null)
                catalog = await _apiClient.GetCatalog(_catalogId);
            return await (await SetHeaders(catalog)).SetItems(catalog);
        }

        /// <summary>
        /// Устанавливаем заголовки справочника из текущего справочника.
        /// <param name="catalog" - каталог из которого необходимо установить заголовки, если null то метод будет брать данные запросом в api/>
        /// </summary>
        public async Task<UpdateCatalog> SetHeaders(Catalog catalog = null)
        {
            if (catalog == null)
                catalog = await _apiClient.GetCatalog(_catalogId);
            _entityForJson.CatalogHeaders = catalog.CatalogHeaders.Select(x => x.Name).ToList();
            return this;
        }

        /// <summary>
        /// Добавляем заголовки справочника.
        /// </summary>
        public UpdateCatalog AddHeaders(List<string> headers)
        {
            _entityForJson.CatalogHeaders = headers;
            return this;
        }

        /// <summary>
        /// Устанавливаем позиции справочника из текущего справочника.
        /// <param name="catalog" - каталог из которого необходимо установить заголовки, если null то метод будет брать данные запросом в api/>
        /// </summary>
        public async Task<UpdateCatalog> SetItems(Catalog catalog = null)
        {
            if (catalog == null)
                catalog = await _apiClient.GetCatalog(_catalogId);
            _entityForJson.Items = catalog.Items.Select(x => new ValuesList(x.Values)).ToList();
            return this;
        }

        /// <summary>
        /// Добавляем позицию справочника.
        /// </summary>
        public UpdateCatalog AddOrUpdateItem(int idntifyIndex, string idntifyValue, List<string> itemValues)
        {
            if (_entityForJson.Items == null || _entityForJson.Items.Count == 0)
                return AddItem(new ValuesList(itemValues));
            var entity = _entityForJson.Items.FirstOrDefault(x => x.Values[idntifyIndex] == idntifyValue);
            if (entity == null)
                return AddItem(new ValuesList(itemValues));
            else
            {
                var count = Math.Min(entity.Values.Count, itemValues.Count);
                for (int i = 0; i < count; i++)
                    entity.Values[i] = itemValues[i];
            }
            return this;
        }

        /// <summary>
        /// Добавляем позицию справочника.
        /// </summary>
        public UpdateCatalog AddItem(ValuesList item)
        {
            if (_entityForJson.Items == null)
                _entityForJson.Items = new List<ValuesList>();
            _entityForJson.Items.Add(item);
            return this;
        }

        /// <summary>
        /// Добавляем позиции справочника.
        /// </summary>
        public UpdateCatalog AddItems(List<ValuesList> items)
        {
            if (_entityForJson.Items == null)
                _entityForJson.Items = new List<ValuesList>();
            _entityForJson.Items.AddRange(items);
            return this;
        }
    }
}
