pr-dash
----
`pr-dash` is a command line tool which allows you to monitor the status
of all Azure Dev Ops pull requests you are assigned a reviewer of, right
at home in your terminal.

Note: This tool is not created by, affiliated with, or supported by Microsoft/Azure DevOps.

[![Build Status](https://travis-ci.org/bgianfo/pr-dash.svg?branch=master)](https://travis-ci.org/bgianfo/pr-dash)
[![GitHub license](https://img.shields.io/github/license/bgianfo/pr-dash.svg)]()

### Building/Installation

`pr-dash` is written in C#, targetting dotnet core so you'll need to grab a
[dotnet core 3.0 installation](https://dotnet.microsoft.com/download/dotnet-core/3.0) in order to compile it.
Building is easy:

```
$ git clone https://github.com/bgianfo/pr-dash
$ cd pr-dash
$ dotnet build
$ dotnet test
$ dotnet run
```

### Running tests

To run the [xunit](https://xunit.net/) based test suite, use:

```
$ cd pr-dash
$ dotnet test
```

### Configuration

To configure pr-dash you simply need to setup a yaml file at ~/.pr-dash.yml.

Example ~/.pr-dash.yml:

```
accounts:
  - project_name: sample-project
    org_url: https://dev.azure.com/example
    pat: <your-personal-authentication-token>
```

Now you can run pr-dash and try it out.
