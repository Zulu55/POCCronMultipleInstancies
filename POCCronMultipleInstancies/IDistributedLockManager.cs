namespace POCCronMultipleInstancies
{
    public interface IDistributedLockManager
    {
        Task<bool> AcquireLockAsync(string lockKey, TimeSpan expiryTime);
        Task ReleaseLockAsync(string lockKey);
    }
}