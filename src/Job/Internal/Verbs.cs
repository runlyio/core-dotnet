using CommandLine;

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
		[Value(0, MetaName = "Config Path", Required = true, HelpText = "The config file containing the job configuration to run.")]
		public string ConfigPath { get; set; }

		[Value(1, MetaName = "Results Path", Required = false, Default = null, HelpText = "The file path to write the results of the job to. Overrides Execution.ResultsFilePath and sets Execution.ResultsToFile to true.")]
		public string ResultsPath { get; set; }

		[Option('d', HelpText = "Prompts the user to attach a debugger when the job starts.")]
		public bool Debug { get; set; }

		[Option('s', HelpText = "Silences console output. Overrides Config.Execution.OutputToConsole, setting it to false.")]
		public bool Silent { get; set; }
	}
}
