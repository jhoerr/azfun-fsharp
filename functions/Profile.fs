namespace MyFunctions

open Chessie.ErrorHandling
open Common
open AuthUtil
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host

module Profile =

    type ResponseModel = {
        id: Id
        username: string
        displayName: string
        department: string
        expertise: string
    }

    let getProfileRecordByName name = async {
        let! result = async.Return { 
            id=1
            username="bmoberly"
            displayName="Brent Moberly"
            department="UITS"
            expertise="typing, chivalry" 
        }
        return ok result
    }

    let getProfileRecordById id = async {
        let! result = async.Return { 
            id=1
            username="bmoberly"
            displayName="Brent Moberly"
            department="UITS"
            expertise="typing, chivalry" 
        }
        return ok result
    }

    let updateProfileDatabaseRecord id record = async {
        let! result = async.Return record
        return ok result
    }

///<summary>
/// This module provides a function to return "Pong!" to the calling client. 
/// It demonstrates a basic GET request and response.
///</summary>
module ProfileGetMe =
    open Profile
    
    let workflow (req: HttpRequest) (config:AppConfig) getProfileRecordByName = asyncTrial {
        let! claims = requireUserRole config req
        let! profile = bindAsyncResult (fun () -> getProfileRecordByName claims.UserName)
        let response = profile |> jsonResponse Status.OK
        return response
    }

    /// <summary>
    /// Say hello to a person by name.
    /// </summary>
    let run (req: HttpRequest) (log: TraceWriter) config = async {
        let! result = workflow req config getProfileRecordByName |> Async.ofAsyncResult
        return constructResponse log result
    }

///<summary>
/// This module provides a function to return "Pong!" to the calling client. 
/// It demonstrates a basic GET request and response.
///</summary>
module ProfileGet =
    open Profile
    
    let workflow (req: HttpRequest) (config:AppConfig) getProfileRecordById = asyncTrial {
        let! _ = requireUserRole config req
        let! profile = bindAsyncResult (fun () -> getProfileRecordById)
        let response = profile |> jsonResponse Status.OK
        return response
    }

    /// <summary>
    /// Say hello to a person by name.
    /// </summary>
    let run (req: HttpRequest) (log: TraceWriter) id config = async {
        let getProfileRecordById = getProfileRecordById id
        let! result = workflow req config getProfileRecordById |> Async.ofAsyncResult
        return constructResponse log result
    }

module ProfilePut =
    open Profile

    let validatePostBody body = ok body

    let validateUserCanEditRecord claims record = ok record

    let workflow (req: HttpRequest) (config:AppConfig) getProfileRecord updateProfileRecord = asyncTrial {
        let! claims = requireUserRole config req
        let! body = deserializeBody<Profile.ResponseModel> req
        let! validBody = validatePostBody body
        let! record = bindAsyncResult (fun () -> getProfileRecord)
        let! _ = validateUserCanEditRecord claims record
        let! updatedRecord = bindAsyncResult (fun () -> updateProfileRecord validBody)
        let response = updatedRecord |> jsonResponse Status.OK
        return response
    }

    let run req log id config = async {
        let getProfileRecord = getProfileRecordById id
        let updateProfileRecord = updateProfileDatabaseRecord id
        let! result = workflow req config getProfileRecord updateProfileRecord |> Async.ofAsyncResult
        return constructResponse log result
    }
