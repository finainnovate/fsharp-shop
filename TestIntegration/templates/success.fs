module TestIntegration.templates.success

open Oxpecker.ViewEngine
open TestIntegration.templates.shared

let html transactionId status errorMessage =
    Fragment() {
        match status with
        | "success" ->
            h1 () { "Success" }
            p () { $"Your transaction #{transactionId} was successful." }
        | "failed" ->
            h1 () { "Failed!" }
            p () { $"Your transaction #{transactionId} was failed." }
        | _ ->
            h1 () { $"Payment Status: {status}" }
            p () { $"Your transaction #{transactionId} was unknown status: {errorMessage}." }

    }
    |> layout.html
