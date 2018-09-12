namespace MyFunctions

open Microsoft.Azure.WebJobs
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open Microsoft.Extensions.Configuration

///<summary>
/// This module defines the bindings and triggers for all functions in the project
///</summary
module Functions =

    let config (context:ExecutionContext) = 
        ConfigurationBuilder()
            .SetBasePath(context.FunctionAppDirectory)
            .AddJsonFile("local.settings.json", optional=true, reloadOnChange= true)
            .AddEnvironmentVariables()
            .Build();

    [<FunctionName("Ping")>]
    let ping
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "get", Route = "ping")>]
        req: HttpRequest,
        log: TraceWriter) =
            Ping.run req log

    [<FunctionName("Hello")>]
    let helloYou
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "post", Route = "hello")>]
        req: HttpRequest,
        log: TraceWriter) =
            Hello.run req log

    [<FunctionName("Asset")>]
    let assetFiles
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "get", Route = "asset/{path}")>]
        req: HttpRequest,
        log: TraceWriter) =
            Asset.run req log

    [<FunctionName("Auth")>]
    let auth
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "get", Route = "auth")>]
        req: HttpRequest,
        log: TraceWriter,
        context: ExecutionContext) =
            context |> config |> Auth.run req log
