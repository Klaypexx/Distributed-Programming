namespace Valuator.Services;

public interface IRedisService
{
    Task<string> StringGetAsync(string key);
    IEnumerable<string> GetAllKeys();
    Task StringSetAsync(string key, string value);
}
