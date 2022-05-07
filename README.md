# Order Book Feed Processor

### What
This program processes messages for trade orders, and keeps track of an order book and the current market depth for any traded symbol.

### Why
Programmz.

### Invocation

Instructions from the [OrderBookFeedProcessor](./OrderBookFeedProcessor) directory

You need to specify the target depth at startup: `dotnet run . <depth>`

You can assert correctness against the test binary data using the following: `cat ../TestFeedProcessor/data/input1.stream | dotnet run . 5 > test.out && diff test.out ../TestFeedProcessor/data/output.log` (Fish shell)

