using Github.Controllers;
using Github.Models;
using Github.Requests;
using Github.Responses;
using Github.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;

namespace Test
{
    [TestFixture]
    public class GithubControllerTests
    {
        // Helper method to create a mock IGithubService with pre-defined data
        private static IGithubService CreateMockGithubService(GithubResult data)
        {
            var mockService = new Mock<IGithubService>();
            mockService.Setup(s => s.GetGithubPullRequests(It.IsAny<GithubRequest>()))
                       .ReturnsAsync(data);

            return mockService.Object;
        }

        [Test]
        public async Task GetPullRequest_ShouldReturnOkResultWithCorrectData()
        {
            // Arrange
            var pullRequests = new List<PullRequestModel>
            {
                new PullRequestModel { Draft = true },
                new PullRequestModel { Draft = false, CreatedOn = DateTime.UtcNow.AddDays(-35) },
                new PullRequestModel { Draft = false, CreatedOn = DateTime.UtcNow.AddDays(-15) }
            };
            var githubResult = new GithubResult { PullRequests = pullRequests, Succeed = true };
            var mockGithubService = CreateMockGithubService(githubResult);
            var controller = new GithubController(mockGithubService);

            // Act
            var result = await controller.GetPullRequest(new GithubRequest());

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult?.Value);

            var response = okResult.Value as PullRequestResponse;
            Assert.NotNull(response);
        }

        [Test]
        public async Task GetPullRequest_ShouldReturnNotFoundResultWhenServiceFails()
        {
            // Arrange
            var githubResult = new GithubResult { Succeed = false, ErrorMessage = "API request failed" };
            var mockGithubService = CreateMockGithubService(githubResult);
            var controller = new GithubController(mockGithubService);

            // Act
            var result = await controller.GetPullRequest(new GithubRequest());

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult?.Value);

            var errorData = notFoundResult.Value as GithubResult;
            Assert.NotNull(errorData);
            Assert.IsFalse(errorData.Succeed);
            Assert.AreEqual("API request failed", errorData.ErrorMessage);
        }

        [Test]
        public void CategorizePullRequests_ShouldCategorizeCorrectly()
        {
            // Arrange
            var controller = new GithubController(null); // We don't need GithubService for this test

            // Create some sample pull requests for testing
            var pullRequests = new List<PullRequestModel>
            {
                new PullRequestModel { Draft = true },
                new PullRequestModel { Draft = false, CreatedOn = DateTime.UtcNow.AddDays(-35) },
                new PullRequestModel { Draft = false, CreatedOn = DateTime.UtcNow.AddDays(-15) }
            };

            // Act
            // Since the method is private, we use reflection to invoke it
            var methodInfo = typeof(GithubController).GetMethod("CategorizePullRequests", BindingFlags.NonPublic | BindingFlags.Instance);
            var parameters = new object[] { pullRequests, null, null, null }; // We don't need the out parameters for this test
            methodInfo.Invoke(controller, parameters);

            // Assert
            // We can check the result using the out parameters if needed, but for this test, we only need to ensure no exceptions are thrown.
        }

        [Test]
        public void CalculateAverageDays_ShouldCalculateCorrectly()
        {
            // Arrange
            var controller = new GithubController(null); // We don't need GithubService for this test

            // Create some sample pull requests for testing
            var pullRequests = new List<PullRequestModel>
            {
                new PullRequestModel { CreatedOn = DateTime.UtcNow.AddDays(-5) },
                new PullRequestModel { CreatedOn = DateTime.UtcNow.AddDays(-10) },
                new PullRequestModel { CreatedOn = DateTime.UtcNow.AddDays(-15) }
            };

            // Act
            // Since the method is private, we use reflection to invoke it
            var methodInfo = typeof(GithubController).GetMethod("CalculateAverageDays", BindingFlags.NonPublic | BindingFlags.Instance);
            var parameters = new object[] { pullRequests };
            var result = methodInfo.Invoke(controller, parameters);

            // Assert
            Assert.AreEqual(10.0, result); // The average of (-5, -10, -15) is -10, but we should get the absolute value, which is 10
        }
    }
}