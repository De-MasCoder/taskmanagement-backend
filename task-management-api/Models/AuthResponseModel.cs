using Newtonsoft.Json;

namespace task_management_api.Models
{
    public class AuthResponseModel
    {
        [JsonProperty("code")]
        public string? Code { get; set; }
        [JsonProperty("error_code")]
        public string? ErrorCode { get; set; }
        [JsonProperty("msg")]
        public string? ErrorMessage { get; set; }
    }
}
