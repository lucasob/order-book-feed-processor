module OrderBookFeedProcessor.Message

open System
    
type Side =
    | Buy
    | Sell
    
exception InvalidOrderSide of string

let private OrderSideFromByte (byte: Byte) =
    match byte with
    | 'B'B -> Buy
    | 'S'B -> Sell
    | _ -> raise (InvalidOrderSide($"{byte} is not a valid OrderSide"))

///
/// Price reflects the signed, little-endian
/// binary-encoded integer stored in 4 bytes.
///
/// Fixed left by 4 decimal places,
/// 318800 => $31.88
/// 

type Order = {
    Symbol: string
    OrderId: int64
    Side: Side
    Size: int64
    Price: int
}

let private OrderFromBytes (bytes: Byte array) =
    {
        Symbol = bytes.[1..3] |> System.Text.Encoding.ASCII.GetString
        OrderId = BitConverter.ToInt64(bytes.[4..12], 0)
        Side = OrderSideFromByte bytes.[12]
        Size = BitConverter.ToInt64(bytes.[16..24], 0)
        Price = BitConverter.ToInt32(bytes.[24..28], 0)
    }

type Deletion = {
    Symbol: string
    OrderId: int64
    Side: Side
}

let private DeletionFromBytes (bytes: Byte array) =
    {
        Symbol = bytes.[1..3] |> System.Text.Encoding.ASCII.GetString
        OrderId = BitConverter.ToInt64(bytes.[4..12], 0)
        Side = OrderSideFromByte bytes.[12]
    }

type Execution = {
    Symbol: string
    OrderId: int64
    Side: Side
    Quantity: int64
}

let private ExecutionFromBytes (bytes: Byte array) =
    {
        Symbol = bytes.[1..3] |> System.Text.Encoding.ASCII.GetString
        OrderId = BitConverter.ToInt64(bytes.[4..12], 0)
        Side = OrderSideFromByte bytes.[12]
        Quantity = BitConverter.ToInt64(bytes.[16..24], 0)
    }

type Message =
    | Add of Order
    | Update of Order
    | Delete of Deletion
    | Execute of Execution
    
exception InvalidMessage of string

// TODO This should also match head :: tail -> fun () , [] -> None
let DecodeMessage (bytes: Byte array) =
    match bytes.[0] with
    | 'A'B -> (OrderFromBytes bytes) |> Add
    | 'U'B -> (OrderFromBytes bytes) |> Update
    | 'D'B -> (DeletionFromBytes bytes) |> Delete
    | 'E'B -> (ExecutionFromBytes bytes) |> Execute
    | _ -> raise (InvalidMessage("Unknown Message Type"))