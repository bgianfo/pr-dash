using System;
using System.Threading;
using System.Threading.Tasks;

namespace PrDash.View
{
    public partial class PullRequestView
    {
        /// <summary>
        /// Dedicated task used to refreshing the pull request view data.
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        private class RefreshTask : IDisposable
        {
            /// <summary>
            /// Event used to notify the main thread of pending refresh requests.
            /// </summary>
            private AutoResetEvent m_refreshEvent = new AutoResetEvent(true);

            /// <summary>
            /// The pull request view that needs to be refreshed.
            /// </summary>
            private PullRequestView m_pullRequestView;

            /// <summary>
            /// Creates a new <see cref="RefreshTask"/> bound to the specified <see cref="PullRequestView" />.k
            /// </summary>
            /// <param name="prView">The view to refresh.</param>
            /// <returns>A new <see cref="RefreshTask"/>.</returns>
            public static RefreshTask Create(PullRequestView prView)
            {
                RefreshTask refreshTask = new RefreshTask(prView);

#pragma warning disable EPC13

                Task.Run(() => refreshTask.MainLoop());

#pragma warning restore EPC13

                return refreshTask;
            }

            /// <summary>
            /// Requests that the data to be refreshed.
            /// </summary>
            public void RequestRefresh() => m_refreshEvent.Set();

            /// <summary>
            /// Initializes a new instance of the <see cref="RefreshTask"/> class.
            /// </summary>
            /// <param name="prView">The pr view.</param>
            private RefreshTask(PullRequestView prView) => m_pullRequestView = prView;

            /// <summary>
            /// Mains the loop of the <see cref="Task"/> that this <see cref="RefreshTask"/> is bound to.
            /// </summary>
            private void MainLoop()
            {
                // The task will loop forever waiting on refresh requests.
                //
                do
                {
                    m_refreshEvent.WaitOne();
                    m_pullRequestView.RefreshCallback().Wait();
                }
                while (true);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() => m_refreshEvent.Dispose();
        }
    }
}
