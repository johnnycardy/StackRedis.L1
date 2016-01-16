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

### Limitations

If you use this library for one Redis client, you must use it for all clients of that database. This is because Redis keyspace notifications alone are not enough to accelerate the Hash and Set types; custom messages are required. If you use this library to accelerate only one client, then that client risks holding on to stale data keys expire or are deleted.

No doubt there are other limitations I haven't thought of, so all feedback is of course welcome.

### Project State

Any types/functions that are not accelerated will work as normal, as the request will be passed directly to Redis. Details on what is accelerated is laid out below.

**String**
The `String` type is fully accelerated, using `MemoryCache` for storage.

**Hash**
The `Hash` type is fully accelerated with a `Dictionary` used for storage.

**Set**
The `Set` type is heavily accelerated using a `HashSet` used for in-memory storage.

**SortedSet**
This is work in progress. Only `SortedSet` operations involving `score` are currently accelerated. This is done using the concept of 'disjointed sets' - a collection of sorted subsets of the full sorted set. This is a picture of the full sorted set "with gaps". If a retrieval request is made involving a gap, then the request is passed to Redis and the result is cached - plugging that gap in-memory.

**List**
The `List` type is not accelerated, as it cannot be cached meaningfully in-memory. This is because operations generally involve the head or the tail of the list, and we cannot know whether or not we have the head or tail in-memory. Most operations would involve invalidating any in-memory list data and so there would be little benefit. All `List` operations are passed through directly to Redis.
