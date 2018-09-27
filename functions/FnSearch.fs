namespace MyFunctions.Search

open Chessie.ErrorHandling
open MyFunctions.Types
open MyFunctions.Common
open MyFunctions.Database
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open System.Net
open System.Net.Http

module GetSimple =

    type SimpleSearchResult = {
        Term: string
        Page: int
        Items: int
        Results: array<User>
    }

    let workflow (req: HttpRequest) config getSearchResults = asyncTrial {
        let! _ = requireMembership config req
        let! term = getQueryParam "term" req
        let! page = getQueryParamInt "page" req
        let! items = getQueryParamInt' "items" 1 100 req 
        let! results = bindAsyncResult (fun () -> getSearchResults term page items)
        let response = 
            { Term=term; Page=page; Items=items; Results=results }
            |> jsonResponse Status.OK
        return response
    }

    let run (req: HttpRequest) (log: TraceWriter) config = async {
        let querySearch = querySearch config.DbConnectionString
        let! result = workflow req config querySearch |> Async.ofAsyncResult
        return constructResponse log result
    }
