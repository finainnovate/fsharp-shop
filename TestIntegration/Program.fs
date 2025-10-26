open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Oxpecker
open Oxpecker.ViewEngine
open TestIntegration.Handlers
open TestIntegration.Api

let endpoints =
    [
        GET [
            route "/" <| redirectTo "/product" true
            subRoute "/product" [
                route "/" getContacts
                routef "/{%i}/buy" buyContact
            ]
            subRoute "/success" [ route "/" displaySuccess ]
        ]
        POST [
            subRoute "/product" [ route "/buy" buyContacts ]
        ]
    ]

let errorView errorCode (errorText: string) =
    html () {
        body (style = "width: 800px; margin: 0 auto") {
            h1 (style = "text-align: center; color: red") { raw $"Error <i>%d{errorCode}</i>" }
            p () { errorText }
        }
    }

let notFoundHandler (ctx: HttpContext) =
    let logger = ctx.GetLogger()
    logger.LogWarning("Unhandled 404 error")
    ctx.SetStatusCode 404
    ctx.WriteHtmlView(errorView 404 "Page not found!")

let errorHandler (ctx: HttpContext) (next: RequestDelegate) =
    task {
        try
            return! next.Invoke(ctx)
        with
        | :? ModelBindException
        | :? RouteParseException as ex ->
            let logger = ctx.GetLogger()
            logger.LogWarning(ex, "Unhandled 400 error")
            ctx.SetStatusCode StatusCodes.Status400BadRequest
            return! ctx.WriteHtmlView(errorView 400 (string ex))
        | ex ->
            let logger = ctx.GetLogger()
            logger.LogError(ex, "Unhandled 500 error")
            ctx.SetStatusCode StatusCodes.Status500InternalServerError
            return! ctx.WriteHtmlView(errorView 500 (string ex))
    }
    :> Task

let configureApp (appBuilder: IApplicationBuilder) =
    appBuilder.UseStaticFiles().UseRouting().Use(errorHandler).UseOxpecker(endpoints).Run(notFoundHandler)

let configureServices (services: IServiceCollection) =
    services.AddRouting().AddOxpecker() |> ignore

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    apiHost <- builder.Configuration.["ApiHost"]
    configureServices builder.Services
    let app = builder.Build()
    configureApp app
    app.Run()

    0 // Exit code
