module OrderBookFeedProcessor.Header

open System

type Header = {
    SequenceNumber: int
    Size: int
}

let DecodeHeader (bytes: Byte array) = {
        SequenceNumber = BitConverter.ToInt32(bytes.[0..3], 0)
        Size = BitConverter.ToInt32(bytes.[4..8], 0)
    }
