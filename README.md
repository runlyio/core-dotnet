<img src="logo.svg" width="350" alt="Runly.NET" />

> Multi-threaded batch processing and background jobs for .NET Core

Runly is an opinionated framework for writing multi-threaded jobs. You focus on writing your business logic and Runly gives you a CLI, multi-threading, retries, and more out-of-the-box.

## :rocket: Get Started

Create a new job app from the [GitHub template](https://github.com/runlyio/template-dotnet) or via the dotnet tool template:

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
