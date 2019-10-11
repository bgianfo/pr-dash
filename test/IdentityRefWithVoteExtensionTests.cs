using System;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using PrDash.DataSource;
using Xunit;

namespace PrDash.Tests
{
    /// <summary>
    /// Test cases for extension methods on <see cref="IdentityRefWithVote"/>.
    /// </summary>
    public class IdentityRefWithVoteExtensionTests
    {
        /// <summary>
        /// Tests the has final vote been cast extension method.
        /// </summary>
        [Fact]
        public void TestHasFinalVoteBeenCastExtension()
        {
            IdentityRefWithVote identity = new IdentityRefWithVote();

            // No vote should not be a valid final vote.
            //
            identity.Vote = 0;
            Assert.False(identity.HasFinalVoteBeenCast(),
                "No Vote incorrectly marked as final.");

            // Waiting should not be a valid final vote.
            //
            identity.Vote = -5;
            Assert.False(identity.HasFinalVoteBeenCast(),
                "Waiting incorrectly marked as final.");

            // Rejected should be a valid final vote.
            //
            identity.Vote = -10;
            Assert.True(identity.HasFinalVoteBeenCast(),
                "Rejected incorrectly marked as not a final vote.");

            // Approved should be a valid final vote.
            //
            identity.Vote = 10;
            Assert.True(identity.HasFinalVoteBeenCast(),
                "Approved incorrectly marked as not a final vote.");

            // Approved with suggestions should be a valid final vote.
            //
            identity.Vote = 5;
            Assert.True(identity.HasFinalVoteBeenCast(),
                "Approved with suggestions incorrectly marked as not a final vote.");
        }

        /// <summary>
        /// Tests the has final vote been cast extension, when called on a null object.
        /// </summary>
        [Fact]
        public void TestHasFinalVoteBeenCastOnNull()
        {
            IdentityRefWithVote nullIdentity = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                nullIdentity.HasFinalVoteBeenCast();
            });
        }
    }
}
