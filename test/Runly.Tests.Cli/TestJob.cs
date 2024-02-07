namespace Runly.Tests.Cli;
public class TestJob : Job<Config>
{
    public TestJob(Config config)
        : base(config) { }

    public override async Task<Result> ProcessAsync()
    {
        await Task.Delay(2000);

        return Result.Success();
    }
}
