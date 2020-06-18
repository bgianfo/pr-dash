using System;
using System.Linq;
using System.Text;
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

            if (pullRequest.Commits != null && pullRequest.Commits.Any())
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
        /// <exception cref="ArgumentNullException">identityRef</exception>
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

        /// <summary>
        /// Returns a summarization of the changes in the pull request.
        /// </summary>
        /// <param name="pr">The pull request to process.</param>
        /// <exception cref="ArgumentNullException">identityRef</exception>
        public static string ChangeSize(this GitPullRequest pr)
        {
            if (pr == null)
            {
                throw new ArgumentNullException(nameof(pr));
            }

            if (pr.Commits == null)
            {
                return string.Empty;
            }

            int added = 0;
            int edits = 0;
            int delete = 0;
            foreach (var change in pr.Commits)
            {
                foreach (var count in change.ChangeCounts)
                {
                    switch (count.Key)
                    {
                        case VersionControlChangeType.Add:
                            added += count.Value;
                            break;
                        case VersionControlChangeType.Edit:
                            edits += count.Value;
                            break;
                        case VersionControlChangeType.Delete:
                            delete += count.Value;
                            break;
                    }
                }
            }

            StringBuilder builder = new StringBuilder();

            if (added > 0)
            {
                builder.Append($"++{added} ");
            }

            if (delete > 0)
            {
                builder.Append($"--{delete} ");
            }

            if (edits > 0)
            {
                builder.Append($"--{edits}");
            }

            return builder.ToString();
        }
    }
}
