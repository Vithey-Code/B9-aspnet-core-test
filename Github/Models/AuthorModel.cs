using Newtonsoft.Json;

namespace Github.Models
{
    public class CreatorModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        [JsonProperty("Avatar_Url")]
        public string AvatarUrl { get; set; }
    }
}
