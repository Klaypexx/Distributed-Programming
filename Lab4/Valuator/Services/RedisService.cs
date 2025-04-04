using StackExchange.Redis;

namespace Valuator.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _redisDatabase;
        private readonly IConnectionMultiplexer _connection;

        public RedisService(string connectionString)
        {
            _connection = ConnectionMultiplexer.Connect(connectionString);
            _redisDatabase = _connection.GetDatabase();
        }

        public async Task<string> StringGetAsync( string key )
        {
            RedisValue result = await _redisDatabase.StringGetAsync(key);
            return result.ToString();
        }

        public IEnumerable<string> GetAllKeys()
        {
            var server = _connection.GetServer(_connection.GetEndPoints().First());
            var keys = server.Keys(database: _redisDatabase.Database);

            return keys.Select(k => (string)k!);
        }

        public async Task StringSetAsync( string key, string value )
        {
            await _redisDatabase.StringSetAsync(key, value);
        }
    }
}
