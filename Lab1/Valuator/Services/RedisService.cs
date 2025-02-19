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

        public string StringGet( string key )
        {
            RedisValue result = _redisDatabase.StringGet(key);
            return result.ToString();
        }

        public IEnumerable<string> GetAllKeys()
        {
            var server = _connection.GetServer(_connection.GetEndPoints().First());
            var keys = server.Keys(database: _redisDatabase.Database);

            return keys.Select(k => (string)k);
        }

        public void StringSet( string key, string value )
        {
            _redisDatabase.StringSet(key, value);
        }
    }
}
