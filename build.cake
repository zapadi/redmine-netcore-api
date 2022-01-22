var target = Argument("target", "Default");
var slackApiKey = EnvironmentVariable("slack_api_key");
var branch = EnvironmentVariable("APPVEYOR_REPO_BRANCH");
var isWindows = true;
var shouldNotify = false;

Task("Add-NuGet-Feed")
  .WithCriteria(!NuGetHasSource("https://somedomain/nuget/v3/index.json"))
  .Does(() =>
{
  Information("Adding NuGet Source...");
});

Task("NuGet-Restore")
  .IsDependentOn("Add-NuGet-Feed")
  .Does(() =>
{
  Information("Restoring Packages...");
});

Task("Build-Windows")
  .WithCriteria(isWindows)
  .IsDependentOn("NuGet-Restore")
  .Does(() =>
{
  Information("Building for Windows...");
});

Task("Build-Linux")
  .WithCriteria(!isWindows)
  .IsDependentOn("NuGet-Restore")
  .Does(() =>
{
  Information("Building for Linux...");
});

Task("Build")
  .IsDependentOn("Build-Windows")
  .IsDependentOn("Build-Linux");


Task("Package")
  .IsDependentOn("Build")
  .Does(() =>
{
  Information("Packing...");
});

Task("Deploy")
  .WithCriteria(branch == "master")
  .IsDependentOn("Package")
  .Does(() =>
{
  Information("Deploying...");
});

Task("Notify-Slack")
  .WithCriteria(!string.IsNullOrEmpty(slackApiKey))
  .IsDependentOn("Deploy")
  .Does(() =>
{
  Information("Notifying Slack...");
});

Task("Important-Task")
  .Does(() =>
{
  Information("Doing something important...");
  shouldNotify = true;
});

Task("Notify")
   .WithCriteria(() => shouldNotify)
  .IsDependentOn("Important-Task")
  .Does(() =>
{
  Information("Notifying...");
});

Task("Default")
  .IsDependentOn("Notify")
  .IsDependentOn("Notify-Slack");

RunTarget(target);

