using FluentAssertions;
using Runly.Internal;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Runly.Tests.Scenarios.Configuration
{
	public class Generating_config_schema
	{
		readonly JsonSchema schema;

		public Generating_config_schema()
		{
			schema = new JsonSchema(new FakeLogger<JsonSchema>());
		}

		Task<string> LoadExpectedSchema(string name)
		{
			var ass = Assembly.GetExecutingAssembly();
			using var str = ass.GetManifestResourceStream($"{GetType().Namespace}.{name}.json");
			using var reader = new StreamReader(str);
			return reader.ReadToEndAsync();
		}

		[Fact]
		public async Task Should_generate_schema_from_config()
		{
			string expected = await LoadExpectedSchema("config");
			string json = schema.Generate(typeof(Config)).ToString();
			json.Should().Be(expected);
		}

		[Fact]
		public async Task Should_generate_schema_from_config_without_xml_comments()
		{
			string expected = await LoadExpectedSchema("other-config");
			string json = schema.Generate(typeof(OtherConfig)).ToString();
			json.Should().Be(expected);
		}
	}
}
