using Newtonsoft.Json;

namespace Github.Models
{
    public class PullRequestModel
    {
        public string Url { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }

        [JsonProperty("Body")]
        public string Description { get; set; }

        public int Comments { get; set; }

        [JsonProperty("Created_At")]
        public DateTime CreatedOn { get; set; }
        public CreatorModel Creator { get; set; }
        public bool Draft { get; set; }
        public List<CommitModel> Commits { get; set; }
        public int StaleDays { get; set; }
        public UserModel User { get; set; }
        public string Message { get; set; }
    }

    public class CommentModel
    {
        public int Id { get; set; }
    }

    public class UserModel
    {
        public string Login { get; set; }
    }
}
