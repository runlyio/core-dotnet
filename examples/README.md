# Runly Examples

> Example [runly](https://www.runly.io/) processes

## Running Locally

This project builds as a .NET Core console application. It contains a number of Runly processes that can be run from the CLI. For a full list of commands, run:

```
dotnet run -- help
```

> Note! The "--" argument shown in the samples here is only a construct of using the "dotnet run" command to separate arguments that should apply to the dotnet tool on the left from arguments that should be passed into the example application. If you run the compiled application itself, you will not need the "--" separator.

### Processes

* [`CopyDirectory`](#CopyDirectory)
* more coming soon...

#### `CopyDirectory`

Create a file called `copydirectory.json` in the root of this directory with the following contents:

```json
{
  "source": "/some/path/to/a/folder/with/files/in/it",
  "destination": "/another/path/to/copy/those/files/to",
  "ignoreUnauthorizedAccessException": false,
  "process": "Runly.Examples.CopyDirectory"
}
```

Change the settings to your liking and then run the process:

```
dotnet run -- run copydirectory.json
```

This will copy files from the `source` folder to the `destination` folder.

## Running on Runly

...
