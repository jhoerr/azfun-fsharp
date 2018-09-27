namespace MyFunctions

open System
open System.Collections.Generic
open System.IO
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open Chessie.ErrorHandling
open Dapper
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open Newtonsoft.Json
open Newtonsoft.Json.Converters
open Newtonsoft.Json.Serialization
open JWT
open JWT.Algorithms
open JWT.Builder

module Types = 

    let ROLE_ADMIN = "admin"
    let ROLE_USER = "user"

    type Status = HttpStatusCode

    type ErrorModel = {
        errors: array<string>
    }

    type AppConfig = {
        OAuth2ClientId: string
        OAuth2ClientSecret: string
        OAuth2TokenUrl: string
        OAuth2RedirectUrl: string
        JwtSecret: string
        DbConnectionString: string
    }

    type Id = int
    type Name = string
    type NetId = string
    type Role =
        | SelfReport=1
        | ItPro=2
        | Manager=3
        | Admin=4

    [<CLIMutable>]
    [<Table("Users")>]
    type User = {
        Id: Id
        Hash: string
        NetId: NetId
        Name: Name
        Position: string
        LocationCode: string
        Location: string
        CampusPhone: string
        CampusEmail: string
        Expertise: string
    }


    [<CLIMutable>]
    [<Table("Departments")>]
    type Department = {
        Id: Id
        Name: Name
    }

    [<CLIMutable>]
    [<Table("UserDepartments")>]
    type UserDepartment = {
        [<Key>]
        UserId: Id
        [<Key>]
        DepartmentId: Id
        Role: Role
    }

    [<CLIMutable>]
    type UserRole = {
        UserId: Id
        Name: Name
        NetId: NetId
        DepartmentId: Id
        Department: Name
        [<JsonConverter(typedefof<StringEnumConverter>)>]
        Role: Role
    }


    type UserRequest = {
        NetId: string
    }

    type UserReponse = {
        User: User
        Roles: array<UserRole>
    }


///<summary>
/// This module contains common types and functions to facilitate request 
/// handling and response creation. 
///</summary>
module Common =
    open Types 

    // STATIC 
    let client = new HttpClient()

    // UTILITY FUNCTIONS

    /// <summary>
    /// An active pattern to identify empty sequences
    /// </summary>
    let (|EmptySeq|_|) a = if Seq.isEmpty a then Some () else None

    let (|Int|_|) str =
       match System.Int32.TryParse(str) with
       | (true,int) -> Some(int)
       | _ -> None

    /// <summary>
    /// Checks whether the string is null or empty
    /// </summary>
    let isEmpty str = String.IsNullOrWhiteSpace str

    ///<summary>
    /// Given a list of tuples, check whether the first item
    /// of any element matches the provided predicate
    let any pred s = s |> Seq.exists (fun li -> fst li = pred)

    /// <summary>
    /// ROP: Attempt to execute a function.
    /// If it succeeds, pass along the result. 
    /// If it throws, wrap the exception message in a failure with the provided status.
    /// </summary>
    let tryf status fn = 
        try
            fn() |> ok
        with
        | exn -> fail (status, exn.Message)

    /// <summary>
    /// ROP: Attempt to execute a function.
    /// If it succeeds, pass along the result. 
    /// If it throws, wrap the exception message in a failure with the provided status.
    /// </summary>
    let tryf' status msg fn = 
        try
            fn() |> ok
        with
        | exn -> fail (status, sprintf "%s: %s" msg (exn.Message))

    
    // HTTP REQUEST

    let tryDeserialize<'T> status str =
        tryf status (fun () -> str |> JsonConvert.DeserializeObject<'T>)

    /// <summary>
    /// Attempt to deserialize the request body as an object of the given type.
    /// </summary>
    let deserializeBody<'T> (req:HttpRequest) = 
        use stream = new StreamReader(req.Body)
        let body = stream.ReadToEndAsync() |> Async.AwaitTask |> Async.RunSynchronously
        match body with
        | null -> fail (Status.BadRequest, "Expected a request body but received nothing")
        | ""   -> fail (Status.BadRequest, "Expected a request body but received nothing")
        | _    -> tryDeserialize<'T> Status.BadRequest body 

    /// <summary>
    /// Attempt to retrieve a parameter of the given name from the query string
    /// </summary>
    let getQueryParam paramName (req: HttpRequest) =
        if req.Query.ContainsKey paramName
        then ok (req.Query.[paramName].ToString())
        else fail (Status.BadRequest,  (sprintf "Query parameter '%s' is required." paramName))

    let getQueryParamInt paramName (req: HttpRequest) = trial {
        let asInt p =
            match p with
            | Int i -> ok i
            | _ -> fail (Status.BadRequest, (sprintf "Query parameter '%s' must be a number" paramName))

        let! queryParam = getQueryParam paramName req
        let! result = asInt queryParam
        return result
    }

    let getQueryParamInt' paramName min max (req: HttpRequest) = trial {
        let inRange min max queryParam = 
            if (queryParam < min || queryParam > max)
            then fail (Status.BadRequest, (sprintf "Query parameter '%s' must be in range [%d, %d]" paramName min max))
            else ok queryParam
        let! intParam = getQueryParamInt paramName req
        let! result = inRange min max intParam
        return result
    }

    let postAsync<'T> (url:string) (content:HttpContent) : Async<Result<'T,(HttpStatusCode*string)>> = async {
        try
            let! response = client.PostAsync(url, content) |> Async.AwaitTask
            let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            if (response.IsSuccessStatusCode)
            then return tryDeserialize Status.InternalServerError content
            else return fail (response.StatusCode, content)
        with 
        | exn -> return fail (Status.InternalServerError, exn.Message)
    }

    // HTTP RESPONSE

    /// <summary>
    /// Look up common MIME types based on the file extension
    /// </summary>
    let mimeType (path:string) =
        let extension = 
            path.Split([|'.'|]) 
            |> Array.last 
            |> (fun str -> str.ToLowerInvariant())
        match extension with
        | "css" -> "text/css"
        | "js" -> "application/javascript"
        | "html" -> "text/html"
        | "png" -> "image/png"
        | "ico" -> "image/x-icon"
        | "svg" -> "image/svg+xml"
        | _ -> "text/plain"

    /// <summary>
    /// Construct an HTTP response.
    /// </summary>
    let httpResponse status content contentType =
        let response = new HttpResponseMessage(status)
        response.Content <- content
        response.Content.Headers.ContentType <- contentType |> MediaTypeHeaderValue;
        response

    /// <summary>
    /// Construct an HTTP response with file stream content
    /// </summary>
    let fileResponse status path = 
        let stream = new FileStream(path, FileMode.Open)
        let content = new StreamContent(stream)
        httpResponse status content (path |> mimeType)

    /// <summary>
    /// Construct an HTTP response with string content
    /// </summary>
    let stringResponse status str =
        let content = new StringContent(str)
        httpResponse status content "text/plain"

    let jsonSettings = JsonSerializerSettings(ContractResolver=CamelCasePropertyNamesContractResolver())

    /// <summary>
    /// Construct an HTTP response with JSON content
    /// </summary>
    let jsonResponse status model = 
        let content = 
            JsonConvert.SerializeObject(model, jsonSettings)
            |> (fun s -> new StringContent(s))
        httpResponse status content "application/json"

    // ROP

    /// <summary>
    /// Organize the errors into a status code and a collection of error messages. 
    /// If multiple errors are found, the aggregate status will be that of the 
    /// most severe error (500, then 404, then 400, etc.)
    /// </summary>
    let failure msgs =
        let l = msgs |> Seq.toList

        // Determine the aggregate status code based on the most severe error.
        let statusCode = 
            if l |> any Status.InternalServerError then Status.InternalServerError
            elif l |> any Status.NotFound then Status.NotFound
            elif l |> any Status.BadRequest then Status.BadRequest
            else l.Head |> fst

        // Flatten all error messages into a single array.
        let errors = 
            l 
            |> Seq.map snd 
            |> Seq.toArray 
            |> (fun es -> { errors = es } )
        
        ( statusCode, errors )

    /// <summary>
    /// Convert an ROP trial into an HTTP response. 
    /// The result of a successful trial will be passed to the provided success function.
    /// The result(s) of a failed trial will be aggregated, logged, and returned as a 
    /// JSON error message with an appropriate status code.
    /// </summary>
    let constructResponse (log:TraceWriter) trialResult : HttpResponseMessage =
        match trialResult with
        | Ok(result, _) -> result
        | Bad(msgs) -> 
            let (status, errors) = failure (msgs)
            sprintf "%A %O" status errors |> log.Error
            jsonResponse status errors


    /// <summary>
    /// Given an async computation expression that returns a Result<TSuccess,TFailure>,
    /// bind and return the TSuccess.
    /// </summary>
    let bindAsyncResult<'T> (asyncFn: unit -> Async<Result<'T,(HttpStatusCode*string)>>) = asyncTrial {
        let! result = asyncFn()
        let! bound = result
        return bound
    }


/// *******************
/// **  JWT          **
/// *******************

    type JwtClaims = {
        UserName: NetId
        Expiration: System.DateTime
    }

    let ExpClaim = "exp"
    let UserNameClaim = "user_name"
    let UserRoleClaim = "user_role"
    let epoch = DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc)

    // Create and sign a JWT
    let encodeJwt secret exp username userrole = 
        let fn() =
            JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secret)
                .ExpirationTime(exp)
                .AddClaim(UserNameClaim, username)
                .AddClaim(UserRoleClaim, userrole)
                .Build();
        tryf' Status.InternalServerError "Failed to create access token" fn

    /// Convert the "exp" unix timestamp into a Datetime
    let decodeExp (exp:obj) = 
        exp 
        |> string 
        |> System.Double.Parse 
        |> (fun unixTicks -> epoch.AddSeconds(unixTicks))

    /// <summary>
    /// Decode a JWT from the UAA service
    /// </summary>
    let decodeUaaJwt (jwt:string) = 
        try
            let decoded = 
                JwtBuilder()
                    .Decode<IDictionary<string, obj>>(jwt)
            let claims = {
                UserName = decoded.[UserNameClaim] |> string
                Expiration = decoded.[ExpClaim] |> decodeExp
            }
            ok claims
        with 
        | :? TokenExpiredException as ex -> 
            fail (Status.Unauthorized, "Access token has expired")
        | exn ->
            fail (Status.Unauthorized, sprintf "Failed to decode access token: %s" (exn.Message))

    /// <summary>
    /// Decode a JWT issued by the /api/auth function.
    /// </summary>
    let decodeAppJwt (secret:string) (jwt:string) =
        try
            let decoded = 
                JwtBuilder()
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode<IDictionary<string, string>>(jwt)
            let claims = {
                UserName = decoded.[UserNameClaim] |> string
                Expiration = decoded.[ExpClaim] |> decodeExp
            }
            ok claims
        with 
        | :? TokenExpiredException as ex -> 
            fail (Status.Unauthorized, "Access token has expired")
        | :? SignatureVerificationException as ex -> 
            fail (Status.Unauthorized, "Access token has invalid signature")
        | exn ->
            fail (Status.Unauthorized, sprintf "Failed to decode access token: %s" (exn.Message))       


/// *******************
/// **  AUTH         **
/// *******************

    let extractAuthHeader (req: HttpRequest) =
        let authHeader = 
            if req.Headers.ContainsKey("Authorization")
            then string req.Headers.["Authorization"]
            else String.Empty
        if (isEmpty authHeader || authHeader.StartsWith("Bearer ") = false)
        then fail (Status.Unauthorized, "Authorization header is required in the form of 'Bearer <token>'.")
        else authHeader |> ok

    let extractJwt (authHeader: string) =
        let parts = authHeader.Split([|' '|])
        if parts.Length <> 2 
        then fail (Status.Unauthorized, "Authorization header is required in the form of 'Bearer <token>'.")
        else parts.[1] |> ok

    let validateAuth (secret:string) (req: HttpRequest) = trial {
        let! authHeader = extractAuthHeader req
        let! jwt = extractJwt authHeader
        let! claims = decodeAppJwt secret jwt
        return claims
    }
    
    let requireMembership (config:AppConfig) (req: HttpRequest) = trial {
        let! claims = validateAuth config.JwtSecret req
        return claims
    }
