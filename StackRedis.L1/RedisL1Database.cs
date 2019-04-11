using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using StackRedis.L1.MemoryCache;
using StackRedis.L1.MemoryCache.Types;
using StackRedis.L1.KeyspaceNotifications;
using StackRedis.L1.Notifications;

namespace StackRedis.L1
{
    public class RedisL1Database : IDatabase, IDisposable
    {
        private readonly IDatabase _redisDb;
        private readonly DatabaseInstanceData _dbData;

        private readonly string _uniqueId;
        
        /// <summary>
        /// Constructs a memory-caching layer for Redis IDatabase
        /// </summary>
        /// <param name="redisDb">IDatabase instance</param>
        public RedisL1Database(IDatabase redisDb)
            : this($"{redisDb.Multiplexer.Configuration}:db={redisDb.Database}", redisDb)
        { }

        /// <summary>
        /// Constructs a memory-caching layer that simulates Redis.
        /// </summary>
        /// <param name="uniqueId">Unique string to represent this database instance.</param>
        public RedisL1Database(string uniqueId)
            : this(uniqueId, null)
        { }

        private RedisL1Database(string uniqueId, IDatabase redisDb)
        {
            _uniqueId = uniqueId;

            if (redisDb != null)
            {
                _redisDb = new NotificationDatabase(redisDb);
            }

            //Register for subscriptions and get the in-memory data store
            _dbData = DatabaseRegister.Instance.GetDatabaseInstanceData(uniqueId, redisDb);
        }
        
        public void Flush()
        {
            _dbData.MemoryCache.Flush();
        }

        public IConnectionMultiplexer Multiplexer => _redisDb?.Multiplexer;

        public int Database => _redisDb?.Database ?? 0;

        public IBatch CreateBatch(object asyncState = null)
        {
            if (_redisDb == null) throw new NotImplementedException();

            return _redisDb.CreateBatch(asyncState);
        }

        public ITransaction CreateTransaction(object asyncState = null)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.CreateTransaction(asyncState);
        }

        public RedisValue DebugObject(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.DebugObject(key, flags);
        }

        public bool GeoAdd(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoAdd(key, longitude, latitude, member, flags);
        }

        public bool GeoAdd(RedisKey key, GeoEntry value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoAdd(key, value, flags);
        }

        public long GeoAdd(RedisKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoAdd(key, values, flags);
        }

        public bool GeoRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoRemove(key, member, flags);
        }

        public double? GeoDistance(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoDistance(key, member1, member2, unit, flags);
        }

        public string[] GeoHash(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoHash(key, members, flags);
        }

        public string GeoHash(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoHash(key, member, flags);
        }

        public GeoPosition?[] GeoPosition(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoPosition(key, members, flags);
        }

        public GeoPosition? GeoPosition(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoPosition(key, member, flags);
        }

        public GeoRadiusResult[] GeoRadius(RedisKey key, RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoRadius(key, member, radius, unit, count, order, options, flags);
        }

        public GeoRadiusResult[] GeoRadius(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoRadius(key, longitude, latitude, radius, unit, count, order, options, flags);
        }

        public Task<RedisValue> DebugObjectAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.DebugObjectAsync(key, flags);
        }

        public Task<bool> GeoAddAsync(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoAddAsync(key, longitude, latitude, member, flags);
        }

        public Task<bool> GeoAddAsync(RedisKey key, GeoEntry value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoAddAsync(key, value, flags);
        }

        public Task<long> GeoAddAsync(RedisKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoAddAsync(key, values, flags);
        }

        public Task<bool> GeoRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoRemoveAsync(key, member, flags);
        }

        public Task<double?> GeoDistanceAsync(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoDistanceAsync(key, member1, member2, unit, flags);
        }

        public Task<string[]> GeoHashAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoHashAsync(key, members, flags);
        }

        public Task<string> GeoHashAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoHashAsync(key, member, flags);
        }

        public Task<GeoPosition?[]> GeoPositionAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoPositionAsync(key, members, flags);
        }

        public Task<GeoPosition?> GeoPositionAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoPositionAsync(key, member, flags);
        }

        public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoRadiusAsync(key, member, radius, unit, count, order, options, flags);
        }

        public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.GeoRadiusAsync(key, longitude, latitude, radius, unit, count, order, options, flags);
        }

        public double HashDecrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _dbData.MemoryHashes.Delete(key, new[] { hashField });
            return _redisDb.HashDecrement(key, hashField, value, flags);
        }

        public long HashDecrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _dbData.MemoryHashes.Delete(key, new[] { hashField });
            return _redisDb.HashDecrement(key, hashField, value, flags);
        }

        public Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _dbData.MemoryHashes.Delete(key, new[] { hashField });
            return _redisDb.HashDecrementAsync(key, hashField, value, flags);
        }

        public Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _dbData.MemoryHashes.Delete(key, new[] { hashField });
            return _redisDb.HashDecrementAsync(key, hashField, value, flags);
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public long HashDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            var deleted = _dbData.MemoryHashes.Delete(key, hashFields);
            return _redisDb?.HashDelete(key, hashFields, flags) ?? deleted;
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public bool HashDelete(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            var deleted = _dbData.MemoryHashes.Delete(key, new[] { hashField });
            return _redisDb?.HashDelete(key, hashField, flags) ?? deleted > 0;
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public async Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            var deleted = _dbData.MemoryHashes.Delete(key, hashFields);
            return _redisDb != null ? await _redisDb.HashDeleteAsync(key, hashFields, flags).ConfigureAwait(false) : deleted;
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public async Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            var deleted = _dbData.MemoryHashes.Delete(key, new[] { hashField });
            return _redisDb != null ? await _redisDb.HashDeleteAsync(key, hashField, flags).ConfigureAwait(false) : (deleted > 0);
        }
        
        public bool HashExists(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            if (_dbData.MemoryHashes.Contains(key, hashField)) return true;
            return _redisDb != null && _redisDb.HashExists(key, hashField, flags);
        }

        public Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            if (_dbData.MemoryHashes.Contains(key, hashField))
            {
                return Task.FromResult(true);
            }
            return _redisDb == null ? Task.FromResult(false) : _redisDb.HashExistsAsync(key, hashField, flags);
        }

        public Lease<byte> HashGetLease(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashGetLease(key, hashField, flags);
        }

        public RedisValue[] HashGet(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryHashes.GetMulti(key, hashFields,
                    missingKeys => Task.FromResult(_redisDb != null ? _redisDb.HashGet(key, missingKeys, flags) : new RedisValue[0]))
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public RedisValue HashGet(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryHashes.GetMulti(key, new[] { hashField },
                    missingKeys => Task.FromResult(_redisDb != null ? _redisDb.HashGet(key, missingKeys, flags) : new RedisValue[0]))
                .ConfigureAwait(false).GetAwaiter().GetResult().FirstOrDefault();
        }

        public HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _dbData.MemoryHashes.GetAll(key, () => Task.FromResult(_redisDb.HashGetAll(key, flags))).Result;
        }

        public Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _dbData.MemoryHashes.GetAll(key, () => _redisDb.HashGetAllAsync(key, flags));
        }

        public Task<Lease<byte>> HashGetLeaseAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashGetLeaseAsync(key, hashField, flags);
        }

        public Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryHashes.GetMulti(key, hashFields,                 missingKeys => Task.FromResult(_redisDb != null ? _redisDb.HashGet(key, missingKeys, flags) : new RedisValue[0]));
        }

        public Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryHashes.Get(key, hashField, async (hashEntryKey) =>
            {
                if (_redisDb != null)
                {
                    return await _redisDb.HashGetAsync(key, hashField, flags).ConfigureAwait(false);
                }
                return new RedisValue();
            });
        }

        public double HashIncrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _dbData.MemoryHashes.Delete(key, new[] { hashField });
            return _redisDb.HashIncrement(key, hashField, value, flags);
        }

        public long HashIncrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _dbData.MemoryHashes.Delete(key, new[] { hashField });
            return _redisDb.HashIncrement(key, hashField, value, flags);
        }

        public Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _dbData.MemoryHashes.Delete(key, new[] { hashField });
            return _redisDb.HashIncrementAsync(key, hashField, value, flags);
        }

        public Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _dbData.MemoryHashes.Delete(key, new[] { hashField });
            return _redisDb.HashIncrementAsync(key, hashField, value, flags);
        }

        public RedisValue[] HashKeys(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //can't do much in memory
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashKeys(key, flags);
        }

        public Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //can't do much in memory
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashKeysAsync(key, flags);
        }

        public long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //can't do much in memory
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashLength(key, flags);
        }

        public Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //can't do much in memory
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashLengthAsync(key, flags);
        }

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            if (_redisDb == null) throw new NotImplementedException();
            var result = _redisDb.HashScan(key, pattern, pageSize, flags);

            foreach(var hashEntry in result)
            {
                //Store in the hash
                _dbData.MemoryHashes.Set(key, new[] { hashEntry });

                yield return hashEntry;
            }
        }

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            var result = _redisDb.HashScan(key, pattern, pageSize, cursor, pageOffset, flags);

            foreach (var hashEntry in result)
            {
                //Store in the hash
                _dbData.MemoryHashes.Set(key, new[] { hashEntry });

                yield return hashEntry;
            }
        }

        /// <summary>
        /// Sets the values in memory and in redis.
        /// </summary>
        public void HashSet(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            //Set the values in memory
            _dbData.MemoryHashes.Set(key, hashFields);
            _redisDb?.HashSet(key, hashFields, flags);
        }

        public bool HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemoryHashes.Set(key, new[] { new HashEntry(hashField, value) }, when);

            return _redisDb?.HashSet(key, hashField, value, when, flags) ?? result > 0;
        }

        public async Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryHashes.Set(key, hashFields);

            if (_redisDb != null)
            {
                await _redisDb.HashSetAsync(key, hashFields, flags).ConfigureAwait(false);
            }
        }

        public async Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemoryHashes.Set(key, new[] { new HashEntry(hashField, value) }, when);
            return _redisDb != null ? await _redisDb.HashSetAsync(key, hashField, value, when, flags).ConfigureAwait(false) : (result > 0);
        }

        public RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            //Can't do much in memory
            return _redisDb.HashValues(key, flags);
        }

        public Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            //Can't do much in memory
            return _redisDb.HashValuesAsync(key, flags);
        }

        public bool HyperLogLogAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HyperLogLogAdd(key, values, flags);
        }

        public bool HyperLogLogAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HyperLogLogAdd(key, value, flags);
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HyperLogLogAddAsync(key, values, flags);
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HyperLogLogAddAsync(key, value, flags);
        }

        public long HyperLogLogLength(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HyperLogLogLength(keys, flags);
        }

        public long HyperLogLogLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HyperLogLogLength(key, flags);
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HyperLogLogLengthAsync(keys, flags);
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HyperLogLogLengthAsync(key, flags);
        }

        public void HyperLogLogMerge(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _redisDb.HyperLogLogMerge(destination, sourceKeys, flags);
        }

        public void HyperLogLogMerge(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _redisDb.HyperLogLogMerge(destination, first, second, flags);
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HyperLogLogMergeAsync(destination, sourceKeys, flags);
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HyperLogLogMergeAsync(destination, first, second, flags);
        }

        public EndPoint IdentifyEndpoint(RedisKey key = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.IdentifyEndpoint(key, flags);
        }

        public Task<EndPoint> IdentifyEndpointAsync(RedisKey key = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.IdentifyEndpointAsync(key, flags);
        }

        public bool IsConnected(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.IsConnected(key, flags);
        }

        public long KeyDelete(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            var removed = _dbData.MemoryCache.Remove(keys.Select(k => (string)k).ToArray());
            return _redisDb?.KeyDelete(keys, flags) ?? removed;
        }

        public bool KeyDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            var removed = _dbData.MemoryCache.Remove(new[] { (string)key });
            return _redisDb?.KeyDelete(key, flags) ?? removed > 0;
        }

        public Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            var removed = _dbData.MemoryCache.Remove(keys.Select(k => (string)k).ToArray());
            return _redisDb == null ? Task.FromResult(removed) : _redisDb.KeyDeleteAsync(keys, flags);
        }

        public Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemoryCache.Remove(new[] { (string)key });
            return _redisDb == null ? Task.FromResult(result > 0) : _redisDb.KeyDeleteAsync(key, flags);
        }

        public byte[] KeyDump(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyDump(key, flags);
        }

        public Task<byte[]> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyDumpAsync(key, flags);
        }

        public bool KeyExists(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_dbData.MemoryCache.ContainsKey(key)) return true;

            //We need to check redis since we don't know what we *don't* have
            return _redisDb != null && _redisDb.KeyExists(key, flags);
        }

        public long KeyExists(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyExists(keys, flags);
        }

        public async Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_dbData.MemoryCache.ContainsKey(key)) return true;

            //There's no redis so we know assume the key doesn't exist in memory
            if (_redisDb == null) return false;

            //We need to check redis since we don't know what we *don't* have
            return await _redisDb.KeyExistsAsync(key, flags);
        }

        public Task<long> KeyExistsAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyExistsAsync(keys, flags);
        }

        public bool KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemoryCache.Expire(key, expiry);
            return _redisDb?.KeyExpire(key, expiry, flags) ?? result;
        }

        public TimeSpan? KeyIdleTime(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyIdleTime(key, flags);
        }

        public bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemoryCache.Expire(key, expiry);
            return _redisDb?.KeyExpire(key, expiry, flags) ?? result;
        }

        public Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemoryCache.Expire(key, expiry);
            return _redisDb != null ? _redisDb.KeyExpireAsync(key, expiry, flags) : Task.FromResult(result);
        }

        public Task<TimeSpan?> KeyIdleTimeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyIdleTimeAsync(key, flags);
        }

        public Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemoryCache.Expire(key, expiry);
            return _redisDb != null ? _redisDb.KeyExpireAsync(key, expiry, flags) : Task.FromResult(result);
        }

        public void KeyMigrate(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            _redisDb?.KeyMigrate(key, toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
        }

        public Task KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            return _redisDb == null ? Task.CompletedTask : _redisDb.KeyMigrateAsync(key, toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
        }

        public bool KeyMove(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            return _redisDb == null || _redisDb.KeyMove(key, database, flags);
        }

        public Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            return _redisDb == null ? Task.FromResult(true) : _redisDb.KeyMoveAsync(key, database, flags);
        }

        public bool KeyPersist(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyPersist(key, flags);
        }

        public Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyPersistAsync(key, flags);
        }

        public RedisKey KeyRandom(CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyRandom(flags);
        }

        public Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyRandomAsync(flags);
        }

        public bool KeyRename(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemoryCache.RenameKey(key, newKey);
            return _redisDb?.KeyRename(key, newKey, when, flags) ?? result;
        }

        public async Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemoryCache.RenameKey(key, newKey);
            if (_redisDb == null) return result;
            return await _redisDb.KeyRenameAsync(key, newKey, when, flags).ConfigureAwait(false);
        }
        
        public void KeyRestore(RedisKey key, byte[] value, TimeSpan? expiry = default(TimeSpan?), CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _redisDb.KeyRestore(key, value, expiry, flags);
        }

        public Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = default(TimeSpan?), CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyRestoreAsync(key, value, expiry, flags);
        }

        public TimeSpan? KeyTimeToLive(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            var ttlStored = _dbData.MemoryCache.GetExpiry(key);
            if (ttlStored.HasValue && ttlStored.Value.HasValue)
            {
                return ttlStored.Value;
            }

            var ttl = _redisDb.KeyTimeToLive(key, flags);

            //Update the in-memory expiry
            _dbData.MemoryCache.Expire(key, ttl);

            return ttl;
        }

        public async Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            var ttlStored = _dbData.MemoryCache.GetExpiry(key);
            if (ttlStored.HasValue)
            {
                return ttlStored.Value;
            }

            var ttl = await _redisDb.KeyTimeToLiveAsync(key, flags);

            //Update the in-memory expiry
            _dbData.MemoryCache.Expire(key, ttl);

            return ttl;
        }

        public RedisType KeyType(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyType(key, flags);
        }

        public Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.KeyTypeAsync(key, flags);
        }

        public RedisValue ListGetByIndex(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListGetByIndex(key, index, flags);
        }

        public Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListGetByIndexAsync(key, index, flags);
        }

        public long ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListInsertAfter(key, pivot, value, flags);
        }

        public Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListInsertAfterAsync(key, pivot, value, flags);
        }

        public long ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListInsertBefore(key, pivot, value, flags);
        }

        public Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListInsertBeforeAsync(key, pivot, value, flags);
        }

        public RedisValue ListLeftPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListLeftPop(key, flags);
        }

        public Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListLeftPopAsync(key, flags);
        }

        public long ListLeftPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListLeftPush(key, values, flags);
        }

        public long ListLeftPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListLeftPush(key, value, when, flags);
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListLeftPushAsync(key, values, flags);
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListLeftPushAsync(key, value, when, flags);
        }

        public long ListLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListLength(key, flags);
        }

        public Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListLengthAsync(key, flags);
        }

        public RedisValue[] ListRange(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRange(key, start, stop, flags);
        }

        public Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRangeAsync(key, start, stop, flags);
        }

        public long ListRemove(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRemove(key, value, count, flags);
        }

        public Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRemoveAsync(key, value, count, flags);
        }

        public RedisValue ListRightPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRightPop(key, flags);
        }

        public Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRightPopAsync(key, flags);
        }

        public RedisValue ListRightPopLeftPush(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRightPopLeftPush(source, destination, flags);
        }

        public Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRightPopLeftPushAsync(source, destination, flags);
        }

        public long ListRightPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRightPush(key, values, flags);
        }

        public long ListRightPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRightPush(key, value, when, flags);
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRightPushAsync(key, values, flags);
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListRightPushAsync(key, value, when, flags);
        }

        public void ListSetByIndex(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _redisDb.ListSetByIndex(key, index, value, flags);
        }

        public Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListSetByIndexAsync(key, index, value, flags);
        }

        public void ListTrim(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            _redisDb.ListTrim(key, start, stop, flags);
        }

        public Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ListTrimAsync(key, start, stop, flags);
        }

        public bool LockExtend(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.LockExtend(key, value, expiry, flags);
        }

        public Task<bool> LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.LockExtendAsync(key, value, expiry, flags);
        }

        public RedisValue LockQuery(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.LockQuery(key, flags);
        }

        public Task<RedisValue> LockQueryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.LockQueryAsync(key, flags);
        }

        public bool LockRelease(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.LockRelease(key, value, flags);
        }

        public Task<bool> LockReleaseAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.LockReleaseAsync(key, value, flags);
        }

        public bool LockTake(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.LockTake(key, value, expiry, flags);
        }

        public Task<bool> LockTakeAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.LockTakeAsync(key, value, expiry, flags);
        }

        public TimeSpan Ping(CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.Ping(flags);
        }

        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.PingAsync(flags);
        }

        public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.Publish(channel, message, flags);
        }

        public RedisResult Execute(string command, params object[] args)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.Execute(command, args);
        }

        public RedisResult Execute(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.Execute(command, args, flags);
        }

        public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.PublishAsync(channel, message, flags);
        }

        public Task<RedisResult> ExecuteAsync(string command, params object[] args)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ExecuteAsync(command, args);
        }

        public Task<RedisResult> ExecuteAsync(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ExecuteAsync(command, args, flags);
        }

        public RedisResult ScriptEvaluate(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ScriptEvaluate(script, parameters, flags);
        }

        public RedisResult ScriptEvaluate(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ScriptEvaluate(script, parameters, flags);
        }

        public RedisResult ScriptEvaluate(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ScriptEvaluate(hash, keys, values, flags);
        }

        public RedisResult ScriptEvaluate(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ScriptEvaluate(script, keys, values, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ScriptEvaluateAsync(script, parameters, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ScriptEvaluateAsync(script, parameters, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ScriptEvaluateAsync(hash, keys, values, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.ScriptEvaluateAsync(script, keys, values, flags);
        }

        public long SetAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemorySets.Add(key, values);
            return _redisDb?.SetAdd(key, values, flags) ?? result;
        }

        public bool SetAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemorySets.Add(key, new[] { value }) > 0;
            return _redisDb?.SetAdd(key, value, flags) ?? result;
        }

        public Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemorySets.Add(key, values);
            return _redisDb != null ? _redisDb.SetAddAsync(key, values, flags) : Task.FromResult(result);
        }

        public Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemorySets.Add(key, new[] { value }) > 0;
            return _redisDb != null ? _redisDb.SetAddAsync(key, value, flags) : Task.FromResult(result);
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            //No in-memory capability
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetCombine(operation, keys, flags);
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            //No in-memory capability
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetCombine(operation, first, second, flags);
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            //No in-memory capability
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetCombineAndStore(operation, destination, keys, flags);
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            //No in-memory capability
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetCombineAndStore(operation, destination, first, second, flags);
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            //No in-memory capability
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetCombineAndStoreAsync(operation, destination, keys, flags);
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            //No in-memory capability
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetCombineAndStoreAsync(operation, destination, first, second, flags);
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            //No in-memory capability
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetCombineAsync(operation, keys, flags);
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            //No in-memory capability
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetCombineAsync(operation, first, second, flags);
        }

        public bool SetContains(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_dbData.MemorySets.Contains(key, value)) return true;
            return _redisDb != null && _redisDb.SetContains(key, value, flags);
        }

        public Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_dbData.MemorySets.Contains(key, value)) return Task.FromResult(true);
            return _redisDb != null ? _redisDb.SetContainsAsync(key, value, flags) : Task.FromResult(false);
        }

        public long SetLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //No in-memory capability
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetLength(key, flags);
        }

        public Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //No in-memory capability
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetLengthAsync(key, flags);
        }

        public RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            var result = _redisDb.SetMembers(key, flags);

            //Store the values in memory
            _dbData.MemorySets.Add(key, result);

            return result;
        }

        public async Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            var result = await _redisDb.SetMembersAsync(key, flags).ConfigureAwait(false);

            //Store the values in memory
            _dbData.MemorySets.Add(key, result);

            return result;
        }
        
        public bool SetMove(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            var inMemResult = _dbData.MemorySets.Move(source, destination, value);
            return _redisDb?.SetMove(source, destination, value, flags) ?? inMemResult;
        }

        public Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            var inMemResult = _dbData.MemorySets.Move(source, destination, value);
            return _redisDb != null ? _redisDb.SetMoveAsync(source, destination, value, flags) : Task.FromResult(inMemResult);
        }

        public RedisValue SetPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            
            var value = _redisDb.SetPop(key, flags);

            //Remove it from memory
            _dbData.MemorySets.Remove(key, new[] { value });

            return value;
        }

        public RedisValue[] SetPop(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetPop(key, count, flags);
        }

        public async Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            var value = await _redisDb.SetPopAsync(key, flags);

            //Remove it from memory
            _dbData.MemorySets.Remove(key, new[] { value });

            return value;
        }

        public Task<RedisValue[]> SetPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SetPopAsync(key, count, flags);
        }

        public RedisValue SetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            var value = _redisDb.SetRandomMember(key, flags);

            _dbData.MemorySets.Add(key, new[] { value }); //Cache it

            return value;
        }

        public async Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            var value = await _redisDb.SetRandomMemberAsync(key, flags);

            _dbData.MemorySets.Add(key, new[] { value }); //Cache it

            return value;
        }

        public RedisValue[] SetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            var values = _redisDb.SetRandomMembers(key, count, flags);

            _dbData.MemorySets.Add(key, values); //Cache it

            return values;
        }

        public async Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            var values = await _redisDb.SetRandomMembersAsync(key, count, flags);

            _dbData.MemorySets.Add(key, values); //Cache it

            return values;
        }

        public long SetRemove(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            var inMemResult = _dbData.MemorySets.Remove(key, values);
            return _redisDb?.SetRemove(key, values, flags) ?? inMemResult;
        }

        public bool SetRemove(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            var inMemResult = _dbData.MemorySets.Remove(key, new[] { value });
            return _redisDb?.SetRemove(key, value, flags) ?? inMemResult > 0;
        }

        public Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            var inMemResult = _dbData.MemorySets.Remove(key, values);
            return _redisDb != null ? _redisDb.SetRemoveAsync(key, values, flags) : Task.FromResult(inMemResult);
        }

        public Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            var inMemResult = _dbData.MemorySets.Remove(key, new[] { value });
            return _redisDb != null ? _redisDb.SetRemoveAsync(key, value, flags) : Task.FromResult(inMemResult > 0);
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            if (_redisDb == null) throw new NotImplementedException();

            foreach(var value in _redisDb.SetScan(key, pattern, pageSize, flags))
            {
                //Save off the value
                _dbData.MemorySets.Add(key, new[] { value });
                yield return value;
            }
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            foreach(var value in _redisDb.SetScan(key, pattern, pageSize, cursor, pageOffset, flags))
            {
                _dbData.MemorySets.Add(key, new[] { value });
                yield return value;
            }
        }

        public RedisValue[] Sort(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.Sort(key, skip, take, order, sortType, by, get, flags);
        }

        public long SortAndStore(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortAndStore(destination, key, skip, take, order, sortType, by, get, flags);
        }

        public Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortAndStoreAsync(destination, key, skip, take, order, sortType, by, get, flags);
        }

        public Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortAsync(key, skip, take, order, sortType, by, get, flags);
        }

        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetAdd(key, member, score, when, flags);
        }

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.AddDiscontinuous(key, values);

            return _redisDb.SortedSetAdd(key, values, flags);
        }

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetAdd(key, values, when, flags);
        }

        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.AddDiscontinuous(key, new[] { new SortedSetEntry(member, score) });

            return _redisDb.SortedSetAdd(key, member, score, flags);
        }

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetAddAsync(key, member, score, when, flags);
        }

        public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.AddDiscontinuous(key, values);

            return _redisDb.SortedSetAddAsync(key, values, flags);
        }

        public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetAddAsync(key, values, when, flags);
        }

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.AddDiscontinuous(key, new[] { new SortedSetEntry(member, score) });

            return _redisDb.SortedSetAddAsync(key, member, score, flags);
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetCombineAndStore(operation, destination, keys, weights, aggregate, flags);
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetCombineAndStore(operation, destination, first, second, aggregate, flags);
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate, flags);
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate, flags);
        }

        public double SortedSetDecrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.DecrementOrAdd(key, member, value);

            return _redisDb.SortedSetDecrement(key, member, value, flags);
        }

        public Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.DecrementOrAdd(key, member, value);

            return _redisDb.SortedSetDecrementAsync(key, member, value, flags);
        }

        public double SortedSetIncrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.IncrementOrAdd(key, member, value);

            return _redisDb.SortedSetIncrement(key, member, value, flags);
        }

        public Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.IncrementOrAdd(key, member, value);

            return _redisDb.SortedSetIncrementAsync(key, member, value, flags);
        }

        public long SortedSetLength(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetLength(key, min, max, exclude, flags);
        }

        public Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetLengthAsync(key, min, max, exclude);
        }

        public long SortedSetLengthByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetLengthByValue(key, min, max, exclude, flags);
        }

        public Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetLengthByValueAsync(key, min, max, exclude, flags);
        }

        public RedisValue[] SortedSetRangeByRank(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetRangeByRank(key, start, stop, order, flags);
        }

        /// <summary>
        /// Does not cache values since caching is done by score.
        /// </summary>
        public Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetRangeByRankAsync(key, start, stop, order, flags);
        }

        /// <summary>
        /// Caches returned values by their score only.
        /// </summary>
        public SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            var result = _redisDb.SortedSetRangeByRankWithScores(key, start, stop, order, flags);

            _dbData.MemorySortedSets.AddDiscontinuous(key, result);

            return result;
        }

        public async Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            var result = await _redisDb.SortedSetRangeByRankWithScoresAsync(key, start, stop, order, flags).ConfigureAwait(false);

            _dbData.MemorySortedSets.AddDiscontinuous(key, result);

            return result;
        }
        
        //Does not cache the result since scores are not returned
        public RedisValue[] SortedSetRangeByScore(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (skip <= int.MaxValue && take <= int.MaxValue)
            {
                var result = _dbData.MemorySortedSets.GetByScore(key, start, stop, exclude, order, (int)skip, (int)take);
                if (result != null) return result.Select(e => e.Element).ToArray();
            }

            if (_redisDb == null) throw new NotImplementedException();

            var values = _redisDb.SortedSetRangeByScore(key, start, stop, exclude, order, skip, take, flags);

            //We know these values are continuous
            _dbData.MemorySortedSets.MarkValuesAsContinuous(key, values);

            return values;
        }

        //Does not cache the result since scores are not returned
        public async Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            //Can't use skip and take if they're larger than int.maxvalue
            if (skip <= int.MaxValue && take <= int.MaxValue)
            {
                var result = _dbData.MemorySortedSets.GetByScore(key, start, stop, exclude, order, (int)skip, (int)take);
                if (result != null) return result.Select(e => e.Element).ToArray();
            }

            if (_redisDb == null) throw new NotImplementedException();

            var values = await _redisDb.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take, flags).ConfigureAwait(false);

            _dbData.MemorySortedSets.MarkValuesAsContinuous(key, values);

            return values;
        }

        //Caches results
        public SortedSetEntry[] SortedSetRangeByScoreWithScores(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            IEnumerable<SortedSetEntry> result = null;
            if (skip <= int.MaxValue && take <= int.MaxValue)
            {
                result = _dbData.MemorySortedSets.GetByScore(key, start, stop, exclude, order, (int)skip, (int)take);
            }

            if (result != null)
            {
                return result.ToArray();
            }

            var resultArr = _redisDb.SortedSetRangeByScoreWithScores(key, start, stop, exclude, order, skip, take, flags);

            //It's too hard to cache when skip or take are specified. There could be other items with the same score not returned.
            if (skip == 0 && take == -1)
            {
                _dbData.MemorySortedSets.AddContinuous(key, resultArr, start, stop);
            }

            return resultArr;
        }

        public async Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            IEnumerable<SortedSetEntry> result = null;
            if (skip <= int.MaxValue && take <= int.MaxValue)
            {
                result = _dbData.MemorySortedSets.GetByScore(key, start, stop, exclude, order, (int)skip, (int)take);
            }

            if (result != null)
            {
                return order == Order.Descending ? result.Reverse().ToArray() : result.ToArray();
            }

            var resultArr = await _redisDb.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take, flags).ConfigureAwait(false);

            //It's too hard to cache when skip or take are specified. There could be other items with the same score not returned.
            if (skip == 0 && take == -1)
            {
                _dbData.MemorySortedSets.AddContinuous(key, resultArr, start, stop);
            }

            return resultArr;
        }

        public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min = default(RedisValue), RedisValue max = default(RedisValue), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetRangeByValue(key, min, max, exclude, skip, take, flags);
        }

        public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min = new RedisValue(), RedisValue max = new RedisValue(), Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetRangeByValue(key, min, max, exclude, order, skip, take, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = default(RedisValue), RedisValue max = default(RedisValue), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetRangeByValueAsync(key, min, max, exclude, skip, take, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = new RedisValue(), RedisValue max = new RedisValue(), Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetRangeByValueAsync(key, min, max, exclude, order, skip, take, flags);
        }

        public long? SortedSetRank(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetRank(key, member, order, flags);
        }

        public Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetRankAsync(key, member, order, flags);
        }

        public long SortedSetRemove(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.Delete(key, members);

            return _redisDb.SortedSetRemove(key, members, flags);
        }

        public bool SortedSetRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.Delete(key, new[] { member });

            return _redisDb.SortedSetRemove(key, member, flags);
        }

        public Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.Delete(key, members);

            return _redisDb.SortedSetRemoveAsync(key, members, flags);
        }

        public Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.Delete(key, new[] { member });

            return _redisDb.SortedSetRemoveAsync(key, member, flags);
        }

        public long SortedSetRemoveRangeByRank(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemoryCache.Remove(new[] { (string)key }); //Invalidate the whole in-memory set because we can't update it properly.

            return _redisDb.SortedSetRemoveRangeByRank(key, start, stop, flags);
        }

        public Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemoryCache.Remove(new[] { (string)key }); //Invalidate the whole in-memory set because we can't update it properly

            return _redisDb.SortedSetRemoveRangeByRankAsync(key, start, stop, flags);
        }

        public long SortedSetRemoveRangeByScore(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.DeleteByScore(key, start, stop, exclude);

            return _redisDb.SortedSetRemoveRangeByScore(key, start, stop, exclude, flags);
        }

        public Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemorySortedSets.DeleteByScore(key, start, stop, exclude);

            return _redisDb.SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude, flags);
        }

        public long SortedSetRemoveRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemoryCache.Remove(new string[] { key });

            return _redisDb.SortedSetRemoveRangeByValue(key, min, max, exclude, flags);
        }

        public Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            _dbData.MemoryCache.Remove(new string[] { key });

            return _redisDb.SortedSetRemoveRangeByValueAsync(key, min, max, exclude, flags);
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            if (_redisDb == null) throw new NotImplementedException();

            foreach (var resultEntry in _redisDb.SortedSetScan(key, pattern, pageSize, flags))
            {
                _dbData.MemorySortedSets.AddDiscontinuous(key, new[] { resultEntry });

                yield return resultEntry;
            }
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            foreach (var resultEntry in _redisDb.SortedSetScan(key, pattern, pageSize, cursor, pageOffset, flags))
            {
                _dbData.MemorySortedSets.AddDiscontinuous(key, new[] { resultEntry });

                yield return resultEntry;
            }
        }

        public double? SortedSetScore(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            //Try and get it from memory
            var entry = _dbData.MemorySortedSets.GetEntry(key, member);
            if (entry.HasValue)
            {
                return entry.Value.Score;
            }

            //Get it from Redis
            var score = _redisDb.SortedSetScore(key, member, flags);

            if (score.HasValue)
            {
                //Cache it
                _dbData.MemorySortedSets.AddContinuous(key, new[] { new SortedSetEntry(member, score.Value) }, score.Value, score.Value);
            }

            return score;
        }

        public SortedSetEntry? SortedSetPop(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetPop(key, order, flags);
        }

        public SortedSetEntry[] SortedSetPop(RedisKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetPop(key, count, order, flags);
        }

        public long StreamAcknowledge(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamAcknowledge(key, groupName, messageId, flags);
        }

        public long StreamAcknowledge(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamAcknowledge(key, groupName, messageIds, flags);
        }

        public RedisValue StreamAdd(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamAdd(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, flags);
        }

        public RedisValue StreamAdd(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamAdd(key, streamPairs, messageId, maxLength, useApproximateMaxLength, flags);
        }

        public StreamEntry[] StreamClaim(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamClaim(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
        }

        public RedisValue[] StreamClaimIdsOnly(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamClaimIdsOnly(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
        }

        public bool StreamConsumerGroupSetPosition(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamConsumerGroupSetPosition(key, groupName, position, flags);
        }

        public StreamConsumerInfo[] StreamConsumerInfo(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamConsumerInfo(key, groupName, flags);
        }

        public bool StreamCreateConsumerGroup(RedisKey key, RedisValue groupName, RedisValue? position = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamCreateConsumerGroup(key, groupName, position, flags);
        }

        public long StreamDelete(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamDelete(key, messageIds, flags);
        }

        public long StreamDeleteConsumer(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamDeleteConsumer(key, groupName, consumerName, flags);
        }

        public bool StreamDeleteConsumerGroup(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamDeleteConsumerGroup(key, groupName, flags);
        }

        public StreamGroupInfo[] StreamGroupInfo(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamGroupInfo(key, flags);
        }

        public StreamInfo StreamInfo(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamInfo(key, flags);
        }

        public long StreamLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamLength(key, flags);
        }

        public StreamPendingInfo StreamPending(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamPending(key, groupName, flags);
        }

        public StreamPendingMessageInfo[] StreamPendingMessages(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamPendingMessages(key, groupName, count, consumerName, minId, maxId, flags);
        }

        public StreamEntry[] StreamRange(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamRange(key, minId, maxId, count, messageOrder, flags);
        }

        public StreamEntry[] StreamRead(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamRead(key, position, count, flags);
        }

        public RedisStream[] StreamRead(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamRead(streamPositions, countPerStream, flags);
        }

        public StreamEntry[] StreamReadGroup(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamReadGroup(key, groupName, consumerName, position, count, flags);
        }

        public RedisStream[] StreamReadGroup(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamReadGroup(streamPositions, groupName, consumerName, countPerStream, flags);
        }

        public long StreamTrim(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamTrim(key, maxLength, useApproximateMaxLength, flags);
        }

        public async Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();

            var entry = _dbData.MemorySortedSets.GetEntry(key, member);
            if (entry.HasValue)
            {
                return entry.Value.Score;
            }

            var score = await _redisDb.SortedSetScoreAsync(key, member, flags).ConfigureAwait(false);

            if (score.HasValue)
            {
                //Cache it
                _dbData.MemorySortedSets.AddContinuous(key, new[] { new SortedSetEntry(member, score.Value) }, score.Value, score.Value);
            }

            return score;
        }

        public Task<SortedSetEntry?> SortedSetPopAsync(RedisKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetPopAsync(key, order, flags);
        }

        public Task<SortedSetEntry[]> SortedSetPopAsync(RedisKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.SortedSetPopAsync(key, count, order, flags);
        }

        public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamAcknowledgeAsync(key, groupName, messageId, flags);
        }

        public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamAcknowledgeAsync(key, groupName, messageIds, flags);
        }

        public Task<RedisValue> StreamAddAsync(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, flags);
        }

        public Task<RedisValue> StreamAddAsync(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength, flags);
        }

        public Task<StreamEntry[]> StreamClaimAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
        }

        public Task<RedisValue[]> StreamClaimIdsOnlyAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
        }

        public Task<bool> StreamConsumerGroupSetPositionAsync(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamConsumerGroupSetPositionAsync(key, groupName, position, flags);
        }

        public Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamConsumerInfoAsync(key, groupName, flags);
        }

        public Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamCreateConsumerGroupAsync(key, groupName, position, flags);
        }

        public Task<long> StreamDeleteAsync(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamDeleteAsync(key, messageIds, flags);
        }

        public Task<long> StreamDeleteConsumerAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamDeleteConsumerAsync(key, groupName, consumerName, flags);
        }

        public Task<bool> StreamDeleteConsumerGroupAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamDeleteConsumerGroupAsync(key, groupName, flags);
        }

        public Task<StreamGroupInfo[]> StreamGroupInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamGroupInfoAsync(key, flags);
        }

        public Task<StreamInfo> StreamInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamInfoAsync(key, flags);
        }

        public Task<long> StreamLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamLengthAsync(key, flags);
        }

        public Task<StreamPendingInfo> StreamPendingAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamPendingAsync(key, groupName, flags);
        }

        public Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId, flags);
        }

        public Task<StreamEntry[]> StreamRangeAsync(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamRangeAsync(key, minId, maxId, count, messageOrder, flags);
        }

        public Task<StreamEntry[]> StreamReadAsync(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamReadAsync(key, position, count, flags);
        }

        public Task<RedisStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamReadAsync(streamPositions, countPerStream, flags);
        }

        public Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamReadGroupAsync(key, groupName, consumerName, position, count, flags);
        }

        public Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, flags);
        }

        public Task<long> StreamTrimAsync(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StreamTrimAsync(key, maxLength, useApproximateMaxLength, flags);
        }

        /// <summary>
        /// Appends the string both in Redis and in-memory.
        /// </summary>
        public long StringAppend(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemoryStrings.AppendToString(key, value);

            if (_redisDb == null) return result;

            var redisResult = _redisDb.StringAppend(key, value, flags);

            //If the in-mem result is different from the redis result then clear memory
            if (redisResult != result)
            {
                _dbData.MemoryCache.Remove(new[] { (string)key });
            }

            return redisResult;
        }

        /// <summary>
        /// Appends the string both in Redis and in-memory.
        /// </summary>
        public async Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            var result = _dbData.MemoryStrings.AppendToString(key, value);

            if (_redisDb == null) return result;

            var redisResult = await _redisDb.StringAppendAsync(key, value, flags).ConfigureAwait(false);

            //If the in-mem result is different from the redis result then clear memory
            if (redisResult != result)
            {
                _dbData.MemoryCache.Remove(new[] { (string)key });
            }

            return redisResult;
        }

        /// <summary>
        /// Returns the value directly from Redis.
        /// </summary>
        public long StringBitCount(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringBitCount(key, start, end, flags);
        }

        /// <summary>
        /// Returns the value directly from Redis.
        /// </summary>
        public Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringBitCountAsync(key, start, end, flags);
        }

        /// <summary>
        /// Performs and returns the value directly from Redis.
        /// </summary>
        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringBitOperation(operation, destination, keys, flags);
        }

        /// <summary>
        /// Performs and returns the value directly from Redis.
        /// </summary>
        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringBitOperation(operation, destination, first, second, flags);
        }

        /// <summary>
        /// Performs and returns the value directly from Redis.
        /// </summary>
        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringBitOperationAsync(operation, destination, keys, flags);
        }

        /// <summary>
        /// Performs and returns the value directly from Redis.
        /// </summary>
        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringBitOperationAsync(operation, destination, first, second, flags);
        }

        /// <summary>
        /// Returns the value directly from Redis.
        /// </summary>
        public long StringBitPosition(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringBitPosition(key, bit, start, end, flags);
        }

        /// <summary>
        /// Returns the value directly from Redis.
        /// </summary>
        public Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringBitPositionAsync(key, bit, start, end, flags);
        }

        /// <summary>
        /// Decrements a string in Redis, and removes the string from memory if present.
        /// </summary>
        public double StringDecrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException();

            return _redisDb.StringDecrement(key, value, flags);
        }

        /// <summary>
        /// Decrements a string in Redis, and removes the string from memory if present.
        /// </summary>
        public long StringDecrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException();

            return _redisDb.StringDecrement(key, value, flags);
        }

        /// <summary>
        /// Decrements a string in Redis, and removes the string from memory if present.
        /// </summary>
        public Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException();

            return _redisDb.StringDecrementAsync(key, value, flags);
        }
        
        /// <summary>
        /// Decrements a string in Redis, and removes the string from memory if present.
        /// </summary>
        public Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException();

            return _redisDb.StringDecrementAsync(key, value, flags);
        }

        /// <summary>
        /// Gets a string from memory, or from Redis if it isn't present.
        /// </summary>
        public RedisValue[] StringGet(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryStrings.GetFromMemoryMulti(keys, retrieveKeys => Task.FromResult(_redisDb == null ? new RedisValue[0] : _redisDb.StringGet(retrieveKeys, flags)))
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Lease<byte> StringGetLease(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringGetLease(key, flags);
        }

        /// <summary>
        /// Gets a string from memory, or from Redis if it isn't present.
        /// </summary>
        public RedisValue StringGet(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryStrings.GetFromMemory(key, () =>
                {
                    System.Diagnostics.Debug.WriteLine("Getting key from redis: " + (string)key);
                    return Task.FromResult(_redisDb?.StringGet(key, flags) ?? new RedisValue());
                })
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets a string from memory, or from Redis if it isn't present.
        /// </summary>
        public async Task<RedisValue[]> StringGetAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return await _dbData.MemoryStrings.GetFromMemoryMulti(keys, retrieveKeys =>
                    _redisDb == null ? Task.FromResult(new RedisValue[0]) : _redisDb.StringGetAsync(retrieveKeys, flags))
                .ConfigureAwait(false);
        }

        public Task<Lease<byte>> StringGetLeaseAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringGetLeaseAsync(key, flags);
        }

        /// <summary>
        /// Calls redis to get a string bit.
        /// </summary>
        public async Task<RedisValue> StringGetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _dbData.MemoryStrings.GetFromMemory(key, () => _redisDb == null ? Task.FromResult(new RedisValue()) : _redisDb.StringGetAsync(key, flags)).ConfigureAwait(false);
        }

        /// <summary>
        /// Calls redis to get a string bit.
        /// </summary>
        public bool StringGetBit(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringGetBit(key, offset, flags);
        }

        /// <summary>
        /// Calls redis to get a string bit.
        /// </summary>
        public Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringGetBitAsync(key, offset, flags);
        }

        /// <summary>
        /// Calls redis to get a string range.
        /// </summary>
        public RedisValue StringGetRange(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringGetRange(key, start, end, flags);
        }

        /// <summary>
        /// Calls redis to get a string range.
        /// </summary>
        public Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.StringGetRangeAsync(key, start, end, flags);
        }

        /// <summary>
        /// Sets value in redis, and gets it from memory if possible.
        /// </summary>
        public RedisValue StringGetSet(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            var hasSetInRedis = false;

            var result = _dbData.MemoryStrings.GetFromMemory(key, () =>
            {
                if (_redisDb == null) return Task.FromResult(new RedisValue());
                
                hasSetInRedis = true;
                return Task.FromResult(_redisDb.StringGetSet(key, value, flags));
            }).ConfigureAwait(false).GetAwaiter().GetResult();

            //Set it in memory
            _dbData.MemoryCache.Add(key, value, null, When.Always);

            //Set it in redis if necessary
            if(!hasSetInRedis)
            {
                _redisDb?.StringSet(key, value, null, When.Always, flags);
            }

            return result;
        }

        /// <summary>
        /// Sets value in redis, and gets it from memory if possible.
        /// </summary>
        public async Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            var hasSetInRedis = false;

            var result = await _dbData.MemoryStrings.GetFromMemory(key, () =>
            {
                if (_redisDb == null) return Task.FromResult(new RedisValue());

                hasSetInRedis = true;
                return _redisDb.StringGetSetAsync(key, value, flags);
            }).ConfigureAwait(false);

            //Set it in memory
            _dbData.MemoryCache.Add(key, value, null, When.Always);

            //Set it in redis if necessary
            if (!hasSetInRedis && _redisDb != null)
            {
                await _redisDb.StringGetSetAsync(key, value, flags).ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Gets value from Memory, or from Redis if not present.
        /// </summary>
        public RedisValueWithExpiry StringGetWithExpiry(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryStrings.GetFromMemoryWithExpiry(key, () =>
            {
                System.Diagnostics.Debug.WriteLine("Getting key from redis: " + (string)key);

                return Task.FromResult(_redisDb?.StringGetWithExpiry(key, flags) ?? new RedisValueWithExpiry());
            }).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets value from Memory, or from Redis if not present.
        /// </summary>
        public Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryStrings.GetFromMemoryWithExpiry(key, () =>  _redisDb == null ? Task.FromResult(new RedisValueWithExpiry()) : _redisDb.StringGetWithExpiryAsync(key, flags));
        }

        /// <summary>
        /// Forwards the request to Redis and invalidates in-memory value.
        /// </summary>
        public double StringIncrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException("StringIncrement not yet supported as an in-memory operation");

            return _redisDb.StringIncrement(key, value, flags);
        }

        /// <summary>
        /// Forwards the request to Redis and invalidates in-memory value.
        /// </summary>
        public long StringIncrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException("StringIncrement not yet supported as an in-memory operation");

            return _redisDb.StringIncrement(key, value, flags);
        }

        /// <summary>
        /// Forwards the request to Redis and invalidates in-memory value.
        /// </summary>
        public Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException("StringIncrement not yet supported as an in-memory operation");

            return _redisDb.StringIncrementAsync(key, value, flags);
        }

        /// <summary>
        /// Forwards the request to Redis and invalidates in-memory value.
        /// </summary>
        public Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException("StringIncrement not yet supported as an in-memory operation");

            return _redisDb.StringIncrementAsync(key, value, flags);
        }

        /// <summary>
        /// Gets the string length from memory, if the string is stored in memory.
        /// Otherwise, gets it from Redis.
        /// </summary>
        public long StringLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //Try and get the string from memory
            var length = _dbData.MemoryStrings.GetStringLength(key);

            if (length > 0 || _redisDb == null) return length;

            //Todo: we could cache the length
            return _redisDb.StringLength(key, flags);
        }

        /// <summary>
        /// Gets the string length from memory, if the string is stored in memory.
        /// Otherwise, gets it from Redis.
        /// </summary>
        public Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            var length = _dbData.MemoryStrings.GetStringLength(key);

            if (length > 0 || _redisDb == null)
            {
                return Task.FromResult(length);
            }

            //Todo: we could cache the length
            return _redisDb.StringLengthAsync(key, flags);
        }

        /// <summary>
        /// Sets a string in both memory and Redis.
        /// </summary>
        public bool StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            foreach(var kvp in values)
            {
                _dbData.MemoryCache.Add(kvp.Key, kvp.Value, null, when);
            }

            return _redisDb == null || _redisDb.StringSet(values, when, flags);
        }

        /// <summary>
        /// Sets a string in both memory and Redis
        /// </summary>
        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Add(key, value, expiry, when);
            return _redisDb == null || _redisDb.StringSet(key, value, expiry, when, flags);
        }

        /// <summary>
        /// Sets a string in both memory and Redis
        /// </summary>
        public Task<bool> StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            foreach (var kvp in values)
            {
                _dbData.MemoryCache.Add(kvp.Key, kvp.Value, null, when);
            }

            return _redisDb == null ? Task.FromResult(true) : _redisDb.StringSetAsync(values, when, flags);
        }

        /// <summary>
        /// Sets a string in both memory and Redis.
        /// </summary>
        public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Add(key, value, expiry, when);
            return _redisDb == null ? Task.FromResult(true) : _redisDb.StringSetAsync(key, value, expiry, when, flags);
        }

        /// <summary>
        /// Forwards request to redis and invalidates in-memory value.
        /// </summary>
        public bool StringSetBit(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException("StringSetBit not yet supported as an in-memory operation.");

            return _redisDb.StringSetBit(key, offset, bit, flags);
        }

        /// <summary>
        /// Forwards request to redis and invalidates in-memory value.
        /// </summary>
        public Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException("StringSetBit not yet supported as an in-memory operation.");

            return _redisDb.StringSetBitAsync(key, offset, bit, flags);
        }

        /// <summary>
        /// Forwards request to redis and invalidates in-memory value.
        /// </summary>
        public RedisValue StringSetRange(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException();

            return _redisDb.StringSetRange(key, offset, value, flags);
        }

        /// <summary>
        /// Forwards request to redis and invalidates in-memory value.
        /// </summary>
        public Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null) throw new NotImplementedException();

            return _redisDb.StringSetRangeAsync(key, offset, value, flags);
        }

        public bool TryWait(Task task)
        {
            return _redisDb?.TryWait(task) ?? Task.WaitAll(new[] { task }, 1000);
        }

        public void Wait(Task task)
        {
            if (_redisDb == null)
            {
                Task.WaitAll(task);
            }
            else
            {
                _redisDb.Wait(task);
            }
        }

        public T Wait<T>(Task<T> task)
        {
            if (_redisDb != null) return _redisDb.Wait(task);

            Task.WaitAll(task);
            return task.Result;
        }

        public void WaitAll(params Task[] tasks)
        {
            if (_redisDb == null)
            {
                Task.WaitAll(tasks);
            }
            else
            {
                _redisDb.WaitAll(tasks);
            }
        }

        public void Dispose()
        {
            DatabaseRegister.Instance.RemoveInstanceData(_uniqueId);
            _dbData.Dispose();
        }
    }
}
