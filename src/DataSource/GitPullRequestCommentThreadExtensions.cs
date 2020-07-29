using System;
using System.Linq;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace PrDash.DataSource
{
    /// <summary>
    /// Extension methods for <see cref="GitPullRequestCommentThread"/>
    /// </summary>
    internal static class GitPullRequestCommentThreadExtensions
    {
        /// <summary>
        /// Determines if a comment thread involves the specified user.
        /// </summary>
        /// <param name="thread">PR comment thread.</param>
        /// <param name="userId">The GUID of the user to look for.</param>
        /// <returns>True if the thread involves the user, otherwise false.</returns>
        public static bool InvolvesUser(this GitPullRequestCommentThread thread, Guid userId) => thread.Comments.Any(c => Guid.Parse(c.Author.Id) == userId);

        /// <summary>
        /// Determines if a comment thread is active (active or pending).
        /// </summary>
        /// <param name="thread">PR comment thread.</param>
        /// <returns>True is thread status is Active or Pending.</returns>
        public static bool IsActive(this GitPullRequestCommentThread thread) => thread.Status == CommentThreadStatus.Active || thread.Status == CommentThreadStatus.Pending;
    }
}
