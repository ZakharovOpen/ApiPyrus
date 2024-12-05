using Newtonsoft.Json;

namespace ApiPyrus.Models.DTOs
{
    public class PyrusRequestMember
    {
        [JsonProperty("first_name")]
        public object FirstName { get; set; }

        [JsonProperty("last_name")]
        public object LastName { get; set; }

        [JsonProperty("email")]
        public object Email { get; set; }

        [JsonProperty("department_id")]
        public long? DepartmentId { get; set; }

        [JsonProperty("banned")]
        public bool? Banned { get; set; }

        [JsonProperty("position")]
        public object Position { get; set; }

        [JsonProperty("skype")]
        public object Skype { get; set; }

        [JsonProperty("phone")]
        public object Phone { get; set; }
    }
}
