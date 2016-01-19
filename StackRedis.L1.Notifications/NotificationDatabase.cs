using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Globalization;

namespace StackRedis.L1.Notifications
{
    /// <summary>
    /// Raises redis events for keyspace changes not covered by keyspace notifications.
    /// </summary>
    public class NotificationDatabase : IDatabase
    {
        protected IDatabase _redisDb;
        private ISubscriber _sub;
        private string _channelDb;
        private string _process;
        
        public NotificationDatabase(IDatabase redisDb, string processId = null)
        {
            if (redisDb == null)
                throw new ArgumentException();

            _redisDb = redisDb;
            _sub = redisDb.Multiplexer.GetSubscriber();
            _channelDb = "__keyspace_detailed@" + redisDb.Database + "__:";
            _process = processId ?? GetProcessId();
        }

        /// <summary>
        /// Returns the current machine unique ID and the process.
        /// </summary>
        public static string GetProcessId()
        {
            string result = NetworkInterface.GetAllNetworkInterfaces()
                            .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                            .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

            if (string.IsNullOrEmpty(result))
                result = Environment.MachineName;

            return result + Process.GetCurrentProcess().Id.ToString();
        }

        private void PublishEvent(string key, string keyMessage)
        {
            string channel = _channelDb + key;
            string message = $"{_process}:{keyMessage}";

            //Publish the event
            _sub.Publish(channel, message);
        }

        public ConnectionMultiplexer Multiplexer
        {
            get
            {

                return _redisDb.Multiplexer;
            }
        }

        public int Database
        {
            get
            {

                return _redisDb.Database;
            }
        }

        public IBatch CreateBatch(object asyncState = null)
        {

            return _redisDb.CreateBatch(asyncState);
        }

        public ITransaction CreateTransaction(object asyncState = null)
        {


            return _redisDb.CreateTransaction(asyncState);
        }

        public RedisValue DebugObject(RedisKey key, CommandFlags flags = CommandFlags.None)
        {


            return _redisDb.DebugObject(key, flags);
        }

        public Task<RedisValue> DebugObjectAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {


            return _redisDb.DebugObjectAsync(key, flags);
        }

        public double HashDecrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "hdecrbyfloat:" + hashField);
            return _redisDb.HashDecrement(key, hashField, value, flags);
        }

        public long HashDecrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "hdecr:" + hashField);
            return _redisDb.HashDecrement(key, hashField, value, flags);
        }

        public Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "hdecrbyfloat:" + hashField);
            return _redisDb.HashDecrementAsync(key, hashField, value, flags);
        }

        public Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "hdecr:" + hashField);
            return _redisDb.HashDecrementAsync(key, hashField, value, flags);
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public long HashDelete(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            foreach(var hashField in hashFields)
                this.PublishEvent(key, "hdel:" + (string)hashField);

            return _redisDb.HashDelete(key, hashFields, flags);
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public bool HashDelete(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "hdel:" + (string)hashField);
            return _redisDb.HashDelete(key, hashField, flags);
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            foreach (var hashField in hashFields)
                this.PublishEvent(key, "hdel:" + (string)hashField);
            return _redisDb.HashDeleteAsync(key, hashFields, flags);
        }

        /// <summary>
        /// Deletes hash key from memory and Redis.
        /// </summary>
        public Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "hdel:" + (string)hashField);
            return _redisDb.HashDeleteAsync(key, hashField, flags);
        }

        public bool HashExists(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {

            return _redisDb.HashExists(key, hashField, flags);
        }

        public Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {

            return _redisDb.HashExistsAsync(key, hashField, flags);
        }

        public RedisValue[] HashGet(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {

            return _redisDb.HashGet(key, hashFields, flags);
        }

        public RedisValue HashGet(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {

            return _redisDb.HashGet(key, hashField, flags);
        }

        public HashEntry[] HashGetAll(RedisKey key, CommandFlags flags = CommandFlags.None)
        {


            return _redisDb.HashGetAll(key, flags);
        }

        public Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {


            return _redisDb.HashGetAllAsync(key, flags);
        }

        public Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {

            return _redisDb.HashGetAsync(key, hashFields, flags);
        }

        public Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {

            return _redisDb.HashGetAsync(key, hashField, flags);
        }

        public double HashIncrement(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "hincrbyfloat:" + hashField);
            return _redisDb.HashIncrement(key, hashField, value, flags);
        }

        public long HashIncrement(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "hincr:" + hashField);
            return _redisDb.HashIncrement(key, hashField, value, flags);
        }

        public Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "hincrbyfloat:" + hashField);
            return _redisDb.HashIncrementAsync(key, hashField, value, flags);
        }

        public Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "hincr:" + hashField);
            return _redisDb.HashIncrementAsync(key, hashField, value, flags);
        }

        public RedisValue[] HashKeys(RedisKey key, CommandFlags flags = CommandFlags.None)
        {


            return _redisDb.HashKeys(key, flags);
        }

        public Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {


            return _redisDb.HashKeysAsync(key, flags);
        }

        public long HashLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {


            return _redisDb.HashLength(key, flags);
        }

        public Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {


            return _redisDb.HashLengthAsync(key, flags);
        }

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {


            return _redisDb.HashScan(key, pattern, pageSize, flags);
        }

        public IEnumerable<HashEntry> HashScan(RedisKey key, RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {


            return _redisDb.HashScan(key, pattern, pageSize, cursor, pageOffset, flags);
        }

        /// <summary>
        /// Sets the values in memory and in redis.
        /// </summary>
        public void HashSet(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            _redisDb.HashSet(key, hashFields, flags);
            
            foreach (HashEntry entry in hashFields)
                PublishEvent(key, "hset:" + entry.Name);
        }

        public bool HashSet(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "hset:" + hashField);
            return _redisDb.HashSet(key, hashField, value, when, flags);
        }

        public Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            foreach (HashEntry entry in hashFields)
                PublishEvent(key, "hset:" + entry.Name);
            return _redisDb.HashSetAsync(key, hashFields, flags);
        }

        public Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {

            PublishEvent(key, "hset:" + hashField);
            return _redisDb.HashSetAsync(key, hashField, value, when, flags);
        }

        public RedisValue[] HashValues(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.HashValues(key, flags);
        }

        public Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            

            return _redisDb.HashValuesAsync(key, flags);
        }

        public bool HyperLogLogAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            

            return _redisDb.HyperLogLogAdd(key, values, flags);
        }

        public bool HyperLogLogAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            

            return _redisDb.HyperLogLogAdd(key, value, flags);
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            

            return _redisDb.HyperLogLogAddAsync(key, values, flags);
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            

            return _redisDb.HyperLogLogAddAsync(key, value, flags);
        }

        public long HyperLogLogLength(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            

            return _redisDb.HyperLogLogLength(keys, flags);
        }

        public long HyperLogLogLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            

            return _redisDb.HyperLogLogLength(key, flags);
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            

            return _redisDb.HyperLogLogLengthAsync(keys, flags);
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            

            return _redisDb.HyperLogLogLengthAsync(key, flags);
        }

        public void HyperLogLogMerge(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            

            _redisDb.HyperLogLogMerge(destination, sourceKeys, flags);
        }

        public void HyperLogLogMerge(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            

            _redisDb.HyperLogLogMerge(destination, first, second, flags);
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            

            return _redisDb.HyperLogLogMergeAsync(destination, sourceKeys, flags);
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            

            return _redisDb.HyperLogLogMergeAsync(destination, first, second, flags);
        }

        public EndPoint IdentifyEndpoint(RedisKey key = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.IdentifyEndpoint(key, flags);
        }

        public Task<EndPoint> IdentifyEndpointAsync(RedisKey key = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.IdentifyEndpointAsync(key, flags);
        }

        public bool IsConnected(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.IsConnected(key, flags);
        }

        public long KeyDelete(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            foreach (var key in keys)
                PublishEvent(key, "del");

            return _redisDb.KeyDelete(keys, flags);
        }

        public bool KeyDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "del");
            return _redisDb.KeyDelete(key, flags);
        }

        public Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            foreach(var key in keys)
                PublishEvent(key, "del");

            return _redisDb.KeyDeleteAsync(keys, flags);
        }

        public Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "del");
            return _redisDb.KeyDeleteAsync(key, flags);
        }

        public byte[] KeyDump(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.KeyDump(key, flags);
        }

        public Task<byte[]> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.KeyDumpAsync(key, flags);
        }

        public bool KeyExists(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.KeyExists(key, flags);
        }

        public Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.KeyExistsAsync(key, flags);
        }

        public bool KeyExpire(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "expire");
            return _redisDb.KeyExpire(key, expiry, flags);
        }

        public bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "expire");
            return _redisDb.KeyExpire(key, expiry, flags);
        }

        public Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "expire");
            return _redisDb.KeyExpireAsync(key, expiry, flags);
        }

        public Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "expire");
            return _redisDb.KeyExpireAsync(key, expiry, flags);

        }

        public void KeyMigrate(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            _redisDb.KeyMigrate(key, toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
        }

        public Task KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.KeyMigrateAsync(key, toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);

        }

        public bool KeyMove(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.KeyMove(key, database, flags);
        }

        public Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.KeyMoveAsync(key, database, flags);
        }

        public bool KeyPersist(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.KeyPersist(key, flags);
        }

        public Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.KeyPersistAsync(key, flags);
        }

        public RedisKey KeyRandom(CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.KeyRandom(flags);
        }

        public Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.KeyRandomAsync(flags);
        }

        public bool KeyRename(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "rename_key:" + (string)newKey);
            return _redisDb.KeyRename(key, newKey, when, flags);
        }

        public Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "rename_key:" + (string)newKey);
            return _redisDb.KeyRenameAsync(key, newKey, when, flags);
        }

        public void KeyRestore(RedisKey key, byte[] value, TimeSpan? expiry = default(TimeSpan?), CommandFlags flags = CommandFlags.None)
        {
            
            _redisDb.KeyRestore(key, value, expiry, flags);
        }

        public Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = default(TimeSpan?), CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.KeyRestoreAsync(key, value, expiry, flags);
        }

        public TimeSpan? KeyTimeToLive(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.KeyTimeToLive(key, flags);
        }

        public Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.KeyTimeToLiveAsync(key, flags);
        }

        public RedisType KeyType(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.KeyType(key, flags);
        }

        public Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.KeyTypeAsync(key, flags);
        }

        public RedisValue ListGetByIndex(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListGetByIndex(key, index, flags);
        }

        public Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListGetByIndexAsync(key, index, flags);
        }

        public long ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListInsertAfter(key, pivot, value, flags);
        }

        public Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListInsertAfterAsync(key, pivot, value, flags);
        }

        public long ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListInsertBefore(key, pivot, value, flags);
        }

        public Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListInsertBeforeAsync(key, pivot, value, flags);
        }

        public RedisValue ListLeftPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListLeftPop(key, flags);
        }

        public Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListLeftPopAsync(key, flags);
        }

        public long ListLeftPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListLeftPush(key, values, flags);
        }

        public long ListLeftPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListLeftPush(key, value, when, flags);
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListLeftPushAsync(key, values, flags);
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListLeftPushAsync(key, value, when, flags);
        }

        public long ListLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListLength(key, flags);
        }

        public Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListLengthAsync(key, flags);
        }

        public RedisValue[] ListRange(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRange(key, start, stop, flags);
        }

        public Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRangeAsync(key, start, stop, flags);
        }

        public long ListRemove(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRemove(key, value, count, flags);
        }

        public Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRemoveAsync(key, value, count, flags);
        }

        public RedisValue ListRightPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRightPop(key, flags);
        }

        public Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRightPopAsync(key, flags);
        }

        public RedisValue ListRightPopLeftPush(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRightPopLeftPush(source, destination, flags);
        }

        public Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRightPopLeftPushAsync(source, destination, flags);
        }

        public long ListRightPush(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRightPush(key, values, flags);
        }

        public long ListRightPush(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRightPush(key, value, when, flags);
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRightPushAsync(key, values, flags);
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListRightPushAsync(key, value, when, flags);
        }

        public void ListSetByIndex(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            _redisDb.ListSetByIndex(key, index, value, flags);
        }

        public Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListSetByIndexAsync(key, index, value, flags);
        }

        public void ListTrim(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            



            _redisDb.ListTrim(key, start, stop, flags);
        }

        public Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ListTrimAsync(key, start, stop, flags);
        }

        public bool LockExtend(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.LockExtend(key, value, expiry, flags);
        }

        public Task<bool> LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.LockExtendAsync(key, value, expiry, flags);
        }

        public RedisValue LockQuery(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.LockQuery(key, flags);
        }

        public Task<RedisValue> LockQueryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.LockQueryAsync(key, flags);
        }

        public bool LockRelease(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.LockRelease(key, value, flags);
        }

        public Task<bool> LockReleaseAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.LockReleaseAsync(key, value, flags);
        }

        public bool LockTake(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.LockTake(key, value, expiry, flags);
        }

        public Task<bool> LockTakeAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.LockTakeAsync(key, value, expiry, flags);
        }

        public TimeSpan Ping(CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.Ping(flags);
        }

        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.PingAsync(flags);
        }

        public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.Publish(channel, message, flags);
        }

        public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.PublishAsync(channel, message, flags);
        }

        public RedisResult ScriptEvaluate(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ScriptEvaluate(script, parameters, flags);
        }

        public RedisResult ScriptEvaluate(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.ScriptEvaluate(script, parameters, flags);
        }

        public RedisResult ScriptEvaluate(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ScriptEvaluate(hash, keys, values, flags);
        }

        public RedisResult ScriptEvaluate(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ScriptEvaluate(script, keys, values, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ScriptEvaluateAsync(script, parameters, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ScriptEvaluateAsync(script, parameters, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ScriptEvaluateAsync(hash, keys, values, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.ScriptEvaluateAsync(script, keys, values, flags);
        }

        public long SetAdd(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetAdd(key, values, flags);
        }

        public bool SetAdd(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetAdd(key, value, flags);
        }

        public Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetAddAsync(key, values, flags);
        }

        public Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetAddAsync(key, value, flags);
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetCombine(operation, keys, flags);
        }

        public RedisValue[] SetCombine(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetCombine(operation, first, second, flags);
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetCombineAndStore(operation, destination, keys, flags);
        }

        public long SetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetCombineAndStore(operation, destination, first, second, flags);
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetCombineAndStoreAsync(operation, destination, keys, flags);
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetCombineAndStoreAsync(operation, destination, first, second, flags);
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetCombineAsync(operation, keys, flags);
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetCombineAsync(operation, first, second, flags);
        }

        public bool SetContains(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetContains(key, value, flags);
        }

        public Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetContainsAsync(key, value, flags);
        }

        public long SetLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetLength(key, flags);
        }

        public Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetLengthAsync(key, flags);
        }

        public RedisValue[] SetMembers(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetMembers(key, flags);
        }

        public Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.SetMembersAsync(key, flags);
        }

        public bool SetMove(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if(_redisDb.SetMove(source, destination, value, flags))
            {
                PublishEvent(source, "srem:" + RedisValueHashCode.GetStableHashCode(value));
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            if (await _redisDb.SetMoveAsync(source, destination, value, flags))
            {
                PublishEvent(source, "srem:" + RedisValueHashCode.GetStableHashCode(value));
                return true;
            }
            else
            {
                return false;
            }
        }

        public RedisValue SetPop(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            RedisValue result = _redisDb.SetPop(key, flags);
            if(!result.IsNull)
            {
                PublishEvent(key, "srem:" + RedisValueHashCode.GetStableHashCode(result));
            }

            return result;
        }

        public async Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            RedisValue result = await _redisDb.SetPopAsync(key, flags);
            if (!result.IsNull)
            {
                PublishEvent(key, "srem:" + RedisValueHashCode.GetStableHashCode(result));
            }

            return result;
        }

        public RedisValue SetRandomMember(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetRandomMember(key, flags);
        }

        public Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetRandomMemberAsync(key, flags);
        }

        public RedisValue[] SetRandomMembers(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetRandomMembers(key, count, flags);
        }

        public Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetRandomMembersAsync(key, count, flags);
        }

        public long SetRemove(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            foreach (RedisValue value in values)
                PublishEvent(key, "srem:" + RedisValueHashCode.GetStableHashCode(value));

            return _redisDb.SetRemove(key, values, flags);
        }

        public bool SetRemove(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "srem:" + RedisValueHashCode.GetStableHashCode(value));
            return _redisDb.SetRemove(key, value, flags);
        }

        public Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            foreach(RedisValue value in values)
                PublishEvent(key, "srem:" + RedisValueHashCode.GetStableHashCode(value));

            return _redisDb.SetRemoveAsync(key, values, flags);
        }

        public Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "srem:" + RedisValueHashCode.GetStableHashCode(value));
            return _redisDb.SetRemoveAsync(key, value, flags);
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            



            return _redisDb.SetScan(key, pattern, pageSize, flags);
        }

        public IEnumerable<RedisValue> SetScan(RedisKey key, RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SetScan(key, pattern, pageSize, cursor, pageOffset, flags);
        }

        public RedisValue[] Sort(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.Sort(key, skip, take, order, sortType, by, get, flags);
        }

        public long SortAndStore(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortAndStore(destination, key, skip, take, order, sortType, by, get, flags);
        }

        public Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortAndStoreAsync(destination, key, skip, take, order, sortType, by, get, flags);
        }

        public Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortAsync(key, skip, take, order, sortType, by, get, flags);
        }

        public long SortedSetAdd(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            long result = _redisDb.SortedSetAdd(key, values, flags);

            foreach (var value in values)
            {
                //Publish an event containing the member hashcode and the score score.
                PublishEvent(key, $"zadd:{RedisValueHashCode.GetStableHashCode(value.Element)}");
            }

            return result;
        }

        public bool SortedSetAdd(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
        {
            if(_redisDb.SortedSetAdd(key, member, score, flags))
            {
                //Publish an event containing the member hashcode and the score score.
                PublishEvent(key, $"zadd:{RedisValueHashCode.GetStableHashCode(member)}");
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            long result = await _redisDb.SortedSetAddAsync(key, values, flags);

            foreach (var value in values)
            {
                //Publish an event containing the member hashcode and the score score.
                PublishEvent(key, $"zadd:{RedisValueHashCode.GetStableHashCode(value.Element)}");
            }

            return result;
        }

        public async Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags = CommandFlags.None)
        {
            bool result = await _redisDb.SortedSetAddAsync(key, member, score, flags);

            if(result)
            {
                PublishEvent(key, $"zadd:{RedisValueHashCode.GetStableHashCode(member)}");
            }

            return result;
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetCombineAndStore(operation, destination, keys, weights, aggregate, flags);
        }

        public long SortedSetCombineAndStore(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetCombineAndStore(operation, destination, first, second, aggregate, flags);
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate, flags);
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate, flags);
        }

        public double SortedSetDecrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetDecrement(key, member, value, flags);
        }

        public Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetDecrementAsync(key, member, value, flags);
        }

        public double SortedSetIncrement(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetIncrement(key, member, value, flags);
        }

        public Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetIncrementAsync(key, member, value, flags);
        }

        public long SortedSetLength(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetLength(key, min, max, exclude, flags);
        }

        public Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetLengthAsync(key, min, max, exclude);
        }

        public long SortedSetLengthByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetLengthByValue(key, min, max, exclude, flags);
        }

        public Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetLengthByValueAsync(key, min, max, exclude, flags);
        }

        public RedisValue[] SortedSetRangeByRank(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRangeByRank(key, start, stop, order, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRangeByRankAsync(key, start, stop, order, flags);
        }

        public SortedSetEntry[] SortedSetRangeByRankWithScores(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRangeByRankWithScores(key, start, stop, order, flags);
        }

        public Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRangeByRankWithScoresAsync(key, start, stop, order, flags);
        }

        public RedisValue[] SortedSetRangeByScore(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRangeByScore(key, start, stop, exclude, order, skip, take, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take, flags);
        }

        public SortedSetEntry[] SortedSetRangeByScoreWithScores(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRangeByScoreWithScores(key, start, stop, exclude, order, skip, take, flags);
        }

        public Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take, flags);
        }

        public RedisValue[] SortedSetRangeByValue(RedisKey key, RedisValue min = default(RedisValue), RedisValue max = default(RedisValue), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRangeByValue(key, min, max, exclude, skip, take, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = default(RedisValue), RedisValue max = default(RedisValue), Exclude exclude = Exclude.None, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRangeByValueAsync(key, min, max, exclude, skip, take, flags);
        }

        public long? SortedSetRank(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRank(key, member, order, flags);
        }

        public Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRankAsync(key, member, order, flags);
        }

        public long SortedSetRemove(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            long result = _redisDb.SortedSetRemove(key, members, flags);
            foreach (var value in members)
            {
                PublishEvent(key, $"zrem:{RedisValueHashCode.GetStableHashCode(value)}");
            }
            return result;
        }

        public bool SortedSetRemove(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (_redisDb.SortedSetRemove(key, member, flags))
            {
                PublishEvent(key, $"zrem:{RedisValueHashCode.GetStableHashCode(member)}");
                return true;
            }
            else
                return false;
        }

        public async Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            long result = await _redisDb.SortedSetRemoveAsync(key, members, flags);

            foreach (var value in members)
            {
                PublishEvent(key, $"zrem:{RedisValueHashCode.GetStableHashCode(value)}");
            }
            return result;
        }

        public async Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            if (await _redisDb.SortedSetRemoveAsync(key, member, flags))
            {
                PublishEvent(key, $"zrem:{RedisValueHashCode.GetStableHashCode(member)}");
                return true;
            }
            else
                return false;
        }

        public long SortedSetRemoveRangeByRank(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRemoveRangeByRank(key, start, stop, flags);
        }

        public Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRemoveRangeByRankAsync(key, start, stop, flags);
        }

        public long SortedSetRemoveRangeByScore(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRemoveRangeByScore(key, start, stop, exclude, flags);
        }

        public Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude, flags);
        }

        public long SortedSetRemoveRangeByValue(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRemoveRangeByValue(key, min, max, exclude, flags);
        }

        public Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetRemoveRangeByValueAsync(key, min, max, exclude, flags);
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern, int pageSize, CommandFlags flags)
        {
            



            return _redisDb.SortedSetScan(key, pattern, pageSize, flags);
        }

        public IEnumerable<SortedSetEntry> SortedSetScan(RedisKey key, RedisValue pattern = default(RedisValue), int pageSize = 10, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetScan(key, pattern, pageSize, cursor, pageOffset, flags);
        }

        public double? SortedSetScore(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.SortedSetScore(key, member, flags);
        }

        public Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.SortedSetScoreAsync(key, member, flags);
        }

        public long StringAppend(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "append");
            return _redisDb.StringAppend(key, value, flags);

        }

        public Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "append");
            return _redisDb.StringAppendAsync(key, value, flags);
        }

        public long StringBitCount(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.StringBitCount(key, start, end, flags);
        }

        public Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.StringBitCountAsync(key, start, end, flags);
        }

        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.StringBitOperation(operation, destination, keys, flags);
        }

        public long StringBitOperation(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.StringBitOperation(operation, destination, first, second, flags);
        }

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.StringBitOperationAsync(operation, destination, keys, flags);
        }

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.StringBitOperationAsync(operation, destination, first, second, flags);
        }

        public long StringBitPosition(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.StringBitPosition(key, bit, start, end, flags);
        }

        public Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.StringBitPositionAsync(key, bit, start, end, flags);
        }

        public double StringDecrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "decrbyfloat");
            return _redisDb.StringDecrement(key, value, flags);
        }

        public long StringDecrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "decrby");
            return _redisDb.StringDecrement(key, value, flags);
        }

        public Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "decrbyfloat");
            return _redisDb.StringDecrementAsync(key, value, flags);
        }

        public Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            this.PublishEvent(key, "decrby");
            return _redisDb.StringDecrementAsync(key, value, flags);
        }

        public RedisValue[] StringGet(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.StringGet(keys, flags);
        }

        public RedisValue StringGet(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.StringGet(key, flags);
        }

        public Task<RedisValue[]> StringGetAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.StringGetAsync(keys, flags);
        }

        public Task<RedisValue> StringGetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.StringGetAsync(key, flags);
        }

        public bool StringGetBit(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.StringGetBit(key, offset, flags);
        }

        public Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.StringGetBitAsync(key, offset, flags);
        }

        public RedisValue StringGetRange(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            



            return _redisDb.StringGetRange(key, start, end, flags);
        }

        public Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.StringGetRangeAsync(key, start, end, flags);
        }

        public RedisValue StringGetSet(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "set");
            return _redisDb.StringGetSet(key, value, flags);
        }

        public Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "set");
            return _redisDb.StringGetSetAsync(key, value, flags);
        }

        public RedisValueWithExpiry StringGetWithExpiry(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.StringGetWithExpiry(key, flags);
        }

        public Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            
            return _redisDb.StringGetWithExpiryAsync(key, flags);
        }

        public double StringIncrement(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "incrbyfloat");
            return _redisDb.StringIncrement(key, value, flags);
        }

        public long StringIncrement(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "incrby");
            return _redisDb.StringIncrement(key, value, flags);
        }

        public Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "incrbyfloat");
            return _redisDb.StringIncrementAsync(key, value, flags);
        }

        public Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "incrby");
            return _redisDb.StringIncrementAsync(key, value, flags);
        }

        public long StringLength(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.StringLength(key, flags);
        }

        public Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return _redisDb.StringLengthAsync(key, flags);
        }

        public bool StringSet(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            foreach (var kvp in values)
            {
                PublishEvent(kvp.Key, "set");
            }
            return _redisDb.StringSet(values, when, flags);
        }

        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "set");   
            return _redisDb.StringSet(key, value, expiry, when, flags);
        }

        public Task<bool> StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            foreach(var kvp in values)
            {
                PublishEvent(kvp.Key, "set");
            }

            return _redisDb.StringSetAsync(values, when, flags);
        }

        public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "set");
            return _redisDb.StringSetAsync(key, value, expiry, when, flags);
        }

        public bool StringSetBit(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "setbit");
            return _redisDb.StringSetBit(key, offset, bit, flags);
        }

        public Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "setbit");
            return _redisDb.StringSetBitAsync(key, offset, bit, flags);
        }


        public RedisValue StringSetRange(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "setrange");
            return _redisDb.StringSetRange(key, offset, value, flags);
        }

        public Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            PublishEvent(key, "setrange");
            return _redisDb.StringSetRangeAsync(key, offset, value, flags);
        }

        public bool TryWait(Task task)
        {
            return _redisDb.TryWait(task);
        }

        public void Wait(Task task)
        {   
            _redisDb.Wait(task);
        }

        public T Wait<T>(Task<T> task)
        {
            
            return _redisDb.Wait(task);
        }

        public void WaitAll(params Task[] tasks)
        {
            
            _redisDb.WaitAll(tasks);
        }
    }
}
