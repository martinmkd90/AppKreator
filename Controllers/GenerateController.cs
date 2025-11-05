namespace AppKreator.Controllers
{
    using AppKreator.Dtos;
    using AppKreator.Prompts;
    using AppKreator.Services;
    using AppKreator.Validation;
    using Microsoft.AspNetCore.Mvc;
    using System.Text.Json;

    [ApiController]
    [Route("api/[controller]")]
    public class GenerateController : ControllerBase
    {
        private readonly IOpenAiClient _ai;
        private readonly IConfiguration _cfg;

        public GenerateController(IOpenAiClient ai, IConfiguration cfg)
        {
            _ai = ai; _cfg = cfg;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GenerateRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Idea))
                return BadRequest("Idea is required.");

            var model = req.Model ?? _cfg["OpenAI:Model"] ?? "gpt-4.1-mini";
            var system = PromptTemplates.System(req.DefaultStack);
            var user = PromptTemplates.User(req.Idea, req.UiFramework, req.Language, req.Backend, req.Constraints);

            var text = await _ai.ChatCompletionAsync(model, system, user, ct);

            // Validate schema before returning to client
            var (ok, error) = JsonValidator.Validate(text);
            if (!ok) return BadRequest(new { message = "Model output failed schema validation", error, raw = text });

            var payload = JsonSerializer.Deserialize<GenPayload>(text);
            if (payload is null) return BadRequest(new { message = "Invalid JSON", raw = text });

            return Ok(payload);
        }
    }

}
