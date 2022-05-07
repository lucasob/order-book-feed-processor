module TestFeedProcessor.TestDepth

open OrderBookFeedProcessor.Message
open OrderBookFeedProcessor.Depth
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``When multiple orders at different prices exist, only those prices within the specified depth should be shown`` () =
    let orderOne = {
        Symbol = "VC0";
        OrderId = 1L;
        Side = Buy;
        Size = 1L;
        Price = 1
    }
    
    let orderTwo = {
        Symbol = "VC0";
        OrderId = 2L;
        Side = Buy;
        Size = 1L;
        Price = 2
    }
    
    let expected = [Buy, [ (1, 1L); ]]
    
    Depth 1 [orderOne; orderTwo] |> should equal expected
    

[<Fact>]
let ``The depth should be respected across both sides`` () =
    let orderOne = {
        Symbol = "VC0";
        OrderId = 1L;
        Side = Buy;
        Size = 1L;
        Price = 1
    }
    
    let orderTwo = {
        Symbol = "VC0";
        OrderId = 2L;
        Side = Buy;
        Size = 1L;
        Price = 2
    }
    
    let orderThree = {
        Symbol = "VC0";
        OrderId = 3L;
        Side = Sell;
        Size = 1L;
        Price = 1
    }
    
    let orderFour = {
        Symbol = "VC0";
        OrderId = 4L;
        Side = Sell;
        Size = 1L;
        Price = 2
    }
    
    let expected = [Buy, [ (1, 1L); ]; Sell, [ (1, 1L) ]]
    
    Depth 1 [orderOne; orderTwo; orderThree; orderFour] |> should equal expected
    

[<Fact>]
let ``Two orders for the same symbol, on the same side with different prices should be stored correctly`` () =
    let orderOne = {
        Symbol = "VC0";
        OrderId = 1L;
        Side = Buy;
        Size = 1L;
        Price = 1
    }
    
    let orderTwo = {
        Symbol = "VC0";
        OrderId = 2L;
        Side = Buy;
        Size = 1L;
        Price = 2
    }
    
    let expected = [Buy, [ (1, 1L); (2, 1L); ]]
    
    Depth 2 [orderOne; orderTwo] |> should equal expected 

[<Fact>]
let ``Two orders for the same symbol, on different sides should be stored correctly`` () =
    let orderOne = {
        Symbol = "VC0";
        OrderId = 1L;
        Side = Buy;
        Size = 1L;
        Price = 1
    }
    
    let orderTwo = {
        Symbol = "VC0";
        OrderId = 2L;
        Side = Sell;
        Size = 1L;
        Price = 1
    }
    
    let expected = [Buy, [ (1, 1L); ]; Sell, [ (1, 1L) ]]
    
    Depth 2 [orderOne; orderTwo] |> should equal expected