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

namespace StackRedis.L1
{
    public class RedisL1Database : IDatabase, IDisposable
    {
        private IDatabase _redisDb;
        private DatabaseInstanceData _dbData;

        private string _uniqueId;
        
        /// <summary>
        /// Constructs a memory-caching layer for Redis IDatabase
        /// </summary>
        /// <param name="redisDb">IDatabase instance</param>
        public RedisL1Database(IDatabase redisDb)
            : this(string.Format("{0}:db={1}", redisDb.Multiplexer.Configuration, redisDb.Database), redisDb)
        { }

        /// <summary>
        /// Constructs a memory-caching layer that simulates Redis.
        /// </summary>
        /// <param name="uniqueStr">Unique string to represent this database instance.</param>
        public RedisL1Database(string uniqueId)
            : this(uniqueId, null)
        { }

        private RedisL1Database(string uniqueId, IDatabase redisDb)
        {
            _uniqueId = uniqueId;

            if(redisDb != null)
                _redisDb = new NotificationDatabase(redisDb);

            //Register for subscriptions and get the in-memory data store
            _dbData = DatabaseRegister.Instance.GetDatabaseInstanceData(uniqueId, redisDb);
        }
        
        public void Flush()
        {
            _dbData.MemoryCache.Flush();
        }

        public ConnectionMultiplexer Multiplexer
        {
            get { return _redisDb == null ? null : _redisDb.Multiplexer; }
        }

        public int Database
        {
            get { return _redisDb == null ? 0 : _redisDb.Database; }
        }

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

        public Task<RedisValue> DebugObjectAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.DebugObjectAsync(key, flags);
        }

        public double HashDecrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.HashDecrement(key, hashField, value, flags);
        }

        public long HashDecrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashDecrement(key, hashField, value, flags);
        }

        public Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashDecrementAsync(key, hashField, value, flags);
        }

        public Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashDecrementAsync(key, hashField, value, flags);
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public long HashDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            long deleted = _dbData.MemoryHashes.Delete(key, hashFields);

            if (_redisDb != null)
                return _redisDb.HashDelete(key, hashFields, flags);
            else
                return deleted;
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public bool HashDelete(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            long deleted = _dbData.MemoryHashes.Delete(key, new[] { hashField });

            if (_redisDb != null)
                return _redisDb.HashDelete(key, hashField, flags);
            else
                return deleted > 0;
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public async Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            long deleted = _dbData.MemoryHashes.Delete(key, hashFields);

            if (_redisDb != null)
                return await _redisDb.HashDeleteAsync(key, hashFields, flags);
            else
                return deleted;
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public async Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            long deleted = _dbData.MemoryHashes.Delete(key, new[] { hashField });

            if (_redisDb != null)
                return await _redisDb.HashDeleteAsync(key, hashField, flags);
            else
                return deleted > 0;
        }
        
        public bool HashExists(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            if (_dbData.MemoryHashes.Contains(key, hashField))
                return true;

            if (_redisDb == null)
                return false;
            else
                return _redisDb.HashExists(key, hashField, flags);
        }

        public Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            if (_dbData.MemoryHashes.Contains(key, hashField))
                return Task.FromResult(true);

            if (_redisDb == null)
                return Task.FromResult(false);
            else
                return _redisDb.HashExistsAsync(key, hashField, flags);
        }

        public RedisValue[] HashGet(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryHashes.GetMulti(key, hashFields, (missingKeys) =>
            {
                if(_redisDb != null)
                {
                    return Task.FromResult(_redisDb.HashGet(key, missingKeys, flags));
                }
                else
                {
                    return Task.FromResult(new RedisValue[0]);
                }
            }).Result;
        }

        public RedisValue HashGet(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryHashes.GetMulti(key, new[] { hashField }, (missingKeys) =>
            {
                if (_redisDb != null)
                {
                    return Task.FromResult(_redisDb.HashGet(key, missingKeys, flags));
                }
                else
                {
                    return Task.FromResult(new RedisValue[0]);
                }
            }).Result.FirstOrDefault();
        }

        public HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _dbData.MemoryHashes.GetAll(key, () => Task.FromResult(_redisDb.HashGetAll(key, flags))).Result;
        }

        public Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _dbData.MemoryHashes.GetAll(key, () => _redisDb.HashGetAllAsync(key, flags));
        }

        public Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryHashes.GetMulti(key, hashFields, (missingKeys) =>
            {
                if (_redisDb != null)
                {
                    return Task.FromResult(_redisDb.HashGet(key, missingKeys, flags));
                }
                else
                {
                    return Task.FromResult(new RedisValue[0]);
                }
            });
        }

        public Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryHashes.Get(key, hashField, async (hashEntryKey) =>
            {
                if (_redisDb != null)
                {
                    return await _redisDb.HashGetAsync(key, hashField, flags);
                }
                else
                {
                    return new RedisValue();
                }
            });
        }

        public double HashIncrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashIncrement(key, hashField, value, flags);
        }

        public long HashIncrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashIncrement(key, hashField, value, flags);
        }

        public Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashIncrementAsync(key, hashField, value, flags);
        }

        public Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashIncrementAsync(key, hashField, value, flags);
        }

        public RedisValue[] HashKeys(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashKeys(key, flags);
        }

        public Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashKeysAsync(key, flags);
        }

        public long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashLength(key, flags);
        }

        public Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
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

            if (_redisDb != null)
                _redisDb.HashSet(key, hashFields, flags);
        }

        public bool HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            long result = _dbData.MemoryHashes.Set(key, new[] { new HashEntry(hashField, value) }, when);

            if (_redisDb != null)
                return _redisDb.HashSet(key, hashField, value, when, flags);
            else
                return result > 0;
        }

        public async Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryHashes.Set(key, hashFields);

            if (_redisDb != null)
                await _redisDb.HashSetAsync(key, hashFields, flags);
        }

        public async Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            long result = _dbData.MemoryHashes.Set(key, new[] { new HashEntry(hashField, value) }, when);

            if (_redisDb != null)
                return await _redisDb.HashSetAsync(key, hashField, value, when, flags);
            else
                return result > 0;
        }

        public RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
            return _redisDb.HashValues(key, flags);
        }

        public Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null) throw new NotImplementedException();
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
            long removed = _dbData.MemoryCache.Remove(keys.Select(k => (string)k).ToArray());

            if (_redisDb == null)
            {
                return removed;
            }
            else
            {
                return _redisDb.KeyDelete(keys, flags);
            }
        }

        public bool KeyDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            long removed = _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                return removed > 0;
            else
                return _redisDb.KeyDelete(key, flags);
        }

        public Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            long removed = _dbData.MemoryCache.Remove(keys.Select(k => (string)k).ToArray());

            if (_redisDb == null)
                return Task.FromResult(removed);
            else
                return _redisDb.KeyDeleteAsync(keys, flags);
        }

        public Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            long result = _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                return Task.FromResult(result > 0);
            else
                return _redisDb.KeyDeleteAsync(key, flags);
        }

        public byte[] KeyDump(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.KeyDump(key, flags);
        }

        public Task<byte[]> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.KeyDumpAsync(key, flags);
        }

        public bool KeyExists(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_dbData.MemoryCache.ContainsKey(key))
                return true;

            if (_redisDb == null)
                return false; //There's no redis so we know the key doesn't exist in memory
            
            //We need to check redis since we don't know what we *don't* have
            return _redisDb.KeyExists(key, flags);
        }

        public async Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_dbData.MemoryCache.ContainsKey(key))
                return true;

            if (_redisDb != null)
            {
                //We need to check redis since we don't know what we *don't* have
                return await _redisDb.KeyExistsAsync(key, flags);
            }

            //There's no redis so we know assume the key doesn't exist in memory
            return false;
        }

        public bool KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            bool result = _dbData.MemoryCache.Expire(key, expiry);

            if (_redisDb != null)
            {
                return _redisDb.KeyExpire(key, expiry, flags);
            }
            else
            {
                return result;
            }
        }

        public bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            bool result = _dbData.MemoryCache.Expire(key, expiry);

            if (_redisDb != null)
            {
                return _redisDb.KeyExpire(key, expiry, flags);
            }
            else
            {
                return result;
            }
        }

        public Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            bool result = _dbData.MemoryCache.Expire(key, expiry);
            if (_redisDb != null)
            {
                return _redisDb.KeyExpireAsync(key, expiry, flags);
            }
            else
            {
                return Task.FromResult(result);
            }
        }

        public Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            bool result = _dbData.MemoryCache.Expire(key, expiry);

            if (_redisDb != null)
            {
                return _redisDb.KeyExpireAsync(key, expiry, flags);
            }
            else
            {
                return Task.FromResult(result);
            }
        }

        public void KeyMigrate(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb != null)
            {
                _redisDb.KeyMigrate(key, toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
            }
        }

        public Task KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
            {
                return Task.FromResult(0);
            }
            else
            { 
                return _redisDb.KeyMigrateAsync(key, toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
            }

        }

        public bool KeyMove(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
            {
                return true;
            }
            { 
                return _redisDb.KeyMove(key, database, flags);
            }
        }

        public Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
            {
                return Task.FromResult(true);
            }
            else
            {
                return _redisDb.KeyMoveAsync(key, database, flags);
            }
        }

        public bool KeyPersist(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.KeyPersist(key, flags);
        }

        public Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.KeyPersistAsync(key, flags);
        }

        public RedisKey KeyRandom(CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.KeyRandom(flags);
        }

        public Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.KeyRandomAsync(flags);
        }

        public bool KeyRename(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            bool result = _dbData.MemoryCache.RenameKey(key, newKey);

            if (_redisDb == null)
                return result;

            return _redisDb.KeyRename(key, newKey, when, flags);
        }

        public async Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            bool result = _dbData.MemoryCache.RenameKey(key, newKey);

            if (_redisDb == null)
                return result;

            return await _redisDb.KeyRenameAsync(key, newKey, when, flags);
        }
        
        public void KeyRestore(RedisKey key, byte[] value, TimeSpan? expiry = default(TimeSpan?), CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            _redisDb.KeyRestore(key, value, expiry, flags);
        }

        public Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = default(TimeSpan?), CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.KeyRestoreAsync(key, value, expiry, flags);
        }

        public TimeSpan? KeyTimeToLive(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.KeyTimeToLive(key, flags);
        }

        public Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.KeyTimeToLiveAsync(key, flags);
        }

        public RedisType KeyType(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.KeyType(key, flags);
        }

        public Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.KeyTypeAsync(key, flags);
        }

        public RedisValue ListGetByIndex(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListGetByIndex(key, index, flags);
        }

        public Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListGetByIndexAsync(key, index, flags);
        }

        public long ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListInsertAfter(key, pivot, value, flags);
        }

        public Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListInsertAfterAsync(key, pivot, value, flags);
        }

        public long ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListInsertBefore(key, pivot, value, flags);
        }

        public Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListInsertBeforeAsync(key, pivot, value, flags);
        }

        public RedisValue ListLeftPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListLeftPop(key, flags);
        }

        public Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListLeftPopAsync(key, flags);
        }

        public long ListLeftPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListLeftPush(key, values, flags);
        }

        public long ListLeftPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListLeftPush(key, value, when, flags);
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListLeftPushAsync(key, values, flags);
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListLeftPushAsync(key, value, when, flags);
        }

        public long ListLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListLength(key, flags);
        }

        public Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListLengthAsync(key, flags);
        }

        public RedisValue[] ListRange(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRange(key, start, stop, flags);
        }

        public Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRangeAsync(key, start, stop, flags);
        }

        public long ListRemove(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRemove(key, value, count, flags);
        }

        public Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRemoveAsync(key, value, count, flags);
        }

        public RedisValue ListRightPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRightPop(key, flags);
        }

        public Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRightPopAsync(key, flags);
        }

        public RedisValue ListRightPopLeftPush(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRightPopLeftPush(source, destination, flags);
        }

        public Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRightPopLeftPushAsync(source, destination, flags);
        }

        public long ListRightPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRightPush(key, values, flags);
        }

        public long ListRightPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRightPush(key, value, when, flags);
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRightPushAsync(key, values, flags);
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListRightPushAsync(key, value, when, flags);
        }

        public void ListSetByIndex(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            _redisDb.ListSetByIndex(key, index, value, flags);
        }

        public Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListSetByIndexAsync(key, index, value, flags);
        }

        public void ListTrim(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            _redisDb.ListTrim(key, start, stop, flags);
        }

        public Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ListTrimAsync(key, start, stop, flags);
        }

        public bool LockExtend(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.LockExtend(key, value, expiry, flags);
        }

        public Task<bool> LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.LockExtendAsync(key, value, expiry, flags);
        }

        public RedisValue LockQuery(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.LockQuery(key, flags);
        }

        public Task<RedisValue> LockQueryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.LockQueryAsync(key, flags);
        }

        public bool LockRelease(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.LockRelease(key, value, flags);
        }

        public Task<bool> LockReleaseAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.LockReleaseAsync(key, value, flags);
        }

        public bool LockTake(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.LockTake(key, value, expiry, flags);
        }

        public Task<bool> LockTakeAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.LockTakeAsync(key, value, expiry, flags);
        }

        public TimeSpan Ping(CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.Ping(flags);
        }

        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.PingAsync(flags);
        }

        public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.Publish(channel, message, flags);
        }

        public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.PublishAsync(channel, message, flags);
        }

        public RedisResult ScriptEvaluate(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ScriptEvaluate(script, parameters, flags);
        }

        public RedisResult ScriptEvaluate(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ScriptEvaluate(script, parameters, flags);
        }

        public RedisResult ScriptEvaluate(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ScriptEvaluate(hash, keys, values, flags);
        }

        public RedisResult ScriptEvaluate(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ScriptEvaluate(script, keys, values, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ScriptEvaluateAsync(script, parameters, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ScriptEvaluateAsync(script, parameters, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ScriptEvaluateAsync(hash, keys, values, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.ScriptEvaluateAsync(script, keys, values, flags);
        }

        public long SetAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetAdd(key, values, flags);
        }

        public bool SetAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetAdd(key, value, flags);
        }

        public Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetAddAsync(key, values, flags);
        }

        public Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetAddAsync(key, value, flags);
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetCombine(operation, keys, flags);
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetCombine(operation, first, second, flags);
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetCombineAndStore(operation, destination, keys, flags);
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetCombineAndStore(operation, destination, first, second, flags);
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetCombineAndStoreAsync(operation, destination, keys, flags);
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetCombineAndStoreAsync(operation, destination, first, second, flags);
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetCombineAsync(operation, keys, flags);
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetCombineAsync(operation, first, second, flags);
        }

        public bool SetContains(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetContains(key, value, flags);
        }

        public Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetContainsAsync(key, value, flags);
        }

        public long SetLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetLength(key, flags);
        }

        public Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetLengthAsync(key, flags);
        }

        public RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetMembers(key, flags);
        }

        public Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetMembersAsync(key, flags);
        }

        public bool SetMove(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetMove(source, destination, value, flags);
        }

        public Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetMoveAsync(source, destination, value, flags);
        }

        public RedisValue SetPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetPop(key, flags);
        }

        public Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetPopAsync(key, flags);
        }

        public RedisValue SetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetRandomMember(key, flags);
        }

        public Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetRandomMemberAsync(key, flags);
        }

        public RedisValue[] SetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetRandomMembers(key, count, flags);
        }

        public Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetRandomMembersAsync(key, count, flags);
        }

        public long SetRemove(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetRemove(key, values, flags);
        }

        public bool SetRemove(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetRemove(key, value, flags);
        }

        public Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetRemoveAsync(key, values, flags);
        }

        public Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetRemoveAsync(key, value, flags);
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetScan(key, pattern, pageSize, flags);
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SetScan(key, pattern, pageSize, cursor, pageOffset, flags);
        }

        public RedisValue[] Sort(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.Sort(key, skip, take, order, sortType, by, get, flags);
        }

        public long SortAndStore(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortAndStore(destination, key, skip, take, order, sortType, by, get, flags);
        }

        public Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortAndStoreAsync(destination, key, skip, take, order, sortType, by, get, flags);
        }

        public Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortAsync(key, skip, take, order, sortType, by, get, flags);
        }

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetAdd(key, values, flags);
        }

        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetAdd(key, member, score, flags);
        }

        public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetAddAsync(key, values, flags);
        }

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetAddAsync(key, member, score, flags);
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetCombineAndStore(operation, destination, keys, weights, aggregate, flags);
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetCombineAndStore(operation, destination, first, second, aggregate, flags);
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate, flags);
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate, flags);
        }

        public double SortedSetDecrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetDecrement(key, member, value, flags);
        }

        public Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetDecrementAsync(key, member, value, flags);
        }

        public double SortedSetIncrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetIncrement(key, member, value, flags);
        }

        public Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetIncrementAsync(key, member, value, flags);
        }

        public long SortedSetLength(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetLength(key, min, max, exclude, flags);
        }

        public Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetLengthAsync(key, min, max, exclude);
        }

        public long SortedSetLengthByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetLengthByValue(key, min, max, exclude, flags);
        }

        public Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetLengthByValueAsync(key, min, max, exclude, flags);
        }

        public RedisValue[] SortedSetRangeByRank(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRangeByRank(key, start, stop, order, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRangeByRankAsync(key, start, stop, order, flags);
        }

        public SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRangeByRankWithScores(key, start, stop, order, flags);
        }

        public Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRangeByRankWithScoresAsync(key, start, stop, order, flags);
        }

        public RedisValue[] SortedSetRangeByScore(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRangeByScore(key, start, stop, exclude, order, skip, take, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take, flags);
        }

        public SortedSetEntry[] SortedSetRangeByScoreWithScores(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRangeByScoreWithScores(key, start, stop, exclude, order, skip, take, flags);
        }

        public Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take, flags);
        }

        public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min = default(RedisValue), RedisValue max = default(RedisValue), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRangeByValue(key, min, max, exclude, skip, take, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = default(RedisValue), RedisValue max = default(RedisValue), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRangeByValueAsync(key, min, max, exclude, skip, take, flags);
        }

        public long? SortedSetRank(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRank(key, member, order, flags);
        }

        public Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRankAsync(key, member, order, flags);
        }

        public long SortedSetRemove(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRemove(key, members, flags);
        }

        public bool SortedSetRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRemove(key, member, flags);
        }

        public Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRemoveAsync(key, members, flags);
        }

        public Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRemoveAsync(key, member, flags);
        }

        public long SortedSetRemoveRangeByRank(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRemoveRangeByRank(key, start, stop, flags);
        }

        public Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRemoveRangeByRankAsync(key, start, stop, flags);
        }

        public long SortedSetRemoveRangeByScore(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRemoveRangeByScore(key, start, stop, exclude, flags);
        }

        public Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude, flags);
        }

        public long SortedSetRemoveRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRemoveRangeByValue(key, min, max, exclude, flags);
        }

        public Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetRemoveRangeByValueAsync(key, min, max, exclude, flags);
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetScan(key, pattern, pageSize, flags);
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetScan(key, pattern, pageSize, cursor, pageOffset, flags);
        }

        public double? SortedSetScore(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetScore(key, member, flags);
        }

        public Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.SortedSetScoreAsync(key, member, flags);
        }

        /// <summary>
        /// Appends the string both in Redis and in-memory.
        /// </summary>
        public long StringAppend(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            long result = _dbData.MemoryStrings.AppendToString(key, value);

            if (_redisDb == null)
                return result;
            else
            {
                long redisResult = _redisDb.StringAppend(key, value, flags);

                //If the in-mem result is different from the redis result then clear memory
                if (redisResult != result)
                    _dbData.MemoryCache.Remove(new[] { (string)key });
                
                return redisResult;
            }
        }

        /// <summary>
        /// Appends the string both in Redis and in-memory.
        /// </summary>
        public async Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            long result = _dbData.MemoryStrings.AppendToString(key, value);

            if (_redisDb == null)
                return result;
            else
            {
                long redisResult = await _redisDb.StringAppendAsync(key, value, flags);

                //If the in-mem result is different from the redis result then clear memory
                if (redisResult != result)
                    _dbData.MemoryCache.Remove(new[] { (string)key });

                return redisResult;
            }
        }
        
        /// <summary>
        /// Returns the value directly from Redis.
        /// </summary>
        public long StringBitCount(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringBitCount(key, start, end, flags);
        }

        /// <summary>
        /// Returns the value directly from Redis.
        /// </summary>
        public Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringBitCountAsync(key, start, end, flags);
        }

        /// <summary>
        /// Performs and returns the value directly from Redis.
        /// </summary>
        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringBitOperation(operation, destination, keys, flags);
        }

        /// <summary>
        /// Performs and returns the value directly from Redis.
        /// </summary>
        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringBitOperation(operation, destination, first, second, flags);
        }

        /// <summary>
        /// Performs and returns the value directly from Redis.
        /// </summary>
        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringBitOperationAsync(operation, destination, keys, flags);
        }

        /// <summary>
        /// Performs and returns the value directly from Redis.
        /// </summary>
        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringBitOperationAsync(operation, destination, first, second, flags);
        }

        /// <summary>
        /// Returns the value directly from Redis.
        /// </summary>
        public long StringBitPosition(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringBitPosition(key, bit, start, end, flags);
        }

        /// <summary>
        /// Returns the value directly from Redis.
        /// </summary>
        public Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringBitPositionAsync(key, bit, start, end, flags);
        }

        /// <summary>
        /// Decrements a string in Redis, and removes the string from memory if present.
        /// </summary>
        public double StringDecrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringDecrement(key, value, flags);
        }

        /// <summary>
        /// Decrements a string in Redis, and removes the string from memory if present.
        /// </summary>
        public long StringDecrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringDecrement(key, value, flags);
        }

        /// <summary>
        /// Decrements a string in Redis, and removes the string from memory if present.
        /// </summary>
        public Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringDecrementAsync(key, value, flags);
        }
        
        /// <summary>
        /// Decrements a string in Redis, and removes the string from memory if present.
        /// </summary>
        public Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringDecrementAsync(key, value, flags);
        }

        /// <summary>
        /// Gets a string from memory, or from Redis if it isn't present.
        /// </summary>
        public RedisValue[] StringGet(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryStrings.GetFromMemoryMulti(keys, retrieveKeys =>
            {
                if (_redisDb == null)
                {
                    return Task.FromResult(new RedisValue[0]);
                }
                else
                {
                    return Task.FromResult(_redisDb.StringGet(retrieveKeys, flags));
                }
            }).Result;
        }

        /// <summary>
        /// Gets a string from memory, or from Redis if it isn't present.
        /// </summary>
        public RedisValue StringGet(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryStrings.GetFromMemory(key, () =>
            {
                System.Diagnostics.Debug.WriteLine("Getting key from redis: " + (string)key);

                if (_redisDb == null)
                {
                    return Task.FromResult(new RedisValue());
                }
                else
                {
                    return Task.FromResult(_redisDb.StringGet(key, flags));
                }
            }).Result;
        }

        /// <summary>
        /// Gets a string from memory, or from Redis if it isn't present.
        /// </summary>
        public async Task<RedisValue[]> StringGetAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return await _dbData.MemoryStrings.GetFromMemoryMulti(keys, retrieveKeys =>
            {
                if (_redisDb == null)
                {
                    return Task.FromResult(new RedisValue[0]);
                }
                else
                {
                    return _redisDb.StringGetAsync(retrieveKeys, flags);
                }
            });
        }

        /// <summary>
        /// Calls redis to get a string bit.
        /// </summary>
        public async Task<RedisValue> StringGetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return await _dbData.MemoryStrings.GetFromMemory(key, () =>
            {
                if (_redisDb == null)
                {
                    return Task.FromResult(new RedisValue());
                }
                else
                {
                    return _redisDb.StringGetAsync(key, flags);
                }
            });
        }

        /// <summary>
        /// Calls redis to get a string bit.
        /// </summary>
        public bool StringGetBit(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringGetBit(key, offset, flags);
        }

        /// <summary>
        /// Calls redis to get a string bit.
        /// </summary>
        public Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringGetBitAsync(key, offset, flags);
        }

        /// <summary>
        /// Calls redis to get a string range.
        /// </summary>
        public RedisValue StringGetRange(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringGetRange(key, start, end, flags);
        }

        /// <summary>
        /// Calls redis to get a string range.
        /// </summary>
        public Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringGetRangeAsync(key, start, end, flags);
        }

        /// <summary>
        /// Sets value in redis, and gets it from memory if possible.
        /// </summary>
        public RedisValue StringGetSet(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            bool hasSetInRedis = false;

            RedisValue result = _dbData.MemoryStrings.GetFromMemory(key, () =>
            {
                if (_redisDb == null)
                    return Task.FromResult(new RedisValue());
                
                hasSetInRedis = true;
                return Task.FromResult(_redisDb.StringGetSet(key, value, flags));
            }).Result;

            //Set it in memory
            _dbData.MemoryCache.Add(key, value, null, When.Always);

            //Set it in redis if necessary
            if(!hasSetInRedis && _redisDb != null)
            {
                _redisDb.StringSet(key, value, null, When.Always, flags);
            }

            return result;
        }

        /// <summary>
        /// Sets value in redis, and gets it from memory if possible.
        /// </summary>
        public async Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            bool hasSetInRedis = false;
            RedisValue result = await _dbData.MemoryStrings.GetFromMemory(key, () =>
            {
                if (_redisDb == null)
                    return Task.FromResult(new RedisValue());

                hasSetInRedis = true;
                return _redisDb.StringGetSetAsync(key, value, flags);
            });

            //Set it in memory
            _dbData.MemoryCache.Add(key, value, null, When.Always);

            //Set it in redis if necessary
            if(!hasSetInRedis && _redisDb != null)
                await _redisDb.StringGetSetAsync(key, value, flags);

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

                if (_redisDb == null)
                {
                    return Task.FromResult(new RedisValueWithExpiry());
                }
                else
                {
                    return Task.FromResult(_redisDb.StringGetWithExpiry(key, flags));
                }
            }).Result;
            
        }

        /// <summary>
        /// Gets value from Memory, or from Redis if not present.
        /// </summary>
        public Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _dbData.MemoryStrings.GetFromMemoryWithExpiry(key, () =>
            {
                if (_redisDb == null)
                {
                    return Task.FromResult(new RedisValueWithExpiry());
                }
                else
                {
                    return _redisDb.StringGetWithExpiryAsync(key, flags);
                }
            });
        }

        /// <summary>
        /// Forwards the request to Redis and invalidates in-memory value.
        /// </summary>
        public double StringIncrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                throw new NotImplementedException("StringIncrement not yet supported as an in-memory operation");

            return _redisDb.StringIncrement(key, value, flags);
        }

        /// <summary>
        /// Forwards the request to Redis and invalidates in-memory value.
        /// </summary>
        public long StringIncrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                throw new NotImplementedException("StringIncrement not yet supported as an in-memory operation");

            return _redisDb.StringIncrement(key, value, flags);
        }

        /// <summary>
        /// Forwards the request to Redis and invalidates in-memory value.
        /// </summary>
        public Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                throw new NotImplementedException("StringIncrement not yet supported as an in-memory operation");

            return _redisDb.StringIncrementAsync(key, value, flags);
        }

        /// <summary>
        /// Forwards the request to Redis and invalidates in-memory value.
        /// </summary>
        public Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                throw new NotImplementedException("StringIncrement not yet supported as an in-memory operation");

            return _redisDb.StringIncrementAsync(key, value, flags);
        }

        /// <summary>
        /// Gets the string length from memory, if the string is stored in memory.
        /// Otherwise, gets it from Redis.
        /// </summary>
        public long StringLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            //Try and get the string from memory
            long length = _dbData.MemoryStrings.GetStringLength(key);
            if (length > 0 || _redisDb == null)
                return length;
            else
                //Todo: we could cache the length
                return _redisDb.StringLength(key, flags);
        }

        /// <summary>
        /// Gets the string length from memory, if the string is stored in memory.
        /// Otherwise, gets it from Redis.
        /// </summary>
        public Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            long length = _dbData.MemoryStrings.GetStringLength(key);
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

            if (_redisDb == null)
                return true;

            return _redisDb.StringSet(values, when, flags);
        }

        /// <summary>
        /// Sets a string in both memory and Redis
        /// </summary>
        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Add(key, value, expiry, when);

            if (_redisDb == null)
                return true;

            return _redisDb.StringSet(key, value, expiry, when, flags);
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

            if (_redisDb == null)
                return Task.FromResult(true);

            return _redisDb.StringSetAsync(values, when, flags);
        }

        /// <summary>
        /// Sets a string in both memory and Redis.
        /// </summary>
        public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Add(key, value, expiry, when);

            if (_redisDb == null)
                return Task.FromResult(true);

            return _redisDb.StringSetAsync(key, value, expiry, when, flags);
        }

        /// <summary>
        /// Forwards request to redis and invalidates in-memory value.
        /// </summary>
        public bool StringSetBit(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
            {
                throw new NotImplementedException("StringSetBit not yet supported as an in-memory operation.");
            }
            else
            {
                return _redisDb.StringSetBit(key, offset, bit, flags);
            }
        }

        /// <summary>
        /// Forwards request to redis and invalidates in-memory value.
        /// </summary>
        public Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
            {
                throw new NotImplementedException("StringSetBit not yet supported as an in-memory operation.");
            }
            else
            {
                return _redisDb.StringSetBitAsync(key, offset, bit, flags);
            }
        }

        /// <summary>
        /// Forwards request to redis and invalidates in-memory value.
        /// </summary>
        public RedisValue StringSetRange(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringSetRange(key, offset, value, flags);
        }

        /// <summary>
        /// Forwards request to redis and invalidates in-memory value.
        /// </summary>
        public Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            _dbData.MemoryCache.Remove(new[] { (string)key });

            if (_redisDb == null)
                throw new NotImplementedException();

            return _redisDb.StringSetRangeAsync(key, offset, value, flags);
        }

        public bool TryWait(Task task)
        {
            if (_redisDb == null)
            {
                //todo - what timeout should be used?
                return Task.WaitAll(new[] { task }, 1000);
            }
            else
            {
                return _redisDb.TryWait(task);
            }
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
            if (_redisDb == null)
            {
                Task.WaitAll(task);
                return task.Result;
            }
            else
            {
                return _redisDb.Wait(task);
            }
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
