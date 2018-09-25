namespace MyFunctions

open Chessie.ErrorHandling
open Common
open Types
open Dapper
open System.Data.SqlClient

module Database =

    let like (term:string)  = 
        term.Replace("[", "[[]").Replace("%", "[%]") 
        |> sprintf "%%%s%%"


    /// USER

    type UserQuery = {
        NetId: string
    }

    let queryUserByName (cn:SqlConnection) netid = async {
        let! queryResultSeq = cn.GetListAsync<User>({NetId=netid}) |> Async.AwaitTask
        match queryResultSeq |> Seq.tryHead with
        | None -> return fail (Status.NotFound, sprintf "No user found with netid '%s'" netid)
        | Some (resp) -> return ok resp
    }

    let queryUser (cn:SqlConnection) id = async {
        let! queryResult = cn.GetAsync<User>(id) |> Async.AwaitTask
        match box queryResult with
        | null -> return fail (Status.NotFound, sprintf "No user found with id %d" id)
        | _ -> return ok queryResult
    }

    let updateUser (cn:SqlConnection) id record update = async {
        let update = {record with Expertise=update.Expertise}
        let! cmdResult = cn.UpdateAsync<User>(update) |> Async.AwaitTask
        match cmdResult with
        | 0 -> return fail (Status.NotFound, sprintf "No user found with id %d" id)
        | _ -> return ok update
    }


    /// SEARCH

    type SimpleSearchQuery = {
        Term: string
    }

    let querySearch connStr term page items = async {
        let query = "WHERE Department LIKE @Term OR Name LIKE @Term or NetId LIKE @Term"
        let order = "Department ASC, Name ASC"
        use cn = new SqlConnection(connStr)
        let! queryResultSeq = 
            cn.GetListPagedAsync<User>(page, items, query, order, {Term=(like term)}) 
            |> Async.AwaitTask
        return queryResultSeq |> Seq.toArray |> ok
    }
