using Newtonsoft.Json;
using System.Collections.Generic;

namespace ApiPyrus.Models.DTOs
{
    public class PyrusRequestCatalogItems
    {
        [JsonProperty("upsert")]
        public List<ValuesList> UpsertItems { get; set; }

        [JsonProperty("delete")]
        public List<string> DeleteItemsKeys { get; set; }
    }
}
