using Github.Models;
using Github.Requests;
using Github.Responses;
using Github.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Github.Controllers
{

    public class GithubController : Controller
    {
        private readonly IGithubService _githubService;

        public GithubController(IGithubService githubService)
        {
            _githubService = githubService;
        }

        public async Task<IActionResult> GetPullRequest(GithubRequest request)
        {
            // Fetch pull requests from the Github API using the service
            var data = await _githubService.GetGithubPullRequests(request);
            if(data.Succeed)
            {
                List<PullRequestModel> activePullRequests;
                List<PullRequestModel> draftPullRequests;
                List<PullRequestModel> stalePullRequests;

                CategorizePullRequests(data.PullRequests.ToList(), out activePullRequests, out draftPullRequests, out stalePullRequests);

                double activeAverageDays = CalculateAverageDays(activePullRequests);
                double draftAverageDays = CalculateAverageDays(draftPullRequests);
                double staleAverageDays = CalculateAverageDays(stalePullRequests);
                double allGroupsAverageDays = CalculateAverageDays(data.PullRequests);

                var response = new PullRequestResponse
                {
                    ActivePullRequests = activePullRequests,
                    DraftPullRequests = draftPullRequests,
                    StalePullRequests = stalePullRequests,
                    ActiveAverageDays = activeAverageDays,
                    DraftAverageDays = draftAverageDays,
                    StaleAverageDays = staleAverageDays,
                    AllGroupsAverageDays = allGroupsAverageDays
                };

                return Ok(response);
            }
            else
            {
                return NotFound(data);
            }

        }

        private void CategorizePullRequests(List<PullRequestModel> pullRequests, out List<PullRequestModel> active, out List<PullRequestModel> draft, out List<PullRequestModel> stale)
        {
            active = new List<PullRequestModel>();
            draft = new List<PullRequestModel>();
            stale = new List<PullRequestModel>();

            // Implement the logic to categorize pull requests into active, draft, and stale lists
            foreach (var pr in pullRequests)
            {
                if (pr.Draft)
                    draft.Add(pr);
                else if (!pr.Draft && pr.CreatedOn < DateTime.UtcNow.AddDays(-30))
                {
                    pr.StaleDays = (DateTime.UtcNow.AddDays(-30) - pr.CreatedOn).Days;
                    stale.Add(pr);
                }
                else
                    active.Add(pr);
            }
        }

        private double CalculateAverageDays(List<PullRequestModel> pullRequests)
        {
            if (pullRequests == null || pullRequests.Count == 0)
            {
                return 0; // Return 0 if there are no pull requests to calculate average days.
            }

            double totalDays = 0;

            foreach (var pr in pullRequests)
            {
                // Calculate the number of days since the creation date of the pull request
                TimeSpan daysSinceCreation = DateTime.UtcNow - pr.CreatedOn;
                totalDays += daysSinceCreation.TotalDays;
            }

            // Calculate the average number of days
            double averageDays = totalDays / pullRequests.Count;

            // Round the average days to 2 decimal places
            averageDays = Math.Round(averageDays, 2);

            return averageDays;
        }
    }
}
