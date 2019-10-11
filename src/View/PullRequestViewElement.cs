
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace PrDash.View
{

    public class PullRequestViewElement
    {
        private GitPullRequest m_pullRequest;

        public PullRequestViewElement(GitPullRequest request)
        {
            m_pullRequest = request;
        }

        public GitPullRequest PullRequest { get { return m_pullRequest; }  }

        public override string ToString() 
        {
            return string.Format("{0} - {1}",
                    m_pullRequest.Title,
                    m_pullRequest.CreatedBy.DisplayName).Trim();
        }
    }
}