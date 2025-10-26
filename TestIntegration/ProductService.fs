module TestIntegration.ProductService


open System
open System.Threading
open TestIntegration.Models

let internal productsDb =
    ResizeArray(
        [
            {
                Id = 1
                ProductName = "Premium Photo Editing Software"
                SKU = "PES-2024-PRO"
                Price = 12999
            }
            {
                Id = 2
                ProductName = "Complete Web Development Course"
                SKU = "WDC-FULL-001"
                Price = 8999
            }
            {
                Id = 3
                ProductName = "Stock Photo Bundle - Nature Pack"
                SKU = "SPB-NAT-500"
                Price = 2999
            }
            {
                Id = 4
                ProductName = "AI-Powered Logo Designer"
                SKU = "ALD-AI-PRO"
                Price = 5999
            }
            {
                Id = 5
                ProductName = "Digital Marketing Masterclass"
                SKU = "DMM-2024-001"
                Price = 159999
            }
            {
                Id = 6
                ProductName = "3D Model Library - Furniture Set"
                SKU = "3DM-FUR-100"
                Price = 7999
            }
            {
                Id = 7
                ProductName = "Cybersecurity Certification Prep"
                SKU = "CSC-CERT-024"
                Price = 19999
            }
            {
                Id = 8
                ProductName = "Mobile App UI/UX Template Pack"
                SKU = "MAU-TMP-050"
                Price = 3999
            }
            {
                Id = 9
                ProductName = "Data Science with Python Course"
                SKU = "DSP-PY-2024"
                Price = 11999
            }
            {
                Id = 10
                ProductName = "Royalty-Free Music Collection"
                SKU = "RFM-COL-200"
                Price = 4999
            }
            {
                Id = 11
                ProductName = "E-commerce Website Builder"
                SKU = "EWB-PRO-001"
                Price = 999999
            }
            {
                Id = 12
                ProductName = "Digital Art Brushes - Watercolor"
                SKU = "DAB-WC-075"
                Price = 1999
            }
            {
                Id = 13
                ProductName = "Cloud Storage Solution - 1TB"
                SKU = "CSS-1TB-001"
                Price = 6999
            }
            {
                Id = 14
                ProductName = "Video Editing Transitions Pack"
                SKU = "VET-TRN-150"
                Price = 2499
            }
            {
                Id = 15
                ProductName = "Cryptocurrency Trading Bot"
                SKU = "CTB-AUTO-01"
                Price = 24999
            }
            {
                Id = 16
                ProductName = "WordPress Theme - Business Pro"
                SKU = "WPT-BIZ-PRO"
                Price = 4999
            }
            {
                Id = 17
                ProductName = "Online Language Learning Suite"
                SKU = "OLL-SUITE-5"
                Price = 13999
            }
            {
                Id = 18
                ProductName = "Social Media Management Tool"
                SKU = "SMM-TOOL-24"
                Price = 7999
            }
            {
                Id = 19
                ProductName = "Digital Invoice Generator"
                SKU = "DIG-INV-PRO"
                Price = 3499
            }
            {
                Id = 20
                ProductName = "Virtual Reality Game Asset Pack"
                SKU = "VRG-AST-001"
                Price = 16999
            }
        ]
    )

let count () =
    Thread.Sleep 2000
    productsDb.Count

let searchProduct (search: string) =
    productsDb
    |> Seq.filter (fun c ->
        c.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase)
        || c.SKU.Contains(search, StringComparison.OrdinalIgnoreCase))

let getById id =
    productsDb |> Seq.tryFind (fun c -> c.Id = id)

let all page =
    productsDb |> Seq.skip ((page - 1) * 5) |> Seq.truncate 5
