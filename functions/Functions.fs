namespace MyFunctions

open Microsoft.Azure.WebJobs
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host

///<summary>
/// This module defines the bindings and triggers for all functions in the project
///</summary
module Functions =

    let getResponse fn = fn()

    [<FunctionName("Ping")>]
    let ping
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "get", Route = "ping")>]
        req: HttpRequest,
        log: TraceWriter) =
            Ping.run req log |> getResponse

    [<FunctionName("Hello")>]
    let helloYou
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "post", Route = "hello")>]
        req: HttpRequest,
        log: TraceWriter) =
            Hello.run req log |> getResponse

    [<FunctionName("Asset")>]
    let assetFiles
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "get", Route = "asset/{path}")>]
        req: HttpRequest,
        log: TraceWriter) =
            Asset.run req log |> getResponse
