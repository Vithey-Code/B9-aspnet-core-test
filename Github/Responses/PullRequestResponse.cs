using Github.Models;

namespace Github.Responses
{
    public class PullRequestResponse
    {
        public List<PullRequestModel> ActivePullRequests { get; set; }
        public List<PullRequestModel> DraftPullRequests { get; set; }
        public List<PullRequestModel> StalePullRequests { get; set; }
        public double ActiveAverageDays { get; set; }
        public double DraftAverageDays { get; set; }
        public double StaleAverageDays { get; set; }
        public double AllGroupsAverageDays { get; set; }
    }
}
