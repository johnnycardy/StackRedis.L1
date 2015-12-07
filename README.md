# StackRedis.L1
In-Memory "L1" cache for the .NET StackExchange.Redis library.

This library implements the `StackExchange.Redis.IDatabase` interface to provide an in-memory cache layer between your .NET code and Redis.

### Why?

Network latency is your primary bottleneck when talking to Redis. Usually this is solved via the use of in-memory caching on the application server; this project is an attempt to generalise that. Any data it sees is stored in memory using the .NET MemoryCache, and later requests for that data are returned from MemoryCache if possible.

### What about multiple clients?

If you have multiple clients all talking to Redis, then each one can still use this cache layer. Data is kept up-to-date in the background by using [Redis keyspace notifications](http://redis.io/topics/notifications).

### Usage

If you are already using StackExchange.Redis, then integration into your project is a 2-line change:

    IDatabase database = connection.GetDatabase(); //Get your Redis IDatabase instance as normal
    IDatabase l1Database = new StackRedis.L1Database(database) //Create the in-memory cached database on top
  
Since the `StackRedis.L1Database` implements `IDatabase`, it's a simple swap.

### Project State

It's early days... at the moment, calls involving the `String` type are accelerated, but nothing else. Unimplemented calls are passed directly to Redis. In other words, dropping this library in will speed up StringGet.
