# Azure Functions + SPA Experiments in F#

This repo contains (or will contain) a functional React SPA + Azure Functions implementation. 
Azure Functions provide a serverless (and cheap!) API alternative to the standard ASP.Net Web API application. 
In serverless architectures, API controllers are eschewed in favor of discrete functions that are triggered by HTTP requests. 
The functions are ultimately hosted within a web application by the Azure Functions runtime, but a key advange of serverless architectures is that the web app configuration and startup is abstracted away and we don't have to worry about it.

## Prerequisites

1. Install the [.NET Core SDK](https://www.microsoft.com/net/learn/get-started)
2. Install [Node.JS and NPM](https://nodejs.org/en/) 
3. Install the Azure Functions CLI:

```
npm i -g azure-functions-core-tools@core --unsafe-perm true
```

## Running the code

1. Clone this repo.
2. Open the folder in Visual Studio Code.
3. From the command palette exec `build`, then `run`. You should see the terminal light up as the Azure Functions runtime starts. 

## Functions

This repo contains three functions. They are documented below along with example `curl` scripts. Note that the http `<port>` in the example script will be different on every machine. The Azure Functions runtime will tell you which port its hosting the functions on.  

**Ping** 

A GET endpoint that returns "Pong!" if everything is working properly.

*Request*
```
curl http://localhost:<port>/api/ping
```

*Response*
```
Pong!
```

**Hello**

A POST function that takes a JSON object with a `FirstName` and `LastName` and returns a friendly greeting.
This function demonstrates how to deserialize and validate a POST body, and create a complex response object. 

*Request*
```
curl -X POST -d '{"FirstName":"John","LastName":"Hoerr"}' http://localhost:<port>/api/hello
```

*Response*
```
{ "Message": "Hello, John Hoerr!"}
```

**Asset**

A GET function that returns a static file from the file system. When serving SPAs it is necessary to provide static assets (.html, .js, .css)
to the client. This function demonstrates how those assets can be delivered from the file system.

*Request*
```
curl http://localhost:<port>/
```

*Response*
```
<html>
<head>
    <title>Hello!</title>
    <link rel="stylesheet" href="/asset/site.css">
</head>
<body>
    <p>Hello, world!</p>
</body>
</html>
```

## Proxies

The Azure Functions platform recently introduced [Proxies](https://docs.microsoft.com/en-us/azure/azure-functions/functions-proxies) as a way to discretely route and redirect requests to other web/API services.

Proxies play an important role in serving an SPA from a serverless platform. At a minimum, the client will need an `index.html` file and will potentially need other `.js`/`.css` assets. Proxies allow us to transparently route a request for the function webroot (`http://localhost:<port>/`) to some function that can satisfy a request for the static `index.html` file (such as our `Asset` function). In this repo we serve the static assets from the file system, but in production you may wish to serve those assets from Azure Blob Storage or CDN.

The `proxies.json` file contains the following proxy definitions to satisfy requests to the webroot and subsequent assets:  
* `/` -> `/api/asset/index.html`  
* `/index.html` -> `/api/asset/index.html`  
* `/asset/{*path}` -> `/api/asset/{path}`  

## Error Handling

This project uses [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/) to manage execution and handle errors. ROP is a pattern that keeps code clean and ensures all errors are handled and meaningfully reported. 

To get a sense of ROP, consider a POST request such as `Hello` above. To satisfy this request the POST body must be deserialized and validated. In the ROP pattern we break up these operations into discrete workflow steps:

```
request
>> deserialize post body
>> validate post body
>> create response
```

If any step in that workflow fails, the remaining steps are skipped and error information is returned to the client. If all steps succeed, a friendly greeting is returned to the client. ROP keeps the workflow front and center while abstracting the management of error information. 

An F# implementation of ROP is provded by the [Chessie](https://github.com/fsprojects/Chessie) library.

## Deploying to Azure

This repo comes with a [Circle CI](https://circleci.com) [configuration](.circleci/config.yml) file that will build, test, package, and deploy the Functions app to Azure via the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest). 

The Azure CLI requires authentication credentials in order to perform the deployment. It's best to use a non-person, [Service Principal](https://docs.microsoft.com/en-us/cli/azure/create-an-azure-service-principal-azure-cli?view=azure-cli-latest) account for this purpose. To create a Service Principal, you will need to get an Azure CLI instance authenticated with the account associated with your Functions App. You can either:  
+ install the Azure CLI locally and sign in with `az login`;  
+ or sign in to [Azure Cloud Shell](https://shell.azure.com/).

Once you have an Azure CLI instance, run:

```
az ad sp create-for-rbac --name USERNAME --password PASSWORD
```

The result should be a block of text similar to:

```
{
  "appId": "38f6...",
  "displayName": "USERNAME",
  "name": "http://USERNAME",
  "password": "PASSWORD",
  "tenant": "1113..."
}
```

Browse to Circle CI. If you don't have a Circle CI account, create one now. It's free for public GitHub repos. Circle CI uses "Contexts" as a store for secrets that need to be available during the build. We'll create a Context now and add the appropriate secrets to it. 

1. Browse to the *Add Projects* section, choose your repo, click *Set Up Project* and then *Start Building*. 
2. In the *Settings* -> *Contexts* section, click *Create Context*.
3. Name the context `azfun-fsharp`. Note: you can name it something different, but you'll need to update the context reference at the bottom of .circleci/config.yml.
4. Add the following environment variables:  
    + `SERVICE_PRINCIPAL_USER`: The Service Principal username url (e.g. http://USERNAME)  
    + `SERVICE_PRINCIPAL_PASSWORD`: The Service Principal password  
    + `SERVICE_PRINCIPAL_TENANT`: The Service Principal tenant  
    + `FUNCTION_APP_TEST`: The name of your *test* Function App   
    + `FUNCTION_APP_TEST_RESOURCE_GROUP`: The name of the Azure resource group your *test* Function App is in  

Circle CI should now have the information it needs to build, test, package, and deploy your Function App + SPA.
