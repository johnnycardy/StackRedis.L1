# StackRedis.L1
In-Memory "L1" cache for the .NET StackExchange.Redis library.

This library implements the `StackExchange.Redis.IDatabase` interface to provide an in-memory cache layer between your .NET application and Redis.

Additionally, it can be used as a purely in-memory cache, without any Redis instance. This is useful for simulating redis use, in test cases, for example.

### Why?

Network latency is your primary bottleneck when talking to Redis. Usually this is solved via the use of in-memory caching on the application server; this project is an attempt to generalise that. Any data sent to Redis is intercepted and stored in memory using the .NET MemoryCache, and later requests for that data are returned from MemoryCache if possible.

### What about multiple clients?

If you have multiple clients all talking to Redis, then each one can still use this cache layer without the risk of stale data. This is achieved by invalidating data appropriately in the background by using [Redis keyspace notifications](http://redis.io/topics/notifications).

### Usage

If you are already using StackExchange.Redis, then integration into your project is a 2-line change:

    IDatabase database = connection.GetDatabase(); //Get your Redis IDatabase instance as normal
    IDatabase l1Database = new StackRedis.L1.RedisL1Database(database) //Create the in-memory cached database on top
  
Since the `RedisL1Database` implements `IDatabase`, it's a simple swap.

If you wish to clear your in-memory state, simply `Dispose` it and re-create it.

### Project State

It's early days... at the moment, most calls involving the `String` type are accelerated, but nothing else. Unimplemented calls are passed directly to Redis. In other words, dropping this library in will speed up StringGet but won't affect other parts of IDatabase.

### Limitations

There is currently a trade-off between performance and data integrity in a specific scenario. When two instances of this in-memory cache are connected to the same Redis database, and update the same key within a configurable timespan (currently 1s), then one of the servers will be left with an out-of-date value.

This is remediated by one or more of the following methods:
 - Decide whether it's a risk: in your scenario, you may be doing frequent reads and infrequent writes. This is when the in-memory solution is most effective, and also renders the out-of-date value problem unlikely.
 - Key timeouts will expire the invalid value within a known time.
 - A subsequent update will overwrite the invalid key.
