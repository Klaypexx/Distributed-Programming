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

    public string Id { get; set; }
    public string Rank { get; set; }
    public string Similarity { get; set; }

    public async Task OnGetAsync( string id )
    {
        _logger.LogDebug(id);

        Id = id;

        string textedSimilarity = await _redisService!.StringGetAsync("SIMILARITY-" + id);
        if (textedSimilarity == null)
        {
            throw new ArgumentNullException(nameof(textedSimilarity));
        }

        Similarity = textedSimilarity;
    }
}
