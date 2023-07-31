using Newtonsoft.Json;

namespace Github.Models
{
    public class CommitModel
    {
        [JsonProperty("sha")]
        public string Hash { get; set; }

        [JsonProperty("commit")]
        public CommitInfo CommitInfo { get; set; }
    }

    public class CommitInfo
    {
        public CommitterModel author { get; set; }
        public string message { get; set; }
    }

    public class CommitterModel
    {
        public string name { get; set; }
        public string email { get; set; }
        public DateTime date { get; set; }
    }

}
