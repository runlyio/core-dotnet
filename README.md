<img src="logo.svg" width="350" alt="Runly.NET" />

> Multi-threaded batch processing and background jobs for .NET Core

Runly is an opinionated framework for writing multi-threaded jobs. You focus on writing your business logic and Runly gives you a CLI, multi-threading, retries, and more out-of-the-box.

TODO: awesome animated SVG of CLI in action goes here

## How Does It Work?

You build your jobs as classes that inherit from `Job` into a new console app. You reference the [`Runly` nuget package](https://www.nuget.org/packages/Runly/) and you immediately have a fully functioning and robust CLI app.

## :rocket: Get Started

There are two ways to bootstrap a Runly job app:

1. Create a new job app from the [GitHub template](https://github.com/runlyio/template-dotnet). This will create a new repository for you that you can clone and immediately start running.

2. Skip the Git repo and create a local app via the `dotnet new` template:

First, install the template:

```
dotnet new -i Runly.Templates
```

Then create a new project:

```
mkdir myproject
cd myproject
dotnet new runly-app
```

That's it - you can now start running! Learn more about how to build jobs in the [Runly Quickstart](https://www.runly.io/docs/building/).

## Example

Here is a potential job that can read a CSV file (using [CsvHelper](https://joshclose.github.io/CsvHelper/)) and then send an email to each person in the file.

```c#
public class CsvEmailer : Job<CsvConfig, Person>
{
    readonly MyEmailService emails;

    public CsvEmailer(Config config, MyEmailService emails)
        : base(config)
    {
        this.emails = emails;
    }

    public override IAsyncEnumerable<Person> GetItemsAsync()
    {
        var csv = new CsvReader(File.Open(Config.CsvFilePath), CultureInfo.InvariantCulture);
        csv.Configuration.Delimiter = "|";

        return csv.GetRecordsAsync<Person>();
    }

    public override async Task<Result> ProcessAsync(Person person)
    {
        // Do the work to process each item.
        await emails.SendReminderEmail(person.Email, person.Name)
        return Result.Success();
    }
}
```

Using command line flags or JSON config, this can then easily be multi-threaded to send multiple emails at a time and allow for retries to deal with intermittent errors while sending the email.

See [more in-depth examples](./examples) or [start reading the docs](https://www.runly.io/docs/).

## Why Write Code Like This?



## The Runly Platform

You can turbocharge your job apps using the [Runly Platform](https://www.runly.io/platform/). The Runly Platform is a <abbr title="software as a service">SaaS</abbr> which helps shepard your jobs from deployment to monitoring and scaling in production.

* Deploy your jobs as nuget packages using tools you already know and love.
* Use familiar REST concepts to queue and schedule background jobs from your web or mobile app.
* Use CRON expressions for fine-grained control of job scheduling
* Get insights into job failures and performance issues before your users notice.
* Run and scale your jobs in the cloud or on your own hardware.

You can get started on a [generous free tier](https://www.runly.io/pricing/) with no commitment and no credit card. [Get started for free using your GitHub account](https://www.runly.io/dashboard/)!

### The Runly Platform sounds cool. Why isn't it OSS too?

There are plenty of job platforms out there which are completely OSS. We are trying a different model. Sustainability in OSS is a problem. We are trying to work on this project full-time and in order to do that, we have to feed our families. We think providing value via a paid service is the best way to do that while still solving problems for people with the core OSS offering.

We have plenty of ideas on how to make background jobs easy to build and easy to scale. We need your support to continue working on this project. You can support us by building a Runly job and letting us know what you think. If you find value in writing jobs this way, you can [give the Runly Platform a try](https://www.runly.io/platform). Or, just give us a GitHub star while you are here. It allows us to brag about all the internet points we are getting when our mothers and spouses ask what we are doing all day :wink:.

Keep up with the things we are working on by [following us on Twitter](https://twitter.com/runlyio).

--------------------------------------------------------

## Consuming Pre-release Packages

Pre-release packages are published to this project's [GitHub Packages Feed](https://github.com/runlyio/core-dotnet/packages). If you want to consume one of these pre-release packages for testing purposes, you can add a `NuGet.config` file to the root of your project:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="runly-github" value="https://nuget.pkg.github.com/runlyio/index.json" />
    <add key="NuGet.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

This will instruct your nuget client to look for packages via Runly's pre-release feed first and then the official NuGet feed.
