using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IDistributedCache? _cache;
    private readonly ConnectionMultiplexer _redis;

    public IndexModel( ILogger<IndexModel> logger, IDistributedCache cache )
    {
        _logger = logger;
        _cache = cache;
        _redis = ConnectionMultiplexer.Connect("localhost:6379");
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost( string text )
    {
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();

        string rankKey = "RANK-" + id;
        double rank = CalculateRank(text);
        _cache.SetString(rankKey, rank.ToString());

        string similarityKey = "SIMILARITY-" + id;
        double similarity = CheckSimilarity(text);
        _cache.SetString(similarityKey, similarity.ToString());

        string textKey = "TEXT-" + id;
        _cache.SetString(textKey, text);

        return Redirect($"summary?id={id}");
    }

    private static double CalculateRank( string text )
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        int alphabeticCount = text.Count(c =>
            (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
            (c >= 'а' && c <= 'я') || (c >= 'А' && c <= 'Я'));

        int totalCharacters = text.Length;
        int nonAlphabeticCount = totalCharacters - alphabeticCount;

        double rank = (double)nonAlphabeticCount / totalCharacters;

        return Math.Round(rank, 1);
    }

    private int CheckSimilarity( string text )
    {
        var server = _redis.GetServer(_redis.GetEndPoints()[0]);
        var keys = server.Keys(pattern: "TEXT-*");

        foreach (var key in keys)
        {
            string value = _cache.GetString(key);
            if (value == null)
            {
                continue;
            }

            if (value.Contains(text))
            {
                return 1; 
            }
        }

        return 0;
    }
}
