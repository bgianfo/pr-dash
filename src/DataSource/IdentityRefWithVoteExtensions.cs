
using System;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace PrDash.DataSource
{
    public static class IdentityRefWithVoteExtensions
    {
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