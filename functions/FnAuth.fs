namespace MyFunctions.Auth

open Chessie.ErrorHandling
open MyFunctions.Common
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open System.Net.Http
open System.Collections.Generic

///<summary>
/// This module provides a function to return "Pong!" to the calling client. 
/// It demonstrates a basic GET request and response.
///</summary>
module Post =
    
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

    let getAppRole username = async {
        let! role = async.Return (ok ROLE_USER)
        return role
    }

    let returnToken token = 
        let fn() = 
            { access_token = token } 
            |> jsonResponse Status.OK
        tryf Status.InternalServerError fn
    
    let workflow (req: HttpRequest) config = asyncTrial {
        let getUaaJwt request = bindAsyncResult (fun () -> postAsync<ResponseModel> config.OAuth2TokenUrl request)
        let getAppRole username = bindAsyncResult (fun () -> getAppRole username)

        let! oauthCode = getQueryParam "code" req
        let! uaaRequest = createTokenRequest config.OAuth2ClientId config.OAuth2ClientSecret config.OAuth2RedirectUrl oauthCode
        let! uaaJwt = getUaaJwt uaaRequest
        let! uaaClaims = decodeUaaJwt uaaJwt.access_token
        let! appRole = getAppRole uaaClaims.UserName
        let! appJwt = encodeJwt config.JwtSecret uaaClaims.Expiration uaaClaims.UserName appRole
        let! response = returnToken appJwt         
        return response
    }

    /// <summary>
    /// Say hello to a person by name.
    /// </summary>
    let run (req: HttpRequest) (log: TraceWriter) config = async {
        let! result = workflow req config |> Async.ofAsyncResult
        return constructResponse log result
    }
