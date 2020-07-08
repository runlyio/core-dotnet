# Runly Core

> The core application/API
 
## Running Locally

### Create your local database

```
cd src/Init
dotnet run database "server=.;database=runly;integrated security=true;"
```

Replace the connection string as necessary, ensuring that the same connection string is found in `src/Api/settings.json` and/or `src/Api/settings.user.json`.

### Run the API server

```
cd ../Api
dotnet run
```

This will start the API server and configure it to listen at `https://localhost:5001` and `http://localhost:5000`.

### Pack System Jobs

Now that the database and API are up, we need to create the default org and packages. Open a second console and **build and pack the System app**:

```
rm src/System/bin/Debug/*.nupkg
dotnet pack src/System

rm src/System.Internal/bin/Debug/*.nupkg
dotnet pack src/System.Internal
```

This will build `nupkg` files for the two system jobs packages. The `rm` command ensures old package files are not used in the next step.

### Set up Runly org

Use the init tool to create the initial _Runly_ system organization and upload the System package:

```
cd src/Init
dotnet run api
```

### Run the App

With the API up and running with data, the app can be launched to begin using the system. Clone the [web repo](https://github.com/runlyio/web) and follow the instructions there to start the app server.

### Start a Node

The final step in setting up the Runly system to run jobs is to run a node.

1. Open a new console and cd to the `src/Cli` folder.

2. **Get a cluster key.** In the app, login and switch to the organization you want to connect a node to. Navigate to the org's clusters page and copy the API key of the cluster the node will connect to.

3. **Set the API key.** Use the CLI to set the API key for the node. Replace `CLUSTER_API_KEY` with the API key copied in the previous step.

   ```
   dotnet run -- set-apikey CLUSTER_API_KEY
   ```

4. **Start the node.**

   ```
   dotnet run -- start-node -u http://localhost:5000
   ```

## Overriding API Settings

If you need to change any of the settings in `settings.json` for your machine, rather than edit that file, change `settings.user.json` which will override what is in `settings.json`. This file is not in source control and won't affect other developers.

## Run the Tests

```
dotnet test
```

The API tests use a sql database. If the default connection string in `test-settings.json` doesn't work for your machine, update `test-settings.user.json` with a different connection string. This file is not in source control.

## Deployment Process

Every commit to `master` runs a CD build and deploys the resulting API artifacts to the Azure App Service at [`api.runly.io`](https://api.runly.io/).

