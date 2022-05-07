open System
open System.IO

open OrderBookFeedProcessor.Header
open OrderBookFeedProcessor.Message
open OrderBookFeedProcessor.Processor


[<EntryPoint>]
let main argv =
    
    // Don't bother dying gracefully
    let depth = Int32.Parse(argv.[1])
    let stdin = Console.OpenStandardInput()
    let reader = new BinaryReader(stdin)
    
    // Set our processor 
    let _process = Process depth
    
    // sighs, not quite stateless :(
    let mutable depthCache = Map.empty
    let mutable run = true

    while run do
        try
            let header = DecodeHeader (reader.ReadBytes(8))
            let message = DecodeMessage (reader.ReadBytes(header.Size))
            depthCache <- _process depthCache message header.SequenceNumber
        with
        | :? ArgumentOutOfRangeException -> run <- false
    
    0
