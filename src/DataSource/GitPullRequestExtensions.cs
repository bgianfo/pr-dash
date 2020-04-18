using System;
using System.Linq;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace PrDash.DataSource
{
    public static class GitPullRequestExtensions
    {
        public static DateTime LatestCommitDate(this GitPullRequest pullRequest)
        {
            if (pullRequest == null)
            {
                throw new ArgumentNullException(nameof(pullRequest));
            }

            return pullRequest.Commits.OrderBy(c => c.Committer.Date).Last().Committer.Date;
        }
    }
}
