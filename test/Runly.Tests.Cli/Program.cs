using Microsoft.Extensions.Hosting;
using Runly;

await JobHost.CreateDefaultBuilder(args)
    .Build()
    .RunAsync();