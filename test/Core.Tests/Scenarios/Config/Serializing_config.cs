using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Runly.Diagnostics;
using Xunit;

namespace Runly.Tests.Scenarios.Configuration
{
	public class Reduced_form_config : UnitTest
	{
		readonly ServiceProvider sp;
		readonly JobCache jobCache;
		readonly ConfigReader reader;

		public Reduced_form_config()
		{
			var services = new ServiceCollection();
			services.AddRunlyJobs(new[] { "list" }, typeof(TestJob).Assembly);
			sp = services.BuildServiceProvider();

			jobCache = sp.GetRequiredService<JobCache>();
			reader = new ConfigReader(jobCache);
		}

		public override void Dispose()
		{
			sp?.Dispose();
		}

		[Fact]
		public void Should_serialize_reduced_config()
		{
			var config = new Config() { Job = new JobConfig() { Type = "SomeType" } };

			ConfigWriter.ToReducedJson(config).Replace("\r", "").Replace("\n", "").Should().Be("{  \"job\": \"SomeType\"}");
		}

		[Fact]
		public void Should_serialize_full_config()
		{
			var expected = new TestConfig();
			expected.Job.Package = "Runly.Debug";
			expected.Job.Version = "0.1.0";
			expected.Job.Type = typeof(TestJob).FullName;

			var actual = JObject.Parse(ConfigWriter.ToJson(expected));

			actual["job"].Should().NotBeNull();
			actual["job"]["package"].ToString().Should().Be("Runly.Debug");
			actual["job"]["version"].ToString().Should().Be("0.1.0");
			actual["job"]["type"].ToString().Should().Be(typeof(TestJob).FullName);
		}

		[Fact]
		public void Should_deserialize_reduced_config()
		{
			var expected = new TestConfig();
			expected.Job.Type = typeof(TestJob).FullName;

			var actual = reader.FromJson(ConfigWriter.ToReducedJson(expected));

			actual.Should().NotBeNull();
			actual.Job.Type.Should().Be(typeof(TestJob).FullName);
		}

		[Fact]
		public void Should_deserialize_full_config()
		{
			var expected = new TestConfig();
			expected.Job.Package = "Runly.Debug";
			expected.Job.Version = "0.1.0";
			expected.Job.Type = typeof(TestJob).FullName;

			var actual = reader.FromJson(ConfigWriter.ToJson(expected));

			actual.Job.Should().NotBeNull();
			actual.Job.Package.Should().Be("Runly.Debug");
			actual.Job.Version.Should().Be("0.1.0");
			actual.Job.Type.Should().Be(typeof(TestJob).FullName);
		}
	}
}
