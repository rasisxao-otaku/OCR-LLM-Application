using System.Net.Http.Json;

public class OllamaEmbeddingService
{
    private readonly HttpClient _httpClient;

    public OllamaEmbeddingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<float[]> CreateEmbeddingAsync(string text)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/embeddings",
            new
            {
                model = "nomic-embed-text",
                prompt = text
            });

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<EmbeddingResponse>();

        return result!.Embedding;
    }
}