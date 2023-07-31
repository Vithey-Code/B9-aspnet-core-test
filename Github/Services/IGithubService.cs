using Github.Requests;
using Github.Responses;

namespace Github.Services
{
    public interface IGithubService
    {
        Task<GithubResult> GetGithubPullRequests(GithubRequest request);
    }
}
