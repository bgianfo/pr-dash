﻿using System;
using PrDash.Handlers;

namespace PrDash.Configuration
{
    /// <summary>
    /// Represents an account that the dashboard should poll for status.
    /// </summary>
    public sealed class AccountConfig
    {
        /// <summary>
        /// Access token for authenticating to Azure DevOps.
        /// See: https://docs.microsoft.com/azure/devops/integrate/get-started/authentication/pats
        /// </summary>
        /// <remarks>
        /// If PAT token is null, then the client attempts to fall back to Azure AD Authentication.
        /// </remarks>
        public string? PersonalAccessToken { get; set; }

        /// <summary>
        /// Should we hide ancient pull requests or not.
        /// </summary>
        public bool? HideAncientPullRequests { get; set; }

        /// <summary>
        /// Organization Url, for example: https://dev.azure.com/fabrikam
        /// </summary>
        public Uri? OrganizationUrl { get; set; }

        /// <summary>
        /// The project name to query inside the organization.
        /// </summary>
        public string? Project { get; set; }

        /// <summary>
        /// Handler to be executed when a pull request from this account is selected.
        /// </summary>
        public IPullRequestHandler? Handler { get; set; }
    }
}
