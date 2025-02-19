using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Services;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IRedisService _redisService;

        public IndexModel( ILogger<IndexModel> logger, IRedisService redisService )
        {
            _logger = logger;
            _redisService = redisService;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost( string text )
        {
            _logger.LogDebug(text);

            if (!String.IsNullOrEmpty(text))
            {
                string id = Guid.NewGuid().ToString();
                string rankKey = "RANK-" + id;
                double rank = CalculateRank(text);
                _redisService.StringSet(rankKey, rank.ToString());

                string similarityKey = "SIMILARITY-" + id;
                double similarity = CheckSimilarity(text);
                _redisService.StringSet(similarityKey, similarity.ToString());

                string textKey = "TEXT-" + id;
                _redisService.StringSet(textKey, text);

                return Redirect($"summary?id={id}");
            }
            return Redirect("/");
        }

        private static double CalculateRank( string text )
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            int nonAlphabetic = text.Count(word => !char.IsLetter(word));
            int totalCharacters = text.Length;

            double rank = (double)nonAlphabetic / totalCharacters;
            return Math.Round(rank, 1);
        }

        private int CheckSimilarity( string text )
        {
            List<string> keys = _redisService.GetAllKeys().ToList();

            foreach (var key in keys)
            {
                var value = _redisService.StringGet(key);
                if (value == null)
                {
                    continue;
                }

                if (text == value)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}
