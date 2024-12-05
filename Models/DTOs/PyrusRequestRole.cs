using Newtonsoft.Json;
using System.Collections.Generic;

namespace ApiPyrus.Models.DTOs
{
    public class PyrusRequestRole
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("member_add")]
        public List<long> AddMember { get; set; }

        [JsonProperty("member_remove")]
        public List<long> RemoveMember { get; set; }

        [JsonProperty("banned")]
        public bool? Banned { get; set; }
    }
}
