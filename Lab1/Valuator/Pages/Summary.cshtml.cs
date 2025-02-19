using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Services;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IRedisService? _redisService;

    public SummaryModel(ILogger<SummaryModel> logger, IRedisService redisService )
    {
        _logger = logger;
        _redisService = redisService;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        string textedRank = _redisService.StringGet("RANK-" + id);
        if (textedRank == null)
        {
            throw new ArgumentNullException(nameof(textedRank));
        }

        string textedSimilarity = _redisService.StringGet("SIMILARITY-" + id);
        if (textedSimilarity == null)
        {
            throw new ArgumentNullException(nameof(textedSimilarity));
        }

        Rank = double.Parse(textedRank);
        Similarity = double.Parse(textedSimilarity);
    }
}
