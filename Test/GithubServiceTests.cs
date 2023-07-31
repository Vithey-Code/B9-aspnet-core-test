using Github.Models;
using Github.Requests;
using Github.Services;
using Moq.Protected;
using Moq;
using Newtonsoft.Json;
using System.Net;

namespace Github.Test
{
    [TestFixture]
    public class GithubServiceTests
    {
        [Test]
        public async Task GetGithubPullRequests_ShouldReturnCorrectResult()
        {
            // Arrange
            var expectedPullRequests = new List<PullRequestModel>
            {
                new PullRequestModel { Draft = true },
                new PullRequestModel { Draft = false, CreatedOn = DateTime.UtcNow.AddDays(-35) },
                new PullRequestModel { Draft = false, CreatedOn = DateTime.UtcNow.AddDays(-15) }
            };

            var httpClientHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpClientHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(expectedPullRequests)),
                });

            var httpClient = new HttpClient(httpClientHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.github.com/")
            };

            var githubService = new GithubService(httpClient);

            // Act
            var githubRequest = new GithubRequest { Owner = "dotnet", RepositoryName = "dotnet/runtime" };
            var actualGithubResult = await githubService.GetGithubPullRequests(githubRequest); // Github rate limit can make it error

            // Assert
            Assert.IsTrue(actualGithubResult.Succeed);
            Assert.IsNotNull(actualGithubResult.PullRequests);
        }


        [Test]
        public async Task GetGithubPullRequests_ShouldReturnErrorResultOnApiFailure()
        {
            // Arrange
            var httpClientHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpClientHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound, // Simulate an API failure
                    ReasonPhrase = "Not Found"
                });

            var httpClient = new HttpClient(httpClientHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.github.com/")
            };

            var githubService = new GithubService(httpClient);

            // Act
            var githubRequest = new GithubRequest { Owner = "dotnet", RepositoryName = "runtime" };
            var actualGithubResult = await githubService.GetGithubPullRequests(githubRequest);

            // Assert
            Assert.IsFalse(actualGithubResult.Succeed);
            Assert.AreEqual("API request failed with the reason: Not Found", actualGithubResult.ErrorMessage);
        }
    }
}
