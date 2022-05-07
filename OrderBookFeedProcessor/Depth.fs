module OrderBookFeedProcessor.Depth

open OrderBookFeedProcessor.Message


let private sumOrders orders =
    List.sumBy (fun order -> order.Size) orders

let private groupByPrice side orders =
    let sorted =
        orders
            |> List.groupBy (fun (order: Order) -> order.Price)
            |> List.sortBy fst
    
    if side = Buy then List.rev sorted else sorted


let private groupBySide (orders: Order list) =
    orders
    |> List.groupBy (fun (order: Order) -> order.Side)
    |> List.sortBy fst


type Depth = (Side * (int * int64) list ) list

/// From a list of orders, all of the same Symbol,
/// calculate the Depth for all of the orders but only up to the specified depth
let Depth (depth: int) (ordersForSymbol: Order list) : Depth =
    ordersForSymbol
    |> groupBySide
    |> List.map
        (fun (side, ordersForSide) ->
            (side,
             ordersForSide
             |> (groupByPrice side)
             |> List.truncate depth
             |> List.map (fun (price, ordersByPrice) -> (price, sumOrders ordersByPrice))))
