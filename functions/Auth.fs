namespace MyFunctions

open Chessie.ErrorHandling
open Common
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open System.Net.Http
open System.Collections.Generic
open Newtonsoft.Json
open Newtonsoft.Json
open Microsoft.Extensions.Configuration

///<summary>
/// This module provides a function to return "Pong!" to the calling client. 
/// It demonstrates a basic GET request and response.
///</summary>
module Auth =
    
    let client = new HttpClient()

    type ResponseModel = {
        access_token: string
    }

    let createTokenRequest clientId clientSecret redirectUrl code =
        let fn () = 
            dict[
                "grant_type", "authorization_code"
                "code", code
                "client_id", clientId
                "client_secret", clientSecret
                "redirect_uri", redirectUrl
            ]
            |> Dictionary
            |> (fun d-> new FormUrlEncodedContent(d))
        tryf Status.InternalServerError fn
    
    let exchangeCodeForToken (log: TraceWriter) (oauthTokenUrl: string) (tokenRequest: FormUrlEncodedContent) =
        let fn () = 
            client.PostAsync(oauthTokenUrl, tokenRequest) 
            |> Async.AwaitTask 
            |> Async.RunSynchronously
            |> (fun resp -> resp.Content.ReadAsStringAsync())
            |> Async.AwaitTask
            |> Async.RunSynchronously
            // TODO -- add some error handling to deal with failed UAA request { "error":"...", "error_description":"..."}
            |> JsonConvert.DeserializeObject<ResponseModel>
        tryf Status.InternalServerError fn      

    /// <summary>
    /// Send a friendly hello message to the client
    /// </summary>
    let returnToken token = 
        token |> jsonResponse Status.OK

    /// <summary>
    /// Say hello to a person by name.
    /// </summary>
    let run (req: HttpRequest) (log: TraceWriter) (config:IConfigurationRoot)  =
        
        let clientId = config.["OauthClientId"]
        let clientSecret = config.["OauthClientSecret"]
        let tokenUrl = config.["OauthTokenUrl"]
        let redirectUrl = config.["OauthRedirectUrl"]

        let fromQueryString = getQueryParam "code"
        let createTokenRequest = createTokenRequest clientId clientSecret redirectUrl
        let exchangeCodeForToken = exchangeCodeForToken log tokenUrl

        (fun () -> req)
        >> fromQueryString
        >> bind createTokenRequest
        >> bind exchangeCodeForToken
        >> constructResponse returnToken log
