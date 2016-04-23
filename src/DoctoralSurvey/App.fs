﻿module App

open Models
open Suave
open Suave.Operators
open Suave.Writers
open Suave.Successful
open Suave.Filters
open Suave.Json
open Suave.Files

let defaultMimeTypesMap = function
  | ".css" -> mkMimeType "text/css" true
  | ".gif" -> mkMimeType "image/gif" false
  | ".png" -> mkMimeType "image/png" false
  | ".htm"
  | ".html" -> mkMimeType "text/html" true
  | ".jpe"
  | ".jpeg"
  | ".jpg" -> mkMimeType "image/jpeg" false
  | ".js"  -> mkMimeType "application/x-javascript" true
  | _      -> None

let mimeTypes =
    defaultMimeTypesMap
        @@ (function | ".svg" -> mkMimeType "image/svg+xml" false | _ -> None)
        @@ (function | ".ttf" -> mkMimeType "application/octet-stream" false | _ -> None)
        @@ (function | ".woff" -> mkMimeType "application/font-woff" false | _ -> None)
        @@ (function | ".woff2" -> mkMimeType "application/font-woff2" false | _ -> None)

let buildApp staticFileRoot =
  let questions = getQuestions () |> Models.toJson
  choose
    [ GET >=> choose
        [ path "/hello" >=> OK "Hello GET"
          path "/goodbye" >=> OK "Good bye GET"
          path "/questions" >=> OK questions ]
      POST >=> choose
        [ path "/hello" >=> OK "Hello POST"
          path "/goodbye" >=> OK "Good bye POST" ] 
          
      path "/json" >=> (mapJson (fun (a: Foo) -> { bar = a.foo}))
      path "/" >=> browseFile staticFileRoot "index.html"
      pathRegex "(.*)\.(css|png|gif|js|woff2|ttf|woff|ico|html)" >=> Files.browseHome
       ]
