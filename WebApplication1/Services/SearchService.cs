public class SearchService
{
    private readonly EmbeddingService _embeddingService;
    private readonly QdrantService _qdrantService;

    public SearchService(
        EmbeddingService embeddingService,
        QdrantService qdrantService)
    {
        _embeddingService = embeddingService;
        _qdrantService = qdrantService;
    }

    public async Task<string> SearchAsync(string query)
    {
        Console.WriteLine("======================================");
        Console.WriteLine($"SEARCH : {query}");
        Console.WriteLine("======================================");

        //
        // Embedding生成
        //

        var embedding =
            await _embeddingService.CreateEmbeddingAsync(query);

        Console.WriteLine(
            $"Embedding Size = {embedding.Count}");

        //
        // Qdrant検索
        //

        var result =
            await _qdrantService.SearchAsync(embedding);

        Console.WriteLine("Search Complete");

        return result;
    }
}