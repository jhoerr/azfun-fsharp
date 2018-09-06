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
3. From the command palette (cmd+shift+p or ctrl+shift+p) exec `restore` to restore NuGet packages.
4. From the command palette exec `build`, then `run`. You should see the terminal light up as the Azure Functions runtime starts. 

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

Proxies play an important role in serving an SPA from a serverless platform. At a minimum, the client will need an `index.html` file and will potentially need other `.js`/`.css` assets. Proxies allow us to transparently satisfy a request to function webroot (`http://localhost:<port>/`) to a function (such as `Asset`) that can satisfy a request for the static `index.html` file. In this repo we serve the static assets from the file system, but in production you may wish to serve those assets from Azure Blob Storage or CDN.

The `proxies.json` file contains the following proxy definitions to satisfy requests to the webroot and subsequent assets:  
* `/` -> `/api/asset/index.html`  
* `/index.html` -> `/api/asset/index.html`  
* `/asset/{*path} -> `/api/asset/{path}`  

## Error Handling

This project uses [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/) to manage execution and handle errors. ROP is a pattern that keeps code clean anbd ensures all errors are handled and meaningfully reported. 

Consider a POST request such as `Hello` above. To satisfy this request the POST body must be deserialized and validated. The essence of ROP is that we can break up these operations into discrete steps and create a workflow:

```
request
>> deserialize post body
>> validate post body
>> create response
```

If any step in that workflow fails, the remaining steps are skipped and error information is returned to the client. If all steps succeed, a friendly greeting is returned to the client. ROP keeps the workflow front and center while abstracting the management of error information. 

An F# implementation of ROP is provded by the [Chessie](https://github.com/fsprojects/Chessie) library.
