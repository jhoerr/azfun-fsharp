namespace MyFunctions

open Common
open Chessie.ErrorHandling
open Microsoft.Azure.WebJobs
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open Microsoft.Extensions.Configuration

///<summary>
/// This module defines the bindings and triggers for all functions in the project
///</summary
module Functions =

    let appConfig (context:ExecutionContext) = 
        let config = 
            ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional=true, reloadOnChange= true)
                .AddEnvironmentVariables()
                .Build();
        {
            OAuth2ClientId = config.["OAuthClientId"]
            OAuth2ClientSecret = config.["OAuthClientSecret"]
            OAuth2TokenUrl = config.["OAuthTokenUrl"]
            OAuth2RedirectUrl = config.["OAuthRedirectUrl"]
            JwtSecret = config.["JwtSecret"]
            DbConnectionString = config.["DbConnectionString"]
        }

    [<FunctionName("Ping")>]
    let ping
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "get", Route = "ping")>]
        req: HttpRequest,
        log: TraceWriter) =
            Ping.Get.run req log |> Async.StartAsTask

    [<FunctionName("Auth")>]
    let auth
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "get", Route = "auth")>]
        req: HttpRequest,
        log: TraceWriter,
        context: ExecutionContext) =
            context |> appConfig |> Auth.Post.run req log |> Async.StartAsTask

    [<FunctionName("ProfileGet")>]
    let profileGet
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "get", Route = "profile/{id}")>]
        req: HttpRequest,
        log: TraceWriter,
        context: ExecutionContext,
        id: Id) =
            context |> appConfig |> User.GetId.run req log id |> Async.StartAsTask

    [<FunctionName("ProfileGetMe")>]
    let profileGetMe
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "get", Route = "me")>]
        req: HttpRequest,
        log: TraceWriter,
        context: ExecutionContext) =
            context |> appConfig |> User.GetMe.run req log |> Async.StartAsTask

    [<FunctionName("ProfilePut")>]
    let profilePut
        ([<HttpTrigger(Extensions.Http.AuthorizationLevel.Anonymous, "put", Route = "profile/{id}")>]
        req: HttpRequest,
        log: TraceWriter,
        context: ExecutionContext,
        id: Id) =
            context |> appConfig |> User.Put.run req log id |> Async.StartAsTask
