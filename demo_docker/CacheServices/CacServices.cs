using Microsoft.Extensions.Caching.Distributed;

namespace demo_docker.CacheServices
{
    public class CacServices
    {
        IDistributedCache _cache;
        public CacServices(IDistributedCache  cache) { _cache = cache; }

        public async Task RemoveFromCache()
        {
            await _cache.RemoveAsync("Note_Id");
            await _cache.RemoveAsync("Message");
            await _cache.RemoveAsync("Password");
            await _cache.RemoveAsync("IsPassword");
        }
    }
}
