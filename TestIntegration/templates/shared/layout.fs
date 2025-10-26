module TestIntegration.templates.shared.layout

open System
open Microsoft.AspNetCore.Http
open Oxpecker.ViewEngine
open Oxpecker.Htmx
open TestIntegration.Tools

let html (content: HtmlElement) (ctx: HttpContext) =
    let flashMessage = getFlashedMessage ctx

    html (lang = "") {
        head () {
            title () { "Game Shop" }
            script (src = "https://unpkg.com/htmx.org@1.9.10", crossorigin = "anonymous")
            meta (name = "viewport", content = "width=device-width, initial-scale=1")
            link (rel = "stylesheet", href = "/site.css")

            link (
                rel = "stylesheet",
                href = "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css",
                integrity = "sha384-sRIl4kxILFvY47J16cr9ZwB07vP4J8+LH7qKQnuqkuIAvNWLzeN8tE5YBujZqJLB",
                crossorigin = "anonymous"
            )

            script (src = "app.js")
        }

        body (class' = "container", hxBoost = true) {
            main () {
                header () {
                    h1 () { span (style = "text-transform:uppercase;") { a (href = "/") { "Game Shop" } } }
                    h2 () { "FinaInnovate Demo Integration Application" }

                    if String.IsNullOrEmpty flashMessage |> not then
                        div (class' = "alert fadeOut") { flashMessage }
                }

                hr ()
                content
            }
        }
    }
