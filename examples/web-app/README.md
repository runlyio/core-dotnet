# Web App Example

This is an example MVC web application rendered with razor pages. It is intended to show how you can integrate a Runly job into a web application workflow. The web application queues a long-running, asynchronous job in response to a user action. The application provides real-time feedback to the user on the state of their long-running job.

## Running Locally

Clone this repo and update `runly` section in `appsettings.json` to suit your local environment. [Read the associated documentation](https://www.runly.io/docs/examples/web/#setup) for more information on setting up this example.

To start the web application on `localhost:5000`:

### Via [Visual Studio](https://visualstudio.microsoft.com/)

1. Open the `WebApp.sln` in Visual Studio.
2. Set the `Web` project as the _Startup Project_.
3. Hit `F5` to start debugging the web app.

### Via [Visual Studio Code](https://code.visualstudio.com/)

1. Open the `web-app` folder in VS Code.
2. Hit `F5` to start debugging the web app.

### Via [.NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/)

`cd` to the `Web` folder:

```
cd web-app/Web
dotnet run
```
