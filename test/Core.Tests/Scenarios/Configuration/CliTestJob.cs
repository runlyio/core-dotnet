using System;
using System.Threading.Tasks;

namespace Runly.Tests.Scenarios.Configuration
{
	public class CliTestJob : Job<CliTestJobConfig>
	{
		public CliTestJob(CliTestJobConfig config)
			: base(config) { }

		public override Task<Result> ProcessAsync() => Task.FromResult(Result.Success());
	}

	public class CliTestJobConfig : Config
	{
		public bool IsBool { get; set; }
		public JobMethod Enum { get; set; }
		public string[] StringArray { get; set; }
		public string String { get; set; }
		public int Int32 { get; set; }
		public Guid? Guid { get; set; }
		public ComplexType Complex { get; set; }
		public ComplexType[] ComplexArray { get; set; }

		public class ComplexType
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}
	}
}
