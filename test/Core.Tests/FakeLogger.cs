using Microsoft.Extensions.Logging;
using System;

namespace Runly.Tests
{
	public class FakeLogger<T> : ILogger<T>
	{
		public IDisposable BeginScope<TState>(TState state) => new FakeDisposer();
		class FakeDisposer : IDisposable { public void Dispose() { } }

		public bool IsEnabled(LogLevel logLevel) => false;
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }
	}
}
