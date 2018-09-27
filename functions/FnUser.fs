namespace MyFunctions.User

open Chessie.ErrorHandling
open System.Data.SqlClient
open MyFunctions.Types
open MyFunctions.Common
open MyFunctions.Database
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open System.Net
open System.Net.Http


///<summary>
/// This module provides a function to return "Pong!" to the calling client. 
/// It demonstrates a basic GET request and response.
///</summary>
module GetMe =
    let workflow (req: HttpRequest) (config:AppConfig) queryUserByNetId queryRolesByUser = asyncTrial {
        let! claims = requireMembership config req
        let! user = bindAsyncResult<User> (fun () -> queryUserByNetId claims.UserName)
        let! roles = bindAsyncResult (fun () -> queryRolesByUser user.Id)
        let response = {User=user; Roles=Seq.toArray roles} |> jsonResponse Status.OK
        return response
    }

    /// <summary>
    /// Say hello to a person by name.
    /// </summary>
    let run (req: HttpRequest) (log: TraceWriter) config = async {
        use cn = new SqlConnection(config.DbConnectionString);
        let queryUserByNetId = queryUserByNetId cn
        let queryRolesByUser = queryRolesByUser cn
        let! result = workflow req config queryUserByNetId queryRolesByUser |> Async.ofAsyncResult
        return constructResponse log result
    }

///<summary>
/// This module provides a function to return "Pong!" to the calling client. 
/// It demonstrates a basic GET request and response.
///</summary>
module GetId =
    let workflow (req: HttpRequest) (config:AppConfig) queryUserById queryRolesByUser = asyncTrial {
        let! _ = requireMembership config req
        let! user = bindAsyncResult (fun () -> queryUserById)
        let! roles = bindAsyncResult (fun () -> queryRolesByUser)
        let response = {User=user; Roles=Seq.toArray roles} |> jsonResponse Status.OK
        return response
    }

    /// <summary>
    /// Say hello to a person by name.
    /// </summary>
    let run (req: HttpRequest) (log: TraceWriter) id config = async {
        use cn = new SqlConnection(config.DbConnectionString);
        let queryUserById = queryUser cn id
        let queryRolesByUser = queryRolesByUser cn id
        let! result = workflow req config queryUserById queryRolesByUser |> Async.ofAsyncResult
        return constructResponse log result
    }

module Put =
    let validatePostBody body = ok body

    let validateUserCanEditRecord claims record = ok record

    let workflow (req: HttpRequest) (config:AppConfig) getProfileRecord updateProfileRecord = asyncTrial {
        let! claims = requireMembership config req
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
        let getUser = queryUser cn id
        let updateUser = updateUser cn id
        let! result = workflow req config getUser updateUser |> Async.ofAsyncResult
        return constructResponse log result
    }
