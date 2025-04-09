using StackExchange.Redis;

namespace RankCalculator.Services
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisService(string hostName)
        {
            _redis = ConnectionMultiplexer.Connect(hostName);
        }

        public async Task<string> GetTextAsync(string id)
        {
            IDatabase db = _redis.GetDatabase();
            return await db.StringGetAsync("TEXT-" + id);
        }

        public async Task SetRankAsync(string id, double rank)
        {
            IDatabase db = _redis.GetDatabase();
            await db.StringSetAsync("RANK-" + id, rank.ToString());
        }
    }
}
