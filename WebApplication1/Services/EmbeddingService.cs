using System.Net.Http.Json;

public class EmbeddingService
{
    private readonly HttpClient _http;

    public EmbeddingService(HttpClient http)
    {
        _http = http;
    }


    public async Task<List<float>> CreateEmbeddingAsync(string text)
    {
        var request = new
        {
            model = "nomic-embed-text",
            prompt = text
        };


        var response =
            await _http.PostAsJsonAsync(
                "api/embeddings",
                request);


        response.EnsureSuccessStatusCode();


        var result =
            await response.Content
                .ReadFromJsonAsync<OllamaEmbeddingResponse>();


        return result!.Embedding;
    }
}