
[![Build Status](https://travis-ci.org/bgianfo/pr-dash.svg?branch=master)](https://travis-ci.org/bgianfo/pr-dash)
[![GitHub license](https://img.shields.io/github/license/bgianfo/pr-dash.svg)]()
[![Dependabot Status](https://api.dependabot.com/badges/status?host=github&repo=bgianfo/pr-dash)](https://dependabot.com)
[![Total alerts](https://img.shields.io/lgtm/alerts/g/bgianfo/pr-dash.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/bgianfo/pr-dash/alerts/)

pr-dash
-------

`pr-dash` is a command line tool which allows you to monitor the status
of all Azure Dev Ops pull requests you are assigned a reviewer of, right
at home in your terminal. `pr-dash` was inspired by the **iron** code review
system from janestreet, you can read more about that here:
[PR Dash: A command line pull request dashboard](https://bjg.io/pr-dash/)

pr-dash is currently tested on Windows and Linux.

Note: This tool is not created by, affiliated with, or supported by Microsoft/Azure DevOps.

### Screenshot

![Screenshot of pr-dash running in demo mode](https://raw.githubusercontent.com/bgianfo/pr-dash/master/.assets/demo.png)

### Configuration

To configure pr-dash you simply need to setup a yaml file with one or more
projects/repo's to monitor for reviews.

The location of the yaml file differs based on the OS:
- Linux: ~/.pr-dash.yml
- Windows: C:\Users\USER-NAME\AppData\Roaming\pr-dash.yml (%APPDATA%\pr-dash.yml)

Example pr-dash.yml:

```yaml
accounts:
  - project_name: sample-project1
    org_url: https://dev.azure.com/example1
    pat: <your-personal-authentication-token>

  - project_name: sample-project2
    org_url: https://example2.visualstudio.com
    pat: <your-personal-authentication-token>

  # Would only work on windows
  - project_name: sample-project3
    org_url: https://example3.visualstudio.com

```

Notes:
- **pat** is optional when running on windows, if it's omitted the code will attempt to use AAD authentication.
- You can read how to create an Azure DevOps PAT token [here](https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops).

Now you can run pr-dash and try it out.

### Key Bindings

    Action Keys:
     h - Display this help dialog.
     r - Force refresh the current PR view.
     a - Switch the current view to actionable PRs.
     c - Switch the current view to created PRs.
     d - Switch the current view to draft PRs.
     s - Switch the current view to signed off PRs.
     w - Switch the current view to waiting PRs.
     q - Quit the program.
     Enter - Open the currently selected PR.

    Movement Keys:
     ↑ - Select one pull request up.
     ↓ - Select one pull request down.
     k - Select one pull request up.
     j - Select one pull request down.

    Mouse:
     Scroll Up   - Select one pull request up.
     Scroll Down - Select one pull request down.
     Left Click  - Open the currently selected PR.

### Building

`pr-dash` is written in C#, targeting dotnet core so you'll need to grab a
[dotnet core 3.1 installation](https://dotnet.microsoft.com/download/dotnet-core/3.1) in order to compile it.
Building is easy:

```
$ git clone https://github.com/bgianfo/pr-dash

$ cd pr-dash\src

$ dotnet build
Microsoft (R) Build Engine version 16.6.0+5ff7b0c9e for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  pr-dash -> C:\src\pr-dash\src\bin\Debug\netcoreapp3.1\pr-dash.dll
  pr-dash-test -> C:\src\pr-dash\test\bin\Debug\netcoreapp3.1\pr-dash-test.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:08.29
```

### Running Tests

To run the [xUnit.net](https://xunit.net/) based test suite, use:

```
$ cd pr-dash

$ dotnet test
Test run for C:\src\pr-dash\test\bin\Debug\netcoreapp3.1\pr-dash-test.dll(.NETCoreApp,Version=v3.1)
Microsoft (R) Test Execution Command Line Tool Version 16.6.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...

A total of 1 test files matched the specified pattern.

Test Run Successful.
Total tests: 8
     Passed: 8
 Total time: 11.6563 Seconds

```

