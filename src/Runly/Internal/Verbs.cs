using CommandLine;
using System.Collections.Generic;

namespace Runly.Internal
{
	[Verb("list", HelpText = "Lists the jobs discovered by the job host.")]
	class ListVerb
	{
		[Option('v', Default = false, HelpText = "Outputs the verbose default config for each job")]
		public bool Verbose { get; set; }

		[Option('j', Default = false, HelpText = "Formats the output as JSON")]
		public bool Json { get; set; }
	}

	[Verb("get", HelpText = "Gets a config file for the specified type.")]
	class GetVerb
	{
		[Value(0, MetaName = "Type", Required = true, HelpText = "The name of the type.")]
		public string Type { get; set; }

		[Value(1, MetaName = "File Path", Required = false, Default = null, HelpText = "The file path to write the config to. If an existing directory is provided, such as '.', a file named <job-type>.json will be written to the directory.")]
		public string FilePath { get; set; }

		[Option('v', Default = false, HelpText = "Outputs the verbose default config for each job")]
		public bool Verbose { get; set; }
	}

	[Verb("run", HelpText = "Runs the job using the supplied config file.")]
	class RunVerb
	{
		[Value(0, MetaName = "Job Type or Config Path", Required = true, HelpText = "The job type or config file to run.")]
		public string JobOrConfigPath { get; set; }

		[Value(1, MetaName = "Config Arguments", Required = false, HelpText = "Zero or more config arguments separated by a space in the format 'property.subproperty=value'. These arguments override the values set in the default config or config file.")]
		public IEnumerable<string> Props { get; set; }

		[Option('d', HelpText = "Prompts the user to attach a debugger when the job starts. Overrides Config.Execution.LaunchDebugger, setting it to true.")]
		public bool Debug { get; set; }

		[Option('s', HelpText = "Silences console output. Overrides Config.Execution.OutputToConsole, setting it to false.")]
		public bool Silent { get; set; }
	}
}