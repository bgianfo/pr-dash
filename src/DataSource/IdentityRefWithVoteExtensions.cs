using System;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace PrDash.DataSource
{
    public static class IdentityRefWithVoteExtensions
    {
        /// <summary>
        /// Extension method that determines whether the final vote has been cast on a pull request.
        /// </summary>
        /// <param name="identityRef">The identity reference.</param>
        /// <returns>
        ///   <c>true</c> if [has final vote been cast] [the specified identity reference]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">identityRef</exception>
        public static bool HasFinalVoteBeenCast(this IdentityRefWithVote identityRef)
        {
            if (identityRef == null)
            {
                throw new ArgumentNullException(nameof(identityRef));
            }

            return identityRef.Vote == 10 || // Approved
                   identityRef.Vote == 5 || // Approved with suggestions
                   identityRef.Vote == -10;  // Rejected
        }

        /// <summary>
        /// Extension method that determines whether this instance is waiting.
        /// </summary>
        /// <param name="identityRef">The identity reference.</param>
        /// <returns>
        ///   <c>true</c> if the specified identity reference is waiting; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">identityRef</exception>
        public static bool IsWaiting(this IdentityRefWithVote identityRef)
        {
            if (identityRef == null)
            {
                throw new ArgumentNullException(nameof(identityRef));
            }

            // Vote -5 == waiting.
            //
            return identityRef.Vote == -5;
        }
    }
}