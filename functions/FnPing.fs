namespace MyFunctions.Ping

open Chessie.ErrorHandling
open MyFunctions.Types
open MyFunctions.Common
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open System.Net
open System.Net.Http

///<summary>
/// This module provides a function to return "Pong!" to the calling client. 
/// It demonstrates a basic GET request and response.
///</summary>
module Get =
    
    type ResponseModel = {
        token: string
    }

    /// <summary> 
    /// Asynchronously generate some result.
    let doSync x =
        ok (x @ ["doSync"])

    let doAsync x = async {
        do! Async.Sleep(1000)
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
        let! asyncResult = bindAsyncResult (fun () -> doAsync syncResult)
        let! syncResult2 = doSync asyncResult
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
