using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Runly.Tests
{
	public abstract class UnitTest : IAsyncLifetime, IDisposable
	{
		static UnitTest()
		{
			AssertionOptions.AssertEquivalencyUsing(opts => opts.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(2000))).WhenTypeIs<DateTime>());
		}

		public UnitTest() { }
		public virtual void Dispose() { }
		public virtual Task InitializeAsync() => Task.CompletedTask;
		public virtual Task DisposeAsync() => Task.CompletedTask;
	}
}
