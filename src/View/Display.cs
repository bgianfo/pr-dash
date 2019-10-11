using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Mono.Terminal;
using PrDash.DataSource;
using Terminal.Gui;

namespace PrDash
{
    public class Display
    {
        private readonly IPullRequestSource m_pullRequestSource;

        public Display(IPullRequestSource pullRequestSource)
        {
            m_pullRequestSource = pullRequestSource;
        }

        private IList FetchPrData()
        {
            IEnumerable<GitPullRequest> pullRequests = m_pullRequestSource.FetchActivePullRequsts();
            return pullRequests.Select(pr => pr.Title).ToList();
        }

        public void UiMain()
        {
            Application.Init();

            var win = new Window("PR's To Review:")
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            ListView content = new ListView(FetchPrData());

            win.Add(content);

            Application.Top.Add(win);
            Application.Run();
        }
    }
}
