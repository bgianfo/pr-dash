using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Humanizer;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using PrDash.DataSource;
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
    [SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "We just want to sort.")]
    public class PullRequestViewElement : IComparable<PullRequestViewElement>, IEqualityComparer<PullRequestViewElement>
    {
        /// <summary>
        /// The pull request we are wrapping.
        /// </summary>
        private readonly GitPullRequest m_pullRequest;

        /// <summary>
        /// The handler to call when this pull request has been selected.
        /// </summary>
        private readonly IPullRequestHandler m_handler;

        private readonly DateTime m_updatedTime;

        /// <summary>
        /// The width in characters of the author column.
        /// </summary>
        private const int AuthorColumnWidth = 25;

        /// <summary>
        /// The width in characters of the date column.
        /// </summary>
        private const int DateColumnWidth = 16;

        /// <summary>
        /// The width in characters of the vote ratio column.
        /// </summary>
        private const int VoteRatioColumnWidth = 8;

        /// <summary>
        /// The width in characters of the pull request title column.
        /// </summary>
        private int TitleColumnWidth
        {
            get
            {
                if (CreatedMode)
                {
                    return Application.Top.Frame.Size.Width - (VoteRatioColumnWidth + DateColumnWidth);
                }
                else
                {
                    return Application.Top.Frame.Size.Width - (AuthorColumnWidth + DateColumnWidth);
                }
            }
        }

        /// <summary>
        /// Gets the title bound to the confines of the window.
        /// </summary>
        private string BoundedTitle
        {
            get => FitStringToBound(" " + m_pullRequest.Title.Trim(), TitleColumnWidth);
        }

        /// <summary>
        /// Gets the author bound to the confines of the window.
        /// </summary>
        private string BoundedAuthor
        {
            get => FitStringToBound(m_pullRequest.CreatedBy.DisplayName, AuthorColumnWidth);
        }

        /// <summary>
        /// Gets the pull request updated date bound to the confines of the window.
        /// </summary>
        private string BoundedUpdatedDate
        {
            get => FitStringToBound(m_updatedTime.Humanize().Transform(To.TitleCase), DateColumnWidth, leftPad: true);
        }

        /// <summary>
        /// Gets the pull request vote ratio bound to the confines of the window.
        /// </summary>
        public string BoundedVoteRatio
        {
            get => FitStringToBound(m_pullRequest.VoteRatio(), VoteRatioColumnWidth);
        }

        /// <summary>
        /// Expose Pull Request description
        /// </summary>
        public string Description => m_pullRequest.Description;

        /// <summary>
        /// If the pull request item was created in Create mode.
        /// </summary>
        public bool CreatedMode { get; set; }

        /// <summary>
        /// Constructs a new element which wraps a <see cref="GitPullRequest"/> object.
        /// </summary>
        /// <param name="request">The pull request this element represents.</param>
        /// <param name="handler">The handler to execute on the request.</param>
        public PullRequestViewElement(GitPullRequest request, IPullRequestHandler handler)
        {
            m_handler = handler;
            m_pullRequest = request;
            m_updatedTime = request.LatestCommitDate();
        }

        /// <summary>
        /// Invokes the pull request action handler callback on this pull request.
        /// </summary>
        public void OpenPullRequest() => m_handler.InvokeHandler(m_pullRequest);

        /// <summary>
        /// Special ToString implementation that will be called by the ListView control when it renders each element.
        /// </summary>
        /// <returns>The string representation of the pull request.</returns>
        public override string ToString()
        {
            if (CreatedMode)
            {
                return BoundedTitle + BoundedVoteRatio + BoundedUpdatedDate;
            }
            else
            {
                return BoundedTitle + BoundedAuthor + BoundedUpdatedDate;
            }
        }

        /// <summary>
        /// Fits the string to bounded size by trimming or padding with spaces.
        /// </summary>
        /// <param name="value">The string value to fit.</param>
        /// <param name="maxLength">The maxLength to constrain the string to.</param>
        /// <returns> The string that has been trimmed and padded.</returns>
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

        /// <summary>
        /// Implements IComparable<![CDATA[T]]> so we can sort elements correctly.
        /// </summary>
        /// <param name="other">The other element to compare to.</param>
        /// <returns>
        /// Less than zero, this instance precedes other in the sort order.
        /// Zero, this instance occurs in the same position in the sort order as other.
        /// Greater than zero, this instance follows other in the sort order.
        /// </returns>
        public int CompareTo([AllowNull] PullRequestViewElement other)
        {
            // If other is null, than we must be greater.
            //
            if (other == null)
            {
                return 1;
            }

            // Force sort order descending by negating the default sort order.
            //
            return -m_updatedTime.CompareTo(other.m_updatedTime);
        }

        /// <summary>
        /// Compares two pull requests for equality.
        /// </summary>
        /// <param name="x">Pull request to compare.</param>
        /// <param name="y">Pull request to compare</param>
        /// <returns>True if equal, false otherwise.</returns>
        public bool Equals([AllowNull] PullRequestViewElement x, [AllowNull] PullRequestViewElement y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.m_pullRequest.ArtifactId == y.m_pullRequest.ArtifactId;
        }

        /// <summary>
        /// Computes hash code for the pull request value.
        /// </summary>
        /// <param name="pr">Pull request to compare</param>
        public int GetHashCode([DisallowNull] PullRequestViewElement pr)
        {
            if (pr == null)
            {
                return 0;
            }

            return string.GetHashCode(pr.m_pullRequest.ArtifactId, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
