module TestFeedProcessor.TestHeader

open System
open Xunit

open OrderBookFeedProcessor.Header

[<Fact>]
let ``Test Valid Byte Stream Is Decoded Into a Header`` () =
   let header = [| 184uy; 136uy; 0uy; 0uy; 32uy; 0uy; 0uy; 0uy; |]
   Assert.True(DecodeHeader header = { SequenceNumber= 35000; Size = 32; })