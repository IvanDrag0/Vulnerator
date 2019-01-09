﻿using log4net;
using Markdig;
using Octokit;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Vulnerator.ViewModel;

namespace Vulnerator.Model.Object
{
    public class GitHubActions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));
        public async void GetGitHubIssues(AsyncObservableCollection<Issue> issueList)
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    GitHubClient githubClient = new GitHubClient(new ProductHeaderValue("Vulnerator"));
                    var issues = await githubClient.Issue.GetAllForRepository("Vulnerator", "Vulnerator");
                    for (int i = 0; i < issues.Count; i++)
                    {
                        if (issues[i].HtmlUrl.Contains(@"/pull/"))
                        { continue; }
                        Issue issue = new Issue();
                        issue.Title = issues[i].Title;
                        issue.Body = issues[i].Body;
                        issue.Number = issues[i].Number;
                        issue.HtmlUrl = issues[i].HtmlUrl;
                        if (issues[i].Milestone != null)
                        { issue.Milestone = issues[i].Milestone.Title; }
                        else
                        { issue.Milestone = @"N/A"; }
                        issue.Comments = issues[i].Comments;
                        foreach (Octokit.Label label in issues[i].Labels)
                        {
                            Label issueLabel = new Label();
                            issueLabel.Color = "#" + label.Color;
                            issueLabel.Name = label.Name;
                            issue.Labels.Add(issueLabel);
                        }
                        issueList.Add(issue);
                    }
                }
                else
                {
                    log.Warn("Github issues are only available if an internet connection is present.");
                    Issue issue = new Issue();
                    issue.Title = "Network connection unavailable";
                    issueList.Add(issue);
                }
            }
            catch (Exception exception)
            {
                log.Error("Unable to retrieve GitHub Vulnerator issue listing.");
                log.Debug("Exception details: " + exception);
            }
        }

        public async void GetGitHubReleases(AsyncObservableCollection<Release> releaseList)
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    GitHubClient githubClient = new GitHubClient(new ProductHeaderValue("Vulnerator"));
                    var releases = await githubClient.Repository.Release.GetAll("Vulnerator", "Vulnerator");
                    for (int i = 0; i < releases.Count; i++)
                    {
                        Release release = new Release();
                        release.Name = releases[i].Name;
                        if (releases[i].TagName != null)
                        { release.TagName = releases[i].TagName; }
                        else
                        { release.TagName = "No Tag Name Assigned"; }
                        release.HtmlUrl = releases[i].HtmlUrl;
                        release.Body = releases[i].Body;
                        release.CreatedAt = releases[i].CreatedAt.Date.ToLongDateString();
                        release.Downloads = releases[i].Assets[0].DownloadCount;
                        releaseList.Add(release);
                    }
                }
                else
                { log.Warn("GitHub releases are only available if an internet connection is available."); }
            }
            catch (Exception exception)
            {
                log.Error("Unable to retrieve GitHub Vulnerator release listing.");
                log.Debug("Exception details: " + exception);
            }
        }

        public async Task<Release> GetLatestGitHubRelease()
        {
            Release _release = new Release();

            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    GitHubClient githubClient = new GitHubClient(new ProductHeaderValue("Vulnerator"));
                    Octokit.Release release = await githubClient.Repository.Release.GetLatest("Vulnerator", "Vulnerator");
                    _release.TagName = release.TagName;
                    _release.HtmlUrl = release.HtmlUrl;
                }
                else
                { _release.TagName = "Unavailable"; }
                return _release;
            }
            catch (Exception exception)
            {
                log.Error("Unable to retrieve GitHub Vulnerator release listing.");
                log.Debug("Exception details: " + exception);
                _release.TagName = "Unavailable";
                return _release;
            }
        }
    }
}
