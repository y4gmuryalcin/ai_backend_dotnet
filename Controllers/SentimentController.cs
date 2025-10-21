using Microsoft.AspNetCore.Mvc;
using ai_backend_dotnet.Data;
using ai_backend_dotnet.Models;
using System.Net.Http.Json;

namespace ai_backend_dotnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SentimentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public SentimentController(AppDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        public class TextInput
        {
            public string Text { get; set; } = string.Empty;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeText([FromBody] TextInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Text))
                return BadRequest(new { error = "Metin boş olamaz." });

            try
            {
                // 🔗 Python API'ye istek at
                var response = await _httpClient.PostAsJsonAsync(
                    "http://127.0.0.1:7863/predict",
                    new { text = input.Text }
                );

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { error = "Python servisi hatası." });
                }

                // Python'dan dönen sonucu al
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                var label = result?["label"]?.ToString() ?? "Bilinmiyor";

                // Veritabanına kaydet
                var message = new Message { Text = input.Text, Sentiment = label };
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                return Ok(new { label });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Bağlantı hatası 😢", details = ex.Message });
            }
        }

        [HttpGet("messages")]
        public IActionResult GetMessages()
        {
            var messages = _context.Messages
                .Select(m => new { m.Id, m.Text, m.Sentiment })
                .ToList();

            return Ok(messages);
        }
    }
}
