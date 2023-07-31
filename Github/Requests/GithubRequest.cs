using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Github.Requests
{
    public class GithubRequest
    {
        public string Owner { get; set; }
        public string RepositoryName { get; set; }
        public string Label { get; set; }
        public string CustomQuery { get; set; }
    }
}
