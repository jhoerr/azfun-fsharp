namespace MyFunctions

open Chessie.ErrorHandling
open System.Data.SqlClient
open Dapper
open Common
open AuthUtil
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host

module Profile =

    [<CLIMutable>]
    [<Table("Users")>]
    type User = {
        Id: Id
        Username: string
        PreferredName: string
        Department: string
        Expertise: string
    }

    type UserRequest = {
        Username: string
    }

    let getProfileRecordByName (cn:SqlConnection) username = async {
        let! queryResultSeq = cn.GetListAsync<User>({Username=username}) |> Async.AwaitTask
        match queryResultSeq |> Seq.tryHead with
        | None -> return fail (Status.NotFound, sprintf "No user found with name '%s'" username)
        | Some (resp) -> return ok resp
    }

    let getProfileRecordById (cn:SqlConnection) id = async {
        let! queryResult = cn.GetAsync<User>(id) |> Async.AwaitTask
        match box queryResult with
        | null -> return fail (Status.NotFound, sprintf "No user found with id %d" id)
        | _ -> return ok queryResult
    }

    let updateProfileDatabaseRecord (cn:SqlConnection) id record update = async {
        let update = {record with Expertise=update.Expertise}
        let! cmdResult = cn.UpdateAsync<User>(update) |> Async.AwaitTask
        match cmdResult with
        | 0 -> return fail (Status.NotFound, sprintf "No user found with id %d" id)
        | _ -> return ok update
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
        use cn = new SqlConnection(config.DbConnectionString);
        let getProfileRecordByName = getProfileRecordByName cn
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
        log.Info
        use cn = new SqlConnection(config.DbConnectionString);
        let getProfileRecordById = getProfileRecordById cn id
        let! result = workflow req config getProfileRecordById |> Async.ofAsyncResult
        return constructResponse log result
    }

module ProfilePut =
    open Profile

    let validatePostBody body = ok body

    let validateUserCanEditRecord claims record = ok record

    let workflow (req: HttpRequest) (config:AppConfig) getProfileRecord updateProfileRecord = asyncTrial {
        let! claims = requireUserRole config req
        let! body = deserializeBody<User> req
        let! update = validatePostBody body
        let! record = bindAsyncResult (fun () -> getProfileRecord)
        let! _ = validateUserCanEditRecord claims record
        let! updatedRecord = bindAsyncResult (fun () -> updateProfileRecord record update)
        let response = updatedRecord |> jsonResponse Status.OK
        return response
    }

    let run req log id config = async {
        use cn = new SqlConnection(config.DbConnectionString);
        let getProfileRecord = getProfileRecordById cn id
        let updateProfileRecord = updateProfileDatabaseRecord cn id
        let! result = workflow req config getProfileRecord updateProfileRecord |> Async.ofAsyncResult
        return constructResponse log result
    }
