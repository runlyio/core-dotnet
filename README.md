<img src="logo.svg" width="350" alt="Runly.NET" />

> Multi-threaded batch processing and background jobs for .NET Core

Runly is an opinionated framework for writing jobs ⁠— background or otherwise. You focus on writing your business logic as a job. Runly gives you a CLI, multi-threading, retries, and more out-of-the-box.

```c#
public class CsvParser : Job<Config, string>
{
  public HelloWorld(Config config)
    : base(config) { }

  public override IAsyncEnumerable<string> GetItemsAsync()
  {
    // Return a collection of items for your job to process.
    string[] names = new[] { "Rick", "Morty" };
    return names.ToAsyncEnumerable();
  }

  public override Task<Result> ProcessAsync(string name)
  {
    // Do the work to process each item.
    logger.LogInformation("Hello, {name}", name);
    return Task.FromResult(Result.Success());
  }
}
```

See [more in-depth examples](./examples).

## :rocket: Get Started

Create a new job app from the [GitHub template](https://github.com/runlyio/net-template) or via the dotnet tool template:

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

## Consuming Pre-release Packages

Pre-release packages are published to this project's [GitHub Packages Feed](https://github.com/runlyio/netcore/packages). If you want to consume one of these pre-release packages for testing purposes, you can add a `NuGet.config` file to the root of your project:

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
