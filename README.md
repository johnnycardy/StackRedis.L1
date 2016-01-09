# StackRedis.L1
In-Memory "L1" cache for the .NET StackExchange.Redis library.

This library implements the `StackExchange.Redis.IDatabase` interface to provide an in-memory cache layer between your .NET application and Redis.

Additionally, it can be used as a purely in-memory cache, without any Redis instance. This is useful for simulating redis use, in test cases, for example.

### Why?

Network latency is your primary bottleneck when talking to Redis. Usually this is solved via the use of in-memory caching on the application server; this project is an attempt to generalise that. Any data sent to Redis is intercepted and stored in memory using the .NET MemoryCache, and later requests for that data are returned from MemoryCache if possible.

### What about multiple clients?

If you have multiple clients all talking to Redis, then each one can still use this cache layer without the risk of stale data. This is achieved by invalidating data appropriately in the background by using a combination of [Redis keyspace notifications](http://redis.io/topics/notifications) and custom pub/sub channels.

### Usage

If you are already using StackExchange.Redis, then integration into your project is a 2-line change:

    IDatabase database = connection.GetDatabase(); //Get your Redis IDatabase instance as normal
    IDatabase l1Database = new StackRedis.L1.RedisL1Database(database) //Create the in-memory cached database on top
  
Since the `RedisL1Database` implements `IDatabase`, it's a simple swap.

If you wish to clear your in-memory state, simply `Dispose` it and re-create it.

### Project State

The `String`, `Set`, and `Hash` types are accelerated. Other types will follow eventually, but unimplemented calls are passed directly to Redis. In other words, dropping this library in will speed up `StringGet`, `HashGet` etc but won't affect other parts of IDatabase.

### Limitations

If you use this library for one Redis client, you must use it for all clients of that database. This is because Redis keyspace notifications alone are not enough to accelerate the Hash and Set types; custom messages are required. If you use this library to accelerate only one client, then that client risks holding on to stale data when other non-accelerated clients expire or delete keys.
