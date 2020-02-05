# Runly Examples

> Example [runly](https://www.runly.io/) processes

## Running Locally

This project builds as a .NET Core console application. It contains a number of Runly processes that can be run from the CLI. For a full list of available commands, run:

```
dotnet run -- help
```

> Note! The "--" argument shown in the samples here is only a construct of using the "dotnet run" command to separate arguments that should apply to the dotnet tool on the left from arguments that should be passed into the example application. If you run the compiled application itself, you will not need the "--" separator.

### Processes

* [`PlaceImporter`](#placeimporter)
* [`CopyDirectory`](#copydirectory)

#### `PlaceImporter`

This process "imports" national places from a US Census publicly available CSV file into a fake database. This process demonstrates processing a CSV file in parallel and doing something with the parsed data.

Create a file called `placeimporter.json` in the the project root with the following contents:

```json
{
	"baseUrl": "http://www2.census.gov/geo/docs/reference/codes/files/",
	"process": "Runly.Examples.Census.PlaceImporter",
	"execution": {
		"parallelTaskCount": 50
	}
}
```

Note the `parallelTaskCount` which determines how many parallel tasks will execute at a time to process the CSV file. Change the settings to your liking and then run the process:

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
  "process": "Runly.Examples.FileSystem.CopyDirectory"
}
```

Change the settings to your liking and then run the process:

```
dotnet run -- run copydirectory.json -c
```

This will copy files from the `source` folder to the `destination` folder and report the results to the console.

## Running on Runly

1. Pack the application into a nuget package.

```
dotnet pack -c Release
```

2. Create a free account at [Runly](https://www.runly.io/).

3. Upload the created package (`bin/Release/Runly.Examples.1.0.0.nupkg`) to your organization.

4. Queue the process using one of the aformentioned json config files.
