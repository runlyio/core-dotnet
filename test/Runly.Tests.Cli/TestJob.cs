namespace Runly.Tests.Cli;
public class TestJob : Job<Config>
{
    public TestJob(Config config)
        : base(config) { }

    public override Task<Result> ProcessAsync()
    {
        Console.WriteLine("TestJob is running");

        return Task.FromResult(Result.Success());
    }
}
