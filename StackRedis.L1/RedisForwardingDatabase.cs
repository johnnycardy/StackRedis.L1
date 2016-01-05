using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using StackRedis.L1.KeyspaceNotifications;

namespace StackRedis.L1
{
    /// <summary>
    /// Forwards calls to another instance of IDatabase
    /// </summary>
    public class RedisForwardingDatabase : IDatabase
    {
        protected IDatabase _redisDb;
        protected Action _onCall;

        public RedisForwardingDatabase(IDatabase redisDb, Action onCall = null)
        {
            if (redisDb == null)
                throw new ArgumentException();

            _redisDb = redisDb;
            _onCall = onCall ?? new Action(() => { });
        }
        
        public virtual ConnectionMultiplexer Multiplexer
        {
            get
            {
                _onCall();
                return _redisDb.Multiplexer;
            }
        }

        public virtual int Database
        {
            get
            {
                _onCall();
                return _redisDb.Database;
            }
        }

        public virtual IBatch CreateBatch(object asyncState = null)
        {
			_onCall();
            return _redisDb.CreateBatch(asyncState);
        }

        public virtual ITransaction CreateTransaction(object asyncState = null)
        {
			_onCall();
            
            return _redisDb.CreateTransaction(asyncState);
        }

        public virtual RedisValue DebugObject(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.DebugObject(key, flags);
        }

        public virtual Task<RedisValue> DebugObjectAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.DebugObjectAsync(key, flags);
        }

        public virtual double HashDecrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashDecrement(key, hashField, value, flags);
        }

        public virtual long HashDecrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashDecrement(key, hashField, value, flags);
        }

        public virtual Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashDecrementAsync(key, hashField, value, flags);
        }

        public virtual Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashDecrementAsync(key, hashField, value, flags);
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public virtual long HashDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashDelete(key, hashFields, flags);
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public virtual bool HashDelete(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashDelete(key, hashField, flags);
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public virtual Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashDeleteAsync(key, hashFields, flags);
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public virtual Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashDeleteAsync(key, hashField, flags);
        }

        public virtual bool HashExists(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashExists(key, hashField, flags);
        }

        public virtual Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashExistsAsync(key, hashField, flags);
        }

        public virtual RedisValue[] HashGet(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashGet(key, hashFields, flags);
        }

        public virtual RedisValue HashGet(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashGet(key, hashField, flags);
        }

        public virtual HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashGetAll(key, flags);
        }

        public virtual Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashGetAllAsync(key, flags);
        }

        public virtual Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashGetAsync(key, hashFields, flags);
        }

        public virtual Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashGetAsync(key, hashField, flags);
        }

        public virtual double HashIncrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashIncrement(key, hashField, value, flags);
        }

        public virtual long HashIncrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashIncrement(key, hashField, value, flags);
        }

        public virtual Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashIncrementAsync(key, hashField, value, flags);
        }

        public virtual Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashIncrementAsync(key, hashField, value, flags);
        }

        public virtual RedisValue[] HashKeys(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashKeys(key, flags);
        }

        public virtual Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashKeysAsync(key, flags);
        }

        public virtual long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashLength(key, flags);
        }

        public virtual Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashLengthAsync(key, flags);
        }

        public virtual IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
			_onCall();
            
            return _redisDb.HashScan(key, pattern, pageSize, flags);
        }

        public virtual IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashScan(key, pattern, pageSize, cursor, pageOffset, flags);
        }

        /// <summary>
        /// Sets the values in memory and in redis.
        /// </summary>
        public virtual void HashSet(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            _redisDb.HashSet(key, hashFields, flags);
        }

        public virtual bool HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashSet(key, hashField, value, when, flags);
        }

        public virtual Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashSetAsync(key, hashFields, flags);
        }

        public virtual Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.HashSetAsync(key, hashField, value, when, flags);
        }

        public virtual RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashValues(key, flags);
        }

        public virtual Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HashValuesAsync(key, flags);
        }

        public virtual bool HyperLogLogAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HyperLogLogAdd(key, values, flags);
        }

        public virtual bool HyperLogLogAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HyperLogLogAdd(key, value, flags);
        }

        public virtual Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HyperLogLogAddAsync(key, values, flags);
        }

        public virtual Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HyperLogLogAddAsync(key, value, flags);
        }

        public virtual long HyperLogLogLength(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HyperLogLogLength(keys, flags);
        }

        public virtual long HyperLogLogLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HyperLogLogLength(key, flags);
        }

        public virtual Task<long> HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HyperLogLogLengthAsync(keys, flags);
        }

        public virtual Task<long> HyperLogLogLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HyperLogLogLengthAsync(key, flags);
        }

        public virtual void HyperLogLogMerge(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            _redisDb.HyperLogLogMerge(destination, sourceKeys, flags);
        }

        public virtual void HyperLogLogMerge(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            _redisDb.HyperLogLogMerge(destination, first, second, flags);
        }

        public virtual Task HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HyperLogLogMergeAsync(destination, sourceKeys, flags);
        }

        public virtual Task HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.HyperLogLogMergeAsync(destination, first, second, flags);
        }

        public virtual EndPoint IdentifyEndpoint(RedisKey key = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.IdentifyEndpoint(key, flags);
        }

        public virtual Task<EndPoint> IdentifyEndpointAsync(RedisKey key = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.IdentifyEndpointAsync(key, flags);
        }

        public virtual bool IsConnected(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
            return _redisDb.IsConnected(key, flags);
        }

        public virtual long KeyDelete(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyDelete(keys, flags);
        }

        public virtual bool KeyDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyDelete(key, flags);
        }

        public virtual Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyDeleteAsync(keys, flags);
        }

        public virtual Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyDeleteAsync(key, flags);
        }

        public virtual byte[] KeyDump(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.KeyDump(key, flags);
        }

        public virtual Task<byte[]> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.KeyDumpAsync(key, flags);
        }

        public virtual bool KeyExists(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyExists(key, flags);
        }

        public virtual Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyExistsAsync(key, flags);
        }

        public virtual bool KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyExpire(key, expiry, flags);
        }

        public virtual bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyExpire(key, expiry, flags);
            
        }

        public virtual Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyExpireAsync(key, expiry, flags);
        }

        public virtual Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyExpireAsync(key, expiry, flags);
            
        }

        public virtual void KeyMigrate(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            _redisDb.KeyMigrate(key, toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
        }

        public virtual Task KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyMigrateAsync(key, toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
          
        }

        public virtual bool KeyMove(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyMove(key, database, flags);
        }

        public virtual Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyMoveAsync(key, database, flags);
        }

        public virtual bool KeyPersist(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
           return _redisDb.KeyPersist(key, flags);
        }

        public virtual Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.KeyPersistAsync(key, flags);
        }

        public virtual RedisKey KeyRandom(CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.KeyRandom(flags);
        }

        public virtual Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.KeyRandomAsync(flags);
        }

        public virtual bool KeyRename(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyRename(key, newKey, when, flags);
        }

        public virtual Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyRenameAsync(key, newKey, when, flags);
        }

        public virtual void KeyRestore(RedisKey key, byte[] value, TimeSpan? expiry = default(TimeSpan?), CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            _redisDb.KeyRestore(key, value, expiry, flags);
        }

        public virtual Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = default(TimeSpan?), CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.KeyRestoreAsync(key, value, expiry, flags);
        }

        public virtual TimeSpan? KeyTimeToLive(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.KeyTimeToLive(key, flags);
        }

        public virtual Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.KeyTimeToLiveAsync(key, flags);
        }

        public virtual RedisType KeyType(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.KeyType(key, flags);
        }

        public virtual Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.KeyTypeAsync(key, flags);
        }

        public virtual RedisValue ListGetByIndex(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListGetByIndex(key, index, flags);
        }

        public virtual Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListGetByIndexAsync(key, index, flags);
        }

        public virtual long ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListInsertAfter(key, pivot, value, flags);
        }

        public virtual Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListInsertAfterAsync(key, pivot, value, flags);
        }

        public virtual long ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListInsertBefore(key, pivot, value, flags);
        }

        public virtual Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListInsertBeforeAsync(key, pivot, value, flags);
        }

        public virtual RedisValue ListLeftPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListLeftPop(key, flags);
        }

        public virtual Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListLeftPopAsync(key, flags);
        }

        public virtual long ListLeftPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListLeftPush(key, values, flags);
        }

        public virtual long ListLeftPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListLeftPush(key, value, when, flags);
        }

        public virtual Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListLeftPushAsync(key, values, flags);
        }

        public virtual Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListLeftPushAsync(key, value, when, flags);
        }

        public virtual long ListLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListLength(key, flags);
        }

        public virtual Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListLengthAsync(key, flags);
        }

        public virtual RedisValue[] ListRange(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRange(key, start, stop, flags);
        }

        public virtual Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRangeAsync(key, start, stop, flags);
        }

        public virtual long ListRemove(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRemove(key, value, count, flags);
        }

        public virtual Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRemoveAsync(key, value, count, flags);
        }

        public virtual RedisValue ListRightPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRightPop(key, flags);
        }

        public virtual Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRightPopAsync(key, flags);
        }

        public virtual RedisValue ListRightPopLeftPush(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRightPopLeftPush(source, destination, flags);
        }

        public virtual Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRightPopLeftPushAsync(source, destination, flags);
        }

        public virtual long ListRightPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRightPush(key, values, flags);
        }

        public virtual long ListRightPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRightPush(key, value, when, flags);
        }

        public virtual Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRightPushAsync(key, values, flags);
        }

        public virtual Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListRightPushAsync(key, value, when, flags);
        }

        public virtual void ListSetByIndex(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            _redisDb.ListSetByIndex(key, index, value, flags);
        }

        public virtual Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListSetByIndexAsync(key, index, value, flags);
        }

        public virtual void ListTrim(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            _redisDb.ListTrim(key, start, stop, flags);
        }

        public virtual Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ListTrimAsync(key, start, stop, flags);
        }

        public virtual bool LockExtend(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.LockExtend(key, value, expiry, flags);
        }

        public virtual Task<bool> LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.LockExtendAsync(key, value, expiry, flags);
        }

        public virtual RedisValue LockQuery(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.LockQuery(key, flags);
        }

        public virtual Task<RedisValue> LockQueryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.LockQueryAsync(key, flags);
        }

        public virtual bool LockRelease(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.LockRelease(key, value, flags);
        }

        public virtual Task<bool> LockReleaseAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.LockReleaseAsync(key, value, flags);
        }

        public virtual bool LockTake(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.LockTake(key, value, expiry, flags);
        }

        public virtual Task<bool> LockTakeAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.LockTakeAsync(key, value, expiry, flags);
        }

        public virtual TimeSpan Ping(CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.Ping(flags);
        }

        public virtual Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.PingAsync(flags);
        }

        public virtual long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.Publish(channel, message, flags);
        }

        public virtual Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.PublishAsync(channel, message, flags);
        }

        public virtual RedisResult ScriptEvaluate(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ScriptEvaluate(script, parameters, flags);
        }

        public virtual RedisResult ScriptEvaluate(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.ScriptEvaluate(script, parameters, flags);
        }

        public virtual RedisResult ScriptEvaluate(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ScriptEvaluate(hash, keys, values, flags);
        }

        public virtual RedisResult ScriptEvaluate(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ScriptEvaluate(script, keys, values, flags);
        }

        public virtual Task<RedisResult> ScriptEvaluateAsync(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ScriptEvaluateAsync(script, parameters, flags);
        }

        public virtual Task<RedisResult> ScriptEvaluateAsync(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ScriptEvaluateAsync(script, parameters, flags);
        }

        public virtual Task<RedisResult> ScriptEvaluateAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ScriptEvaluateAsync(hash, keys, values, flags);
        }

        public virtual Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.ScriptEvaluateAsync(script, keys, values, flags);
        }

        public virtual long SetAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetAdd(key, values, flags);
        }

        public virtual bool SetAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetAdd(key, value, flags);
        }

        public virtual Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetAddAsync(key, values, flags);
        }

        public virtual Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetAddAsync(key, value, flags);
        }

        public virtual RedisValue[] SetCombine(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetCombine(operation, keys, flags);
        }

        public virtual RedisValue[] SetCombine(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetCombine(operation, first, second, flags);
        }

        public virtual long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetCombineAndStore(operation, destination, keys, flags);
        }

        public virtual long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetCombineAndStore(operation, destination, first, second, flags);
        }

        public virtual Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetCombineAndStoreAsync(operation, destination, keys, flags);
        }

        public virtual Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetCombineAndStoreAsync(operation, destination, first, second, flags);
        }

        public virtual Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetCombineAsync(operation, keys, flags);
        }

        public virtual Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetCombineAsync(operation, first, second, flags);
        }

        public virtual bool SetContains(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetContains(key, value, flags);
        }

        public virtual Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetContainsAsync(key, value, flags);
        }

        public virtual long SetLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetLength(key, flags);
        }

        public virtual Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetLengthAsync(key, flags);
        }

        public virtual RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetMembers(key, flags);
        }

        public virtual Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetMembersAsync(key, flags);
        }

        public virtual bool SetMove(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetMove(source, destination, value, flags);
        }

        public virtual Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetMoveAsync(source, destination, value, flags);
        }

        public virtual RedisValue SetPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetPop(key, flags);
        }

        public virtual Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetPopAsync(key, flags);
        }

        public virtual RedisValue SetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetRandomMember(key, flags);
        }

        public virtual Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetRandomMemberAsync(key, flags);
        }

        public virtual RedisValue[] SetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetRandomMembers(key, count, flags);
        }

        public virtual Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetRandomMembersAsync(key, count, flags);
        }

        public virtual long SetRemove(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetRemove(key, values, flags);
        }

        public virtual bool SetRemove(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetRemove(key, value, flags);
        }

        public virtual Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetRemoveAsync(key, values, flags);
        }

        public virtual Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetRemoveAsync(key, value, flags);
        }

        public virtual IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
			_onCall();
            
                

            return _redisDb.SetScan(key, pattern, pageSize, flags);
        }

        public virtual IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SetScan(key, pattern, pageSize, cursor, pageOffset, flags);
        }

        public virtual RedisValue[] Sort(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.Sort(key, skip, take, order, sortType, by, get, flags);
        }

        public virtual long SortAndStore(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortAndStore(destination, key, skip, take, order, sortType, by, get, flags);
        }

        public virtual Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortAndStoreAsync(destination, key, skip, take, order, sortType, by, get, flags);
        }

        public virtual Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortAsync(key, skip, take, order, sortType, by, get, flags);
        }

        public virtual long SortedSetAdd(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetAdd(key, values, flags);
        }

        public virtual bool SortedSetAdd(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetAdd(key, member, score, flags);
        }

        public virtual Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetAddAsync(key, values, flags);
        }

        public virtual Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetAddAsync(key, member, score, flags);
        }

        public virtual long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetCombineAndStore(operation, destination, keys, weights, aggregate, flags);
        }

        public virtual long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetCombineAndStore(operation, destination, first, second, aggregate, flags);
        }

        public virtual Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate, flags);
        }

        public virtual Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate, flags);
        }

        public virtual double SortedSetDecrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetDecrement(key, member, value, flags);
        }

        public virtual Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetDecrementAsync(key, member, value, flags);
        }

        public virtual double SortedSetIncrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetIncrement(key, member, value, flags);
        }

        public virtual Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetIncrementAsync(key, member, value, flags);
        }

        public virtual long SortedSetLength(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetLength(key, min, max, exclude, flags);
        }

        public virtual Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetLengthAsync(key, min, max, exclude);
        }

        public virtual long SortedSetLengthByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetLengthByValue(key, min, max, exclude, flags);
        }

        public virtual Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetLengthByValueAsync(key, min, max, exclude, flags);
        }

        public virtual RedisValue[] SortedSetRangeByRank(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRangeByRank(key, start, stop, order, flags);
        }

        public virtual Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRangeByRankAsync(key, start, stop, order, flags);
        }

        public virtual SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRangeByRankWithScores(key, start, stop, order, flags);
        }

        public virtual Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRangeByRankWithScoresAsync(key, start, stop, order, flags);
        }

        public virtual RedisValue[] SortedSetRangeByScore(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRangeByScore(key, start, stop, exclude, order, skip, take, flags);
        }

        public virtual Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take, flags);
        }

        public virtual SortedSetEntry[] SortedSetRangeByScoreWithScores(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRangeByScoreWithScores(key, start, stop, exclude, order, skip, take, flags);
        }

        public virtual Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take, flags);
        }

        public virtual RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min = default(RedisValue), RedisValue max = default(RedisValue), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRangeByValue(key, min, max, exclude, skip, take, flags);
        }

        public virtual Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = default(RedisValue), RedisValue max = default(RedisValue), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRangeByValueAsync(key, min, max, exclude, skip, take, flags);
        }

        public virtual long? SortedSetRank(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRank(key, member, order, flags);
        }

        public virtual Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRankAsync(key, member, order, flags);
        }

        public virtual long SortedSetRemove(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRemove(key, members, flags);
        }

        public virtual bool SortedSetRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRemove(key, member, flags);
        }

        public virtual Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRemoveAsync(key, members, flags);
        }

        public virtual Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRemoveAsync(key, member, flags);
        }

        public virtual long SortedSetRemoveRangeByRank(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRemoveRangeByRank(key, start, stop, flags);
        }

        public virtual Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRemoveRangeByRankAsync(key, start, stop, flags);
        }

        public virtual long SortedSetRemoveRangeByScore(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRemoveRangeByScore(key, start, stop, exclude, flags);
        }

        public virtual Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude, flags);
        }

        public virtual long SortedSetRemoveRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRemoveRangeByValue(key, min, max, exclude, flags);
        }

        public virtual Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetRemoveRangeByValueAsync(key, min, max, exclude, flags);
        }

        public virtual IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
			_onCall();
            
                

            return _redisDb.SortedSetScan(key, pattern, pageSize, flags);
        }

        public virtual IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetScan(key, pattern, pageSize, cursor, pageOffset, flags);
        }

        public virtual double? SortedSetScore(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetScore(key, member, flags);
        }

        public virtual Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.SortedSetScoreAsync(key, member, flags);
        }
        
        public virtual long StringAppend(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringAppend(key, value, flags);
            
        }

        public virtual Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringAppendAsync(key, value, flags);
            
        }

        public virtual long StringBitCount(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringBitCount(key, start, end, flags);
        }

        public virtual Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringBitCountAsync(key, start, end, flags);
        }

        public virtual long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.StringBitOperation(operation, destination, keys, flags);
        }

        public virtual long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.StringBitOperation(operation, destination, first, second, flags);
        }

        public virtual Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.StringBitOperationAsync(operation, destination, keys, flags);
        }

        public virtual Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.StringBitOperationAsync(operation, destination, first, second, flags);
        }

        public virtual long StringBitPosition(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.StringBitPosition(key, bit, start, end, flags);
        }

        public virtual Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.StringBitPositionAsync(key, bit, start, end, flags);
        }

        public virtual double StringDecrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringDecrement(key, value, flags);
        }

        public virtual long StringDecrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringDecrement(key, value, flags);
        }

        public virtual Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
           return _redisDb.StringDecrementAsync(key, value, flags);
        }

        public virtual Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringDecrementAsync(key, value, flags);
        }

        public virtual RedisValue[] StringGet(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringGet(keys, flags);
        }

        public virtual RedisValue StringGet(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringGet(key, flags);
        }

        public virtual Task<RedisValue[]> StringGetAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringGetAsync(keys, flags);
        }

        public virtual Task<RedisValue> StringGetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringGetAsync(key, flags);
        }

        public virtual bool StringGetBit(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringGetBit(key, offset, flags);
        }

        public virtual Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.StringGetBitAsync(key, offset, flags);
        }

        public virtual RedisValue StringGetRange(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.StringGetRange(key, start, end, flags);
        }

        public virtual Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            
                

            return _redisDb.StringGetRangeAsync(key, start, end, flags);
        }

        public virtual RedisValue StringGetSet(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringGetSet(key, value, flags);
        }

        public virtual Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringGetSetAsync(key, value, flags);
        }

        public virtual RedisValueWithExpiry StringGetWithExpiry(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringGetWithExpiry(key, flags);
        }

        public virtual Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringGetWithExpiryAsync(key, flags);
        }

        public virtual double StringIncrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringIncrement(key, value, flags);
        }

        public virtual long StringIncrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringIncrement(key, value, flags);
        }

        public virtual Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringIncrementAsync(key, value, flags);
        }

        public virtual Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringIncrementAsync(key, value, flags);
        }

        public virtual long StringLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringLength(key, flags);
        }

        public virtual Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringLengthAsync(key, flags);
        }

        public virtual bool StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringSet(values, when, flags);
        }

        public virtual bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringSet(key, value, expiry, when, flags);
        }

        public virtual Task<bool> StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringSetAsync(values, when, flags);
        }

        public virtual Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringSetAsync(key, value, expiry, when, flags);
        }

        public virtual bool StringSetBit(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringSetBit(key, offset, bit, flags);
        }

        public virtual Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringSetBitAsync(key, offset, bit, flags);
        }


        public virtual RedisValue StringSetRange(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringSetRange(key, offset, value, flags);
        }

        public virtual Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
			_onCall();
            return _redisDb.StringSetRangeAsync(key, offset, value, flags);
        }

        public virtual bool TryWait(Task task)
        {
			_onCall();
            return _redisDb.TryWait(task);
        }

        public virtual void Wait(Task task)
        {
			_onCall();
            _redisDb.Wait(task);
        }

        public virtual T Wait<T>(Task<T> task)
        {
			_onCall();
            return _redisDb.Wait(task);
        }

        public virtual void WaitAll(params Task[] tasks)
        {
			_onCall();
            _redisDb.WaitAll(tasks);
        }
    }
}
