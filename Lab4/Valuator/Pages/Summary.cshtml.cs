using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Services;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IRedisService? _redisService;

    public SummaryModel( ILogger<SummaryModel> logger, IRedisService redisService )
    {
        _logger = logger;
        _redisService = redisService;
        Rank = "Загрузка данных...";
        Similarity = "Загрузка данных...";
    }

    public string Rank { get; set; }
    public string Similarity { get; set; }

    public async Task OnGetAsync( string id )
    {
        _logger.LogDebug(id);

        const int maxAttempts = 3;
        int attempts = 0;
        string textedRank = String.Empty;

        while (attempts < maxAttempts)
        {
            textedRank = await _redisService!.StringGetAsync("RANK-" + id);

            if (!String.IsNullOrEmpty(textedRank))
            {
                break;
            }

            attempts++;
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        if (String.IsNullOrEmpty(textedRank))
        {
            throw new InvalidOperationException($"Failed to retrieve 'RANK-{id}' after {maxAttempts} attempts.");
        }

        string textedSimilarity = await _redisService!.StringGetAsync("SIMILARITY-" + id);
        if (textedSimilarity == null)
        {
            throw new ArgumentNullException(nameof(textedSimilarity));
        }

        Rank = textedRank;
        Similarity = textedSimilarity;
    }
}
