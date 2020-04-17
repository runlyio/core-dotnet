# Getting Started Examples

This is an example project that mirrors the [Getting Started Documentation on Runly](https://www.runly.io/docs/getting-started/).

## Running Locally

This project builds as a .NET Core console application. It contains a number of Runly jobs that can be run from the CLI. For a full list of available commands, run:

```
dotnet run -- help
```

> Note! The "--" argument shown in the samples here is only a construct of using the "dotnet run" command to separate arguments that should apply to the dotnet tool on the left from arguments that should be passed into the example application. If you run the compiled application itself, you will not need the "--" separator.

### Jobs

* [`PlaceImporter`](#placeimporter)
* [`CopyDirectory`](#copydirectory)

#### `PlaceImporter`

This job "imports" national places from a US Census publicly available CSV file into a fake database. This job demonstrates processing a CSV file in parallel and doing something with the parsed data.

Create a file called `placeimporter.json` in the the project root with the following contents:

```json
{
	"baseUrl": "http://www2.census.gov/geo/docs/reference/codes/files/",
	"job": "Runly.Examples.Census.PlaceImporter",
	"execution": {
		"parallelTaskCount": 50
	}
}
```

Note the `parallelTaskCount` which determines how many parallel tasks will execute at a time to process the CSV file. Change the settings to your liking and then run the job:

```
dotnet run -- run placeimporter.json -c
```

This will output the parsed place names categorized by state.

#### `CopyDirectory`

Create a file called `copydirectory.json` in the the project root with the following contents:

```json
{
  "source": "/some/path/to/a/folder/with/files/in/it",
  "destination": "/another/path/to/copy/those/files/to",
  "ignoreUnauthorizedAccessException": false,
  "job": "Runly.Examples.FileSystem.CopyDirectory"
}
```

Change the settings to your liking and then run the job:

```
dotnet run -- run copydirectory.json -c
```

This will copy files from the `source` folder to the `destination` folder and report the results to the console.

## Running on Runly

See the [QuickStart Guide](https://www.runly.io/docs/getting-started/) to package this application up and run the jobs on [Runly](https://www.runly.io/).
