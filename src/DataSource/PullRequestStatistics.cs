
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
        public uint Actionable = 0;

        /// <summary>
        /// The number of waiting pull requests.
        /// </summary>
        public uint Waiting = 0;

        /// <summary>
        /// The number of signed off pull requests.
        /// </summary>
        public uint SignedOff = 0;

        /// <summary>
        /// The number of draft pull requests.
        /// </summary>
        public uint Drafts = 0;

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
    }

#pragma warning restore CA1051
}