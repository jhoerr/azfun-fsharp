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
        NetId: NetId
    }

    let queryUserByNetId (cn:SqlConnection) netid = async {
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

    /// USER ROLES

    let queryRolesByUser (cn:SqlConnection) id = async {
        let query = """
SELECT ud.UserId, ud.DepartmentId, u.NetId, u.Name, d.Name as Department, ud.Role
FROM Users u 
JOIN UserDepartments ud on ud.UserId = u.Id
JOIN Departments d on d.Id = ud.DepartmentId
WHERE u.Id = @Id"""
        try
            let! queryResult = cn.QueryAsync<UserRole>(query, Map["Id", id :> obj]) |> Async.AwaitTask
            match box queryResult with
            | null -> return fail (Status.NotFound, sprintf "No roles found for user id %d" id)
            | _ -> return ok queryResult
        with
        | exn -> return fail (Status.InternalServerError, exn.Message)
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
