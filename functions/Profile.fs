namespace MyFunctions

open Chessie.ErrorHandling
open Common
open AuthUtil
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host

///<summary>
/// This module provides a function to return "Pong!" to the calling client. 
/// It demonstrates a basic GET request and response.
///</summary>
module Profile =
    
    type ResponseModel = {
        username: string
        displayName: string
        department: string
        expertise: string
    }

    let getUserProfile username = async {
        let! profile = async.Return { 
            username="bmoberly"
            displayName="Brent Moberly"
            department="UITS"
            expertise="typing, chivalry" 
        }
        return profile |> ok
    }
    /// <summary>
    /// Send a friendly hello message to the client
    /// </summary>
    let toResponse profile = 
        profile
        |> jsonResponse Status.OK 
        |> ok

    let workflow (req: HttpRequest) (config:AppConfig) username = asyncTrial {
        let! _ = requireUserRole config req
        let! profile = bindAsyncResult (fun () -> getUserProfile username)
        let! response = toResponse profile
        return response
    }

    /// <summary>
    /// Say hello to a person by name.
    /// </summary>
    let run (req: HttpRequest) (log: TraceWriter) username config = async {
        let! result = (workflow req config username) |> Async.ofAsyncResult
        return constructResponse log result
    }
