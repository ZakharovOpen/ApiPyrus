using Newtonsoft.Json;
using System.Collections.Generic;

namespace ApiPyrus.Models.DTOs
{
    public class PyrusRequestCatalog
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("apply")]
        public bool? Apply { get; set; }

        [JsonProperty("catalog_headers")]
        public List<string> CatalogHeaders { get; set; }

        [JsonProperty("items")]
        public List<ValuesList> Items { get; set; }
    }
}
