module TestIntegration.templates.index

open Oxpecker.ViewEngine
open Oxpecker.Htmx
open TestIntegration.Models
open TestIntegration.templates.shared

let rows page (products: Product[]) =
    Fragment() {
        for contact in products do
            tr () {
                td () { input (type' = "checkbox", name = "selected_product_ids", value = $"{contact.Id}") }
                td () { contact.ProductName }
                td () { contact.SKU }
                td () { contact.Price.ToString() }

                td () {
                    button (
                        class' = "btn btn-secondary",
                        hxGet = ($"/product/{contact.Id}/buy"),
                        hxDisabledElt = "this",
                        hxInclude = "[name='apiKey'], [name='lang'], [name='currency']",
                        hxTarget = "body"
                    ) {
                        "Buy"
                    }
                }
            }

        if products.Length = 5 then
            tr () {
                td (colspan = 5, style = "text-align: center") {
                    span (
                        hxTarget = "closest tr",
                        hxTrigger = "revealed",
                        hxSwap = "outerHTML",
                        hxSelect = "tbody > tr",
                        hxGet = $"/product?page={page + 1}"
                    ) {
                        "Loading More..."
                    }
                }
            }
    }

let html q page (products: Product[]) lang currency =
    Fragment() {
        form (action = "/product", method = "get") {
            div (class' = "row") {
                div (class' = "col") { label (for' = "search", class' = "form-label") { "Search Term" } }

                div (class' = "col") {


                }
            }

            div (class' = "row") {
                div (class' = "col") {
                    input (
                        id = "search",
                        type' = "search",
                        name = "q",
                        value = q,
                        style = "margin: 0 5px",
                        class' = "form-control",
                        autocomplete = "off",
                        hxGet = "/product",
                        hxTrigger = "search, keyup delay:200ms changed",
                        hxTarget = "tbody",
                        hxPushUrl = "true",
                        hxIndicator = "#spinner"
                    )

                    img (
                        id = "spinner",
                        class' = "spinner htmx-indicator",
                        src = "/spinning-circles.svg",
                        alt = "Request In Flight..."
                    )
                }

                div (class' = "col") { input (type' = "submit", value = "Search", class' = "btn btn-primary") }
            }
        }

        form () {
            div (class' = "row") {
                div (class' = "col") { label (for' = "apiKey", class' = "form-label") { "API key" } }
                div (class' = "col") { label (for' = "lang", class' = "form-label") { "Lang" } }
                div (class' = "col") { label (for' = "currency", class' = "form-label") { "Currency" } }
            }

            div (class' = "row") {
                div (class' = "col") {
                    input (
                        id = "apiKey",
                        type' = "text",
                        name = "apiKey",
                        value = "",
                        class' = "form-control",
                        autocomplete = "off"
                    )
                }

                div (class' = "col") {
                    select (id = "lang", name = "lang", class' = "form-control", autocomplete = "off") {
                        option (value = "ru", selected = (lang = "ru")) { "Russian" }
                        option (value = "en", selected = (lang = "en")) { "English" }
                    }
                }

                div (class' = "col") {
                    select (id = "currency", name = "currency", class' = "form-control", autocomplete = "off") {
                        option (value = "USD", selected = (currency = "USD")) { "USD" }
                        option (value = "KZT", selected = (currency = "KZT")) { "KZT" }
                        option (value = "EUR", selected = (currency = "EUR")) { "EUR" }
                    }
                }
            }

            table (class' = "table") {
                thead () {
                    tr () {
                        th ()
                        th () { "Product" }
                        th () { "SKU" }
                        th () { "Price" }
                        th ()
                    }
                }

                tbody () { rows page products }
            }

            p () {
                span (style = "float: left") {
                    button (
                        hxPost = "/product/buy",
                        hxConfirm = "Are you sure you want to buy these products?",
                        hxDisabledElt = "this",
                        hxTarget = "body",
                        class' = "btn btn-secondary"
                    ) {
                        "Buy Selected Products"
                    }
                }

            //span(style="float: right") {
            //    a(href="/contacts/new") { "Add Contact" }
            //    span(hxGet="/contacts/count", hxTrigger="revealed"){
            //        img(class'="spinner htmx-indicator", src="/spinning-circles.svg")
            //    }
            //}
            }
        }
    }
    |> layout.html
