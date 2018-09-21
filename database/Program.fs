module Program

open SimpleMigrations
open SimpleMigrations.DatabaseProvider
open SimpleMigrations.Console
open System.Reflection
open System.Data.SqlClient

/// Migrate an MSSql (SQL Server) database
let migrate connection args =
    try
        use db = new SqlConnection(connection)
        let provider = MssqlDatabaseProvider(db)
        let assembly = Assembly.GetExecutingAssembly()
        let migrator = SimpleMigrator(assembly, provider)
        let runner = ConsoleRunner(migrator)
        args |> List.toArray |> runner.Run
    with
    | exn -> 
        printf "Error: %s" exn.Message

[<EntryPoint>]
let main argv =

    match argv |> List.ofSeq with
    | connection :: args->
        migrate connection args
    | _ ->
        printf """Usage : dotnet database.dll '<conn>' <args>

<conn>: the SQL Server database connection string
<args>: SimpleMigration args (try 'help')
"""

    0
