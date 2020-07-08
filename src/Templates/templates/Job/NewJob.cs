using Microsoft.Extensions.Logging;
using Runly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp
{
	public class NewJob : Job<NewJobConfig, string>
	{
		public NewJob(NewJobConfig config)
			: base(config) { }

		public override IAsyncEnumerable<string> GetItemsAsync()
		{
			throw new NotImplementedException();
		}

		public override async Task<Result> ProcessAsync(string name)
		{
			throw new NotImplementedException();
		}
	}
}
