using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IDistributedCache? _cache;

    public SummaryModel(ILogger<SummaryModel> logger, IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug(id);

        string textedRank = _cache.GetString("RANK-" + id);
        if (textedRank == null)
        {
            throw new ArgumentNullException(nameof(textedRank));
        }

        string textedSimilarity = _cache.GetString("SIMILARITY-" + id);
        if (textedSimilarity == null)
        {
            throw new ArgumentNullException(nameof(textedSimilarity));
        }

        Rank = double.Parse(textedRank);
        Similarity = double.Parse(textedSimilarity);
    }
}
