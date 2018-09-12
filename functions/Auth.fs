namespace MyFunctions

open Chessie.ErrorHandling
open Common
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open System.Net.Http
open System.Collections.Generic
open Newtonsoft.Json
open Microsoft.Extensions.Configuration

///<summary>
/// This module provides a function to return "Pong!" to the calling client. 
/// It demonstrates a basic GET request and response.
///</summary>
module Auth =
    

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
    
    let returnToken token = 
        tryf Status.InternalServerError (fun () -> token |> jsonResponse Status.OK)

    let workflow (req: HttpRequest) (log: TraceWriter) (config:IConfigurationRoot) = asyncTrial {
        let clientId = config.["OauthClientId"]
        let clientSecret = config.["OauthClientSecret"]
        let tokenUrl = config.["OauthTokenUrl"]
        let redirectUrl = config.["OauthRedirectUrl"]
        
        let! code = getQueryParam "code" req
        let! request = createTokenRequest clientId clientSecret redirectUrl code
        let! response = postAsync<ResponseModel> tokenUrl request
        let! token = bind returnToken response         
        return token
    }

    /// <summary>
    /// Say hello to a person by name.
    /// </summary>
    let run (req: HttpRequest) (log: TraceWriter) (config:IConfigurationRoot) = async {
        let! result = workflow req log config |> Async.ofAsyncResult
        return constructResponse log result
    }
