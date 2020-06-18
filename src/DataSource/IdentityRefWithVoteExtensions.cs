using System;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace PrDash.DataSource
{
    /// <summary>
    /// Truth table for the Vote status is here:
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/api/microsoft.teamfoundation.sourcecontrol.webapi.identityrefwithvote.vote?view=azure-devops-dotnet#Microsoft_TeamFoundation_SourceControl_WebApi_IdentityRefWithVote_Vote"/>
    /// <![CDATA[
    /// 10 - approved
    /// 5 - approved with suggestions
    /// 0 - no vote
    /// -5 - waiting for author
    /// -10 - rejected
    /// ]]>
    /// </summary>
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

            return identityRef.IsSignedOff() || identityRef.Vote == -10;  // Rejected
        }

        /// <summary>
        /// Extension method that determines whether the vote has signed off on a pull request.
        /// </summary>
        /// <param name="identityRef">The identity reference.</param>
        /// <returns>
        ///   <c>true</c> if [vote has signed off] [the specified identity reference]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">identityRef</exception>

        public static bool IsSignedOff(this IdentityRefWithVote identityRef)
        {
            if (identityRef == null)
            {
                throw new ArgumentNullException(nameof(identityRef));
            }

            return identityRef.Vote == 10 || // Approved
                   identityRef.Vote == 5;    // Approved With Suggestions
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