
namespace PrDash.DataSource
{
    /// <summary>
    /// POD type for capturing statistics for assigned pull requests.
    /// </summary>
    public sealed class PullRequestStatistics
    {
        public uint Actionable = 0;

        public uint Waiting = 0;

        public uint SignedOff = 0;

        public uint Drafts = 0;

        public void Reset()
        {
            Actionable = 0;
            Waiting = 0;
            SignedOff = 0;
            Drafts = 0;
        }
    }
}