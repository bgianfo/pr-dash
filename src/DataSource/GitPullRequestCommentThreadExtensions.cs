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
        /// <remarks>ADO can have comments with 'null' content that isn't displayed in the UI but returned by the API. Suspect it's for deleted threads, so exclude those.</remarks>
        /// <param name="thread">PR comment thread.</param>
        /// <param name="userId">The GUID of the user to look for.</param>
        /// <returns>True if the thread involves the user, otherwise false.</returns>
        public static bool InvolvesUser(this GitPullRequestCommentThread thread, Guid userId) =>
            thread.Comments.Any(c => !string.IsNullOrWhiteSpace(c.Content) && Guid.Parse(c.Author.Id) == userId);
    }
}
