namespace MyFunctions

open Chessie.ErrorHandling
open Common
open JwtUtil
open Microsoft.AspNetCore.Http
open System

module AuthUtil =

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
    let validateRole roles (claims:JwtClaims) =
        if roles |> Seq.exists (fun role -> claims.UserRole = role)
        then ok claims
        else fail (Status.Forbidden, "You do not have the required permissions to peform this task.")

    let requireAdminRole (config:AppConfig) (req: HttpRequest) = trial {
        let! claims = validateAuth config.JwtSecret req
        let! user = validateRole [ROLE_ADMIN] claims
        return user
    }
    let requireUserRole (config:AppConfig) (req: HttpRequest) = trial {
        let! claims = validateAuth config.JwtSecret req
        let! user = validateRole [ROLE_USER; ROLE_ADMIN] claims
        return user
    }
