using Github.Models;
using Github.Requests;
using Github.Responses;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Github.Services
{
    public class GithubService : IGithubService, IDisposable
    {
        private HttpClient httpClient;

        public GithubService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<GithubResult> GetGithubPullRequests(GithubRequest request)
        {
            var pullRequests = new List<PullRequestModel>();
            var result = new GithubResult();

            if (string.IsNullOrWhiteSpace(request.Owner))
                request.Owner = "dotnet";
            if (string.IsNullOrWhiteSpace(request.RepositoryName))
                request.RepositoryName = "dotnet/runtime";

            // page number and page size are for testing purposes.
            // I don't put page number and page size a query paramer for now, because it is justing testing purpose only.
            // page=40&per_page=5: draft and Stale have data
            string url = $"repos/{request.RepositoryName}/pulls?page=40&per_page=5&state=open"; 

            if (!string.IsNullOrWhiteSpace(request.Label))
                url += $"&labels={Uri.EscapeDataString(request.Label)}";

            if (!string.IsNullOrWhiteSpace(request.CustomQuery))
                url += $"&q={Uri.EscapeDataString($"repo:{request.Owner}/{request.RepositoryName} {request.CustomQuery}")}";

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri("https://api.github.com/");
                    // Set the User-Agent header (required by GitHub API)
                    httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("B9", "1.0"));

                    // Make the GET request to the GitHub API without authentication
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        pullRequests = JsonConvert.DeserializeObject<List<PullRequestModel>>(jsonString);

                        // Prepare the tasks for parallel execution
                        var tasks = new List<Task>();

                        foreach (PullRequestModel pullRequest in pullRequests)
                        {
                            // Prepare the task for getting the author information
                            var authorTask = GetCreatorAsync(httpClient, pullRequest);

                            // Prepare the task for getting the comments count
                            var commentsTask = GetCommentsCountAsync(httpClient, request.RepositoryName, pullRequest);

                            // Prepare the task for getting the list commits
                            var commitsTask = GetAllCommitsAsync(httpClient, request.RepositoryName, pullRequest);

                            tasks.Add(authorTask);
                            tasks.Add(commentsTask);
                            tasks.Add(commitsTask);
                        }

                        // Wait for all tasks to complete in parallel
                        await Task.WhenAll(tasks);

                        result.PullRequests = pullRequests;
                        result.Succeed = true;
                        return result;
                    }
                    else
                    {
                        result.Succeed = false;
                       result.ErrorMessage = $"API request failed with the reason: {response.ReasonPhrase}";
                        return result;
                    }

                }
            }
            catch (HttpRequestException ex)
            {
                result.Succeed = false;
                result.ErrorMessage = $"API request failed with exception: {ex.Message}";
                return result;
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
        private async Task GetCreatorAsync(HttpClient httpClient, PullRequestModel pullRequest)
        {
            var CreatorResponse = await httpClient.GetAsync($"users/{pullRequest.User.Login}");

            if (CreatorResponse.IsSuccessStatusCode)
            {
                string creatorJsonString = await CreatorResponse.Content.ReadAsStringAsync();
                var creator = JsonConvert.DeserializeObject<CreatorModel>(creatorJsonString);
                pullRequest.Creator = creator;
            }
        }

        private async Task GetCommentsCountAsync(HttpClient httpClient, string repositoryName, PullRequestModel pullRequest)
        {
            string endpointComment = $"repos/{repositoryName}/pulls/{pullRequest.Number}/comments";
            var commentsResponse = await httpClient.GetAsync(endpointComment);

            if (commentsResponse.IsSuccessStatusCode)
            {
                string commentsJsonString = await commentsResponse.Content.ReadAsStringAsync();
                var comments = JsonConvert.DeserializeObject<List<CommentModel>>(commentsJsonString);
                pullRequest.Comments = comments.Count;
            }
        }

        private async Task GetAllCommitsAsync(HttpClient httpClient, string repositoryName, PullRequestModel pullRequest)
        {
            string endpointCommit = $"repos/{repositoryName}/pulls/{pullRequest.Number}/commits?page=1&per_page=3";
            var commitsResponse = await httpClient.GetAsync(endpointCommit);

            if (commitsResponse.IsSuccessStatusCode)
            {
                string commitsJsonString = await commitsResponse.Content.ReadAsStringAsync();
                var commits = JsonConvert.DeserializeObject<List<CommitModel>>(commitsJsonString);
                pullRequest.Commits = commits;
            }
        }
    }
}

