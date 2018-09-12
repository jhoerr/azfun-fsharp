namespace MyFunctions

open Chessie.ErrorHandling
open Common
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host

///<summary>
/// This module provides a function that gives a friendly greeting to the calling client.
/// It demonstrates basic POST body validation and using the body to create a response.
///</summary>
module Hello =
        
    type RequestModel = {
        FirstName: string
        LastName: string
    }

    type ResponseModel = {
        Message: string
    }

    /// <summary>
    /// validate the request model properties
    /// </summary>
    let validateInput input = 
        if isEmpty input.FirstName then fail (Status.BadRequest, "Please pass a JSON object with a FirstName")
        elif isEmpty input.LastName then fail (Status.BadRequest, "Please pass a JSON object with a LastName")
        else ok input

    /// <summary>
    /// Send a friendly hello message to the client
    /// </summary>
    let sayHello model = 
        { Message = sprintf "Hello, %s %s" model.FirstName model.LastName }
        |> jsonResponse Status.OK 
        |> ok

    /// <summary>
    /// Say hello to a person by name.
    /// </summary>
    let run (req: HttpRequest) (log: TraceWriter) =
        req
        |> deserializeBody<RequestModel>
        |> bind validateInput
        |> bind sayHello
        |> constructResponse log
