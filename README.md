# redis-L1
.NET in-Memory "L1" cache for StackExchange.Redis.

This library implements the `StackExchange.Redis.IDatabase` interface to provide an in-memory cache layer between your .NET code and Redis.

### Why?

Network latency is your primary bottleneck when talking to Redis. Usually this is solved via the use of in-memory caching on the application server; this library is an attempt to generalise that. Any data it sees is stored in memory using the .NET MemoryCache, and later requests for that data are returned from MemoryCache if possible.

### What about multiple clients?

If you have multiple servers all talking to Redis, then each server can still use this cache layer. Its data is kept up-to-date in the background by using [Redis keyspace notifications](http://redis.io/topics/notifications).

### Usage

If you are already using StackExchange.Redis, then integration into your project is a 2-line change:

    var database = connection.GetDatabase(); //Get your Redis IDatabase instance as normal
    var cachedDatabase = new RedisL1Database(database) //Create the in-memory cached database on top
  
Since the `RedisL1Database` implements `IDatabase`, it's a simple swap.
