using System;

namespace PrDash.DataSource
{
    // Ignore warning about not declare visible instance fields.
#pragma warning disable CA1051

    /// <summary>
    /// POD type for capturing statistics for assigned pull requests.
    /// </summary>
    public sealed class PullRequestStatistics
    {
        /// <summary>
        /// The number of actionable pull requests.
        /// </summary>
        public uint Actionable;

        /// <summary>
        /// The number of waiting pull requests.
        /// </summary>
        public uint Waiting;

        /// <summary>
        /// The number of signed off pull requests.
        /// </summary>
        public uint SignedOff;

        /// <summary>
        /// The number of draft pull requests.
        /// </summary>
        public uint Drafts;

        /// <summary>
        /// Resets all statistics values.
        /// </summary>
        public void Reset()
        {
            Actionable = 0;
            Waiting = 0;
            SignedOff = 0;
            Drafts = 0;
        }

        /// <summary>
        /// Accumulate a PR status in this statistics object.
        /// </summary>
        /// <param name="state">Status of the PR to accumulate. Can be null for no accumulation.</param>
        public void Accumulate(PrState? state)
        {
            _ = state switch
            {
                null => uint.MinValue,
                PrState.Actionable => Actionable++,
                PrState.Waiting => Waiting++,
                PrState.SignedOff => SignedOff++,
                PrState.Drafts => Drafts++,
                _ => throw new ArgumentException("Unknown pr state: " + state)
            };
        }
    }

#pragma warning restore CA1051
}
