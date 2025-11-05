namespace AppKreator.Services
{
    using System.Net.Http.Headers;
    using System.Text.Json;

    public sealed class OpenAiClient : IOpenAiClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public OpenAiClient(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _apiKey = cfg["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey missing");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _http.Timeout = TimeSpan.FromSeconds(int.TryParse(cfg["OpenAI:RequestTimeoutSeconds"], out var s) ? s : 60);
        }

        public async Task<string> ChatCompletionAsync(string model, string system, string user, CancellationToken ct)
        {
            var body = new
            {
                model,
                messages = new[]
                {

                    new { role = "system", content = system },
                    new { role = "user", content = user }
                },
                response_format = new { type = "json_object" },
                temperature = 0.4
            };

            using var res = await _http.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", body, ct);
            if (!res.IsSuccessStatusCode)
            {
                var err = await res.Content.ReadAsStringAsync(ct);
                // LOG err somewhere (make sure logs redact the key)
                throw new HttpRequestException($"OpenAI error {(int)res.StatusCode}: {res.ReasonPhrase}. Body: {err}");
            }

            using var doc = await JsonDocument.ParseAsync(await res.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString() ?? "{}";
        }

    }

}
