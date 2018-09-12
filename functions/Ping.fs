namespace MyFunctions

open Chessie.ErrorHandling
open Common
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open System

///<summary>
/// This module provides a function to return "Pong!" to the calling client. 
/// It demonstrates a basic GET request and response.
///</summary>
module Ping =
    
    type ResponseModel = {
        token: string
    }

    /// <summary> 
    /// Asynchronously generate some result.
    let doSync x =
        // fail (Status.BadRequest, "doSync failed at " + (x |> String.concat " -> ")) 
        ok (x @ ["doSync"])

    let doAsync x = async {
        do! Async.Sleep(1000)
        // return fail (Status.BadRequest, "doAsync failed at " + (x |> String.concat " -> ")) 
        return x @ ["doAsync"] |> ok
    }
    
    /// <summary>
    /// Send a friendly hello message to the client
    /// </summary>
    let sayHello resp = 
        let chain = resp |> String.concat " -> "
        {token = chain } 
        |> jsonResponse Status.OK 
        |> ok

    let workflow (req: HttpRequest) = asyncTrial {
        let! syncResult = doSync ["workflow start"]
        let! asyncResult = doAsync syncResult
        let! syncResult2 = bind doSync asyncResult
        let! result = sayHello (syncResult2 @ ["workflow end"]) 
        return result
    }

    /// <summary>
    /// Say hello to a person by name.
    /// </summary>
    let run (req: HttpRequest) (log: TraceWriter) = async {
        let! result = (workflow req) |> Async.ofAsyncResult
        return constructResponse log result
    }
