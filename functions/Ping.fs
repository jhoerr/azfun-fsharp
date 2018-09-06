namespace MyFunctions

open Chessie.ErrorHandling
open Common
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host

///<summary>
/// This module provides a function to return "Pong!" to the calling client. 
/// It demonstrates a basic GET request and response.
///</summary>
module Ping =
        
    /// <summary>
    /// Send a friendly hello message to the client
    /// </summary>
    let sayHello req = 
        "Pong!" |> stringResponse Status.OK

    /// <summary>
    /// Say hello to a person by name.
    /// </summary>
    let run (req: HttpRequest) (log: TraceWriter) =
        (fun () -> ok req)
        >> constructResponse sayHello log
