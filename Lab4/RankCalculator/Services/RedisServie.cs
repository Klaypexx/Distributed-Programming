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

        public async Task<string> GetText(string id)
        {
            IDatabase db = _redis.GetDatabase();
            return await db.StringGetAsync("TEXT-" + id);
        }

        public async Task SetRank(string id, double rank)
        {
            IDatabase db = _redis.GetDatabase();
            await db.StringSetAsync("RANK-" + id, rank.ToString());
        }
    }
}
