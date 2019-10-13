using System.Net.NetworkInformation;
using System.Text;
using Humanizer;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Account;
using PrDash.Configuration;
using PrDash.Handlers;
using Terminal.Gui;

namespace PrDash.View
{
    /// <summary>
    /// Represents a pull request element in the <see cref="PullRequestView"/>.
    /// </summary>
    /// <remarks>
    /// This exists purely so that we can customize the ToString implementation.
    /// </remarks>
    public class PullRequestViewElement
    {
        /// <summary>
        /// The pull request we are wrapping.
        /// </summary>
        private readonly GitPullRequest m_pullRequest;

        /// <summary>
        /// The handler to call when this pull request has been selected.
        /// </summary>
        private readonly IPullRequestHandler m_handler;

        private const int AuthorWidth = 25;

        private const int DateWidth = 20;

        /// <summary>
        /// Gets the title bound to the confines of the window.
        /// </summary>
        private string BoundedTitle
        {
            get
            {
                int titleBoundWidth = Application.Top.Frame.Size.Width - (AuthorWidth + DateWidth);
                return FitStringToBound(m_pullRequest.Title.Trim(), titleBoundWidth);
            }
        }

        /// <summary>
        /// Gets the author bound to the confines of the window.
        /// </summary>
        private string BoundedAuthor
        {
            get { return FitStringToBound(m_pullRequest.CreatedBy.DisplayName, AuthorWidth); }
        }

        /// <summary>
        /// Gets the pull request creation date bound to the confines of the window.
        /// </summary>
        private string BoundedCreationDate
        {
            get { return FitStringToBound(m_pullRequest.CreationDate.Humanize(), DateWidth, leftPad: true); }
        }

        private string ChangeSize
        {
            get
            {
                if (m_pullRequest.Commits == null)
                {
                    return string.Empty;
                }

                int added = 0;
                int edits = 0;
                int delete = 0;
                foreach (var change in m_pullRequest.Commits)
                {
                    foreach (var count in change.ChangeCounts)
                    {
                        switch (count.Key)
                        {
                            case VersionControlChangeType.Add:
                                added += count.Value;
                                break;
                            case VersionControlChangeType.Edit:
                                edits += count.Value;
                                break;
                            case VersionControlChangeType.Delete:
                                delete += count.Value;
                                break;
                        }
                    }
                }

                StringBuilder builder = new StringBuilder();

                if (added > 0)
                {
                    builder.Append($"++{added} ");
                }

                if (delete > 0)
                {
                    builder.Append($"--{delete} ");
                }

                if (edits > 0)
                {
                    builder.Append($"--{edits}");
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets the pull request.
        /// </summary>
        /// <value>
        /// The pull request.
        /// </value>
        private GitPullRequest PullRequest { get { return m_pullRequest; } }

        /// <summary>
        /// Constructs a new element which wraps a <see cref="GitPullRequest"/> object.
        /// </summary>
        /// <param name="request"></param>
        public PullRequestViewElement(GitPullRequest request, IPullRequestHandler handler)
        {
            m_handler = handler;
            m_pullRequest = request;
        }

        /// <summary>
        /// Invokes the pull request action handler callback on this pull request.
        /// </summary>
        public void InvokeHandler()
        {
            m_handler.InvokeHandler(m_pullRequest);
        }

        /// <summary>
        /// Special ToString implementation that will be called by the ListView control when it renders each element.
        /// </summary>
        /// <returns>The string representation of the pull request.</returns>
        public override string ToString() => BoundedTitle + BoundedAuthor + BoundedCreationDate;

        /// <summary>
        /// Fits the string to bounded size by trimming or padding with spaces.
        /// </summary>
        /// <param name="value">The string value to fit.</param>
        /// <param name="maxLength">The maxLength to constrain the string to.</param>
        /// <returns>
        /// The string that has been trimmed and padded..
        /// </returns>
        private static string FitStringToBound(string value, int maxLength, bool leftPad = false)
        {
            const char PadChar = ' ';
            if (value.Length < maxLength)
            {
                string pad = new string(PadChar, maxLength - (1 + value.Length));

                string first = leftPad ? pad : value;
                string second = leftPad ? value : pad;

                return string.Concat(first, second);
            }
            else
            {
                string substring = value.Substring(0, maxLength - 1);
                if (leftPad)
                {
                    return string.Concat(PadChar, substring);
                }
                else
                {
                    return string.Concat(substring, PadChar);
                }
            }
        }
    }
}
