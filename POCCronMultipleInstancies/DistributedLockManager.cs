using StackExchange.Redis;

namespace POCCronMultipleInstancies
{
    public class DistributedLockManager : IDistributedLockManager
    {
        private readonly IDatabase _redisDatabase;

        public DistributedLockManager()
        {
            var redis = ConnectionMultiplexer.Connect("zulutest.redis.cache.windows.net:6380,password=OWuJzbESK5HFAE5jMPLxnVk1s9oxUKYQkAzCaAqTIao=,ssl=True,abortConnect=False");
            _redisDatabase = redis.GetDatabase();
        }

        public async Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiryTime)
        {
            return await _redisDatabase.LockTakeAsync(lockKey, "lock", expiryTime);
        }

        public async Task ReleaseLockAsync(string lockKey)
        {
            await _redisDatabase.LockReleaseAsync(lockKey, "lock");
        }
    }
}