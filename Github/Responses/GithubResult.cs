using Github.Models;

namespace Github.Responses
{
    public class GithubResult
    {
        public List<PullRequestModel> PullRequests { get; set; }
        public string ErrorMessage { get; set; }
        public bool Succeed { get; set; }
    }
}
