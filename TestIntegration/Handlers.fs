module TestIntegration.Handlers

open Microsoft.AspNetCore.Http
open Oxpecker
open Oxpecker.Htmx
open TestIntegration.templates
open TestIntegration.Tools

let getContacts: EndpointHandler =
    fun ctx ->
        let page = ctx.TryGetQueryValue "page" |> Option.map int |> Option.defaultValue 1

        match ctx.TryGetQueryValue "q" with
        | Some search ->
            let result = ProductService.searchProduct search |> Seq.toArray

            match ctx.TryGetHeaderValue HxRequestHeader.Trigger with
            | Some "search" -> ctx.WriteHtmlView(index.rows page result)
            | _ -> ctx |> writeHtml (index.html search page result "ru" "KZT")
        | None ->
            let result = ProductService.all page |> Seq.toArray
            ctx |> writeHtml (index.html "" page result "ru" "KZT")

let sendBuyRequest (ctx: HttpContext) apiKey lang currency ids =
    task {
        let products =
            ids
            |> List.map ProductService.getById
            |> List.choose (Option.map (fun p -> p.Price))
            |> List.sum

        let baseHost = $"{ctx.Request.Scheme}://{ctx.Request.Host}"

        let! apiResponse =
            Api.createPaymentIntent
                apiKey
                {
                    request_id = System.Guid.NewGuid().ToString()
                    amount = products
                    currency = currency
                    description = "Test Purchase"
                    metadata = System.Collections.Generic.Dictionary<string, obj>()
                    receipt_email = null
                    success_url = $"{baseHost}/success"
                    cancel_url = $"{baseHost}/failed"
                }
                lang

        match apiResponse with
        | Error err ->
            //ctx.SetStatusCode 500
            flash $"Error: {err.Title} ({err.Detail})" ctx
            let page = 1
            let result = ProductService.all page |> Seq.toArray
            return! ctx |> writeHtml (index.html "" page result lang currency)
        | Ok apiResponse ->
            ctx.Response.Cookies.Append("apiKey", apiKey)
            ctx.Response.Headers.Add(HxResponseHeader.Redirect, apiResponse.payment_page_url)
            flash "Bought product!" ctx
            let page = 1
            let result = ProductService.all page |> Seq.toArray
            return! ctx |> writeHtml (index.html "" page result lang currency)
    }

let isValidApiKey apiKey =
    System.String.IsNullOrWhiteSpace(apiKey) |> not

let buyContacts: EndpointHandler =
    fun (ctx: HttpContext) ->
        let apiKey = ctx.TryGetFormValue "apiKey"
        let lang = ctx.TryGetFormValue "lang" |> Option.defaultValue "ru"
        let currency = ctx.TryGetFormValue "currency" |> Option.defaultValue "USD"

        match apiKey with
        | Some apiKey when isValidApiKey apiKey ->
            match ctx.TryGetFormValues "selected_product_ids" with
            | Some ids -> sendBuyRequest ctx apiKey lang currency (ids |> Seq.map int |> Seq.toList)
            | None ->
                task {
                    let page = 1
                    let result = ProductService.all page |> Seq.toArray
                    return! ctx |> writeHtml (index.html "" page result lang currency)
                }
        | _ ->
            ctx |> flash "API key does not provided"
            let page = 1
            let result = ProductService.all page |> Seq.toArray
            ctx |> writeHtml (index.html "" page result "ru" "KZT")

let buyContact id : EndpointHandler =
    fun (ctx: HttpContext) ->
        let apiKey = ctx.TryGetQueryValue "apiKey"
        let lang = ctx.TryGetQueryValue "lang" |> Option.defaultValue "ru"
        let currency = ctx.TryGetQueryValue "currency" |> Option.defaultValue "USD"

        match apiKey with
        | Some apiKey when isValidApiKey apiKey -> sendBuyRequest ctx apiKey lang currency [ id ]
        | _ ->
            ctx |> flash "API key does not provided"
            let page = 1
            let result = ProductService.all page |> Seq.toArray
            ctx |> writeHtml (index.html "" page result "ru" "KZT")


let displaySuccess: EndpointHandler =
    fun ctx ->
        let apiKey = ctx.TryGetCookieValue "apiKey" |> Option.defaultValue ""
        let lang = ctx.TryGetQueryValue "lang" |> Option.defaultValue "ru"

        let transactionId =
            ctx.TryGetQueryValue "transaction_id" |> Option.map int |> Option.defaultValue 0

        task {
            let! transaction = Api.getPaymentIntent apiKey (int32 transactionId) lang

            match transaction with
            | Ok transaction -> return! ctx |> writeHtml (success.html transactionId transaction.status "")
            | Error err ->
                return!
                    ctx
                    |> writeHtml (success.html transactionId "unknown" $"{err.Title} - {err.Detail}")
        }
