using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalApiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ExternalApiController> _logger;

        public ExternalApiController(IHttpClientFactory httpClientFactory, ILogger<ExternalApiController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost("consume")]
        public async Task<IActionResult> ConsumeExternalApi([FromBody] FormData formData)
        {
            var client = _httpClientFactory.CreateClient();

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("EditarPorEmail", formData.EditarPorEmail),
                new KeyValuePair<string, string>("EditarPorSMS", formData.EditarPorSMS),
                new KeyValuePair<string, string>("EditarNome", formData.EditarNome),
                new KeyValuePair<string, string>("EditarUsuario", formData.EditarUsuario),
                new KeyValuePair<string, string>("EditarSenha", formData.EditarSenha),
                new KeyValuePair<string, string>("EditarEmail", formData.EditarEmail),
                new KeyValuePair<string, string>("EditarCelular", formData.EditarCelular),
                new KeyValuePair<string, string>("obs", formData.obs),
                new KeyValuePair<string, string>("EditarPerfil[]", formData.EditarPerfil.FirstOrDefault())
            });

            var request = new HttpRequestMessage(HttpMethod.Post, "https://pnw7.cc/painel/EnviarAdicionarTeste.php")
            {
                Headers =
                {
                    { "Accept", "*/*" },
                    { "Accept-Encoding", "gzip, deflate, br, zstd" },
                    { "Accept-Language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7" },
                    { "Cookie", "codeRev=z%3DcWVpVjM3UjN; PHPSESSID=gkb2tik0kpgs8atra0nlc7hbtk" },
                    { "Origin", "https://pnw7.cc" },
                    { "Referer", "https://pnw7.cc/painel/index.php?p=teste" },
                    { "Sec-Ch-Ua", "\"Not/A)Brand\";v=\"8\", \"Chromium\";v=\"126\", \"Google Chrome\";v=\"126\"" },
                    { "Sec-Ch-Ua-Mobile", "?0" },
                    { "Sec-Ch-Ua-Platform", "\"Windows\"" },
                    { "Sec-Fetch-Dest", "empty" },
                    { "Sec-Fetch-Mode", "cors" },
                    { "Sec-Fetch-Site", "same-origin" },
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36" },
                    { "X-Requested-With", "XMLHttpRequest" }
                },
                Content = formContent
            };

            try
            {
                _logger.LogInformation("Sending request to external API");
                var response = await client.SendAsync(request);

                _logger.LogInformation("Received response from external API");
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                // Convert the text response to JSON format
                var jsonResponse = new
                {
                    response = responseBody
                };

                var formattedJsonResponse = JsonSerializer.Serialize(jsonResponse, new JsonSerializerOptions { WriteIndented = true });

                _logger.LogInformation("Response body: {FormattedJsonResponse}", formattedJsonResponse);
                return Ok(formattedJsonResponse);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while sending the request");
                return StatusCode(500, "An error occurred while sending the request");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(500, "An unexpected error occurred");
            }
        }
    }

    public class FormData
    {
        public string EditarPorEmail { get; set; }
        public string EditarPorSMS { get; set; }
        public string EditarNome { get; set; }
        public string EditarUsuario { get; set; }
        public string EditarSenha { get; set; }
        public string EditarEmail { get; set; }
        public string EditarCelular { get; set; }
        public string obs { get; set; }
        public string[] EditarPerfil { get; set; }
    }
}
