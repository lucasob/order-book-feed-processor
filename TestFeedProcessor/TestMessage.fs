module TestFeedProcessor.TestMessage

open OrderBookFeedProcessor.Message
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``An Invalid ByteStream Raises an InvalidMessage Exception`` () =
    (fun () -> DecodeMessage [| 0uy; |] |> ignore) |> should throw typeof<InvalidMessage>

[<Fact>]
let ``An Add Order Be Returned From a Valid ByteStream`` () =
    let bytes = [| 65uy; 86uy; 67uy; 48uy; 72uy; 66uy; 2uy; 0uy; 65uy; 140uy; 1uy; 97uy; 66uy; 32uy; 32uy; 32uy; 136uy; 19uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 80uy; 221uy; 4uy; 0uy; 32uy; 32uy; 32uy; 32uy; |]
    Assert.Equal(({Symbol = "VC0"; OrderId = 6990022307456631368L; Side = Buy; Size = 5000L; Price = 318800} |> Add), (DecodeMessage bytes))
    
[<Fact>]
let ``An Update Order Be Returned From a Valid ByteStream`` () =
    let bytes = [| 85uy; 87uy; 68uy; 48uy; 72uy; 66uy; 2uy; 0uy; 65uy; 140uy; 1uy; 97uy; 66uy; 32uy; 32uy; 32uy; 136uy; 19uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 80uy; 221uy; 4uy; 0uy; 32uy; 32uy; 32uy; 32uy; |]
    Assert.Equal(({Symbol = "WD0"; OrderId = 6990022307456631368L; Side = Buy; Size = 5000L; Price = 318800} |> Update), (DecodeMessage bytes))
    
[<Fact>]
let ``A Delete Order Be Returned From a Valid ByteStream`` () =
    let bytes = [| 68uy; 65uy; 88uy; 55uy; 72uy; 66uy; 2uy; 0uy; 65uy; 140uy; 1uy; 97uy; 66uy; 32uy; 32uy; 32uy; |]
    Assert.Equal(({Symbol = "AX7"; OrderId = 6990022307456631368L; Side = Buy} |> Delete), (DecodeMessage bytes))
    
[<Fact>]
let ``An Execute Order Be Returned From a Valid ByteStream`` () =
    let bytes = [| 69uy; 66uy; 67uy; 53uy; 72uy; 66uy; 2uy; 0uy; 65uy; 140uy; 1uy; 97uy; 83uy; 32uy; 32uy; 32uy; 136uy; 19uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; |]
    Assert.Equal(({Symbol = "BC5"; OrderId = 6990022307456631368L; Side = Sell; Quantity = 5000L} |> Execute), (DecodeMessage bytes))
    