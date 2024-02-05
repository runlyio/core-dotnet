using Runly;

await JobHost.CreateDefaultBuilder(args)
    .Build()
    .RunJobAsync();