module OrderBookFeedProcessor.Processor

open OrderBookFeedProcessor.Message
open OrderBookFeedProcessor.Depth

let private addOrder (allOrders: Order list) (order: Order) = (order :: allOrders, order.Symbol)

let private updateOrder (allOrders: Order list) (order: Order) =
    (order
     :: List.filter (fun o -> o.OrderId <> order.OrderId) allOrders,
     order.Symbol)

let private deleteOrder (allOrders: Order list) (deletion: Deletion) =
    (allOrders
     |> List.filter (fun order -> order.OrderId <> deletion.OrderId),
     deletion.Symbol)

let private executeOrder (allOrders: Order list) (execution: Execution) =
    let orderMatchingExecution =
        allOrders
        |> List.find (fun order -> order.OrderId = execution.OrderId)

    let newOrder =
        { orderMatchingExecution with
              Size = orderMatchingExecution.Size - execution.Quantity }

    let allOrdersWithoutExecuted =
        allOrders
        |> List.filter (fun order -> order.OrderId <> orderMatchingExecution.OrderId)

    if newOrder.Size > 0L then
        (newOrder :: allOrdersWithoutExecuted), execution.Symbol
    else
        allOrdersWithoutExecuted, execution.Symbol


type SymbolSummary = { orders: Order list; depth: Depth }
type OrderBook = Map<string, SymbolSummary>
type messageHandler = Message -> Order list

let private printDepth (depth: Depth) (symbol: string) (sequence: int) =

    let sideOrEmpty (side: Side) =
        depth
        |> Map.ofList
        |> Map.tryFind side
        |> Option.defaultValue []

    let askDepth = sideOrEmpty Sell
    let bidDepth = sideOrEmpty Buy

    // This is dogs nuts and is purely because F# formats lists with ;
    $"{sequence}, {symbol}, {bidDepth}, {askDepth}".Replace(";", ",")

let Process (depth: int) (allOrders: OrderBook) (next: Message) (sequenceNumber: int) =

    let forSymbolOrDefault (symbol: string) =
        (Option.defaultValue { orders = []; depth = [] } (Map.tryFind symbol allOrders))

    let updatedOrders, symbol =
        match next with
        | Add order -> addOrder (forSymbolOrDefault order.Symbol).orders order
        | Update order -> updateOrder (forSymbolOrDefault order.Symbol).orders order
        | Delete deletion -> deleteOrder (forSymbolOrDefault deletion.Symbol).orders deletion
        | Execute execution -> executeOrder (forSymbolOrDefault execution.Symbol).orders execution

    let nextDepth = Depth depth updatedOrders

    if nextDepth <> (forSymbolOrDefault symbol).depth then
        printfn $"{printDepth nextDepth symbol sequenceNumber}"

    Map.add
        symbol
        { orders = updatedOrders
          depth = nextDepth }
        allOrders
