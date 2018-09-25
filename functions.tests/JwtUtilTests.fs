module Tests

open Chessie.ErrorHandling
open MyFunctions.Common
open System
open Xunit
open Xunit.Abstractions

module JwtUtilTests =

    let username = "johndoe"
    let userrole = "admin"
    let expiration = DateTime(2030,9,13,15,44,03,DateTimeKind.Utc)
    let secret = "jwt signing secret"

    /// NOTE: You can view the contents of these tokens at jwt.io.
    let jwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOiIxOTE1NTQ0NjQzIiwidXNlcl9uYW1lIjoiam9obmRvZSIsInVzZXJfcm9sZSI6ImFkbWluIn0.bMHKuhHO_KbpUe7eQ5AIrtAOhZp17HdRXwzgA0hdv3Y"
    let expiredJwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOiIxNTE1NTQ0NjQzIiwidXNlcl9uYW1lIjoiam9obmRvZSIsInVzZXJfcm9sZSI6ImFkbWluIn0.rz4RXtrGr1WfX0tUBAu2yj-KU7u1gqwZ4oWInm2vd-4"

    [<Fact>]
    let ``Decode UAA JWT`` () =
        let expected = Ok ({ UserName=username; Expiration=expiration; UserRole="" }, [])
        let actual = decodeUaaJwt jwt
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Encode app JWT`` () =
        let expected = Ok (jwt, [])
        let actual = encodeJwt secret expiration username userrole
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Decode app JWT`` () =
        let expected = Ok ({ UserName=username; UserRole=userrole; Expiration=expiration }, [])
        let actual = decodeAppJwt secret jwt
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Decode app JWT validates signature`` () =
        let expected = Bad ([(Status.Unauthorized, "Access token has invalid signature")])
        let actual = decodeAppJwt "different signing secret" jwt
        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Decode app JWT validates expiration`` () =
        let expected = Bad ([(Status.Unauthorized, "Access token has expired")])
        let actual = decodeAppJwt secret expiredJwt
        Assert.Equal(expected, actual)


    [<Fact>]
    let ``Parse double`` () =
        let actual = "123" |> System.Double.Parse
        let expected = float 123
        Assert.Equal(expected, actual)
        

    