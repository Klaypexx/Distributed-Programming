namespace Valuator.Services;

public interface IRedisService
{
    string StringGet(string key);
    IEnumerable<string> GetAllKeys();
    void StringSet(string key, string value);
}
