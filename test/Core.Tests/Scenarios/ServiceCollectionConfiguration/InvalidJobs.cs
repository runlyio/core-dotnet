using System;
using System.Threading.Tasks;

namespace Runly.Tests.Scenarios.ServiceCollectionConfiguration
{
	public abstract class AbstractJob : Job
	{
		public AbstractJob()
			: base(new Config()) { }

		public override Task InitializeAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync()
		{
			throw new NotImplementedException();
		}
	}

	public class GenericJob<T> : Job
	{
		public GenericJob()
			: base(new Config()) { }

		public override Task InitializeAsync()
		{
			throw new NotImplementedException();
		}

		public override Task<Result> ProcessAsync()
		{
			throw new NotImplementedException();
		}
	}
}
