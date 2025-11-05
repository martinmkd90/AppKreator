namespace AppKreator.Services
{
    public interface IOpenAiClient
    {
        Task<string> ChatCompletionAsync(string model, string system, string user, CancellationToken ct);
    }

}
