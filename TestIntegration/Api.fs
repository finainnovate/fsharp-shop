module TestIntegration.Api

open System
open System.Collections.Generic
open System.Threading.Tasks
open FsHttp
open Microsoft.AspNetCore.Mvc

let mutable apiHost: string = ""

[<CLIMutable>]
type CreatePaymentIntentRequest =
    {
        /// <summary>
        /// A unique identifier for the request, which can be used for idempotency.
        /// </summary>
        request_id: string

        amount: int64

        /// <summary>
        /// Three-letter ISO currency code, in lowercase. Must be a supported currency.
        /// </summary>
        currency: string

        /// <summary>
        /// An arbitrary string attached to the object. Often useful for displaying to users.
        /// </summary>
        description: string

        /// <summary>
        /// Set of key-value pairs that you can attach to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// </summary>
        metadata: Dictionary<string, Object>

        /// <summary>
        /// Email address that the receipt for the resulting payment will be sent to.
        /// </summary>
        receipt_email: string
        success_url: string
        cancel_url: string
    }

[<CLIMutable>]
type CreatePaymentIntentResponse =
    {
        transaction_id: string
        confirm_url: string
        payment_page_url: string
    }

[<CLIMutable>]
type PaymentIntentResponse =
    {
        /// <summary>
        /// A unique identifier for the request, which can be used for idempotency.
        /// </summary>
        request_id: string

        amount: int64

        /// <summary>
        /// Three-letter ISO currency code, in lowercase. Must be a supported currency.
        /// </summary>
        currency: string

        /// <summary>
        /// An arbitrary string attached to the object. Often useful for displaying to users.
        /// </summary>
        description: string

        /// <summary>
        /// Set of key-value pairs that you can attach to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// </summary>
        metadata: Dictionary<string, Object>

        /// <summary>
        /// Status of the transaction. One of values: "pending", "completed", "failed", "canceled"
        /// </summary>
        status: string
    }

let private decodeResponse expectedCode (response: Response) : Task<Result<'T, ProblemDetails>> =
    task {
        return!
            match response.statusCode with
            | code when code = expectedCode ->
                let result =
                    response |> Response.deserializeJsonTAsync<'T> Threading.CancellationToken.None

                result.ContinueWith(fun (t: Task<'T>) ->
                    if t.IsFaulted then
                        Error(
                            ProblemDetails(
                                Title = "Failed to deserialize response",
                                Detail =
                                    (if isNull t.Exception then
                                         null
                                     else
                                         (nonNull t.Exception).Message),
                                Status = Nullable 500
                            )
                        )
                    else
                        Ok(t.Result))
            | _ ->
                let result =
                    response
                    |> Response.deserializeJsonTAsync<ProblemDetails> Threading.CancellationToken.None

                result.ContinueWith(fun (t: Task<ProblemDetails>) ->
                    if t.IsFaulted then
                        Error(
                            ProblemDetails(
                                Title = "Failed to deserialize error response",
                                Detail =
                                    (if isNull t.Exception then
                                         null
                                     else
                                         (nonNull t.Exception).Message),
                                Status = Nullable 500
                            )
                        )
                    else
                        Error(t.Result))
    }

let createPaymentIntent
    apiKey
    (data: CreatePaymentIntentRequest)
    (lang: string)
    : Task<Result<CreatePaymentIntentResponse, ProblemDetails>>
    =
    task {
        let! response =
            http {
                POST $"{apiHost}/api/partners/v1/payment-intent"
                CacheControl "no-cache"
                AcceptLanguage lang
                Authorization("Bearer " + apiKey)
                body
                jsonSerialize data
            }
            |> Request.sendTAsync

        return! decodeResponse Net.HttpStatusCode.OK response
    }

let getPaymentIntent apiKey (id: int32) (lang: string) : Task<Result<PaymentIntentResponse, ProblemDetails>> =
    task {
        let request =
            http {
                GET $"{apiHost}/api/partners/v1/payment-intent/{id}"
                CacheControl "no-cache"
                //ContentType "application/json" Text.Encoding.UTF8
                //header "Content-Type" "application/json"
                AcceptLanguage lang
                Authorization("Bearer " + apiKey)
            }
        //let content = Request.print request
        let! response = request |> Request.sendTAsync
        //let content = Response.print response
        return! decodeResponse Net.HttpStatusCode.OK response
    }
