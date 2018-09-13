namespace MyFunctions

open Chessie.ErrorHandling
open Common
open JWT
open JWT.Algorithms
open JWT.Builder
open System
open System.Collections.Generic

///<summary>
/// 
///</summary>
module JwtUtil =

    type JwtClaims = {
        UserName: string
        UserRole: string
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
                UserRole = ""
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
                UserRole = decoded.[UserRoleClaim] |> string
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
        
