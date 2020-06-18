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

            if (pullRequest.Commits != null &&  pullRequest.Commits.Any())
            {
                return pullRequest.Commits.OrderBy(c => c.Committer.Date).Last().Committer.Date;
            }

            // N.B. Apparently there are cases where a PR Can exist with no commits.
            //
            // If no commits are available, fall back to creation date to give some sort of ordering.
            //
            return pullRequest.CreationDate;
        }

        /// <summary>
        /// Returns the vote ratio string for the pull request.
        /// </summary>
        /// <param name="pr">The pull request to process.</param>
        public static string VoteRatio(this GitPullRequest pr)
        {
            if (pr == null)
            {
                throw new ArgumentNullException(nameof(pr));
            }

            int reviewers = pr.Reviewers.Length;
            int signedOff = pr.Reviewers.Count(r => r.IsSignedOff());

            return $"{signedOff} / {reviewers}";
        }
    }
}
