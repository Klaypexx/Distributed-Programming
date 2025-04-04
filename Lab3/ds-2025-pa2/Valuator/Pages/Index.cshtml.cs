using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Valuator.Services;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IRedisService _redisService;
        private readonly IRabbitMqService _rabbitMqService;

        public IndexModel( ILogger<IndexModel> logger, IRedisService redisService, IRabbitMqService rabbitMqService )
        {
            _logger = logger;
            _redisService = redisService;
            _rabbitMqService = rabbitMqService;
        }

        public void OnGet()
        {
        }

        //загрузка Index до загрузки данных 
        public async Task<IActionResult> OnPostAsync( string text )
        {
            _logger.LogDebug(text);

            if (!String.IsNullOrEmpty(text))
            {
                string id = Guid.NewGuid().ToString();

                string similarityKey = "SIMILARITY-" + id;
                double similarity = await CheckSimilarity(text);
                await _redisService.StringSetAsync(similarityKey, similarity.ToString());

                string textKey = "TEXT-" + id;
                await _redisService.StringSetAsync(textKey, text);

                await _rabbitMqService.SendMessage(id);

                return Redirect($"summary?id={id}");
            }
            return Redirect("/");
        }

        private async Task<int> CheckSimilarity( string text )
        {
            List<string> keys = _redisService.GetAllKeys().ToList();

            foreach (var key in keys)
            {
                var value = await _redisService.StringGetAsync(key);
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
