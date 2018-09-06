namespace MyFunctions

open Common
open Chessie.ErrorHandling
open System
open System.IO
open System.Net
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs.Host
open System.Reflection

///<summary>
/// This module provides a function to return static assets from the file system. 
/// It can be used to in coordination with proxies.json to serve SPA files. 
///</summary>
module Asset =

    ///<summary>
    /// Find the file system directory of the deploy bin folder
    ///</summary>
    let deployPath =
        Assembly.GetExecutingAssembly().CodeBase
        |> UriBuilder
        |> fun uri -> Uri.UnescapeDataString uri.Path
        |> Path.GetDirectoryName // bin folder
        |> Path.GetDirectoryName // root folder
    
    
    let craAssets = ["index.html"; "asset-manifest.json"; "favicon.ico"; "manifest.json"; "service-worker.js"]
    let assetFolder (path:string) =
        if List.exists (fun file -> file = path) craAssets then ""
        elif path.EndsWith(".css") || path.EndsWith(".css.map") then "static/css/"
        elif path.EndsWith(".js") || path.EndsWith(".js.map") then "static/js/"
        elif path.EndsWith(".svg") || path.EndsWith(".png") then "static/media/"
        else ""

    ///<summary>
    /// Resolve the file system path of the requested asset and
    /// ensure that it exists.
    ///</summary>
    let resolveFilePath (log:TraceWriter) (req: HttpRequest) =
        let reqPath = req.Path.ToString().Replace("/api/asset/","")
        let subDir = assetFolder reqPath
        let filePath = sprintf "%s/static_files/%s%s" deployPath subDir reqPath
        sprintf "Retrieving %s from %s" reqPath filePath |> log.Info
        if File.Exists(filePath)
        then ok filePath
        else fail (Status.NotFound, sprintf "Could not find %s" reqPath)

    ///<summary>
    /// Send the file to the client.
    ///</summary> 
    let sendFile path =
        path |> fileResponse Status.OK 

    ///<summary>
    /// Returns a file from the file system. Best used for static assets.
    ///</summary>
    let run (req: HttpRequest) (log: TraceWriter) =
        (fun () -> req)
        >> resolveFilePath log
        >> constructResponse sendFile log
